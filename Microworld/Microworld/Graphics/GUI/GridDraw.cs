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

namespace MicroWorld.Graphics.GUI
{
    static unsafe class GridDraw
    {
        private static List<Texture2D> bgFillIns = new List<Texture2D>();

        public static Vector2 offset = new Vector2();
        public static Color ColorLines = new Color(1f, 1f, 1f);
        public static Color ColorCursor = new Color(0.8f, 0.5f, 0f, 0.5f);
        public static int CursorRadius = 2;
        public static int Step = 8;
        private static Utilities.Random2D fillInRandom;
        public static bool ShouldDrawGrid = false;

        private static Texture2D DistortionTexture;
        private static RenderTarget2D[] DistortionFBO = new RenderTarget2D[5];
        private static RenderTarget2D overlayfbo;
        private static int curfbo = 0;
        private static Color[] DistColors;
        private static Effect Blur, overlayShader;

        public static Texture2D CursorTexture;
        public static SpriteFont EmptyFont;

        public static int CursorX = 0, CursorY = 0;

        private static int BlurRadius = 5;
        private static int UpdateFrequency = 4;
        private static int BlurOpacity = 5;
        private static bool BlurEnabled = false;

        internal static void LoadContent()
        {
            fillInRandom = new Utilities.Random2D(null, 4000);
            bgFillIns.Add(ResourceManager.Load<Texture2D>("GUI/Grid/FillIn0"));
            bgFillIns.Add(ResourceManager.Load<Texture2D>("GUI/Grid/FillIn1"));
            EmptyFont = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_60_b");
            Blur = ResourceManager.Load<Effect>("Shaders/GameBG");
            overlayShader = ResourceManager.Load<Effect>("Shaders/PlacableOverlay");

            DistortionTexture = new Texture2D(GraphicsEngine.Renderer.GraphicsDevice, Main.WindowWidth / 8, Main.WindowHeight / 8);
            DistColors = new Color[DistortionTexture.Width * DistortionTexture.Height];
            overlayfbo = new RenderTarget2D(GraphicsEngine.Renderer.GraphicsDevice, Main.WindowWidth, Main.WindowHeight);
            //DistortionFBO = new RenderTarget2D(GraphicsEngine.Renderer.GraphicsDevice,
            //    Main.WindowWidth, Main.WindowHeight);
        }

        internal static void OnResolutionChange(int w, int h, int oldw, int oldh)
        {
            DistortionTexture.Dispose();
            DistortionTexture = new Texture2D(GraphicsEngine.Renderer.GraphicsDevice, w / 8, h / 8);
            DistColors = new Color[DistortionTexture.Width * DistortionTexture.Height];
            overlayfbo.Dispose();
            overlayfbo = new RenderTarget2D(GraphicsEngine.Renderer.GraphicsDevice, Main.WindowWidth, Main.WindowHeight);
        }

        static float tx = 0, ty = 0;
        static float st;
        public static void DrawGrid()
        {
            offset = -new Vector2((int)(Settings.GameOffset.X / Step) * Step, (int)(Settings.GameOffset.Y / Step) * Step);

            st = Step * Settings.GameScale;
            Vector2 RightBottom = GraphicsEngine.camera.BottomRight;
            Rectangle rect = GraphicsEngine.camera.VisibleRectangle;
            tx = rect.X - rect.X % Step;
            ty = rect.Y - rect.Y % Step;
            Utilities.Tools.GameToScreenCoords(ref tx, ref ty);

            #region Grid
            List<VertexPositionColorTexture> tg = new List<VertexPositionColorTexture>();
            if (ShouldDrawGrid)
            {
                for (float x = 0; x < Main.WindowWidth; x += st)
                {
                    tg.Add(new VertexPositionColorTexture(new Vector3(tx + x, Main.WindowHeight, 0), ColorLines * 0.2f,
                        new Vector2()));
                    tg.Add(new VertexPositionColorTexture(new Vector3(tx + x, 0, 0), ColorLines * 0.2f,
                        new Vector2()));
                }

                for (float y = 0; y < Main.WindowHeight; y += st)
                {
                    tg.Add(new VertexPositionColorTexture(new Vector3(Main.WindowWidth, ty + y, 0), ColorLines * 0.2f,
                        new Vector2()));
                    tg.Add(new VertexPositionColorTexture(new Vector3(0, ty + y, 0), ColorLines * 0.2f,
                        new Vector2()));
                }
            }
            #endregion

            Main.renderer.End();
            Main.renderer.BeginUnscaled();
            Main.renderer.Draw(Graphics.GraphicsEngine.pixel, new Rectangle(0, 0, 0, 0), Color.White);
            Main.renderer.End();
            if (ShouldDrawGrid)
                Main.renderer.DrawLinesList(tg.ToArray());

            #region ...
            /*
            rect.X -= 64;//max text size
            rect.Y -= 64;
            rect.Width += 64;
            rect.Height += 64;
            int ttx = rect.X - rect.X % Step;
            int tty = rect.Y - rect.Y % Step;
            float tcx, tcy;
            int tr;
            for (int x = ttx; x < rect.X + rect.Width; x += Step)
            {
                break;
                for (int y = tty; y < rect.Y + rect.Height; y += Step)
                {
                    tcx = x;
                    tcy = y;
                    Utilities.Tools.GameToScreenCoords(ref tcx, ref tcy);
                    tr = fillInRandom.Next(x, y);
                    switch (tr)
                    {
                        case 0:
                            Main.renderer.Draw(bgFillIns[0], new Vector2(tcx, tcy), Color.White * 0.2f);
                            break;
                        case 1:
                            Main.renderer.Draw(bgFillIns[1], new Vector2(tcx, tcy), Color.White * 0.1f);
                            break;
                        default:
                            break;
                    }
                }
            }//*/
            #endregion

            Main.renderer.BeginUnscaled();
            DrawDistortions();

            /*
            if (!Components.ComponentsManager.ContainsVisibleComponents)
            {
                var sz = EmptyFont.MeasureString("Empty");
                GraphicsEngine.renderer.DrawString(EmptyFont, "Empty",
                    new Rectangle(0, (int)(Main.WindowHeight - sz.Y) / 2, Main.WindowWidth, (int)sz.Y),
                    Color.White * 0.5f, Renderer.TextAlignment.Center);
            }//*/

            Main.renderer.End();
            Main.renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None,
                RasterizerState.CullNone, overlayShader);
            overlayDraw();
            Main.renderer.End();
            Main.renderer.Begin();

