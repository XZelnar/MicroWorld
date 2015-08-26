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
    static class LightAOE
    {
        const int VALUES_DENSITY = 16;//distance in pixels between 2 value measurements
        const float OPACITY = 0.3f;

        static RenderTarget2D[] fbos = new RenderTarget2D[1];
        static Color[] buffer;

        internal static bool Visible = false;
        internal static float FadeOpacity = 1f;

        public static void Initialize()
        {
        }

        public static void LoadContent()
        {

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
                    t = (float)Components.ComponentsManager.GetBrightness(x, y);
                    buffer[ix + iy * fbos[0].Width] = Color.Yellow * t;
                }
            }

            fbos[0].SetData<Color>(buffer);
        }
    }
}
