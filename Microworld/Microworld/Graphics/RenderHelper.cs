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

namespace MicroWorld.Graphics
{
    public static class RenderHelper
    {
        public static Texture2D dottedOutline;
        static Color outlineColor = new Color(255, 255, 255) * 0.75f;

        public static void Init()
        {
            dottedOutline = ResourceManager.Load<Texture2D>("GUI/Checkers");
        }

        public static void SmartDrawRectangle(Texture2D texture, int border, int x, int y, int w, int h, Color c, Renderer r)
        {
            r.Draw(texture, new Rectangle(x, y, border, border), new Rectangle(0, 0, border, border), c);//0,0
            r.Draw(texture, new Rectangle(x + border, y, w - border * 2, border), new Rectangle(border, 0, texture.Width - border * 2, border), c);//1,0
            r.Draw(texture, new Rectangle(x + w - border, y, border, border), new Rectangle(texture.Width - border, 0, border, border), c);//2,0

            r.Draw(texture, new Rectangle(x, y + border, border, h - border * 2), new Rectangle(0, border, border, texture.Height - border * 2), c);//0,1
            r.Draw(texture, new Rectangle(x + border, y + border, w - border * 2, h - border * 2), new Rectangle(border, border, texture.Width - border * 2, texture.Height - border * 2), c);//1,1
            r.Draw(texture, new Rectangle(x + w - border, y + border, border, h - border * 2), new Rectangle(texture.Width - border, border, border, texture.Height - border * 2), c);//2,1

            r.Draw(texture, new Rectangle(x, y + h - border, border, border), new Rectangle(0, texture.Height - border, border, border), c);//0,2
            r.Draw(texture, new Rectangle(x + border, y + h - border, w - border * 2, border), new Rectangle(border, texture.Height - border, texture.Width - border * 2, border), c);//1,2
            r.Draw(texture, new Rectangle(x + w - border, y + h - border, border, border), new Rectangle(texture.Width - border, texture.Height - border, border, border), c);//2,2
        }

        public static void SmartDrawRectangleHollow(Texture2D texture, int border, int x, int y, int w, int h, Color c, Renderer r)
        {
            r.Draw(texture, new Rectangle(x, y, border, border), new Rectangle(0, 0, border, border), c);//0,0
            r.Draw(texture, new Rectangle(x + border, y, w - border * 2, border), new Rectangle(border, 0, texture.Width - border * 2, border), c);//1,0
            r.Draw(texture, new Rectangle(x + w - border, y, border, border), new Rectangle(texture.Width - border, 0, border, border), c);//2,0

            r.Draw(texture, new Rectangle(x, y + border, border, h - border * 2), new Rectangle(0, border, border, texture.Height - border * 2), c);//0,1
            r.Draw(texture, new Rectangle(x + w - border, y + border, border, h - border * 2), new Rectangle(texture.Width - border, border, border, texture.Height - border * 2), c);//2,1

            r.Draw(texture, new Rectangle(x, y + h - border, border, border), new Rectangle(0, texture.Height - border, border, border), c);//0,2
            r.Draw(texture, new Rectangle(x + border, y + h - border, w - border * 2, border), new Rectangle(border, texture.Height - border, texture.Width - border * 2, border), c);//1,2
            r.Draw(texture, new Rectangle(x + w - border, y + h - border, border, border), new Rectangle(texture.Width - border, texture.Height - border, border, border), c);//2,2
        }

        public static void DrawDottedOutline(double x, double y, double w, double h, bool coordsScaled, Renderer r)
        {
            if (!coordsScaled)
            {
                Utilities.Tools.GameToScreenCoords(ref x, ref y, ref w, ref h);
            }
            x -= 2;
            y -= 2;
            w += 4;
            h += 4;
            int offset = -(int)(Main.Ticks % (dottedOutline.Width * 4)) / 4;//- for direction, 8 for smoothness
            bool b = r.IsScaeld;
            r.End();
            r.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, 
                RasterizerState.CullNone);
            //top
            r.Draw(dottedOutline, new Rectangle((int)x, (int)y, (int)w, (int)2),
                new Rectangle(offset, 0, (int)w, (int)2), outlineColor);
            //bottom
            r.Draw(dottedOutline, new Rectangle((int)x, (int)(y + h - 1), (int)w, (int)2),
                new Rectangle(offset, (int)(h - 2), (int)w, (int)2), outlineColor);
            //left
            r.Draw(dottedOutline, new Rectangle((int)x, (int)y, (int)2, (int)h - 2),
                new Rectangle(0, offset, (int)2, (int)h), outlineColor);
            //right
            r.Draw(dottedOutline, new Rectangle((int)(x + w - 2), (int)y, (int)2, (int)h - 1),
                new Rectangle((int)(w - 2), offset, (int)2, (int)h), outlineColor);
            r.End();
            r.Begin(b);
        }