            //CursorX = (int)(InputEngine.curMouse.X / Settings.GameScale + Graphics.GraphicsEngine.camera.TopLeft.X);
            //CursorY = (int)(InputEngine.curMouse.Y / Settings.GameScale + Graphics.GraphicsEngine.camera.TopLeft.Y);
            CursorX = InputEngine.curMouse.X;
            CursorY = InputEngine.curMouse.Y;
            Utilities.Tools.ScreenToGameCoords(ref CursorX, ref CursorY);
        }

        private static void overlayDraw()
        {
            Matrix projection = Matrix.CreateOrthographicOffCenter(0,
                overlayfbo.Width, overlayfbo.Height, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            overlayShader.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);
            overlayShader.Parameters["pixel"].SetValue(new float[] { 1f / (float)overlayfbo.Width, 1f / (float)overlayfbo.Height });

            Main.renderer.Draw(overlayfbo, new Vector2(), Color.White);
        }

        private static void DrawDistortions()
        {
            for (int i = 0; i < DistortionFBO.Length && DistortionFBO[i] != null; i++)
            {
                GraphicsEngine.Renderer.Draw(DistortionFBO[i], new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight),
                    Color.White * ((float)BlurOpacity / 255f));
            }
        }

        public static void Update()
        {
            RenderOverlay();
            Graphics.GraphicsEngine.Renderer.DisableFBO();

            if (Main.Ticks % UpdateFrequency != 0)
                return;

            Random r = new Random();
            float t = 0f;
            for (int i = 0; i < DistColors.Length; i++)
            {
                t = (float)r.NextDouble();
                if (t < 0.6f)
                {
                    DistColors[i] = new Color(0, 0, 0, 0);
                }
                else
                {
                    DistColors[i] = new Color(t, t, t);
                }
            }
            GraphicsEngine.Renderer.GraphicsDevice.Textures[0] = null;
            DistortionTexture.SetData<Color>(DistColors);

            if (DistortionFBO[curfbo] == null)
                DistortionFBO[curfbo] = new RenderTarget2D(GraphicsEngine.Renderer.GraphicsDevice,
                    Main.WindowWidth, Main.WindowHeight);

            GraphicsEngine.Renderer.EnableFBO(DistortionFBO[curfbo]);
            GraphicsEngine.Renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap,
                DepthStencilState.Default, RasterizerState.CullNone, BlurEnabled ? Blur : null);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0,
                DistortionFBO[0].Width, DistortionFBO[0].Height, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            Blur.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);
            float pixel = 1f / (float)DistortionFBO[0].Width;
            Blur.Parameters["PixelSize"].SetValue(pixel);
            Blur.Parameters["Range"].SetValue(BlurRadius);
            GraphicsEngine.Renderer.Draw(DistortionTexture,
                new Rectangle(0, 0, DistortionFBO[0].Width, DistortionFBO[0].Height),
                new Rectangle(0, 0, DistortionFBO[0].Width, DistortionFBO[0].Height), 
                Color.White);
            GraphicsEngine.Renderer.End();
            GraphicsEngine.Renderer.DisableFBO();
            curfbo++;
            if (curfbo >= DistortionFBO.Length)
                curfbo = 0;
        }

        #region OverlayStuff
        static Rectangle tl;

        private static void RenderOverlay()
        {
            GraphicsEngine.Renderer.EnableFBO(overlayfbo);
            GraphicsEngine.Renderer.GraphicsDevice.Clear(Color.Transparent);

            if (Logics.PlacableAreasManager.areas.Count > 0)
            {
                Shortcuts.renderer.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointWrap, null, null);

                var a = Shortcuts.camera.VisibleRectangle;
                Shortcuts.renderer.Draw(Graphics.GraphicsEngine.dottedPattern, new Rectangle(a.X, a.Y, a.Width, a.Height),
                    new Rectangle((int)offset.X, (int)offset.Y, overlayfbo.Width / 4, overlayfbo.Height / 4), Color.White);
                for (int i = 0; i < Logics.PlacableAreasManager.areas.Count; i++)
                {
                    tl = Logics.PlacableAreasManager.areas[i];
                    Shortcuts.renderer.Draw(Shortcuts.pixel, new Rectangle(tl.X, tl.Y, tl.Width, tl.Height), Color.Transparent);
                }

                Shortcuts.renderer.End();
            }

            Shortcuts.renderer.DisableFBO();
        }
        #endregion

    }
}
