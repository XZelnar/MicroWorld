using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MicroWorld.Components.Colliders
{
    public delegate void OnCollisionEvent(CollisionInfo info);

    public class CollidersManager
    {
        internal List<Collider> colliders = new List<Collider>();
        private List<CollisionInfo> collisions = new List<CollisionInfo>();

        #region Events
        public event OnCollisionEvent onCollisionStarted;
        public event OnCollisionEvent onCollidionEnded;
        #endregion



        public CollidersManager() { }

        public CollidersManager(CollidersManager m)
        {
            colliders.AddRange(m.colliders);
            collisions.AddRange(m.collisions);
        }

        public void AddCollider(Collider c)
        {
            if (!colliders.Contains(c))
                colliders.Add(c);
        }

        public void RemoveCollider(Collider c)
        {
            for (int i = 0; i < collisions.Count; i++)
            {
                if (collisions[i].c1 == c)
                {
                    collisions[i].c2.InvokeCollisionEnded(collisions[i]);
                    if (onCollidionEnded != null)
                        onCollidionEnded.Invoke(collisions[i]);
                    collisions.RemoveAt(i);
                    i--;
                    continue;
                }
                if (collisions[i].c2 == c)
                {
                    collisions[i].c1.InvokeCollisionEnded(collisions[i]);
                    if (onCollidionEnded != null)
                        onCollidionEnded.Invoke(collisions[i]);
                    collisions.RemoveAt(i);
                    i--;
                    continue;
                }
            }
            colliders.Remove(c);
        }


        internal void Clear()
        {
            colliders.Clear();
            collisions.Clear();
        }

        internal void Reset()
        {
            collisions.Clear();
        }

        public void Update()
        {
            AABB a;
            for (int i = 0; i < collisions.Count; i++)
            {
                if (CollisionChecker.AreColliding(collisions[i].c1, collisions[i].c2, out a))
                {
                    collisions[i].c1.InvokeColliding(collisions[i]);
                    collisions[i].c2.InvokeColliding(collisions[i]);
                    collisions[i].intersection = a;
                }
                else
                {
                    collisions[i].c1.InvokeCollisionEnded(collisions[i]);
                    collisions[i].c2.InvokeCollisionEnded(collisions[i]);
                    if (onCollidionEnded != null)
                        onCollidionEnded.Invoke(collisions[i]);

                    collisions.RemoveAt(i);
                    i--;
                }
            }
        }

        public void OnColliderMoved(Collider c)
        {
            AABB a;
            for (int i = 0; i < colliders.Count; i++)
            {
                if (colliders[i] != c && ColliderGroupManager.CanCollide(c.Group, colliders[i].Group) && c.ShouldCheckForCollision(colliders[i]))
                {
                    if (CollisionChecker.AreColliding(c, colliders[i], out a))
                    {
                        RegisterCollision(c, colliders[i], a);
                    }
                }
            }
        }

        private CollisionInfo RegisterCollision(Collider c1, Collider c2, AABB intersection)
        {
            for (int i = 0; i < collisions.Count; i++)
            {
                if ((collisions[i].c1 == c1 && collisions[i].c2 == c2) || (collisions[i].c1 == c2 && collisions[i].c2 == c1))
                {
                    return collisions[i];
                }
            }
            CollisionInfo c = new CollisionInfo(c1, c2, intersection);
            collisions.Add(c);

            c1.InvokeCollisionStarted(c);
            c2.InvokeCollisionStarted(c);
            if (onCollisionStarted != null)
                onCollisionStarted.Invoke(c);

            return c;
        }

        internal List<Collider> PushColliders()
        {
            var a = new List<Collider>();
            a.AddRange(colliders);
            return a;
        }

        internal void PopColliders(List<Collider> l)
        {
            collisions.Clear();
            colliders.Clear();
            colliders = l;
        }

    }
}
