using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MicroWorld.Components.Colliders
{
    public static class CollisionChecker
    {
        public static bool AreColliding(Collider c1, Collider c2, out AABB CollisionArea)
        {
            if (c1 is AABB)
            {
                if (c2 is AABB)
                    return AreColliding(c1 as AABB, c2 as AABB, out CollisionArea);
            }
            throw new NotImplementedException("Collision checker not registered for types \"" + c1.GetType().ToString() + "\" and \"" + c2.GetType().ToString() + "\"");
        }

        public static bool AreColliding(AABB c1, AABB c2, out AABB CollisionArea)
        {
            if (c1.HaveSameArea(c2))
            {
                CollisionArea = c1.GetSameArea(c2);
                return true;
            }
            CollisionArea = null;
            return false;
        }
    }
}
