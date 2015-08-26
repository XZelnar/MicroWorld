using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MicroWorld.Graphics.GUI.Scene.MapOverlays
{
    public class OverlayToolTip : HUDScene
    {
        static readonly Color bgColor = new Color(39, 49, 88);

        private string text = "";
        private Vector2 position = new Vector2();
        private Vector2 size = new Vector2();
        private float opacity = 1;
        private RenderTarget2D fbo = null;

        private Components.Properties.IHasMapToolTip _AssociatedComponent = null;
        private Components.Component associatedComponent = null;
        private Components.Component filter = null;


        internal Components.Properties.IHasMapToolTip AssociatedComponent
        {
            get { return _AssociatedComponent; }
            set
            {
                _AssociatedComponent = value;
                associatedComponent = AssociatedComponent as Components.Component;
            }
        }
        public string Text
        {
            get { return text; }
            private set
            {
                text = value;
                ReGenFBO();
            }
        }
        public Vector2 Size
        {
            get { return size; }
            private set
            {
                size = value;
                ReGenFBO();
            }
        }
        public Vector2 Position
        {
            get { return position; }
        }



        public OverlayToolTip()
        {
            isVisible = true;
        }

        public OverlayToolTip(Components.Properties.IHasMapToolTip c)
        {
            isVisible = true;
            AssociatedComponent = c;
        }

        public OverlayToolTip(Components.Properties.IHasMapToolTip c, Components.Component filter)
        {
            isVisible = true;
            AssociatedComponent = c;
            this.filter = filter;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void ReGenFBO()
        {
            if (fbo != null)
                fbo.Dispose();
            fbo = new RenderTarget2D(GraphicsEngine.Renderer.GraphicsDevice, (int)size.X, (int)size.Y);
        }

        public override void Update()
        {
            if (!isVisible)
                return;

            //txt
            String txt = _AssociatedComponent.GetMapOverlayToolTip(filter);
            if (txt != text)
            {
                text = txt;
                size = GUIEngine.font.MeasureString(text) + new Vector2(6, 6);
                ReGenFBO();

                GraphicsEngine.Renderer.EnableFBO(fbo);
                GraphicsEngine.Renderer.GraphicsDevice.Clear(Color.Transparent);
                GraphicsEngine.Renderer.BeginUnscaled();
                DrawFBO(GraphicsEngine.Renderer);
                GraphicsEngine.Renderer.End();
                GraphicsEngine.Renderer.DisableFBO();
            }

            //update pos
            float x = associatedComponent.Graphics.Position.X + associatedComponent.Graphics.GetSize().X,
                  y = associatedComponent.Graphics.Position.Y;
            Utilities.Tools.GameToScreenCoords(ref x, ref y);
            y += (associatedComponent.Graphics.GetSize().Y * Settings.GameScale - size.Y) / 2;
            position = new Vector2(x, y);

            base.Update();
        }

        public override void Draw(Renderer renderer)
        {
            if (!isVisible || fbo == null)
                return;

            renderer.End();
            renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None,
                RasterizerState.CullNone);
            renderer.Draw(fbo, new Vector2((int)position.X, (int)position.Y), Color.White * opacity);
            renderer.End();
            renderer.BeginUnscaled();

            base.Draw(renderer);
        }

        public void DrawFBO(Renderer renderer)
        {
            renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, 0, (int)size.X, (int)size.Y), bgColor);
            renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, 0, 1, (int)size.Y), Color.White);
            renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, 0, (int)size.X, 1), Color.White);
            renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)(size.X - 1), 0, 1, (int)size.Y), Color.White);
            renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, (int)(size.Y - 1), (int)size.X, 1), Color.White);

            renderer.DrawStringLeft(GUIEngine.font, text, new Vector2(3, 3), Color.White);
        }
    }
}
