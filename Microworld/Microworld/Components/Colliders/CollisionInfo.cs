using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Colliders
{
    public class CollisionInfo
    {
        internal Collider c1, c2;
        internal AABB intersection;



        public Collider Collider1
        {
            get { return c1; }
        }
        public Collider Collider2
        {
            get { return c2; }
        }

        public AABB Intersection
        {
            get { return intersection; }
        }



        public CollisionInfo(Collider c1, Collider c2, AABB intersection)
        {
            this.c1 = c1;
            this.c2 = c2;
            this.intersection = intersection;
        }
    }
}
