using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MicroWorld.Components.Colliders
{
    public class AABB : Collider
    {
        private float x1, y1, x2, y2;

        public float X1
        {
            get { return x1; }
            set
            {
                x1 = value;
                SortCoords();
            }
        }
        public float Y1
        {
            get { return y1; }
            set { y1 = value; }
        }
        public float X2
        {
            get { return x2; }
            set
            {
                x2 = value;
                SortCoords();
            }
        }
        public float Y2
        {
            get { return y2; }
            set
            {
                y2 = value;
                SortCoords();
            }
        }

        private void SortCoords()
        {
            if (x1 > x2)
            {
                float t = x1;
                x1 = x2;
                x2 = t;
            }
            if (y1 > y2)
            {
                float t = y1;
                y1 = y2;
                y2 = t;
            }
        }

        public Vector2 Min
        {
            get { return new Vector2(x1, y1); }
        }
        public Vector2 Max
        {
            get { return new Vector2(x2, y2); }
        }
        public override Vector2 Size
        {
            get { return new Vector2(x2 - x1, y2 - y1); }
        }
        public override Vector2 Center
        {
            get { return new Vector2((x1 + x2) / 2, (y1 + y2) / 2); }
        }


        public static explicit operator Rectangle(AABB a)
        {
            return new Rectangle((int)a.x1, (int)a.y1, (int)(a.x2 - a.x1), (int)(a.y2 - a.y1));
        }


        public AABB(Vector2 a, Vector2 b, Components.Component p)
        {
            this.x1 = a.X;
            this.y1 = a.Y;
            this.x2 = b.X;
            this.y2 = b.Y;
            parent = p;
            SortCoords();
        }

        public AABB(float x1, float y1, float x2, float y2, Components.Component p)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
            parent = p;
            SortCoords();
        }

        public bool Contains(AABB a)
        {
            return a.x1 > x1 && a.y1 > y1 && x2 > a.x2 && y2 > a.y2;
        }

        public bool Intersects(AABB a)
        {
            return (a.x1 <= x1 && a.x2 >= x1 && a.y1 >= y1 && a.y1 <= y2) || //a intersects left with top
                   (a.x1 <= x1 && a.x2 >= x1 && a.y2 >= y1 && a.y2 <= y2) || //a intersects left with bottom
                   (a.x1 <= x2 && a.x2 >= x2 && a.y1 >= y1 && a.y1 <= y2) || //a intersects right with top
                   (a.x1 <= x2 && a.x2 >= x2 && a.y2 >= y1 && a.y2 <= y2) || //a intersects right with bottom
                   (a.y1 <= y1 && a.y2 >= y1 && a.x1 >= x1 && a.x1 <= x2) || //a intersects top with left
                   (a.y1 <= y1 && a.y2 >= y1 && a.x2 >= x1 && a.x2 <= x2) || //a intersects top with right
                   (a.y1 <= y2 && a.y2 >= y2 && a.x1 >= x1 && a.x1 <= x2) || //a intersects bottom with left
                   (a.y1 <= y2 && a.y2 >= y2 && a.x2 >= x1 && a.x2 <= x2);   //a intersects bottom with right
        }

        public bool HaveSameArea(AABB a)
        {
            return Contains(a) || a.Contains(this) || Intersects(a);
        }

        public AABB GetSameArea(AABB a)
        {
            if (Contains(a))
                return a;
            if (a.Contains(this))
                return this;
            return GetIntersectionArea(a);
        }

        public AABB GetIntersectionArea(AABB a)
        {
            return new AABB(Math.Max(x1, a.x1), Math.Max(y1, a.y1), Math.Min(x2, a.x2), Math.Min(y1, a.y2), parent);
        }

        public void Move(float dx, float dy)
        {
            x1 += dx;
            x2 += dx;
            y1 += dy;
            y2 += dy;

            ComponentsManager.CollidersManager.OnColliderMoved(this);
        }

        public void SetNew(float x1, float y1, float x2, float y2)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
            SortCoords();

            ComponentsManager.CollidersManager.OnColliderMoved(this);
        }

        public override AABB GetAABB()
        {
            return this;
        }
    }
}
