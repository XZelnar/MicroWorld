using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MicroWorld.Logics.PathFinding
{
    public unsafe class PathFinder
    {
        const int MAX_ITERATIONS = 4000;

        class Node
        {
            public int x, y;
            public Node parent;
            public int h, g, f;

            public Node()
            {
            }

            public Node(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        struct Point
        {
            public int x, y;

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public bool Equals(Point p)
            {
                return p.x == x && p.y == y;
            }

            public static Point operator +(Point p1, Point p2)
            {
                return new Point(p1.x + p2.x, p1.y + p2.y);
            }
        }

        private Point[] directionsLong = new Point[]
        {
            new Point(16, 0),
            new Point(-16, 0),
            new Point(0, 16),
            new Point(0, -16),
        };

        private Point[] directionsShort = new Point[]
        {
            new Point(8, 0),
            new Point(-8, 0),
            new Point(0, 8),
            new Point(0, -8),
        };

        private List<Node> close = new List<Node>();
        private List<Node> open = new List<Node>();
        private Point _start = new Point();
        private Point _end = new Point();
        internal static bool WasPathFound = false;
        internal static TimeSpan LastSearch = new TimeSpan(DateTime.Now.Ticks);

        public List<Microsoft.Xna.Framework.Point> FindPath(Microsoft.Xna.Framework.Point s, Microsoft.Xna.Framework.Point e)
        {
            _start = new Point(s.X, s.Y);
            _end = new Point(e.X, e.Y);

            WasPathFound = false;
            LastSearch = new TimeSpan(DateTime.Now.Ticks);
            close.Clear();
            open.Clear();

            long startTicks = Main.Ticks;
            int iterations = 0;

            Node n;
            Node tn;
            int tni;
            bool longdir = false;
            Node curn = new Node();
            curn.x = _start.x;
            curn.y = _start.y;
            curn.g = 0;
            Heuristic(curn);
            open.Add(curn);

            while (open.Count > 0)
            {
                iterations++;
                curn = open[0];
                open.RemoveAt(0);

                longdir = curn.h > 100;
                for (int i = 0; i < directionsShort.Length; i++)
                {
                    if (longdir)
                        n = new Node(curn.x + directionsLong[i].x, curn.y + directionsLong[i].y);
                    else
                        n = new Node(curn.x + directionsShort[i].x, curn.y + directionsShort[i].y);

                    if (IsClosed(n.x, n.y))
                        continue;

                    n.parent = curn;
                    if (n.x == _end.x && n.y == _end.y)
                    {
                        curn = n;
                        goto End;
                    }

                    if (longdir)
                    {
                        n.g = Components.ComponentsManager.VisibilityMap.GetAStarValue(n.x, n.y);
                        if (n.g == 0)
                            continue;
                        tni = Components.ComponentsManager.VisibilityMap.GetAStarValue(n.x - directionsShort[i].x, n.y - directionsShort[i].y);
                        if (tni == 0)
                            continue;
                        n.g += tni + curn.g;
                        if (n.g == curn.g)
                            continue;
                    }
                    else
                    {
                        n.g = curn.g + Components.ComponentsManager.VisibilityMap.GetAStarValue(n.x, n.y);
                        if (n.g == curn.g)
                            continue;
                    }

                    if (curn.parent != null && (
                        Math.Sign(n.x - n.parent.x) != Math.Sign(curn.x - curn.parent.x) ||
                        Math.Sign(n.y - n.parent.y) != Math.Sign(curn.y - curn.parent.y)))
                        n.g += 6;

                    Heuristic(n);
                    n.f = n.g + n.h;

                    tni = GetOpenIfAny(n.x, n.y, out tn);
                    if (tn != null)
                    {
                        if (tn.f > n.f)
                        {
                            open.RemoveAt(tni);
                            AddOpen(n);
                        }
                    }
                    else
                        AddOpen(n);
                }

                if (open.Count > 2000 || iterations > MAX_ITERATIONS)
                    return null;
            }
            return null;

        End:

            WasPathFound = true;
            List<Microsoft.Xna.Framework.Point> path = new List<Microsoft.Xna.Framework.Point>();
            while (curn != null)
            {
                path.Add(new Microsoft.Xna.Framework.Point(curn.x, curn.y));
                curn = curn.parent;
            }

            return path;
        }

        private void Heuristic(Node n)
        {
            n.h = Math.Abs(n.x - _end.x) + Math.Abs(n.y - _end.y);
        }

        private bool IsClosed(int x, int y)
        {
            foreach (var a in close)
            {
                if (a.x == x && a.y == y)
                    return true;
            }
            return false;
        }

        int goiai = 0;
        private int GetOpenIfAny(int x, int y, out Node n)
        {
            for (goiai = 0; goiai < open.Count; goiai++)
            {
                if (open[goiai].x == x && open[goiai].y == y)
                {
                    n = open[goiai];
                    return goiai;
                }
            }
            n = null;
            return -1;
        }

        private void AddOpen(Node n)
        {
            for (int i = open.Count - 1; i >= 0; i--)
            {
                if (n.f >= open[i].f)
                {
                    open.Insert(i + 1, n);
                    return;
                }
            }
            open.Insert(0, n);
            return;
        }
    }
}
