using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MicroWorld.Components
{
    public class MovementDetectorLaser : Component, Properties.IUnselecrable, Properties.ICollidable
    {
        public enum Direction
        {
            Up = 0,
            Left = 1,
            Down = 2,
            Right = 3
        }

        public Direction type = Direction.Up;
        private int preCollisionLength = 0;
        public int PreCollisionLength
        {
            get { return preCollisionLength; }
        }
        private int length = 0;
        public int Length
        {
            get { return length; }
            set
            {
                if (value != 0)
                {
                }
                if (value < 0)
                    value = 0;
                if (preCollisionLength == length)
                {
                    preCollisionLength = value;
                    length = value;
                }
                else
                {
                    if (value < preCollisionLength)
                    {
                        length = value;
                        preCollisionLength = value;
                    }
                    else
                        preCollisionLength = value;
                }
                UpdateCollider();
            }
        }

        #region ICollidable
        Colliders.AABB collider;
        List<Colliders.CollisionInfo> collisions = new List<Colliders.CollisionInfo>();
        Colliders.ColliderGroup colgroup;

        public Colliders.ColliderGroup ColliderGroup
        {
            get
            {
                if (colgroup == null)
                    colgroup = Colliders.ColliderGroupManager.GetGroup("Laser");
                return colgroup;
            }
        }

        public void RegisterColliders()
        {
            if (collider == null)
                collider = new Colliders.AABB(0, 0, 1, 1, this);
            UpdateCollider();

            collider.onCollisionStarted += new Colliders.OnCollisionEvent(collider_onCollisionStarted);
            collider.onCollidionEnded += new Colliders.OnCollisionEvent(collider_onCollidionEnded);

            ComponentsManager.CollidersManager.AddCollider(collider);
        }

        void collider_onCollisionStarted(Colliders.CollisionInfo info)
        {
            collisions.Add(info);
            OnCollisionsChanged();
        }

        void collider_onCollidionEnded(Colliders.CollisionInfo info)
        {
            collisions.Remove(info);
            OnCollisionsChanged();
        }

        public void OnCollisionsChanged()
        {
            float closest;
            switch (type)
            {
                case MovementDetectorLaser.Direction.Up:
                    closest = Graphics.Position.Y - preCollisionLength;
                    for (int i = 0; i < collisions.Count; i++)
                    {
                        closest = Math.Max(closest, collisions[i].Intersection.Y2);
                    }
                    length = (int)(-closest + Graphics.Position.Y);
                    break;
                case MovementDetectorLaser.Direction.Left:
                    closest = Graphics.Position.X - preCollisionLength;
                    for (int i = 0; i < collisions.Count; i++)
                    {
                        closest = Math.Max(closest, collisions[i].Intersection.X2);
                    }
                    length = (int)(-closest + Graphics.Position.X);
                    break;
                case MovementDetectorLaser.Direction.Down:
                    closest = Graphics.Position.Y + preCollisionLength;
                    for (int i = 0; i < collisions.Count; i++)
                    {
                        closest = Math.Min(closest, collisions[i].Intersection.Y1);
                    }
                    length = (int)(closest - Graphics.Position.Y);
                    break;
                case MovementDetectorLaser.Direction.Right:
                    closest = Graphics.Position.X + preCollisionLength;
                    for (int i = 0; i < collisions.Count; i++)
                    {
                        closest = Math.Min(closest, collisions[i].Intersection.X1);
                    }
                    length = (int)(closest - Graphics.Position.X);
                    break;
                default:
                    break;
            }

            if (length < 1)
                length = 1;
        }

        public void UnRegisterColliders()
        {
            ComponentsManager.CollidersManager.RemoveCollider(collider);
        }

        public void UpdateCollider()
        {
            if (collider == null)
                collider = new Colliders.AABB(0, 0, 1, 1, this);

            switch (type)
            {
                case MovementDetectorLaser.Direction.Up:
                    collider.SetNew((int)Graphics.Position.X - 1, (int)Graphics.Position.Y - preCollisionLength, (int)Graphics.Position.X + 2, (int)Graphics.Position.Y);
                    break;
                case MovementDetectorLaser.Direction.Left:
                    collider.SetNew((int)Graphics.Position.X - preCollisionLength, (int)Graphics.Position.Y - 1, (int)Graphics.Position.X, (int)Graphics.Position.Y + 2);
                    break;
                case MovementDetectorLaser.Direction.Down:
                    collider.SetNew((int)Graphics.Position.X - 1, (int)Graphics.Position.Y, (int)Graphics.Position.X + 2, (int)Graphics.Position.Y + preCollisionLength);
                    break;
                case MovementDetectorLaser.Direction.Right:
                    collider.SetNew((int)Graphics.Position.X, (int)Graphics.Position.Y - 1, (int)Graphics.Position.X + preCollisionLength, (int)Graphics.Position.Y + 2);
                    break;
                default:
                    break;
            }
        }
        #endregion

        private void constructor()
        {
            Logics = new Logics.MovementDetectorLaserLogics();
            Graphics = new Graphics.MovementDetectorLaserGraphics();
            Graphics.parent = this;
            Logics.parent = this;
            ID = -100;

            ToolTip = null;
        }

        public MovementDetectorLaser()
        {
            constructor();
        }

        public MovementDetectorLaser(float x, float y)
        {
            constructor();
            Graphics.Position = new Vector2(x, y);
        }

        public override void AddComponentToManager()
        {
            ID = ComponentsManager.GetFreeID();
            base.AddComponentToManager();
        }

        public override void SetComponentOnVisibilityMap() { }

        public override void RemoveComponentFromVisibilityMap() { }

        public override bool CanDragDrop()
        {
            return false;
        }

        public override void Remove()
        {
            ComponentsManager.Replace(this, new EmptyComponent(ID));
            UnRegisterColliders();
            collisions.Clear();
            length = preCollisionLength;
        }

        public override void OnMove(int dx, int dy) { }

        public override string GetName()
        {
            return "MovementDetectorLaser";
        }

        public override void Reset()
        {
            length = preCollisionLength;
            collisions.Clear();

            base.Reset();
        }

        public override void NonGameUpdate()
        {
            base.NonGameUpdate();

            if (scheduleRemove)
                Remove();
        }

        //============================================================LOGICS========================================================
        
        //==============================================================IO==========================================================


        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);
        }

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);
        }

        bool scheduleRemove = false;
        public override void PostLoad()
        {
            base.PostLoad();
            scheduleRemove = true;
        }
    }
}
