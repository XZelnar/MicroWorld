using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MicroWorld.Components.Colliders
{
    public abstract class Collider
    {
        internal Components.Component parent;
        internal ColliderGroup group;


        public virtual Components.Component Parent
        {
            get { return parent; }
        }
        public virtual Vector2 Size
        {
            get { return new Vector2(); }
        }
        public virtual ColliderGroup Group
        {
            get
            {
                if (!(parent is Properties.ICollidable))
                    return ColliderGroupManager.GetGroup("None");
                return (parent as Properties.ICollidable).ColliderGroup;
            }
        }

        public abstract Vector2 Center { get; }



        #region Events
        public event OnCollisionEvent onCollisionStarted;
        public event OnCollisionEvent onColliding;
        public event OnCollisionEvent onCollidionEnded;

        internal void InvokeCollisionStarted(CollisionInfo c)
        {
            if (onCollisionStarted != null)
                onCollisionStarted.Invoke(c);
        }

        internal void InvokeColliding(CollisionInfo c)
        {
            if (onColliding != null)
                onColliding.Invoke(c);
        }

        internal void InvokeCollisionEnded(CollisionInfo c)
        {
            if (onCollidionEnded != null)
                onCollidionEnded.Invoke(c);
        }
        #endregion



        public virtual double DistanceToApprox(Collider c)
        {
            return Math.Abs(c.Center.X - Center.X) + Math.Abs(c.Center.Y - Center.Y);
        }

        public virtual double DistanceTo(Collider c)
        {
            return Math.Sqrt(Math.Pow(c.Center.X - Center.X, 2) + Math.Pow(c.Center.Y - Center.Y, 2));
        }

        public virtual bool ShouldCheckForCollision(Collider c)
        {
            return (Size.X + Size.Y + c.Size.X + c.Size.Y) / 2 >= DistanceToApprox(c);
        }

        public virtual AABB GetAABB()
        {
            return new AABB(Center - Size / 2, Center + Size / 2, parent);
        }
    }
}