        //MaxValue for screenborder
        public static void DrawDottedLines(float[] x1, float[] y1, float[] x2, float[] y2, Color color, Renderer renderer, 
            bool areGameCoords = false, bool animate = true)
        {
            if (x1.Length == 0 || x1.Length != x2.Length || x1.Length != y1.Length || y1.Length != y2.Length)
                throw new Exception("All coord arrays should be the same length and should not be empty");
            bool a = renderer.IsDrawing;
            bool b = renderer.IsScaeld;
            if (a) renderer.End();
            renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None,
                RasterizerState.CullNone);
            Rectangle r = new Rectangle();
            var vr = GraphicsEngine.camera.VisibleRectangle;
            int animation = -(int)(Main.Ticks % 32) / 4;
            for (int i = 0; i < x1.Length; i++)
            {
                if (areGameCoords)//convert
                {
                    if (x1[i] == float.MaxValue) x1[i] = vr.X + vr.Width;
                    if (y1[i] == float.MaxValue) y1[i] = vr.Y + vr.Height;
                    if (x2[i] == float.MaxValue) x2[i] = vr.X + vr.Width;
                    if (y2[i] == float.MaxValue) y2[i] = vr.Y + vr.Height;

                    Utilities.Tools.GameToScreenCoords(ref x1[i], ref y1[i]);
                    Utilities.Tools.GameToScreenCoords(ref x2[i], ref y2[i]);
                }
                else
                {
                    if (x1[i] == float.MaxValue) x1[i] = Main.WindowWidth;
                    if (y1[i] == float.MaxValue) y1[i] = Main.WindowHeight;
                    if (x2[i] == float.MaxValue) x2[i] = Main.WindowWidth;
                    if (y2[i] == float.MaxValue) y2[i] = Main.WindowHeight;
                }
                r.X = (int)x1[i];
                r.Y = (int)y1[i];
                r.Width = (int)(x2[i] - x1[i]);
                r.Height = (int)(y2[i] - y1[i]);
                r.Width = r.Width <= 0 ? 1 : r.Width;
                r.Height = r.Height <= 0 ? 1 : r.Height;
                if (animate)
                {
                    renderer.Draw(dottedOutline, r,
                        new Rectangle((int)(r.Height == 1 ? r.X + animation : r.X),
                                     (int)(r.Width == 1 ? r.Y + animation : r.Y),
                                     r.Width, r.Height),
                            color);
                }
                else
                    renderer.Draw(dottedOutline, r, r, color);
            }
            renderer.End();
            if (a) renderer.Begin(b);
        }

        public static void DrawDottedLinesToBorders(Vector2[] p, Color color, Renderer renderer,
            bool areGameCoords = false, bool animate = true)
        {
            if (p == null || p.Length == 0)
                throw new Exception("Array must not be empty");
            bool a = renderer.IsDrawing;
            bool b = renderer.IsScaeld;
            if (a) renderer.End();
            renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None,
                RasterizerState.CullNone);
            Rectangle r = new Rectangle();
            var vr = GraphicsEngine.camera.VisibleRectangle;
            int animation = -(int)(Main.Ticks % 32) / 4;
            float xw = vr.X + vr.Width;
            float yh = vr.Y + vr.Height;
            Utilities.Tools.GameToScreenCoords(ref xw, ref yh);
            for (int i = 0; i < p.Length; i++)
            {
                if (areGameCoords)//convert
                {
                    Utilities.Tools.GameToScreenCoords(ref p[i].X, ref p[i].Y);
                }
                //stretch by x
                r.X = (int)p[i].X;
                r.Y = (int)p[i].Y;
                r.Width = (int)(xw - p[i].X);
                r.Height = 1;
                if (animate)
                {
                    renderer.Draw(dottedOutline, r,
                        new Rectangle((int)(r.Height == 1 ? r.X + animation : r.X),
                                     (int)(r.Width == 1 ? r.Y + animation : r.Y),
                                     r.Width, r.Height),
                            color);
                }
                else
                    renderer.Draw(dottedOutline, r, r, color);
                //stretch by y
                r.X = (int)p[i].X;
                r.Y = (int)p[i].Y;
                r.Width = 1;
                r.Height = (int)(yh - p[i].Y);
                if (animate)
                {
                    renderer.Draw(dottedOutline, r,
                        new Rectangle((int)(r.Height == 1 ? r.X + animation : r.X),
                                     (int)(r.Width == 1 ? r.Y + animation : r.Y),
                                     r.Width, r.Height),
                            color);
                }
                else
                    renderer.Draw(dottedOutline, r, r, color);
            }
            renderer.End();
            if (a) renderer.Begin(b);
        }

        public static void DrawDottedLinesToBorders(Point[] p, Color color, Renderer renderer,
            bool areGameCoords = false, bool animate = true)
        {
            if (p == null || p.Length == 0)
                throw new Exception("Array must not be empty");
            Vector2[] r = new Vector2[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                r[i] = new Vector2(p[i].X, p[i].Y);
            }
            DrawDottedLinesToBorders(r, color, renderer, areGameCoords, animate);
        }

        public static void DrawDottedCircle(float radius, Vector2 center, int segments, float rotation, Renderer renderer, 
            Color col)
        {
            if (segments == 0)
                return;
            VertexPositionColorTexture[] arr = new VertexPositionColorTexture[segments];

            int c = 0;
            Vector2 off = center;
            for (double i = 0; i <= Math.PI * 2 - 0.001d; i += Math.PI * 2 / segments)
            {
                arr[c] = new VertexPositionColorTexture(
                    new Vector3((float)Math.Cos(i + rotation) * radius + off.X, (float)Math.Sin(i + rotation) * radius + off.Y, 0),
                    col, new Vector2());
                c++;
            }

            bool a = renderer.IsDrawing;
            bool b = renderer.IsScaeld;
            if (!renderer.IsDrawing)
                renderer.BeginUnscaled();
            Main.renderer.Draw(MicroWorld.Graphics.GraphicsEngine.pixel, new Rectangle(0, 0, 0, 0), Color.White);
            renderer.End();

            renderer.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.LineList,
                arr, 0, arr.Length / 2);

            if (a) renderer.Begin(b);
        }


    }
}
