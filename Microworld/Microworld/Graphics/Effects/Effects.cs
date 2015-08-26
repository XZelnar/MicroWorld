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
    public static class Effects
    {
        static RenderTarget2D GlowTarget, GlowTarget2, full, full2;
        public static Texture2D liquidDistortionTexture;
        public static Effect blurX, blurY, selectedShader, colorLerp, stereoDistortion, lightning, liquid;
        public static Effect laser, gaussianBlur, shockWave;
        internal static List<RemovingComponentVisuals> RemovingComponentVisualsList = new List<RemovingComponentVisuals>();

        internal static void Initialize()
        {
            GlowTarget = new RenderTarget2D(Main.renderer.GraphicsDevice, Main.WindowWidth, Main.WindowHeight, false,
                SurfaceFormat.Color, DepthFormat.None);
            GlowTarget2 = new RenderTarget2D(Main.renderer.GraphicsDevice, Main.WindowWidth / 2, Main.WindowHeight / 2, false,
                SurfaceFormat.Color, DepthFormat.None);
            full = new RenderTarget2D(Main.renderer.GraphicsDevice, Main.WindowWidth, Main.WindowHeight, false,
                SurfaceFormat.Color, DepthFormat.None);
            full2 = new RenderTarget2D(Main.renderer.GraphicsDevice, Main.WindowWidth, Main.WindowHeight, false,
                SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            GlobalEvents.onResolutionChanged += new GlobalEvents.ResolutionEventHandler(GlobalEvents_onResolutionChanged);
        }

        static void GlobalEvents_onResolutionChanged(int w, int h, int oldw, int oldh)
        {
            GlowTarget.Dispose();
            GlowTarget2.Dispose();
            full.Dispose();
            full2.Dispose();

            GlowTarget = new RenderTarget2D(Main.renderer.GraphicsDevice, w, h, false,
                SurfaceFormat.Color, DepthFormat.None);
            GlowTarget2 = new RenderTarget2D(Main.renderer.GraphicsDevice, w / 2, h / 2, false,
                SurfaceFormat.Color, DepthFormat.None);
            full = new RenderTarget2D(Main.renderer.GraphicsDevice, w, h, false,
                SurfaceFormat.Color, DepthFormat.None);
            full2 = new RenderTarget2D(Main.renderer.GraphicsDevice, w, h, false,
                SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        }

        internal static void LoadContent()
        {
            liquidDistortionTexture = ResourceManager.Load<Texture2D>("LiquidDistortion");

            blurX = ResourceManager.Load<Effect>("Shaders/BlurX");
            blurY = ResourceManager.Load<Effect>("Shaders/BlurY");
            selectedShader = ResourceManager.Load<Effect>("Shaders/SelectedComponent");
            colorLerp = ResourceManager.Load<Effect>("Shaders/ColorLerp");
            stereoDistortion = ResourceManager.Load<Effect>("Shaders/StereoDistortion");
            lightning = ResourceManager.Load<Effect>("Shaders/Lightning");
            laser = ResourceManager.Load<Effect>("Shaders/Laser");
            liquid = ResourceManager.Load<Effect>("Shaders/Liquid");
            gaussianBlur = ResourceManager.Load<Effect>("Shaders/GaussianBlur");
            shockWave = ResourceManager.Load<Effect>("Shaders/ShockWave");
        }

        public static void Update()
        {
            for (int i = 0; i < Components.ComponentsManager.Components.Count; i++)
            {
                if (Components.ComponentsManager.Components[i].selectionShaderState != 0)
                    getBluredTextureSelectedComponent(Components.ComponentsManager.Components[i]);
            }
            /*
            foreach (var c in Components.ComponentsManager.Components)
            {
                if (c.selectionShaderState != 0)
                    getBluredTextureSelectedComponent(c);
            }//*/
            /*
            if (GUI.GUIEngine.s_subComponentButtons.isVisible)
            {
                var a = GUI.GUIEngine.s_subComponentButtons.SelectedComponent;
                getBluredTextureSelectedComponent(a);
                //Main.renderer.DisableFBO();
            }//*/

            for (int i = 0; i < RemovingComponentVisualsList.Count; i++)
            {
                RemovingComponentVisualsList[i].Update();
                if (RemovingComponentVisualsList[i].AliveState <= 0)
                {
                    RemovingComponentVisualsList[i].Dispose();
                    RemovingComponentVisualsList.RemoveAt(0);
                    i--;
                }
            }
        }

        public static void DrawSelectedBackground(Components.Component c)
        {
            if (c != null)
            {

                Matrix projection = Matrix.CreateOrthographicOffCenter(0,
                    Main.renderer.GraphicsDevice.Viewport.Width, Main.renderer.GraphicsDevice.Viewport.Height, 0, 0, 1);
                selectedShader.Parameters["MatrixTransform"].SetValue(projection);
                selectedShader.Parameters["state"].SetValue((float)c.selectionShaderState / 20f);
                float[] p = new float[]{
                    1f/Main.renderer.GraphicsDevice.Viewport.Width, 
                    1f/Main.renderer.GraphicsDevice.Viewport.Height};
                selectedShader.Parameters["pixel1"].SetValue(p);
                selectedShader.Parameters["pixel2"].SetValue(new float[] { p[0] * 2, p[1] * 2 });

                Main.renderer.End();
                Main.renderer.BeginUnscaled(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, selectedShader);
                Main.renderer.Draw(c.selectionFBO, new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight),
                    Color.White);
                Main.renderer.End();
                Main.renderer.Begin();
                c.Graphics.DrawBorder(Main.renderer);
                Main.renderer.End();
                Main.renderer.Begin();
            }
            else
            {
                //if (timeSelected > 0)
                //    timeSelected--;
            }
        }

        internal static void getBluredTextureSelectedComponent(Components.Component c)
        {
            GraphicsEngine.isSelectedGlowPass = true;

            float pixel = 1f / (float)Main.WindowWidth * Settings.GameScale;
            var cr = new Vector2();
            int rangeX = (int)cr.X;
            int rangeY = (int)cr.Y;

            Matrix projection = Matrix.CreateOrthographicOffCenter(0,
                Main.renderer.GraphicsDevice.Viewport.Width, Main.renderer.GraphicsDevice.Viewport.Height, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            blurX.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);
            blurY.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);

            #region PreStencil
            Main.renderer.GraphicsDevice.SetRenderTarget(c.selectionFBO);
            Main.renderer.GraphicsDevice.Clear(Color.Transparent);
            Main.renderer.Begin();
            c.Graphics.Draw(Main.renderer);
            Main.renderer.End();
            #endregion

            Main.renderer.GraphicsDevice.SetRenderTarget(null);
        }

        public static void DrawRemovingVisuals()
        {
            GraphicsEngine.Renderer.End();
            for (int i = 0; i < RemovingComponentVisualsList.Count; i++)
            {
                RemovingComponentVisualsList[i].Draw(GraphicsEngine.Renderer);
            }
            GraphicsEngine.Renderer.Begin();
        }

        public static void BeginGaussianBlur(int radius, float viewportWidth, float viewportHeight, Renderer renderer)
        {
            if (radius < 1)
                radius = 1;

            if (renderer.IsDrawing)
                renderer.End();

            float[] pixelSize = new float[] { 1f / viewportWidth, 1f / viewportHeight };
            if (radius > 5)
            {
                pixelSize[0] *= radius / 5;
                pixelSize[1] *= radius / 5;
                radius = 5;
            }

            //Pascal's triangle

            float[] kernel = new float[radius * 2 + 1];
            float[] t = new float[kernel.Length];
            float[] p;
            t[0] = 1;
            kernel[0] = 1;
            kernel[1] = 1;
            for (int i = 2; i < kernel.Length; i++)
            {
                for (int j = 1; j < i; j++)
                    t[j] = kernel[j] + kernel[j - 1];
                t[i] = 1;

                p = t;
                t = kernel;
                kernel = p;
            }

            float sum = 1;
            for (int i = 1; i < kernel.Length; i++)
                sum += kernel[i];

            for (int i = 0; i < kernel.Length; i++)
                kernel[i] /= sum;

            //actual shader setup

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, viewportWidth, viewportHeight, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            gaussianBlur.Parameters["MatrixTransform"].SetValue(renderer.GetTransformMatrix() * halfPixelOffset * projection);
            gaussianBlur.Parameters["range"].SetValue(radius);
            gaussianBlur.Parameters["pixelSize"].SetValue(pixelSize);
            gaussianBlur.Parameters["kernel"].SetValue(kernel);

            renderer.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, gaussianBlur);
        }

    }
}
