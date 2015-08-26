using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MicroWorld.Logics.PathFinding
{
    class Node
    {
        public int H, G, F;
        public int X, Y;
        public int PX, PY;

        public bool IsSamePos(int x, int y)
        {
            return X == x && Y == y;
        }
    }

    struct _Point
    {
        public int X, Y;

        public _Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public _Point(Point p)
        {
            X = p.X;
            Y = p.Y;
        }

        public static _Point operator +(_Point p1, _Point p2)
        {
            return new _Point(p1.X + p2.X, p1.Y + p2.Y);
        }
    }

    public unsafe class PathFinder2
    {
        List<Node> open = new List<Node>();
        List<Node> close = new List<Node>();
        _Point _end = new _Point();
        _Point _start = new _Point();
        static _Point[] directions = new _Point[] { new _Point(8, 0), new _Point(0, 8), new _Point(-8, 0), new _Point(0, -8) };
        const int MAX_OPEN = 2000;//r = this*8

        internal static bool WasPathFound = false;//TODO rm
        internal static TimeSpan LastSearch = new TimeSpan();//TODO rm

        public List<Point> FindPath(Point start, Point end)
        {
            WasPathFound = false;
            LastSearch = new TimeSpan(DateTime.Now.Ticks);

            long StartTicks = Main.Ticks;

            open.Clear();
            close.Clear();
            _end = new _Point(end);
            _start = new _Point(start);
            _Point cur = new _Point(start);
            Node curn, tpn;
            Node tpn2i;

            curn = GetNodeAt(cur);
            curn.F = curn.H;
            close.Add(curn);

            if (InputEngine.curKeyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S))
            {
            }

            while (true)
            {
                for (int i = 0; i < directions.Length; i++)
                {
                    tpn = GetNodeAt(cur + directions[i]);
                    tpn.PX = cur.X;
                    tpn.PY = cur.Y;
                    if (tpn.X == _end.X && tpn.Y == _end.Y)
                    {
                        curn = tpn;
                        goto EndFound;
                    }
                    if (Components.ComponentsManager.VisibilityMap.GetAStarValue(tpn.X, tpn.Y) == 0)
                        continue;
                    //tpn.H = Components.ComponentsManager.MapVisibility.GetAStarValue(tpn.X, tpn.Y) * 2;
                    //if (tpn.G == curn.G)//VisMap is occupied
                    //    continue;
                    if (tpn.X - tpn.PX != curn.X - curn.PX || tpn.Y - tpn.PY != curn.Y - curn.PY)
                        tpn.H += 2;
                    tpn.F = tpn.G + tpn.H;
                    //punish change direction
                    if ((tpn2i = NodeExistsOpenIndex(tpn.X, tpn.Y)) != null)
                        if (tpn.F < tpn2i.F)
                        {
                            open.Remove(tpn2i);
                        }
                        else
                            continue;
                    //open.Add(tpn);
                    InsertOpenNode(tpn);
                }
                if (open.Count > MAX_OPEN || open.Count == 0 ||
                    (close.Count % 100 == 0 && Main.Ticks - StartTicks > 3))//calculations cannot be longer then 60ms
                        return null;
                //quicksortOpen();
                curn = open[0];
                cur.X = curn.X;
                cur.Y = curn.Y;
                open.RemoveAt(0);
                close.Add(curn);
            }
        EndFound:
            List<Point> path = new List<Point>();
            path.Add(end);
            while (curn.X != start.X || curn.Y != start.Y)
            {
                path.Add(new Point(curn.X, curn.Y));
                curn = GetClosedParent(ref curn);
            }
            path.Add(start);
            WasPathFound = true;
            if (InputEngine.curKeyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
                SaveDebug();
            return path;
        }

        private void SaveDebug()
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(1000, 1000);
            foreach (var a in open)
            {
                bmp.SetPixel(a.X + 500, a.Y + 500, System.Drawing.Color.FromArgb(0, 255, 0));
            }
            foreach (var a in close)
            {
                bmp.SetPixel(a.X + 500, a.Y + 500, System.Drawing.Color.FromArgb(a.G, 0, 0));
            }
            bmp.Save("C:/atp.bmp");
        }

        private Node GetClosedParent(ref Node n)
        {
            Node t;
            for (int i = 0; i < close.Count; i++)
            {
                t = close[i];
                if (t.X == n.PX && t.Y == n.PY)
                    return t;
            }
            return new Node();
        }

        private Node NodeExistsClosed(int x, int y)
        {
            int t = close.Count;
            for (int i = 0; i < t; i++)
            {
                if (close[i].X == x && close[i].Y == y)
                    return close[i];
            }
            return null;
        }

        private Node NodeExistsOpenIndex(int x, int y)
        {
            foreach (var a in open)
            {
                if (a.X == x && a.Y == y)
                    return a;
            }
            //for (int i = 0; i < open.Count; i++)
            //{
            //    if (open[i].X == x && open[i].Y == y)
            //        return open[i];
            //}
            //return -1;
            return null;
        }

        private void InsertOpenNode(Node n)
        {
            if (open.Count == 0)
                open.Add(n);
            else
            {
                for (int i = 0; i < open.Count; i++)
                {
                    if (open[i].F >= n.F)
                    {
                        open.Insert(i, n);
                        return;
                    }
                }
                open.Add(n);
            }
        }

        private Node GetNodeAt(_Point p)
        {
            Node n = new Node();
            n.X = p.X;
            n.Y = p.Y;
            n.G = (Math.Abs(p.X - _start.X) + Math.Abs(p.Y - _start.Y)) / 4;
            n.H = (Math.Abs(p.X - _end.X) + Math.Abs(p.Y - _end.Y)) / 4;
            return n;
        }

        #region Sort
        void swap(int i, int j)
        {
            Node temp = open[j];
            open[j] = open[i];
            open[i] = temp;
        }

        void quicksort0(int a, int b)
        {
            if (a >= b)
                return;

            Node key = open[a];
            int i = a + 1, j = b;
            while (i < j)
            {
                while (i < j && open[j].F >= key.F)
                    --j;
                while (i < j && open[i].F <= key.F)
                    ++i;
                if (i < j)
                    swap(i, j);
            }
            if (open[a].F > open[i].F)
            {
                swap(a, i);
                quicksort0(a, i - 1);
                quicksort0(i + 1, b);
            }
            else
            {
                quicksort0(a + 1, b);
            }
        }

        void quicksortOpen()
        {
            quicksort0(0, open.Count - 1);
        }
        #endregion
    }
}
