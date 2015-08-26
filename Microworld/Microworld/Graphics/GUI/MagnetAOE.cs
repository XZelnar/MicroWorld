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
    static class MagnetAOE
    {
        const int VALUES_DENSITY = 16;//distance in pixels between 2 value measurements
        const float OPACITY = 0.3f;

        static RenderTarget2D[] fbos = new RenderTarget2D[1];
        static Color[] buffer;
        static Texture2D arrow;

        internal static bool Visible = false;
        internal static float FadeOpacity = 1f;

        public static void Initialize()
        {
        }

        public static void LoadContent()
        {
            arrow = ResourceManager.Load<Texture2D>("GUI/Maps/MagnetArrow");
        }

        public static void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            for (int i = 0; i < fbos.Length && fbos[i] != null; i++)
            {
                fbos[i].Dispose();
                fbos[i] = null;
            }

            buffer = new Color[w * h / VALUES_DENSITY / VALUES_DENSITY];
        }

        public static void Update()
        {
            if (!Visible)
                return;

            if (fbos[fbos.Length - 1] != null)
                fbos[fbos.Length - 1].Dispose();
            for (int i = fbos.Length - 1; i > 0; i--)
            {
                fbos[i] = fbos[i - 1];
            }
            fbos[0] = new RenderTarget2D(GraphicsEngine.Renderer.GraphicsDevice, Main.WindowWidth / VALUES_DENSITY, Main.WindowHeight / VALUES_DENSITY);

            Renderer r = GraphicsEngine.Renderer;
            DrawFBO(r);
        }

        public static void Draw(Renderer renderer)
        {
            if (!Visible)
                return;

            for (int i = 0; i < fbos.Length; i++)
            {
                if (fbos[i] != null)
                {
                    renderer.Draw(fbos[i], new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight), Color.White * OPACITY * FadeOpacity);
                }
            }

            DrawArrows(renderer);
        }

        private static void DrawFBO(Renderer renderer)
        {
            int sx = 0, sy = 0;
            Utilities.Tools.ScreenToGameCoords(ref sx, ref sy);
            float step = VALUES_DENSITY / Settings.GameScale;

            float x, y;
            int ix, iy;
            float t;
            for (ix = 0, x = sx; ix < fbos[0].Width; ix++, x += step)
            {
                for (iy = 0, y = sy; iy < fbos[0].Height; iy++, y += step)
                {
                    t = Math.Abs(Components.ComponentsManager.GetMagneticField(x, y).Length()) / 255;
                    t = t > 1 ? 1 : t < 0 ? 0 : t;
                    buffer[ix + iy * fbos[0].Width] = Color.Red * t;
                }
            }

            fbos[0].SetData<Color>(buffer);
        }

        private static void DrawArrows(Renderer renderer)
        {
            float sx = 0, sy = 0;
            float tx, ty;
            float mul = Settings.GameScale < 3 ? 2 : 1;
            Utilities.Tools.ScreenToGameCoords(ref sx, ref sy);
            Logics.GridHelper.GridCoords(ref sx, ref sy);
            sx -= sx % (GridDraw.Step * mul);
            sy -= sy % (GridDraw.Step * mul);
            tx = sx;
            ty = sy;
            Utilities.Tools.GameToScreenCoords(ref tx, ref ty);
            float step = GridDraw.Step;

            float x, y;
            float ix, iy;
            Vector2 t;
            float a;
            Color c = Color.White * FadeOpacity * 0.7f;
            for (ix = tx, x = sx; ix < Main.windowWidth; ix += GridDraw.Step * Settings.GameScale * mul, x += step * mul)
                for (iy = ty, y = sy; iy < Main.windowHeight; iy += GridDraw.Step * Settings.GameScale * mul, y += step * mul)
                {
                    t = Components.ComponentsManager.GetMagneticField(x, y);
                    a = (float)Math.Atan2(t.X, -t.Y);
                    renderer.Draw(arrow, new Vector2(ix, iy), null, c, a, new Vector2(8, 16), Math.Min((Math.Abs(t.X) + Math.Abs(t.Y)) / 100, 1));
                }
        }
    }
}
