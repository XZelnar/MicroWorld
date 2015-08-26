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
    public class Frame : Component, Properties.IDragDropPlacable, Properties.IContainer, Properties.IMovable, Properties.ICollidable, Properties.IResizable
    {
        internal Vector2? origPos = null;
        internal RenderTarget2D fbo = null;

        #region IDragDropPlacable
        public bool CanStart(int x, int y)
        {
            var a = Components.ComponentsManager.GetComponent(x, y);
            return Components.ComponentsManager.GetComponent(x, y) == null;
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

            (Graphics as Graphics.FrameGraphics).DrawFrame(x1, y1, x2, y2, renderer, 0.5f, false);

            if (b)
                renderer.Begin();
        }

        public bool CanEnd(int x1, int y1, int x2, int y2)
        {
            MicroWorld.Logics.GridHelper.GridCoords(ref x1, ref y1);
            MicroWorld.Logics.GridHelper.GridCoords(ref x2, ref y2);
            if (Math.Abs(x1 - x2) < 32 || Math.Abs(y1 - y2) < 32)
            {
                return false;
            }
            int sx = x2 - x1;
            int sy = y2 - y1;
            return ComponentsManager.VisibilityMap.IsFree(new Rectangle(x1, y1, 8, sy)) &&
                   ComponentsManager.VisibilityMap.IsFree(new Rectangle(x1, y1, sx, 8)) &&
                   ComponentsManager.VisibilityMap.IsFree(new Rectangle(x2 - 8, y1, 8, sy)) &&
                   ComponentsManager.VisibilityMap.IsFree(new Rectangle(x1, y2 - 8, sx, 8));
        }

        public void Place(int x1, int y1, int x2, int y2)
        {
            MicroWorld.Logics.GridHelper.GridCoords(ref x1, ref y1);
            MicroWorld.Logics.GridHelper.GridCoords(ref x2, ref y2);
            Frame rc = new Frame(new Vector2(x1, y1), new Vector2(x2, y2));
            rc.Initialize();
            rc.AddComponentToManager();
            rc.OnPlaced();
        }
        #endregion
        
        #region IMovable
        List<Object> PushedPositions = new List<object>();
        List<Component> components = new List<Component>();
        Vector2 curTickMovement = new Vector2();
        Vector2 MaxMovement = new Vector2(1f, 1f);
        public void Move(Vector2 delta)
        {
            if (delta.X == 0 && delta.Y == 0)
                return;

            if (curTickMovement.X + delta.X > MaxMovement.X)
            {
                delta.X = MaxMovement.X - curTickMovement.X;
                if (delta.X < 0)
                    delta.X = 0;
            }
            if (curTickMovement.Y + delta.Y > MaxMovement.Y)
            {
                delta.Y = MaxMovement.Y - curTickMovement.Y;
                if (delta.Y < 0)
                    delta.Y = 0;
            }

            if (curTickMovement.X + delta.X < -MaxMovement.X)
            {
                delta.X = -MaxMovement.X - curTickMovement.X;
                if (delta.X < 0)
                    delta.X = 0;
            }
            if (curTickMovement.Y + delta.Y < -MaxMovement.Y)
            {
                delta.Y = -MaxMovement.Y - curTickMovement.Y;
                if (delta.Y < 0)
                    delta.Y = 0;
            }

            curTickMovement += delta;
            
            Graphics.Position += delta;
            Graphics.Position = new Vector2((float)Math.Round(Graphics.Position.X, 2), (float)Math.Round(Graphics.Position.Y, 2));
            collider.SetNew(Graphics.Position.X + 8, Graphics.Position.Y + 8, Graphics.Position.X + Graphics.Size.X - 8, Graphics.Position.Y + Graphics.Size.Y - 8);
            for (int i = 0; i < components.Count; i++)
            {
                components[i].Graphics.MoveVisually(delta);
            }
        }

        public void ResetPosition()
        {
            if (origPos.HasValue)
            {
                Graphics.Position = origPos.Value;
                collider.SetNew(Graphics.Position.X + 8, Graphics.Position.Y + 8, Graphics.Position.X + Graphics.Size.X - 8, Graphics.Position.Y + Graphics.Size.Y - 8);
                origPos = null;
            }

            if (components.Count == PushedPositions.Count)
                for (int i = 0; i < components.Count; i++)
                {
                    components[i].Graphics.PopPosition(PushedPositions[i]);
                }
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
                    colgroup = Colliders.ColliderGroupManager.GetGroup("Frame");
                return colgroup;
            }
        }

        public void RegisterColliders()
        {
            collider = new Colliders.AABB(Graphics.Position + new Vector2(8, 8), Graphics.Position + Graphics.Size - new Vector2(8, 8), this);
            collider.onCollisionStarted += new Colliders.OnCollisionEvent(collider_onCollisionStarted);
            ComponentsManager.CollidersManager.AddCollider(collider);
        }

        void collider_onCollisionStarted(Colliders.CollisionInfo info)
        {
            if (info.Collider1 == collider && info.Collider2.Group.Name == "Blocker")
            {
                Move(-curTickMovement);
            }
            if (info.Collider2 == collider && info.Collider1.Group.Name == "Blocker")
            {
                Move(-curTickMovement);
            }
        }

        public void UnRegisterColliders()
        {
            ComponentsManager.CollidersManager.RemoveCollider(collider);
        }
        #endregion

        #region IResizable
        public Direction CanResize(Vector2 pos)
        {
            if (Settings.GameState == Settings.GameStates.Running)
                return Direction.None;

            Direction p = Direction.None;

            if (pos.X < Graphics.Position.X + 12)
                p |= Direction.Left;
            if (pos.Y < Graphics.Position.Y + 12)
                p |= Direction.Up;
            if (pos.X > Graphics.Position.X + Graphics.Size.X - 12)
                p |= Direction.Right;
            if (pos.Y > Graphics.Position.Y + Graphics.Size.Y - 12)
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
                if (size.Y < 24)
                    size.Y = 24;
                if (!CanBeResizedTo(pos.X, pos.Y, size.X, size.Y))
                    size.Y = Graphics.Size.Y;
                resizeDelta.Y -= size.Y - Graphics.Size.Y;
            }
            if ((point & Direction.Right) > 0)//right grabbed
            {
                size.X += dx;
                if (size.X < 24)
                    size.X = 24;
                if (!CanBeResizedTo(pos.X, pos.Y, size.X, size.Y))
                    size.X = Graphics.Size.X;
                resizeDelta.X -= size.X - Graphics.Size.X;
            }
            if ((point & Direction.Up) > 0)//up grabbed
            {
                size.Y -= dy;
                if (size.Y < 24)
                    size.Y = 24;
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
                if (size.X < 24)
                    size.X = 24;
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
                UpdateComponentsList();
            }
        }

        private bool CanBeResizedTo(float x, float y, float w, float h)
        {
            var a = ComponentsManager.GetIntersectingComponents(x, y, 8, h);
            a.Remove(this);
            if (a.Count > 0) return false;

            a = ComponentsManager.GetIntersectingComponents(x, y, w, 8);
            a.Remove(this);
            if (a.Count > 0) return false;

            a = ComponentsManager.GetIntersectingComponents(x + w - 8, y, 8, h);
            a.Remove(this);
            if (a.Count > 0) return false;

            a = ComponentsManager.GetIntersectingComponents(x, y + h - 8, w, 8);
            a.Remove(this);
            if (a.Count > 0) return false;

            return true;
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

        #region Frame
        internal Component[] containsComponents = new Component[0];

        public void UpdateComponentsList()
        {
            var a = ComponentsManager.GetComponents((int)Graphics.Position.X, (int)Graphics.Position.Y, (int)Graphics.Size.X, (int)Graphics.Size.Y);
            a.Remove(this);
            containsComponents = a.ToArray();
            Component t;
            bool c;
            for (int i = 0; i < containsComponents.Length; i++)
            {
                c = false;
                for (int j = i + 1; j < containsComponents.Length; j++)
                {
                    if (containsComponents[i].Graphics.Layer > containsComponents[j].Graphics.Layer)
                    {
                        c = true;
                        t = containsComponents[i];
                        containsComponents[i] = containsComponents[j];
                        containsComponents[j] = t;
                    }
                }
                if (!c)
                    break;
            }
        }
        #endregion

        private void constructor()
        {
            Logics = new Logics.FrameLogics();
            Graphics = new Graphics.FrameGraphics();
            Graphics.parent = this;
            Logics.parent = this;

            GlobalEvents.onComponentMoved += new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentMoved);
            GlobalEvents.onComponentPlacedByPlayer += new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentPlacedByPlayer);
            GlobalEvents.onComponentRemovedByPlayer += new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentRemovedByPlayer);

            //Graphics.Size = new Vector2(32, 32);
            //ToolTip = new GUI.LEDToolTip();
        }

        void GlobalEvents_onComponentRemovedByPlayer(Component sender)
        {
            UpdateComponentsList();
        }

        void GlobalEvents_onComponentPlacedByPlayer(Component sender)
        {
            UpdateComponentsList();
        }

        void GlobalEvents_onComponentMoved(Component sender)
        {
            UpdateComponentsList();
        }

        public Frame()
        {
            constructor();
        }

        public Frame(Vector2 p1, Vector2 p2)
        {
            constructor();
            Graphics.Position = new Vector2(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
            Graphics.Size = new Vector2(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y)) - Graphics.Position;
        }

        public override void SetComponentOnVisibilityMap()
        {
            if (!Graphics.Visible) return;
            var Position = origPos == null ? Graphics.Position : origPos.Value;
            ComponentsManager.VisibilityMap.SetRectangle(this, Position.X + 4, Position.Y + 4, Graphics.Size.X - 8, 1);
            ComponentsManager.VisibilityMap.SetRectangle(this, Position.X + 4, Position.Y + 4, 1, Graphics.Size.Y - 8);
            ComponentsManager.VisibilityMap.SetRectangle(this, Position.X + 4, Position.Y + Graphics.Size.Y - 4, Graphics.Size.X - 8, 1);
            ComponentsManager.VisibilityMap.SetRectangle(this, Position.X + Graphics.Size.X - 4, Position.Y + 4, 1, Graphics.Size.Y - 8);
        }

        public override void RemoveComponentFromVisibilityMap()
        {
            if (!Graphics.Visible) return;
            var Position = origPos == null ? Graphics.Position : origPos.Value;
            ComponentsManager.VisibilityMap.SetRectangle(null, Position.X + 4, Position.Y + 4, Graphics.Size.X - 8, 1);
            ComponentsManager.VisibilityMap.SetRectangle(null, Position.X + 4, Position.Y + 4, 1, Graphics.Size.Y - 8);
            ComponentsManager.VisibilityMap.SetRectangle(null, Position.X + 4, Position.Y + Graphics.Size.Y - 4, Graphics.Size.X - 8, 1);
            ComponentsManager.VisibilityMap.SetRectangle(null, Position.X + Graphics.Size.X - 4, Position.Y + 4, 1, Graphics.Size.Y - 8);
        }

        public override void Update()
        {
            base.Update();
            curTickMovement.X = 0;
            curTickMovement.Y = 0;
        }

        public override void InitAddChildComponents()
        {
        }

        public override void AddComponentToManager()
        {
            ID = ComponentsManager.GetFreeID();
            base.AddComponentToManager();
        }

        public static void LoadContentStatic()
        {
            Components.Graphics.FrameGraphics.LoadContentStatic();
        }

        public override void Remove()
        {
            if (!IsRemovable) return;

            GlobalEvents.onComponentMoved -= new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentMoved);
            GlobalEvents.onComponentPlacedByPlayer -= new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentPlacedByPlayer);
            GlobalEvents.onComponentRemovedByPlayer -= new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentRemovedByPlayer);

            base.Remove();
        }

        public override bool CanDragDrop()
        {
            return false;
        }

        public override string GetName()
        {
            return "RotatableConnector";
        }

        bool ReDrawFBO = false;
        public override void NonGameUpdate()
        {
            if (ReDrawFBO)
            {
                ReDrawFBO = false;

                if (fbo == null || fbo.Width != Graphics.Size.X * 4 || fbo.Height != Graphics.Size.Y * 4)
                {
                    if (fbo != null)
                        fbo.Dispose();
                    fbo = new RenderTarget2D(MicroWorld.Graphics.GraphicsEngine.Renderer.GraphicsDevice, (int)Graphics.Size.X * 4, (int)Graphics.Size.Y * 4);
                }
                (Graphics as Graphics.FrameGraphics).DrawToFBO(Shortcuts.renderer);
            }
            base.NonGameUpdate();
        }

        //============================================================LOGICS========================================================

        public override void Reset()
        {
            base.Reset();
            ResetPosition();
        }

        bool ignoreNextPosSave = false;
        public override void Start()
        {
            if (!ignoreNextPosSave)
            {
                origPos = Graphics.Position;

                components = ComponentsManager.GetComponents((int)Graphics.Position.X, (int)Graphics.Position.Y, (int)Graphics.Size.X, (int)Graphics.Size.Y);
                containsComponents = new Component[components.Count - 1];
                PushedPositions.Clear();
                for (int i = 0; i < components.Count; i++)
                {
                    if (components[i] == this)
                    {
                        components.RemoveAt(i);
                        i--;
                        continue;
                    }
                    PushedPositions.Add(components[i].Graphics.PushPosition());
                    containsComponents[i] = components[i];
                }
            }
            ignoreNextPosSave = false;

            ReDrawFBO = true;

            base.Start();
        }
        
        //==============================================================IO==========================================================


        List<object> tppushed = new List<object>();
        public override void PreSave()
        {
            /*
            if (components.Count == PushedPositions.Count)
                for (int i = 0; i < components.Count; i++)
                {
                    tppushed.Add(components[i].Graphics.PushPosition());
                    components[i].Graphics.PopPosition(PushedPositions[i]);
                }//*/

            base.PreSave();
        }

        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);

            Compound.Add("OrigPos", Settings.GameState == Settings.GameStates.Stopped ? new Vector2(-1, -1) : origPos.Value);
            for (int i = 0; i < PushedPositions.Count; i++)
            {
                if (PushedPositions[i] is Vector2)
                {
                    Compound.Add("Component" + i.ToString(), (Vector2)PushedPositions[i]);
                }
                if (PushedPositions[i] is List<Vector2>)
                {
                    Compound.Add("Component" + i.ToString(), "list");
                    List<Vector2> t = (List<Vector2>)PushedPositions[i];
                    for (int j = 0; j < t.Count; j++)
                    {
                        Compound.Add("Component" + i.ToString() + j.ToString(), (Vector2)t[j]);
                    }
                }
            }

            for (int i = 0; i < components.Count; i++)
            {
                Compound.Add("Contains" + i.ToString(), components[i].ID);
            }
        }

        public override void PostSave()
        {
            /*
            if (components.Count == tppushed.Count)
                for (int i = 0; i < components.Count; i++)
                {
                    components[i].Graphics.PopPosition(tppushed[i]);
                }
            tppushed.Clear();//*/

            base.PostSave();
        }

        List<int> contains = new List<int>();

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            var o = Compound.GetVector2("OrigPos");
            if (o.X == -1 && o.Y == -1)
                origPos = null;
            else
                origPos = o;

            PushedPositions.Clear();

            int i = 0;
            while (true)
            {
                if (Compound.Contains("Component" + i.ToString()))
                {
                    if (Compound.GetString("Component" + i.ToString()) == "list")
                    {
                        int j = 0;
                        List<Vector2> p = new List<Vector2>();
                        while (true)
                        {
                            if (Compound.Contains("Component" + i.ToString() + j.ToString()))
                            {
                                p.Add(Compound.GetVector2("Component" + i.ToString() + j.ToString()));
                            }
                            else
                            {
                                break;
                            }
                            j++;
                        }
                        PushedPositions.Add(p);
                    }
                    else
                    {
                        PushedPositions.Add(Compound.GetVector2("Component" + i.ToString()));
                    }
                }
                else
                {
                    break;
                }
                i++;
            }

            i = 0;
            contains.Clear();
            while (true)
            {
                if (Compound.Contains("Contains" + i.ToString()))
                {
                    contains.Add(Compound.GetInt("Contains" + i.ToString()));
                }
                else
                {
                    break;
                }
                i++;
            }

            if (origPos != null)
                ignoreNextPosSave = true;
        }

        public override void PostLoad()
        {
            base.PostLoad();

            containsComponents = new Component[contains.Count];
            components.Clear();
            for (int i = 0; i < contains.Count; i++)
            {
                containsComponents[i] = ComponentsManager.GetComponent(contains[i]);
                components.Add(containsComponents[i]);

                if (origPos != null)
                {
                    tppushed.Add(components[i].Graphics.PushPosition());
                    components[i].Graphics.PopPosition(PushedPositions[i]);
                }
            }
            contains.Clear();

            RegisterColliders();
        }

        public override void PostPostLoad()
        {
            if (origPos != null)
            {
                for (int i = 0; i < components.Count; i++)
                {
                    components[i].Graphics.PopPosition(tppushed[i]);
                }
            }
            base.PostPostLoad();
        }
    }
}
