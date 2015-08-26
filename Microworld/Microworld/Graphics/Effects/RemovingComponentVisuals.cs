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

namespace MicroWorld.Graphics.Effects
{
    sealed class RemovingComponentVisuals
    {
        internal RenderTarget2D fbo = null;
        internal int AliveState = 20;
        internal Components.Graphics.GraphicalComponent g;

        public RemovingComponentVisuals(Components.Component c)
        {
            var a = GraphicsEngine.Renderer;
            fbo = new RenderTarget2D(a.GraphicsDevice, Main.WindowWidth, Main.WindowHeight);
            g = c.Graphics;
        }

        public void Dispose()
        {
            fbo.Dispose();
        }

        public void Update()
        {
            AliveState--;
        }

        public void Draw(Renderer renderer)
        {
            var vr = MicroWorld.Graphics.GraphicsEngine.camera.VisibleRectangle;

            Matrix projection = Matrix.CreateOrthographicOffCenter(vr.X, vr.X + vr.Width, vr.Y + vr.Height, vr.Y, 0, 1);
            MicroWorld.Graphics.GraphicsEngine.ComponentFadeEffect.Parameters["MatrixTransform"].SetValue(projection);
            MicroWorld.Graphics.GraphicsEngine.ComponentFadeEffect.Parameters["halfpixel"].SetValue(
                new float[] { 0.5f / vr.Width, 0.5f / vr.Height });
            MicroWorld.Graphics.GraphicsEngine.ComponentFadeEffect.Parameters["Opacity"].SetValue(
                (float)(AliveState < 10 ? (float)AliveState / 10f :
                                                   (float)(10 - (AliveState - 10)) / 10f));
            MicroWorld.Graphics.GraphicsEngine.ComponentFadeEffect.Parameters["Drawtex"].SetValue(
                AliveState >= 10);
            renderer.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone,
                MicroWorld.Graphics.GraphicsEngine.ComponentFadeEffect);

            //renderer.Draw(fbo, new Vector2(), Color.White);
            g.Draw(renderer);
            renderer.End();
        }
    }
}
