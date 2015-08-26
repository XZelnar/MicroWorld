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

namespace MicroWorld.Components.Graphics
{
    public abstract class GraphicalComponent
    {
        private Vector2 position = new Vector2();

        /// <summary>
        /// Component position on the grid
        /// </summary>
        public virtual Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        private Vector2 size = new Vector2();
        /// <summary>
        /// Component size on the grid.
        /// By default recieved from GetSize() on initialization
        /// </summary>
        public virtual Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }
        public virtual Vector2 Center
        {
            get
            {
                return position + GetSize() / 2;
            }
        }
        /// <summary>
        /// Link to parent component
        /// </summary>
        public Component parent;
        /// <summary>
        /// Determines wether to render component or not
        /// </summary>
        public bool Visible = true;

        private int layer = 100;

        /// <summary>
        /// Determines the drawing order. Set it in constructor. 
        /// Low value will render component beneath others.
        /// Joint layer is 50. Default component layer is 100
        /// </summary>
        public int Layer
        {
            get { return layer; }
            set
            {
                layer = value;
                ComponentsManager.AddLayer(layer);
            }
        }

        public bool IgnoreNextDraw = false;
        

        /// <summary>
        /// Use this to initialize graphics
        /// </summary>
        public virtual void Initialize()
        {
            Size = GetSizeRotated(parent.ComponentRotation);
        }

        /// <summary>
        /// Use this to load content for each component
        /// </summary>
        public virtual void LoadContent()
        {
        }

        /// <summary>
        /// Returns the texture name (full path for ContentManager) of a graphical object
        /// that is later used in ComponentSelector
        /// </summary>
        /// <returns></returns>
        public virtual String GetIconName()
        {
            return null;
        }

        /// <summary>
        /// Returns wether component should be added to CS or not.
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldDisplayInCS()
        {
            return true;
        }

        /// <summary>
        /// Returns a String with name/description of the component.
        /// Returns null if none exists.
        /// </summary>
        /// <returns></returns>
        public virtual String GetCSToolTip()
        {
            return null;
        }

        /// <summary>
        /// Returns text tu be displayed near component when mouse-overing over it.
        /// </summary>
        /// <returns></returns>
        public virtual String GetInGameToolTip()
        {
            return GetCSToolTip() + "\r\nID: " + parent.ID.ToString();
        }

        /// <summary>
        /// Returns a path to an object in ComponentSelector.
        /// Each subfolder should be separated with "/"
        /// Multiple folders should be separated with ":"
        /// Returns null for root
        /// </summary>
        /// <returns></returns>
        public virtual String GetComponentSelectorPath()
        {
            return null;
        }

        /// <summary>
        /// Returns a path to a handbook.
        /// Returns null if no such file exists
        /// </summary>
        /// <returns></returns>
        public virtual String GetHandbookFile()
        {
            return null;
        }

        /// <summary>
        /// Returns the center of a graphical object
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 GetCenter(Component.Rotation rotation)
        {
            return new Vector2();
        }

        /// <summary>
        /// Returns the size of graphical object
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 GetSize()
        {
            if (parent == null) return new Vector2();
            return GetSizeRotated(parent.ComponentRotation);
        }

        /// <summary>
        /// Returns the size of the rotated component.
        /// Rotation is aquired from parent.ComponentRotation.
        /// By default returns GetSize().
        /// </summary>
        /// <returns></returns>
        public virtual Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            return new Vector2();
        }

        /// <summary>
        /// Returns array of possible component rotations.
        /// If null or empty array is returned, then the component will stay unrotated.
        /// </summary>
        /// <returns></returns>
        public virtual Component.Rotation[] GetPossibleRotations()
        {
            return new Component.Rotation[] { 
                Component.Rotation.cw0, 
                Component.Rotation.cw90, 
                Component.Rotation.cw180, 
                Component.Rotation.cw270 };
        }

        public virtual Object PushPosition()
        {
            return position;
        }

        public virtual void PopPosition(Object o)
        {
            position = (Vector2)o;
        }

        public virtual void MoveVisually(Vector2 d)
        {
            position += d;
        }

        public virtual void Update()
        {
        }

        public virtual void NonGameUpdate()
        {
        }

        /// <summary>
        /// Use this to render component.
        /// SpriteBatch is already set to draw. It should be in the same state at the end
        /// </summary>
        /// <param name="renderer"></param>
        public virtual void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (!CanDraw()) return;
        }

        internal virtual void PostDraw(MicroWorld.Graphics.Renderer renderer)//TODO rm
        {
        }

        /// <summary>
        /// Renders a dotted border around the component
        /// </summary>
        /// <param name="renderer"></param>
        public virtual void DrawBorder(MicroWorld.Graphics.Renderer renderer)
        {
            var s = GetSizeRotated(parent.ComponentRotation);
            #region Dotted Lines
            MicroWorld.Graphics.RenderHelper.DrawDottedLines(
                new float[] { position.X, position.X + s.X, position.X, position.X },
                new float[] { position.Y, position.Y, position.Y, position.Y + s.Y },
                new float[] { position.X, position.X + s.X, float.MaxValue, float.MaxValue },
                new float[] { float.MaxValue, float.MaxValue, position.Y, position.Y + s.Y },
                Color.White, renderer, true);
            #endregion
        }

        /// <summary>
        /// Renders a dotted border around the component
        /// Called for ghosts.
        /// Sets up parameters and call DrawBorder(Renderer)
        /// </summary>
        /// <param name="renderer"></param>
        public virtual void DrawBorder(float x, float y, Component.Rotation rot, MicroWorld.Graphics.Renderer renderer)
        {
            var a = parent.ComponentRotation;
            parent.ComponentRotation = rot;
            var b = position;
            position = new Vector2(x, y);
            DrawBorder(renderer);
            position = b;
            parent.ComponentRotation = a;
        }

        /// <summary>
        /// Use this to render ghost components (the one that hovers over cursor). Renders nothing by default.
        /// SpriteBatch is already set to draw. It should be in the same state at the end.
        /// </summary>
        /// <param name="x">Coordinates of a ghost</param>
        /// <param name="y">Coordinates of a ghost</param>
        /// <param name="texture"></param>
        public virtual void DrawGhost(int x, int y, MicroWorld.Graphics.Renderer renderer, Component.Rotation rotation)
        {
        }

        /// <summary>
        /// Called upon program closing and component destruction
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Called upon scene reinitialization
        /// </summary>
        public virtual void Reset()
        {
        }

        /// <summary>
        /// Called upon simulation start
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        /// Checks for intersection of this component's rectangle with another rectangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public virtual bool IntersectsWith(int x, int y, int w, int h)
        {
            Rectangle r1 = new Rectangle((int)position.X, (int)position.Y, (int)Size.X, (int)Size.Y),
                      r2 = new Rectangle(x, y, w, h);
            return r1.Intersects(r2) || r1.Contains(r2) || r2.Contains(r1);
        }

        public virtual bool CanDraw()
        {
            Rectangle r1 = new Rectangle((int)position.X, (int)position.Y, (int)Size.X, (int)Size.Y),
                      r2 = new Rectangle(-(int)Settings.GameOffset.X, -(int)Settings.GameOffset.Y,
                          (int)(Main.WindowWidth / Settings.GameScale), (int)(Main.WindowHeight / Settings.GameScale));
            return r1.Intersects(r2) || r2.Contains(r1);
        }
    }
}
