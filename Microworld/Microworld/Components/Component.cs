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
    [Serializable]
    public abstract class Component
    {
        public enum Rotation
        {
            cw0 = 0,
            cw90 = 1,
            cw180 = 2,
            cw270 = 3
        }

        /// <summary>
        /// Logical module of the component
        /// </summary>
        public Logics.LogicalComponent Logics;
        /// <summary>
        /// Graphical module of the component
        /// </summary>
        public Graphics.GraphicalComponent Graphics;
        /// <summary>
        /// ID of the component
        /// </summary>
        public int ID;
        /// <summary>
        /// ToolTip of this component
        /// </summary>
        public GUI.GeneralProperties ToolTip = new GUI.GeneralProperties();
        /// <summary>
        /// For whatever your heart desires most!
        /// </summary>
        public object Tag = null;
        /// <summary>
        /// Gets component rotation
        /// </summary>
        public Rotation ComponentRotation
        {
            get { return rotation; }
            set { SetRotation(value); }
        }
        public bool IsRemovable
        {
            get { return isRemovable; }
            set
            {
                if (isRemovable != value)
                {
                    isRemovable = value;
                    OnRemovabilityChanged();
                }
            }
        }
        public Properties.IContainer container = null;

        protected Rotation rotation = Rotation.cw0;

        private bool isRemovable = true;
        private int ticksSincePlacement = 0;
        private bool wasInitialized = false;
        internal int selectionShaderState = 0;
        internal RenderTarget2D selectionFBO;
        /// <summary>
        /// Used for vismap. Unique for every type
        /// </summary>
        internal short _typeID = -1;
        public short typeID
        {
            get
            {
                return _typeID == -1 ? (_typeID = ComponentsManager.TypeIDs[GetType()]) : _typeID;
            }
        }

        public int TicksSincePlacement
        {
            get { return ticksSincePlacement; }
            internal set { ticksSincePlacement = value; }
        }
        

        /// <summary>
        /// Use this to initialize the component
        /// </summary>
        public virtual void Initialize()
        {
            if (wasInitialized)
                return;
            wasInitialized = true;

            Logics.Initialize();
            Graphics.Initialize();
            if (ToolTip != null && ToolTip.AssociatedComponent == null)
            {
                ToolTip.AssociatedComponent = this;
                ToolTip.Initialize();
                ToolTip.LoadContent();
            }
            SetComponentOnVisibilityMap();

            CreateSelectionFBO(Main.WindowWidth, Main.WindowHeight);
        }

        internal virtual void CreateSelectionFBO(int w, int h)
        {
            if (selectionFBO != null)
                selectionFBO.Dispose();
            selectionFBO = new RenderTarget2D(MicroWorld.Graphics.GraphicsEngine.Renderer.GraphicsDevice, w, h, false, SurfaceFormat.Color, DepthFormat.None);
        }

        public virtual void SetComponentOnVisibilityMap()
        {
            UpdateComponentOnVisibilityMap(true);
        }

        public virtual void RemoveComponentFromVisibilityMap()
        {
            UpdateComponentOnVisibilityMap(false);
        }

        public virtual void UpdateComponentOnVisibilityMap(bool visible)
        {
            if (!Graphics.Visible) return;
            ComponentsManager.VisibilityMap.Generate(Graphics.Position.X, Graphics.Position.Y);
            ComponentsManager.VisibilityMap.SetRectangle(visible ? this : null, Graphics.Position.X + 1, Graphics.Position.Y + 1, Graphics.Size.X - 2, Graphics.Size.Y - 2);
        }

        public void ReSetComponentOnVisibilityMap()
        {
            RemoveComponentFromVisibilityMap();
            SetComponentOnVisibilityMap();
        }

        /// <summary>
        /// Rotates the component to a certain degree
        /// </summary>
        /// <param name="rot"></param>
        public virtual void SetRotation(Rotation rot)
        {
            if (rot == ComponentRotation)
            {
                return;
            }
            rotation = rot;
            Graphics.Size = Graphics.GetSizeRotated(rot);
        }

        /// <summary>
        /// Use this to load content for each component.
        /// To load global parameters (like textures), use this in your class:
        /// public static void LoadContentStatic() { }
        /// It will be called upon file initialization.
        /// </summary>
        public virtual void LoadContent()
        {
        }

        /// <summary>
        /// Calls LogicalComponent Update function by default
        /// </summary>
        public virtual void Update()
        {
            Logics.Update();
            Graphics.Update();
        }

        /// <summary>
        /// Function that is called EVERY update regardless of game state
        /// </summary>
        public virtual void NonGameUpdate()
        {
            Graphics.NonGameUpdate();
            //selection shader
            if (MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.isVisible &&
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent == this)
            {
                if (selectionShaderState < 20)
                    selectionShaderState++;
                if (selectionShaderState == 1)
                    MicroWorld.Graphics.Effects.Effects.getBluredTextureSelectedComponent(this);
            }
            else
            {
                if (selectionShaderState > 0)
                    selectionShaderState--;
            }

            //TT
            if (ToolTip != null && ToolTip.isVisible && Settings.GameState != Settings.GameStates.Stopped)
            {
                int x = InputEngine.curMouse.X, y = InputEngine.curMouse.Y;
                Utilities.Tools.ScreenToGameCoords(ref x, ref y);
                var a = ComponentsManager.GetComponents(x, y);
                for (int i = 0; i < a.Count; i++)
                {
                    if (a[i] is Properties.IRotatable)
                    {
                        ToolTip.isVisible = false;
                        MicroWorld.Graphics.GUI.GUIEngine.RemoveHUDScene(ToolTip);
                    }
                }
            }
        }

        /// <summary>
        /// Calls GraphicalComponent Draw function by default
        /// </summary>
        /// <param name="sb"></param>
        public virtual void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            ticksSincePlacement++;
            Graphics.Draw(renderer);
        }

        /// <summary>
        /// Add child components (like joints and wires) to ComponentManager here
        /// </summary>
        public virtual void InitAddChildComponents()
        {
        }

        /// <summary>
        /// Returns wether component can be Drag'n'Dropped after it's been placed
        /// </summary>
        /// <returns></returns>
        public virtual bool CanDragDrop()
        {
            return true;
        }

        /// <summary>
        /// Check for things like shared Joints
        /// </summary>
        /// <returns></returns>
        public virtual bool IsMovable()
        {
            if (!IsRemovable)
                return false;
            var a = getJoints();
            Joint t;
            for (int i = 0; i < a.Length; i++)
            {
                t = ComponentsManager.GetComponent(a[i]) as Joint;
                if (t.ContainingComponents.Count > 1)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Override if a Component has some specific conditions to be placed.
        /// This SHOULD be used to check if this Component is being placed above or near some other component.
        /// Example: PistonJoint uses it to check if it is being placed on a Piston.
        /// Not called for components that use IDragDropPlacable property.
        /// PS. Joint match is done elsewhere
        /// </summary>
        /// <returns></returns>
        public virtual bool CanPlace(int x, int y, int w, int h)
        {
            return Components.ComponentsManager.VisibilityMap.IsFree(x - 4, y - 4, w + 9, h + 9) &&
                   MicroWorld.Logics.PlacableAreasManager.IsPlacable(x + 4, y + 4, w - 8, h - 8);
        }

        /// <summary>
        /// Called every time component changes its position.
        /// Use this to move component's joints, wires and whatnot...
        /// </summary>
        /// <param name="dx">Change by X axis</param>
        /// <param name="dy">Change by Y axis</param>
        public virtual void OnMove(int dx, int dy)
        {
            RemoveComponentFromVisibilityMap();
            Graphics.MoveVisually(new Vector2(dx, dy));
            SetComponentOnVisibilityMap();

            GlobalEvents.OnComponentMoved(this);
        }

        /// <summary>
        /// Invoked every time isRemovable variable is changed
        /// </summary>
        public virtual void OnRemovabilityChanged()
        {
        }

        /// <summary>
        /// Checks whether coordinates are inside this object or not
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual bool isIn(int x, int y)
        {
            return (x >= Graphics.Position.X && x <= Graphics.Position.X + Graphics.GetSize().X &&
                y >= Graphics.Position.Y && y <= Graphics.Position.Y + Graphics.GetSize().Y);
        }
        public virtual bool isIn(int x, int y, float rad)
        {
            return Math.Sqrt(Math.Pow(Graphics.Center.X - x, 2) + Math.Pow(Graphics.Center.Y - y, 2)) <= rad;
        }

        public virtual bool isIn(int x, int y, int w, int h)
        {
            return (x <= Graphics.Position.X && x + w >= Graphics.Position.X + Graphics.GetSize().X &&
                y <= Graphics.Position.Y && y + h >= Graphics.Position.Y + Graphics.GetSize().Y);
        }

        public virtual bool Intersects(int x, int y, int w, int h)
        {
            //Rectangle r1 = new Rectangle((int)Graphics.Position.X, (int)Graphics.Position.Y, (int)Graphics.Size.X, (int)Graphics.Size.Y);
            //Rectangle r2 = new Rectangle(x, y, w, h);
            //return r1.Contains(r2) || r2.Contains(r1) || r1.Intersects(r2);
            float x1 =  Graphics.Position.X, y1 = Graphics.Position.Y, x2 =  Graphics.Position.X + Graphics.Size.X, y2 = Graphics.Position.Y + Graphics.Size.Y;
            float ax1 = x, ay1 = y, ax2 = x + w, ay2 = y + h;
            return (ax1 <= x1 && ax2 >= x1 && ay1 >= y1 && ay1 <= y2) || //a intersects left with top
                   (ax1 <= x1 && ax2 >= x1 && ay2 >= y1 && ay2 <= y2) || //a intersects left with bottom
                   (ax1 <= x2 && ax2 >= x2 && ay1 >= y1 && ay1 <= y2) || //a intersects right with top
                   (ax1 <= x2 && ax2 >= x2 && ay2 >= y1 && ay2 <= y2) || //a intersects right with bottom
                   (ay1 <= y1 && ay2 >= y1 && ax1 >= x1 && ax1 <= x2) || //a intersects top with left
                   (ay1 <= y1 && ay2 >= y1 && ax2 >= x1 && ax2 <= x2) || //a intersects top with right
                   (ay1 <= y2 && ay2 >= y2 && ax1 >= x1 && ax1 <= x2) || //a intersects bottom with left
                   (ay1 <= y2 && ay2 >= y2 && ax2 >= x1 && ax2 <= x2) || //a intersects bottom with right
                   isIn(x, y, w, h);
        }

        /// <summary>
        /// Add this component to a ComponentManager here
        /// </summary>
        public virtual void AddComponentToManager()
        {
            ComponentsManager.Add(this);
        }

        public virtual void Remove()
        {
            if (!isRemovable) return;
            RemoveComponentFromVisibilityMap();

            ComponentsManager.Replace(this, new EmptyComponent(ID));
            if (MicroWorld.Graphics.GUI.GUIEngine.ContainsHUDScene(ToolTip))
            {
                MicroWorld.Components.ComponentsManager.closingProperties.Add(ToolTip);
                ToolTip.Close();
            }
            if (MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.isVisible &&
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent == this)
            {
                MicroWorld.Graphics.GUI.GUIEngine.RemoveHUDScene(MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons);
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent = null;
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.isVisible = false;
            }

            Statistics.ComponentsRemoved++;
            GlobalEvents.OnComponentRemoved(this);
        }

        /// <summary>
        /// Returns Component name
        /// </summary>
        /// <returns></returns>
        public virtual String GetName()
        {
            return Graphics.GetCSToolTip();
        }

        /// <summary>
        /// Returns an array of Component Joint IDs
        /// </summary>
        /// <returns></returns>
        public virtual int[] getJoints()
        {
            return new int[] { };
        }

        /// <summary>
        /// Returns array, where [2i] is X and [2i+1] is Y of Joint i.
        /// X and Y are local i.e. relative to component position.
        /// Used for placing components on existing joints.
        /// WARNING: do NOT include invisible joints!
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public virtual int[] GetJointCoords(Rotation r)
        {
            return new int[] { };
        }

        /// <summary>
        /// Returns joints that current can pass to from certain joint. These joints don't have to be connected directly.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public virtual Joint[] FindAccessibleJoints(Joint from)
        {
            return new Joint[0];
        }


        //========================================================EVENTS==============================================================

        /// <summary>
        /// Called when the component is placed on the grid.
        /// Spawn particles here.
        /// </summary>
        public virtual void OnPlaced()
        {
            GlobalEvents.OnComponentPlacedByPlayer(this);
            Statistics.ElementsPlaced++;
            Random r = new Random();
            for (int i = 0; i < 50; i++)
            {
                MicroWorld.Graphics.ParticleManager.Add(
                    new MicroWorld.Graphics.Particles.Spark(
                        new Vector2(Graphics.Position.X, Graphics.Position.Y) +
                        new Vector2(r.Next((int)Graphics.Size.X), r.Next((int)Graphics.Size.Y)),
                        new Vector2(2, 2),
                        new Vector2(((float)r.NextDouble() - 0.5f) / 2, ((float)r.NextDouble() - 0.5f) / 2),
                        (float)(r.NextDouble() + 0.1f) / 10));
            }

            if (GetComponentToolTip() != null)
            {
                MicroWorld.Graphics.GUI.GUIEngine.AddHUDScene(GetComponentToolTip());
            }

            if (Settings.GameState != Settings.GameStates.Stopped)
                Start();
        }

        /// <summary>
        /// Called when added to ComponentManager. 
        /// If placed, then called BEFORE OnPlaced.
        /// </summary>
        public virtual void OnAdded()
        {
        }

        /// <summary>
        /// Called whenever a remove button is clicked
        /// </summary>
        public virtual void OnButtonClickedRemove()
        {
            if (!isRemovable) return;
            MicroWorld.Logics.ChangeHistory.Push();

            MicroWorld.Graphics.Effects.Effects.RemovingComponentVisualsList.Add(new MicroWorld.Graphics.Effects.RemovingComponentVisuals(this));

            Remove();
            GlobalEvents.OnComponentRemovedByPlayer(this);
        }


        //=======================================================GRAPHICS=============================================================

        
        /// <summary>
        /// Returns ComponentToolTip object corresponding to this component
        /// </summary>
        /// <returns></returns>
        public virtual GUI.GeneralProperties GetComponentToolTip()
        {
            if (!Graphics.Visible) return null;
            if (ToolTip != null)
                ToolTip.AssociatedComponent = this;
            return ToolTip;
        }

        /// <summary>
        /// Called when Graphics Device is reset. Usually results in corrupted FBOs. Use it to redraw them.
        /// </summary>
        public virtual void OnGraphicsDeviceReset()
        {
            if (ToolTip != null)
                ToolTip.OnGraphicsDeviceReset();
        }


        //========================================================LOGICS==============================================================


        /// <summary>
        /// Called upon scene reinitialization
        /// </summary>
        public virtual void Reset()
        {
            Logics.Reset();
            Graphics.Reset();
        }

        /// <summary>
        /// Called upon simulation start
        /// </summary>
        public virtual void Start()
        {
            Logics.Start();
            Graphics.Start();
        }

        /// <summary>
        /// Called right before circuit update. Reset any temporary values here
        /// </summary>
        public virtual void PreUpdate()
        {
        }


        //========================================================INPUT===============================================================


        public virtual void OnMouseClick(InputEngine.MouseArgs e)
        {
            if (Graphics.Visible)
            {
                //if (Settings.GameState != Settings.GameStates.Stopped)
                //{
                //    MicroWorld.Graphics.OverlayManager.HighlightStop();
                //    return;
                //}
                //if (isIn(e.curState.X, e.curState.Y))// && e.button == 1)
                //{
                    //OnButtonClickedRemove();
                //    e.Handled = true;
                //}
            }
        }

        public virtual void OnMouseDown(InputEngine.MouseArgs e)
        {
        }

        public virtual void OnMouseUp(InputEngine.MouseArgs e)
        {
        }

        public virtual void OnMouseMove(InputEngine.MouseMoveArgs e)
        {
            if (GetComponentToolTip() != null)
                if (isIn(e.curState.X, e.curState.Y))
                    if (!GetComponentToolTip().isVisible)
                        MicroWorld.Graphics.GUI.GUIEngine.AddHUDScene(GetComponentToolTip());
        }

        public virtual void OnMouseWheel(InputEngine.MouseWheelMoveArgs e)
        {
        }


        //==========================================================IO================================================================


        /// <summary>
        /// Called right before SaveAll is called. Use it for stuff like poppoing position...
        /// </summary>
        public virtual void PreSave()
        {
        }

        /// <summary>
        /// Called when a scheme is being saved
        /// </summary>
        /// <param name="Compound">Use this to save data</param>
        public virtual void SaveAll(IO.ComponentData Compound)
        {
            Compound.SetType(GetType());
            Compound.Add("ID", ID);
            Compound.Add("Position", Graphics.Position);
            Compound.Add("Size", Graphics.Size);
            Compound.Add("Visible", Graphics.Visible);
            Compound.Add("Rotation", rotation.GetHashCode());
            Compound.Add("Removable", isRemovable);
        }

        /// <summary>
        /// Called right after SaveAll is called.
        /// </summary>
        public virtual void PostSave()
        {
        }

        /// <summary>
        /// Called when a scheme is being loaded
        /// </summary>
        /// <param name="Compound">Use this to read data</param>
        public virtual void LoadAll(IO.ComponentData Compound)
        {
            ID = Compound.GetInt("ID");
            Graphics.Position = Compound.GetVector2("Position");
            Graphics.Size = Compound.GetVector2("Size");
            Graphics.Visible = Compound.GetBool("Visible");
            rotation = (Rotation)Compound.GetInt("Rotation");
            isRemovable = Compound.GetBool("Removable");
        }

        /// <summary>
        /// Called after global loading is finished
        /// </summary>
        public virtual void PostLoad()
        {
        }

        /// <summary>
        /// Called after global PostLoading is called and all components have been initialized
        /// </summary>
        public virtual void PostPostLoad()
        {
        }

        public override string ToString()
        {
            return base.ToString() + " ID: " + ID.ToString();
        }
    }
}
