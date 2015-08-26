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
    class CoreGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, circle, percent;
        public static Texture2D[] digits = new Texture2D[10];
        Texture2D progressbar;
        Color[] fbobuffer;

        static Color errorColor = new Color(196, 58, 58);

        public CoreGraphics()
        {
            Size = new Vector2(64, 64);
            Layer = 50;
        }

        public override void Initialize()
        {
            progressbar = new Texture2D(Main.renderer.GraphicsDevice, 228, 24);
            fbobuffer = new Color[progressbar.Width * progressbar.Height];
            for (int i = 0; i < fbobuffer.Length; i++)
            {
                fbobuffer[i] = Shortcuts.BG_COLOR;
            }
            progressbar.SetData<Color>(fbobuffer);
            
            base.Initialize();
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/Core/Core0cw");
            circle = ComponentsManager.LoadTexture("Components/Core/Circle");
            percent = ComponentsManager.LoadTexture("Components/Core/Numbers/percent");

            for (int i = 0; i < 10; i++)
            {
                digits[i] = ComponentsManager.LoadTexture("Components/Core/Numbers/" + i.ToString());
            }
        }

        public override string GetIconName()
        {
            return "Components/Icons/Core";
        }

        public override string GetCSToolTip()
        {
            return "Core";
        }

        public override string GetComponentSelectorPath()
        {
            return "Cores";
        }

        public override string GetHandbookFile()
        {
            return "Components/Core.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            return new Vector2(32, 32);
        }

        public override Component.Rotation[] GetPossibleRotations()
        {
            return new Component.Rotation[] { Component.Rotation.cw0 };
        }

        public override Vector2 GetSize()
        {
            return Size;
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            return new Vector2(64, 64);
        }

        public override void Reset()
        {
            _ticks = 0;
            circleColor = Color.White;
            base.Reset();
        }

        int _ticks = 0;
        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            Components.Logics.CoreLogics l = (Components.Logics.CoreLogics)parent.Logics;
            UpdateProgressBar();

            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), null, Color.White);
                    renderer.Draw(progressbar, new Rectangle((int)(Position.X + 3.5f), (int)(Position.Y + 55), 57, (int)6f), null, Color.White);
                    float p = l.CorrectPercent;
                    renderer.End();
                    DrawCircle(renderer, p);
                    renderer.Begin();
                    if (Settings.GameState == Settings.GameStates.Stopped)
                        p = 0;
                    DrawDigits(renderer, p);
                    if (l.cur >= l.target.Length)
                    {
                        bool b = l.IsCorrect();
                        _ticks++;
                        circleColor = Color.Lerp(b ? Shortcuts.BG_COLOR : errorColor, Color.White, Math.Abs((float)(_ticks - 75) / 75f));
                        if (_ticks >= 150) _ticks = 0;
                    }
                    break;
                default:
                    break;
            }
        }

        #region Circle
        Vector2 circleCenter = new Vector2(31.83f, 27.6f);
        float astart = (float)Math.PI * 5 / 4;
        float divider = 3.96f;
        Color circleColor = Color.White;
        private void DrawCircle(MicroWorld.Graphics.Renderer renderer, float state)
        {
            Vector2 Center = circleCenter + Position;
            state = state > 1 ? 1 : state < 0 ? 0 : state;
            float acur = astart + 2 * (float)Math.PI * state;
            List<VertexPositionColorTexture> vs = new List<VertexPositionColorTexture>();

            vs.Add(new VertexPositionColorTexture(
                new Vector3(Center.X, Center.Y, 0),
                circleColor,
                new Vector2(0.5f, 0.5f)));
            vs.Add(GetVForAngle(astart));
            if (acur > Math.PI * 5 / 4)
            {
                vs.Add(new VertexPositionColorTexture(
                    new Vector3(Center.X - circle.Width / 2 / divider, Center.Y + circle.Height / 2 / divider, 0),
                    circleColor,
                    new Vector2(0f, 1f)));
                vs.Add(vs[vs.Count - 1]);
                vs.Add(vs[0]);
            }
            if (acur > Math.PI * 7 / 4)
            {
                vs.Add(new VertexPositionColorTexture(
                    new Vector3(Center.X + circle.Width / 2 / divider, Center.Y + circle.Height / 2 / divider, 0),
                    circleColor,
                    new Vector2(1f, 1f)));
                vs.Add(vs[vs.Count - 1]);
                vs.Add(vs[0]);
            }
            if (acur > Math.PI * 9 / 4)
            {
                vs.Add(new VertexPositionColorTexture(
                    new Vector3(Center.X + circle.Width / 2 / divider, Center.Y - circle.Height / 2 / divider, 0),
                    circleColor,
                    new Vector2(1f, 0f)));
                vs.Add(vs[vs.Count - 1]);
                vs.Add(vs[0]);
            }
            if (acur > Math.PI * 11 / 4)
            {
                vs.Add(new VertexPositionColorTexture(
                    new Vector3(Center.X - circle.Width / 2 / divider, Center.Y - circle.Height / 2 / divider, 0),
                    circleColor,
                    new Vector2(0f, 0f)));
                vs.Add(vs[vs.Count - 1]);
                vs.Add(vs[0]);
            }


            vs.Add(GetVForAngle(acur));


            var a = vs.ToArray();
            renderer.GraphicsDevice.Textures[0] = circle;
            renderer.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, a, 0, a.Length / 3);
        }

        public VertexPositionColorTexture GetVForAngle(double a)
        {
            VertexPositionColorTexture v = new VertexPositionColorTexture();
            Vector2 t = new Vector2((float)Math.Cos(a), (float)Math.Sin(a));
            if (Math.Abs(t.X) > Math.Abs(t.Y))
            {
                t *= 1 / Math.Abs(t.X);
            }
            else
            {
                t *= 1 / Math.Abs(t.Y);
            }
            t += new Vector2(1, 1);
            t /= 2;
            t.Y = 1 - t.Y;
            v.TextureCoordinate = t;
            v.Position = new Vector3((t.X - 0.5f) * circle.Width / divider + circleCenter.X + Position.X, 
                (t.Y - 0.5f) * circle.Height / divider + circleCenter.Y + Position.Y, 0);
            v.Color = circleColor;

            return v;
        }
        #endregion

        #region Digits
        private void DrawDigits(MicroWorld.Graphics.Renderer renderer, float state)
        {
            int t = (int)(state * 100);
            String ts = t.ToString();
            int w = percent.Width + ts.Length * 16;
            w /= 4;
            Vector2 pos = Position + new Vector2((Size.X - w) / 2, circleCenter.Y - 3.5f);
            for (int i = 0; i < ts.Length; i++)
            {
                renderer.Draw(digits[ts[i] - '0'], new Rectangle((int)pos.X + i * 4, (int)pos.Y, 4, 5), Color.White);
            }
            renderer.Draw(percent, new Rectangle((int)pos.X + ts.Length * 4, (int)pos.Y, 6, 5), Color.White);
        }
        #endregion

        public void UpdateProgressBar()
        {
            Components.Logics.CoreLogics l = (Components.Logics.CoreLogics)parent.Logics;

            if (l.target.Length == 0)
            {
                for (int i = 0; i < fbobuffer.Length; i++)
                {
                    fbobuffer[i] = Shortcuts.BG_COLOR;
                }
                return;
            }

            if (!l.isFilled)
            {
                int y;
                for (int i = 0; i < progressbar.Width; i++)
                {
                    for (y = 13; y < 24; y++)
                    {
                        fbobuffer[y * progressbar.Width + i] = Shortcuts.BG_COLOR;
                    }
                }
            }

            if (l.lasttarget != l.target)
            {
                bool v = false;
                for (int x = 0; x < progressbar.Width; x++)
                {
                    v = l.target[x * l.target.Length / progressbar.Width];
                    if (v)
                    {
                        for (int y = 0; y < 10; y++)
                        {
                            fbobuffer[y * progressbar.Width + x] = Color.White;
                        }
                    }
                    else
                    {
                        for (int y = 0; y < 10; y++)
                        {
                            fbobuffer[y * progressbar.Width + x] = Shortcuts.BG_COLOR;
                        }
                    }
                }
                l.lasttarget = l.target;
            }

            if (l.lastresult != l.result)
            {
                bool v = false;
                int i = 0;
                for (int x = 0; x < progressbar.Width; x++)
                {
                    i = x * l.result.Length / progressbar.Width;
                    if (i >= l.cur)
                        break;
                    v = l.result[i];
                    if (v)
                    {
                        for (int y = 13; y < 24; y++)
                        {
                            fbobuffer[y * progressbar.Width + x] = Color.White;
                        }
                    }
                    else
                    {
                        for (int y = 13; y < 24; y++)
                        {
                            fbobuffer[y * progressbar.Width + x] = Shortcuts.BG_COLOR;
                        }
                    }
                }
                l.lasttarget = l.target;
            }

            progressbar.SetData<Color>(fbobuffer);
        }

        public override void DrawGhost(int x, int y, MicroWorld.Graphics.Renderer renderer, Component.Rotation rotation)
        {
            if (texture0cw == null) return;
            switch (rotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw, new Rectangle(x, y, 64, 64), new Color(1f, 1f, 1f, 0.5f));
                    break;
                default:
                    renderer.Draw(texture0cw, new Vector2(x, y) + GetCenter(parent.ComponentRotation), null, 
                        new Color(1f, 1f, 1f, 0.5f), rotation.GetHashCode() * (float)Math.PI / 2f, 
                        GetCenter(parent.ComponentRotation), 1);
                    break;
            }
        }

    }
}
