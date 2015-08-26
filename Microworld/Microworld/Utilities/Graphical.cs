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

namespace MicroWorld.Utilities
{
    public static class Graphical
    {
        public static void MergeTexturesGradient(RenderTarget2D textureBack, RenderTarget2D textureFront, ref RenderTarget2D output, 
            int state, Graphics.Renderer renderer)
        {
            var fbo = renderer.CurFBO;
            renderer.EnableFBO(output);
            renderer.BeginUnscaled();
            renderer.Draw(textureBack, new Rectangle(0, 0, output.Width, output.Height), Color.White);
            renderer.End();
            renderer.MaskClear();
            renderer.MaskBeginDrawOnUnScaled();
            int h = output.Height;
            if (state > 0)
            {
                renderer.Draw(Graphics.GraphicsEngine.FadeTriangle,
                    new Rectangle(0, 0, state, h), new Rectangle(0, 0, 1, 1), Color.White);
            }
            renderer.Draw(Graphics.GraphicsEngine.FadeTriangle,
                new Rectangle(state, 0, h, h),
                new Rectangle(0, 0, 256, 256),
                Color.White);
            if (state < output.Width - h)
            {
                //renderer.Draw(Graphics.GraphicsEngine.pixel,
                //    new Rectangle(state + h, 0,
                //        output.Width - state - h, h), Color.Black);
            }
            renderer.MaskUse();
            renderer.Draw(textureFront, new Rectangle(0, 0, output.Width, output.Height), Color.White);
            renderer.End();
            renderer.EnableFBO(fbo);
        }

        public static void InvertGradient(RenderTarget2D textureInput, ref RenderTarget2D textureOutput, float state,
            Color c1, Color c2)
        {
            if (textureInput.Width != textureOutput.Width || textureInput.Height != textureOutput.Height)
                throw new Exception("Textures must be the same size!");

            Color[] c = new Color[textureInput.Width * textureInput.Height];
            textureInput.GetData<Color>(c);

            float cr = (float)(c1.R + c2.R) / 2 / 255;
            float cg = (float)(c1.G + c2.G) / 2 / 255;
            float cb = (float)(c1.B + c2.B) / 2 / 255;

            float dr = (float)(c1.R - c2.R) / 255;
            float dg = (float)(c1.G - c2.G) / 255;
            float db = (float)(c1.B - c2.B) / 255;

            float tr, tg, tb;

            for (int i = 0; i < c.Length && c[i].A != 0; i++)
            {
                tr = (float)c[i].R / 255;
                tg = (float)c[i].G / 255;
                tb = (float)c[i].B / 255;

                tr = tr - (tr - cr) * 2 * state;
                tg = tg - (tg - cg) * 2 * state;
                tb = tb - (tb - cb) * 2 * state;

                c[i].R = (byte)(tr * 255);
                c[i].G = (byte)(tg * 255);
                c[i].B = (byte)(tb * 255);
            }

            textureOutput.SetData<Color>(c);
        }

        public static Color MergeColors(Color cstart, Color cdest, float state)
        {
            state = state > 1 ? 1 : state < 0 ? 0 : state;

            float cr = (float)(cstart.R + cdest.R) / 2 / 255;
            float cg = (float)(cstart.G + cdest.G) / 2 / 255;
            float cb = (float)(cstart.B + cdest.B) / 2 / 255;

            float dr = (float)(cstart.R - cdest.R) / 255;
            float dg = (float)(cstart.G - cdest.G) / 255;
            float db = (float)(cstart.B - cdest.B) / 255;

            float tr, tg, tb;

            tr = (float)cstart.R / 255;
            tg = (float)cstart.G / 255;
            tb = (float)cstart.B / 255;

            tr = tr - (tr - cr) * 2 * state;
            tg = tg - (tg - cg) * 2 * state;
            tb = tb - (tb - cb) * 2 * state;

            cdest.R = (byte)(tr * 255);
            cdest.G = (byte)(tg * 255);
            cdest.B = (byte)(tb * 255);

            return cdest;
        }
    }
}
