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
using MicroWorld.Graphics.GUI.Elements;

namespace MicroWorld.Graphics.GUI.Scene
{
    public abstract class Scene
    {
        public List<Control> controls = new List<Control>();
        public Background.Background background;
        public bool ShouldBeScaled = false;
        public bool CanOffset = false;

        public int Layer = 0;
        public bool isVisible = false;
        internal bool blockInput = false;

        public object Tag = null;

        public virtual void Initialize()
        {
            if (background != null) background.Initialize();
            foreach (Control c in controls)
            {
                c.Initialize();
            }
        }

        public virtual void LoadContent()
        {
        }

        public virtual void Dispose()
        {
        }

        public virtual void Update()
        {
            if (background != null) background.Update();
            lock (controls)
            {
                foreach (Control c in controls)
                {
                    c.Update();
                }
            }
        }

        public virtual void Draw(Renderer renderer)
        {
            if (background != null) background.Draw(renderer);
            foreach (Control c in controls)
            {
                c.Draw(renderer);
            }
        }

        public virtual void PostDraw()
        {
            foreach (Control c in controls)
            {
                c.PostDraw();
            }
        }

        public virtual void onShow()
        {
            isVisible = true;
        }

        public virtual void onClose()
        {
            isVisible = false;
        }

        /// <summary>
        ///           !!!CALLED IN PARALEL THREAD!!!
        ///         Use this to create scene animation.
        /// (Just use cycles with no concern of sync to redraw)
        /// </summary>
        public virtual void FadeIn()
        {
        }

        /// <summary>
        ///           !!!CALLED IN PARALEL THREAD!!!
        ///         Use this to create scene animation.
        /// (Just use cycles with no concern of sync to redraw)
        /// </summary>
        public virtual void FadeOut()
        {
        }

        /// <summary>
        /// Called between FadeOut completion and OnClose
        /// Use it, for example, to add HUDScenes when starting a level
        /// </summary>
        public virtual void PostFadeOut()
        {
        }

        public virtual void onButtonDown(InputEngine.MouseArgs e)
        {
            foreach (Control c in controls)
            {
                if (c.isVisible) c.onButtonDown(e);
                if (InputEngine.eventHandled) break;
            }
        }

        public virtual void onButtonUp(InputEngine.MouseArgs e)
        {
            foreach (Control c in controls)
            {
                if (c.isVisible) c.onButtonUp(e);
                if (InputEngine.eventHandled) break;
            }
        }

        public virtual void onButtonClick(InputEngine.MouseArgs e)
        {
            lock (controls)
            {
                foreach (Control c in controls)
                {
                    if (c.isVisible) c.onButtonClick(e);
                    if (InputEngine.eventHandled) break;
                }
            }
        }

        public virtual void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            foreach (Control c in controls)
            {
                if (c.isVisible) c.onMouseMove(e);
                if (InputEngine.eventHandled) break;
            }
        }

        public virtual void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            foreach (Control c in controls)
            {
                if (c.isVisible) c.onMouseWheelMove(e);
                if (InputEngine.eventHandled) break;
            }
        }

        public virtual void onKeyDown(InputEngine.KeyboardArgs e)
        {
            foreach (Control c in controls)
            {
                if (c.isVisible && c.isFocused)
                    c.onKeyDown(e);
                if (InputEngine.eventHandled) break;
            }
        }

        public virtual void onKeyUp(InputEngine.KeyboardArgs e)
        {
            foreach (Control c in controls)
            {
                if (c.isVisible && c.isFocused)
                    c.onKeyUp(e);
                if (InputEngine.eventHandled) break;
            }
        }

        public virtual void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            lock (controls)
            {
                foreach (Control c in controls)
                {
                    if (c.isVisible && c.isFocused)
                        c.onKeyPressed(e);
                    if (InputEngine.eventHandled) break;
                }
            }
        }

        public virtual void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
        }

        public virtual void OnGraphicsDeviceReset()
        {
            lock (controls)
            {
                foreach (Control c in controls)
                {
                    c.OnGraphicsDeviceReset();
                }
            }
        }

    }
}
