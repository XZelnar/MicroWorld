using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MicroWorld.Utilities
{
    public struct RectangleF
    {
        public float X, Y, Width, Height;

        public static explicit operator Rectangle(RectangleF r)
        {
            return new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
        }

        public RectangleF(float x, float y, float w, float h)
        {
            this.X = x;
            this.Y = y;
            this.Width = w;
            this.Height = h;
        }

        public RectangleF(RectangleF r)
        {
            X = r.X;
            Y = r.Y;
            Width = r.Width;
            Height = r.Height;
        }

        public RectangleF(Microsoft.Xna.Framework.Rectangle r)
        {
            X = r.X;
            Y = r.Y;
            Width = r.Width;
            Height = r.Height;
        }

        public bool Contains(Microsoft.Xna.Framework.Point p)
        {
            return p.X >= X && p.Y >= Y && p.X <= X + Width && p.Y <= Y + Height;
        }

        public bool Contains(int pX, int pY)
        {
            return pX >= X && pY >= Y && pX <= X + Width && pY <= Y + Height;
        }
    }
}
