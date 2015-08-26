//#define PARALLEL//Parallel actually slower

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MicroWorld.Logics
{
    [Serializable]
    public unsafe class Matrix
    {
#if PARALLEL
        #region Parallel
        public double[,] Values = new double[0, 0];
        public int W, H;

        public unsafe double this[int i, int j]
        {
            get
            {
                return Values[i, j];
            }
            set
            {
                Values[i, j] = value;
            }
        }

        public unsafe Matrix(Matrix m)
        {
            Values = new double[m.W, m.H];
            W = m.W; H = m.H;
            Parallel.For(0, W, i =>
            {
                Parallel.For(0, H, j =>
                {
                    Values[i, j] = m.Values[i, j];
                });
            });
        }

        public unsafe Matrix(int w, int h)
        {
            Values = new double[w, h];
            W = w; H = h;
        }

        public unsafe Matrix(int w, int h, params double[] par)
        {
            Values = new double[w, h];
            W = w; H = h;

            Parallel.For(0, w, i =>
            {
                Parallel.For(0, h, j =>
                {
                    Values[i, j] = par[i + j * w];
                });
            });
        }

        public unsafe Matrix(String[] s)
        {
            int h = s.Length;
            int w = s[0].Split(' ').Length;
            W = w; H = h;

            Values = new double[w, h];
            Parallel.For(0, h, j =>
            {
                var a = s[j].Split(' ');
                Parallel.For(0, w, i =>
                {
                    Values[i, j] = Convert.ToDouble(a[i]);
                });
            });
        }

        public unsafe void Transpose()
        {
            double[,] t = new double[H, W];
            Parallel.For(0, W, i =>
            {
                Parallel.For(0, H, j =>
                {
                    t[j, i] = Values[i, j];
                });
            });
            Values = t;
            int a = W;
            W = H;
            H = a;
        }

        public unsafe static Matrix Mul(Matrix a, Matrix b)
        {
            int w = b.W; int h = a.H;
            Matrix r = new Matrix(w, h);

            Parallel.For(0, h, j =>
            {
                Parallel.For(0, w, i =>
                {
                    double t = 0;
                    for (int k = 0; k < a.W; k++)
                    {
                        t += a.Values[k, j] * b.Values[i, k];
                    }
                    r.Values[i, j] = t;
                });
            });

            return r;
        }

        public unsafe static Matrix Mul(Matrix a, double b)
        {
            Matrix r = new Matrix(a.W, a.H);

            Parallel.For(0, a.H, j =>
            {
                Parallel.For(0, a.W, i =>
                {
                    r.Values[i, j] = a.Values[i, j] * b;
                });
            });


            return r;
        }

        public unsafe static Matrix Dec(Matrix a, Matrix b)
        {
            Matrix r = new Matrix(a.W, a.H);
            Parallel.For(0, a.W, i =>
            {
                Parallel.For(0, a.H, j =>
                {
                    r.Values[i, j] = a.Values[i, j] - b.Values[i, j];
                });
            });
            return r;
        }

        public unsafe static Matrix Plus(Matrix a, Matrix b)
        {
            Matrix r = new Matrix(a.W, a.H);
            Parallel.For(0, a.W, i =>
            {
                Parallel.For(0, a.H, j =>
                {
                    r.Values[i, j] = a.Values[i, j] + b.Values[i, j];
                });
            });
            return r;
        }

        public unsafe void SwapRows(int r1, int r2)
        {
            double t;
            for (int i = 0; i < W; i++)
            {
                t = Values[i, r1];
                Values[i, r1] = Values[i, r2];
                Values[i, r2] = t;
            }
        }

        public unsafe void AddRowMultipliedBy(int addTo, int addFrom, double mul)
        {
            for (int i = 0; i < W; i++)
            {
                Values[i, addTo] += Values[i, addFrom] * mul;
            }
        }

        public unsafe new String ToString()
        {
            String s = "";
            for (int j = 0; j < H; j++)
            {
                for (int i = 0; i < W; i++)
                {
                    s += Values[i, j].ToString() + "    ";
                }
                s += "\r\n";
            }
            return s;
        }
        #endregion
#else
        #region Regular
        public double[,] Values = new double[0, 0];
        public int W, H;

        public unsafe double this[int i, int j]
        {
            get { return Values[i, j]; }
            set { Values[i, j] = value; }
        }

        public unsafe Matrix(Matrix m)
        {
            Values = new double[m.W, m.H];
            W = m.W; H = m.H;

            for(int i = 0; i < W; i++)
                for(int j = 0; j < H; j++)
                    Values[i, j] = m.Values[i, j];
        }

        public unsafe Matrix(int w, int h)
        {
            Values = new double[w, h];
            W = w; H = h;
        }

        public unsafe Matrix(int w, int h, params double[] par)
        {
            Values = new double[w, h];
            W = w; H = h;

            for(int i = 0; i < w; i++)
                for(int j = 0; j < h; j++)
                    Values[i, j] = par[i + j * w];
        }

        public unsafe Matrix(String[] s)
        {
            int h = s.Length;
            int w = s[0].Split(' ').Length;
            W = w; H = h;

            Values = new double[w, h];
            for(int j = 0; j < h; j++)
            {
                var a = s[j].Split(' ');
                for(int i = 0; i < w; i++)
                    Values[i, j] = Convert.ToDouble(a[i]);
            }
        }

        public unsafe void Transpose()
        {
            double[,] t = new double[H, W];
            for(int i = 0; i < W; i++)
                for(int j = 0; j < H; j++)
                    t[j, i] = Values[i, j];
            Values = t;
            int a = W;
            W = H;
            H = a;
        }

        public unsafe void TransposeNormalize()
        {
            double[,] t = new double[H, W];
            for (int i = 0; i < W; i++)
                for (int j = 0; j < H; j++)
                    t[j, i] = Math.Sign(Values[i, j]);//TODO not sign
            Values = t;
            int a = W;
            W = H;
            H = a;
        }

        public unsafe static Matrix Mul(Matrix a, Matrix b)
        {
            int w = b.W; int h = a.H;
            Matrix r = new Matrix(w, h);
            double t;
            int i, j, k;

            fixed (double* ma = a.Values, mb = b.Values, mr = r.Values)
                for (j = 0; j < h; j++)
                    for (i = 0; i < w; i++)
                    {
                        t = 0;
                        for (k = 0; k < a.W; k++)
                            t += *(ma + k * a.H + j) * *(mb + i * b.H + k);
                        *(mr + i * r.H + j) = t;
                    }

            return r;
        }

        internal unsafe static Matrix MulDiag(Matrix a, Matrix b)
        {
            int w = b.W; int h = a.H;
            Matrix r = new Matrix(w, h);
            int i, j;

            fixed (double* ma = a.Values, mb = b.Values, mr = r.Values)
                for (j = 0; j < h; j++)
                    for (i = 0; i < w; i++)
                        *(mr + i * r.H + j) = *(ma + i * a.H + j) * *(mb + i * b.H + i);

            return r;
        }

        internal unsafe static Matrix MulByVertical(Matrix a, Matrix b)
        {
            int h = a.H;
            Matrix r = new Matrix(1, h);
            double t;
            int j, k;

            fixed (double* ma = a.Values, mb = b.Values, mr = r.Values)
                for (j = 0; j < h; j++)
                    {
                        t = 0;
                        for (k = 0; k < a.W; k++)
                            t += *(ma + k * a.H + j) * *(mb + k);
                        *(mr + j) = t;
                    }

            return r;
        }

        public unsafe static Matrix Mul(Matrix a, double b)
        {
            Matrix r = new Matrix(a.W, a.H);

            for(int j = 0; j < a.H; j++)
                for(int i = 0; i < a.W; i++)
                    r.Values[i, j] = a.Values[i, j] * b;


            return r;
        }

        public unsafe static Matrix Dec(Matrix a, Matrix b)
        {
            Matrix r = new Matrix(a.W, a.H);
            for(int i = 0; i < a.W; i++)
                for(int j = 0; j < a.H; j++)
                    r.Values[i, j] = a.Values[i, j] - b.Values[i, j];

            return r;
        }

        public unsafe static Matrix Plus(Matrix a, Matrix b)
        {
            Matrix r = new Matrix(a.W, a.H);
            for(int i = 0; i < a.W; i++)
                for(int j = 0; j < a.H; j++)
                    r.Values[i, j] = a.Values[i, j] + b.Values[i, j];

            return r;
        }

        public unsafe void SwapRows(int r1, int r2)
        {
            double t;
            for (int i = 0; i < W; i++)
            {
                t = Values[i, r1];
                Values[i, r1] = Values[i, r2];
                Values[i, r2] = t;
            }
        }

        public unsafe void AddRowMultipliedBy(int addTo, int addFrom, double mul)
        {
            fixed (double* d = Values)
            {
                for (int i = 0; i < W; i++)
                    *(d + i * H + addTo) += *(d + i * H + addFrom) * mul;
                    //Values[i, addTo] += Values[i, addFrom] * mul;
            }
        }

        public unsafe new String ToString()
        {
            String s = "";
            for (int j = 0; j < H; j++)
            {
                for (int i = 0; i < W; i++)
                    s += Values[i, j].ToString() + "    ";
                s += "\r\n";
            }
            return s;
        }
        #endregion
#endif
    }
}
