using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MicroWorld.IO;

namespace MicroWorld.Logics
{
    public static class PlacableAreasManager
    {
        internal static List<Rectangle> areas = new List<Rectangle>();

        public static void Add(Rectangle r)
        {
            if (r.Width < 0)
            {
                int t = r.X;
                r.X += r.Width;
                r.Width = t - r.X;
            }
            if (r.Height < 0)
            {
                int t = r.Y;
                r.Y += r.Height;
                r.Height = t - r.Y;
            }
            if (r.Width <= 0 || r.Height <= 0) return;
            areas.Add(r);
        }

        internal static void Clear()
        {
            areas.Clear();
        }

        public static void Remove(int i)
        {
            if (i < 0 || i >= areas.Count)
                return;
            areas.RemoveAt(i);
        }

        public static bool Remove(int x, int y)
        {
            if (areas.Count == 0) 
                return false;
            for (int i = 0; i < areas.Count; i++)
                if (areas[i].Contains(x, y))
                {
                    areas.RemoveAt(i);
                    i--;
                    return true;
                }
            return false;
        }

        public static bool IsPlacable(float x, float y)
        {
            if (areas.Count == 0) 
                return true;
            if (Main.curState == "GAMElvlDesign") 
                return true;
            return _isPlacable((int)x, (int)y);
        }

        private static bool _isPlacable(int x, int y)
        {
            foreach (Rectangle r in areas)
            {
                if (x >= r.X && x <= r.X + r.Width &&
                    y >= r.Y && y <= r.Y + r.Height)
                    return true;
            }
            return false;
        }

        public static bool IsPlacable(float x, float y, float w, float h)
        {
            if (areas.Count == 0) 
                return true;
            if (Main.curState == "GAMElvlDesign") 
                return true;

            for (int tx = (int)x; tx <= x + w; tx += Shortcuts.GridSize)
                for (int ty = (int)y; ty <= y + h; ty += Shortcuts.GridSize)
                    if (!_isPlacable(tx, ty))
                        return false;
            return true;
        }

        static int makePlacableDepth = 0;
        public static void MakePlacable(ref int x, ref int y)
        {
            makePlacableDepth = 0;
            makePlacable(ref x, ref y);
        }

        private static void makePlacable(ref int x, ref int y)
        {
            makePlacableDepth++;
            if (makePlacableDepth >= 100) return;
            if (IsPlacable(x, y)) return;
            float md = float.MaxValue;
            float d = 0;

            for (int i = 0; i < areas.Count; i++)
            {
                if (x < areas[i].X)
                {
                    d = Math.Abs(areas[i].X - x);
                    if (d < md) md = d;
                }
                if (y < areas[i].Y)
                {
                    d = Math.Abs(areas[i].Y - y);
                    if (d < md) md = d;
                }
                if (x > areas[i].X + areas[i].Width)
                {
                    d = Math.Abs(areas[i].X + areas[i].Width - x);
                    if (d < md) md = d;
                }
                if (y > areas[i].Y + areas[i].Height)
                {
                    d = Math.Abs(areas[i].Y + areas[i].Height - y);
                    if (d < md) md = d;
                }
            }

            for (int i = 0; i < areas.Count; i++)
            {
                if (x < areas[i].X)
                {
                    d = Math.Abs(areas[i].X - x);
                    if (d == md)
                    {
                        x = areas[i].X;
                        makePlacable(ref x, ref y);
                        return;
                    }
                }
                if (y < areas[i].Y)
                {
                    d = Math.Abs(areas[i].Y - y);
                    if (d == md)
                    {
                        y = areas[i].Y;
                        makePlacable(ref x, ref y);
                        return;
                    }
                }
                if (x > areas[i].X + areas[i].Width)
                {
                    d = Math.Abs(areas[i].X + areas[i].Width - x);
                    if (d == md)
                    {
                        x = areas[i].X + areas[i].Width;
                        makePlacable(ref x, ref y);
                        return;
                    }
                }
                if (y > areas[i].Y + areas[i].Height)
                {
                    d = Math.Abs(areas[i].Y + areas[i].Height - y);
                    if (d == md)
                    {
                        y = areas[i].Y + areas[i].Height;
                        makePlacable(ref x, ref y);
                        return;
                    }
                }
            }
        }

        public static void MakePlacable(ref int xc, ref int yc, int w, int h)
        {
            Rectangle r = new Rectangle(xc - w / 2, yc - h / 2, w - 8, h - 8);
            //Rectangle s = new Rectangle(xc - w / 2, yc - h / 2, w - 8, h - 8);
            //already inside
            for (int i = 0; i < areas.Count; i++)
            {
                if (areas[i].Contains(r)) return;
            }
            //x,y inside
            Point p = new Point(xc, yc);
            for (int i = 0; i < areas.Count; i++)
            {
                if (areas[i].Contains(p))
                {
                    r = areas[i];
                    if (r.Width < w || r.Height < h)//can't fit
                        continue;
                    if (r.X > xc - w / 2)//move right
                    {
                        xc = r.X + w / 2;
                        if ((w / 2) % 8 == 4)
                            xc -= 4;
                    }
                    if (r.Y > yc - h / 2)//move down
                    {
                        yc = r.Y + h / 2 - 4;
                        if ((h / 2) % 8 == 4)
                            yc += 4;
                    }
                    if (r.X + r.Width < xc + w / 2)//move left
                    {
                        xc = r.X + r.Width - w / 2;
                        if ((w / 2) % 8 == 4)
                            xc += 4;
                    }
                    if (r.Y + r.Height < yc + h / 2)//move up
                    {
                        yc = r.Y + r.Height - h / 2;
                        if ((h / 2) % 8 == 4)
                            yc += 4;
                    }
                }
            }
        }

        internal static void Save(ref SaveWriter sw)
        {
            String r = "";
            foreach (var rec in areas)
            {
                r = r + rec.X.ToString() + ";" + rec.Y.ToString() + ";" + rec.Width.ToString() + ";" + rec.Height.ToString() + "\r\n";
            }
            sw.WriteLine(r.Length.ToString());
            sw.WriteLine(r);
        }
    }
}
