using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MicroWorld.Logics
{
    unsafe class MathHelper
    {
        public unsafe static double[] Solve(Matrix a, Matrix b)
        {
            Triangulate(ref a, ref b);
            double[] r = new double[a.H];
            double[] tr = new double[a.H];

            for (int i = a.H - 1; i >= 0; i--)
            {
                double t = 0;
                for (int j = i + 1; j < a.W; j++)
                {
                    t += a[j, i] * tr[j];
                }
                r[i] = (b[0, i] - t) / a[i, i];
                tr[i] = r[i];
                if (Double.IsNaN(tr[i]))
                    tr[i] = 0;
                if (Double.IsInfinity(tr[i]))
                {
                    tr[i] = 0;
                    r[i] = Double.PositiveInfinity;
                }
            }

            return r;
        }

        public unsafe static void Triangulate(ref Matrix a, ref Matrix b)
        {
            for (int i = 0; i < a.H; i++)
                _triangulate(ref a, ref b, i);
        }

        private unsafe static void _triangulate(ref Matrix a, ref Matrix b, int startForm)
        {
            if (a.Values[startForm, startForm] == 0)
            {
                for (int i = startForm + 1; i < a.H; i++)
                    if (a[startForm, i] != 0)
                    {
                        a.SwapRows(startForm, i);
                        double t = b[0, startForm];
                    }

                if (a.Values[startForm, startForm] == 0)
                    return;
            }

            for (int i = startForm + 1; i < a.H; i++)
            {
                double t = -a[startForm, i] / a[startForm, startForm];
                a.AddRowMultipliedBy(i, startForm, t);
                b[0, i] += b[0, startForm] * t;
            }

        }

        #region DistanceFromPointToLine
        private static double sqr(double x) { return x * x; }
        private static double dist2(Vector2 v, Vector2 w) { return sqr(v.X - w.X) + sqr(v.Y - w.Y); }
        private static double distToSegmentSquared(Vector2 p, Vector2 v, Vector2 w)
        {
            var l2 = dist2(v, w);
            if (l2 == 0) return dist2(p, v);
            var t = ((p.X - v.X) * (w.X - v.X) + (p.Y - v.Y) * (w.Y - v.Y)) / l2;
            if (t < 0) return dist2(p, v);
            if (t > 1) return dist2(p, w);
            return dist2(p, new Vector2((float)(v.X + t * (w.X - v.X)),
                              (float)(v.Y + t * (w.Y - v.Y))));
        }
        public static double DistancePointToLineSegment(Vector2 p, Vector2 a1, Vector2 a2)
        {
            return Math.Sqrt(distToSegmentSquared(p, a1, a2));
        }
        #endregion

        #region LineIntersecteRectangle
        public static bool LineIntersectsRect(Vector2 pp1, Vector2 pp2, Rectangle r)
        {
            Point p1 = new Point((int)pp1.X, (int)pp1.Y);
            Point p2 = new Point((int)pp2.X, (int)pp2.Y);
            return LineIntersectsLine(p1, p2, new Point(r.X, r.Y), new Point(r.X + r.Width, r.Y)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y), new Point(r.X + r.Width, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y + r.Height), new Point(r.X, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X, r.Y + r.Height), new Point(r.X, r.Y)) ||
                   (r.Contains(p1) && r.Contains(p2));
        }

        private static bool LineIntersectsLine(Point l1p1, Point l1p2, Point l2p1, Point l2p2)
        {
            float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

            if (d == 0)
            {
                return false;
            }

            float r = q / d;

            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            float s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }
        #endregion


    }
}
