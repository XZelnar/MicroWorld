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
    public class Blocker : Component, Properties.IDragDropPlacable, Properties.ICollidable, Properties.IResizable
    {
        internal RenderTarget2D fbo = null;
        private bool updateFBO = false;

        #region IDragDropPlacable
        public bool CanStart(int x, int y)
        {
            return Components.ComponentsManager.GetComponent(x + 4, y + 4) == null;
        }

        public void DrawGhost(MicroWorld.Graphics.Renderer renderer, int x1, int y1, int x2, int y2)
        {
            MicroWorld.Logics.GridHelper.GridCoords(ref x1, ref y1);
            MicroWorld.Logics.GridHelper.GridCoords(ref x2, ref y2);

            if (x1 > x2)
            {
                int t = x1;
                x1 = x2;
                x2 = t;
            }
            if (y1 > y2)
            {
                int t = y1;
                y1 = y2;
                y2 = t;
            }

            Graphics.Position = new Vector2(x1, y1);
            Graphics.Size = new Vector2(x2 - x1, y2 - y1);

            bool b = renderer.IsDrawing;
            if (b)
                renderer.End();
            renderer.Begin();
            if (CanEnd(x1, y1, x2, y2))
                renderer.Draw(MicroWorld.Graphics.GraphicsEngine.pixel, new Rectangle(x1, y1, x2 - x1, y2 - y1), Color.White * 0.3f);
            else
                renderer.Draw(MicroWorld.Graphics.GraphicsEngine.pixel, new Rectangle(x1, y1, x2 - x1, y2 - y1), Color.Red * 0.3f);
            renderer.End();

            (Graphics as Graphics.BlockerGraphics).DrawBlocker(x1, y1, x2, y2, renderer, 0.5f, false);

            if (b)
                renderer.Begin();
        }

        public bool CanEnd(int x1, int y1, int x2, int y2)
        {
            MicroWorld.Logics.GridHelper.GridCoords(ref x1, ref y1);
            MicroWorld.Logics.GridHelper.GridCoords(ref x2, ref y2);
            if (Math.Abs(x1 - x2) < 8 || Math.Abs(y1 - y2) < 8)
            {
                return false;
            }
            if (x2 < x1)
            {
                int t = x2;
                x2 = x1;
                x1 = t;
            }
            if (y2 < y1)
            {
                int t = y1;
                y1 = y2;
                y2 = t;
            }
            return ComponentsManager.VisibilityMap.IsFree(x1 + 4, y1 + 4, x2 - x1 - 7, y2 - y1 - 7);
        }

        public void Place(int x1, int y1, int x2, int y2)
        {
            MicroWorld.Logics.GridHelper.GridCoords(ref x1, ref y1);
            MicroWorld.Logics.GridHelper.GridCoords(ref x2, ref y2);
            if (x2 < x1)
            {
                int t = x2;
                x2 = x1;
                x1 = t;
            }
            if (y2 < y1)
            {
                int t = y1;
                y1 = y2;
                y2 = t;
            }
            Blocker rc = new Blocker(new Vector2(x1, y1), new Vector2(x2, y2));
            rc.Initialize();
            rc.AddComponentToManager();
            rc.OnPlaced();
            rc.fbo = new RenderTarget2D(Shortcuts.renderer.GraphicsDevice, (int)(x2 - x1) * 4, (int)(y2 - y1) * 4);
            (rc.Graphics as Graphics.BlockerGraphics).DrawToFBO(Shortcuts.renderer);
        }
        #endregion

        #region ICollidable
        Colliders.AABB collider;
        Colliders.ColliderGroup colgroup;

        public Colliders.ColliderGroup ColliderGroup 
        {
            get
            {
                if (colgroup == null)
                    colgroup = Colliders.ColliderGroupManager.GetGroup("Blocker");
                return colgroup;
            }
        }

        public void RegisterColliders()
        {
            collider = new Colliders.AABB(Graphics.Position, Graphics.Position + Graphics.Size, this);
            ComponentsManager.CollidersManager.AddCollider(collider);
        }

        public void UnRegisterColliders()
        {
            ComponentsManager.CollidersManager.RemoveCollider(collider);
        }
        #endregion

        #region IResizable
        public Direction CanResize(Vector2 pos)
        {
            Direction p = Direction.None;

            if (pos.X < Graphics.Position.X + 8)
                p |= Direction.Left;
            if (pos.Y < Graphics.Position.Y + 8)
                p |= Direction.Up;
            if (pos.X > Graphics.Position.X + Graphics.Size.X - 8)
                p |= Direction.Right;
            if (pos.Y > Graphics.Position.Y + Graphics.Size.Y - 8)
                p |= Direction.Down;

            return p;
        }

        public void OnResizeStart(Direction point, Vector2 clickPoint)
        {
            resizeDelta = new Vector2();
            resizeOrigPos = Graphics.Position;
            resizeOrigSize = Graphics.Size;
        }

        Vector2 resizeDelta = new Vector2();
        Vector2 resizeOrigPos = new Vector2();
        Vector2 resizeOrigSize = new Vector2();
        public void Resize(Direction point, float dx, float dy)
        {
            Vector2 pos = Graphics.Position;
            Vector2 size = Graphics.Size;

            resizeDelta += new Vector2(dx, dy);
            dx = (int)(resizeDelta.X / 8) * 8;
            dy = (int)(resizeDelta.Y / 8) * 8;

            if ((point & Direction.Down) > 0)//down grabbed
            {
                size.Y += dy;
                if (size.Y < 16)
                    size.Y = 16;
                if (!CanBeResizedTo(pos.X, pos.Y, size.X, size.Y))
                    size.Y = Graphics.Size.Y;
                resizeDelta.Y -= size.Y - Graphics.Size.Y;
            }
            if ((point & Direction.Right) > 0)//right grabbed
            {
                size.X += dx;
                if (size.X < 16)
                    size.X = 16;
                if (!CanBeResizedTo(pos.X, pos.Y, size.X, size.Y))
                    size.X = Graphics.Size.X;
                resizeDelta.X -= size.X - Graphics.Size.X;
            }
            if ((point & Direction.Up) > 0)//up grabbed
            {
                size.Y -= dy;
                if (size.Y < 16)
                    size.Y = 16;
                pos.Y -= size.Y - Graphics.Size.Y;
                if (!CanBeResizedTo(pos.X, pos.Y, size.X, size.Y))
                {
                    pos.Y = Graphics.Position.Y;
                    size.Y = Graphics.Size.Y;
                }
                resizeDelta.Y += size.Y - Graphics.Size.Y;
            }
            if ((point & Direction.Left) > 0)//left grabbed
            {
                size.X -= dx;
                if (size.X < 16)
                    size.X = 16;
                pos.X -= size.X - Graphics.Size.X;
                if (!CanBeResizedTo(pos.X, pos.Y, size.X, size.Y))
                {
                    pos.X = Graphics.Position.X;
                    size.X = Graphics.Size.X;
                }
                resizeDelta.X += size.X - Graphics.Size.X;
            }

            if (size.X > 1024)
                size.X = 1024;
            if (size.Y > 1024)
                size.Y = 1024;

            if (Graphics.Position != pos || Graphics.Size != size)
            {
                RemoveComponentFromVisibilityMap();
                Graphics.Position = pos;
                Graphics.Size = size;
                SetComponentOnVisibilityMap();
                fbo = new RenderTarget2D(Shortcuts.renderer.GraphicsDevice, (int)(size.X) * 4, (int)(size.Y) * 4);
                (Graphics as Graphics.BlockerGraphics).DrawToFBO(Shortcuts.renderer);
            }
        }

        private bool CanBeResizedTo(float x, float y, float w, float h)
        {
            return ComponentsManager.GetIntersectingComponents(x, y, w, h).Count <= 1;
        }

        public void OnResizeFinished(Direction point)
        {
            resizeDelta = new Vector2();
            resizeOrigPos = new Vector2();
            resizeOrigSize = new Vector2();
        }

        public void CancelResize()
        {
            Graphics.Position = resizeOrigPos;
            Graphics.Size = resizeOrigSize;
        }
        #endregion

        public override bool CanPlace(int x, int y, int w, int h)
        {
            return Components.ComponentsManager.VisibilityMap.IsFree(x + 4, y + 4, w - 7, h - 7) &&
                   MicroWorld.Logics.PlacableAreasManager.IsPlacable(x, y, w, h);
        }

        private void constructor()
        {
            Logics = new Logics.BlockerLogics();
            Graphics = new Graphics.BlockerGraphics();
            Graphics.parent = this;
            Logics.parent = this;
        }

        public Blocker()
        {
            constructor();
        }

        public Blocker(Vector2 p1, Vector2 p2)
        {
            constructor();
            Graphics.Position = new Vector2(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
            Graphics.Size = new Vector2(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y)) - Graphics.Position;
        }

        public override void AddComponentToManager()
        {
            ID = ComponentsManager.GetFreeID();
            base.AddComponentToManager();
        }

        public static void LoadContentStatic()
        {
            Components.Graphics.BlockerGraphics.LoadContentStatic();
        }

        public override void OnGraphicsDeviceReset()
        {
            updateFBO = true;
            base.OnGraphicsDeviceReset();
        }

        public override void NonGameUpdate()
        {
            base.NonGameUpdate();

            if (updateFBO)
            {
                fbo = new RenderTarget2D(Shortcuts.renderer.GraphicsDevice, (int)(Graphics.Size.X) * 4, (int)(Graphics.Size.Y) * 4);
                (Graphics as Graphics.BlockerGraphics).DrawToFBO(Shortcuts.renderer);
                updateFBO = false;
            }
        }

        public override void Remove()
        {
            if (!IsRemovable) return;

            base.Remove();
        }

        public override bool CanDragDrop()
        {
            return false;
        }

        public override void OnMove(int dx, int dy)
        {
            base.OnMove(dx, dy); 
        }

        public override string GetName()
        {
            return "Blocker";
        }

        //============================================================LOGICS========================================================

        
        //==============================================================IO==========================================================


        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);
        }

        List<int> contains = new List<int>();

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);
            updateFBO = true;
        }
    }
}
