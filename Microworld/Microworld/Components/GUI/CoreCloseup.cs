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
using MicroWorld.Graphics.GUI.Scene;
using MicroWorld.Graphics;

namespace MicroWorld.Components.GUI
{
    unsafe class CoreCloseup : HUDScene
    {
        Texture2D progressbar, bg;
        Components.Core selectedCore;
        internal Components.Core SelectedCore
        {
            get { return selectedCore; }
            set
            {
                selectedCore = value;
                if (selectedCore != null) updateFBO = true;
            }
        }
        bool updateFBO = false;
        Color[] fbobuffer;
        MicroWorld.Graphics.GUI.Elements.MenuButton close;

        public override void Initialize()
        {
            Layer = 550;
            ShouldBeScaled = false;

            close = new MenuButton((Main.WindowWidth - 602) / 2 + 555, (Main.WindowHeight - 338) / 2, 41, 17, "x");
            close.foreground = Color.White;
            close.background = Shortcuts.BG_COLOR;
            close.TextOffset = new Vector2(6, 3);
            close.MoveOnMouseOver = false;
            close.onClicked += new Button.ClickedEventHandler(close_onClicked);
            controls.Add(close);

            base.Initialize();
        }

        void close_onClicked(object sender, InputEngine.MouseArgs e)
        {
            close.StaySelected = false;
            close.isMouseOver = false;
            close.WasInitiallyDrawn = false;
            Close();
        }

        public override void LoadContent()
        {
            bg = ResourceManager.Load<Texture2D>("GUI/HUD/CoreCloseup/bg");
            close.Font = ResourceManager.Load<SpriteFont>("Fonts/CourierNew_10");
            progressbar = new Texture2D(GraphicsEngine.Renderer.GraphicsDevice, 558, 37);
            fbobuffer = new Color[progressbar.Width * progressbar.Height];
            base.LoadContent();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            close.Position = new Vector2((w - 602) / 2 + 555, (h - 338) / 2);
            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void OnGraphicsDeviceReset()
        {
            updateFBO = true;
            base.OnGraphicsDeviceReset();
        }

        public override void Update()
        {
            if (selectedCore == null)
                Close();
            if (updateFBO || Settings.GameState == Settings.GameStates.Running)
            {
                UpdateProgressBar();
                updateFBO = false;
            }

            if (fadeopacity < 0.6f) fadeopacity += 0.01f;
            if (OffsetX < 0)
                OffsetX += 15;
            if (OffsetX > 0)
                OffsetX = 0;

            base.Update();
        }

        #region Graph
        #region Circle
        Vector2 circleCenter = new Vector2(297.5f, 142.5f);
        float astart = (float)Math.PI * 5 / 4;
        float divider = 0.995f;
        Color circleColor = Color.White;
        private void DrawCircle(MicroWorld.Graphics.Renderer renderer, float state)
        {
            Vector2 Position = new Vector2((Main.WindowWidth - 602) / 2, (Main.WindowHeight - 338) / 2);
            Texture2D circle = Graphics.CoreGraphics.circle;
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
            Texture2D circle = Graphics.CoreGraphics.circle;
            Vector2 Position = new Vector2((Main.WindowWidth - 602) / 2, (Main.WindowHeight - 338) / 2);
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
            var g = selectedCore.Graphics as Graphics.CoreGraphics;
            int t = (int)(state * 100);
            String ts = t.ToString();
            int w = Graphics.CoreGraphics.percent.Width + ts.Length * 16;
            w /= 4;
            Vector2 pos = new Vector2((Main.windowWidth - 602) / 2 + 247 + (101 - w*2) / 2, (Main.WindowHeight - 338) / 2 + circleCenter.Y - 7f);
            for (int i = 0; i < ts.Length; i++)
            {
                renderer.Draw(Graphics.CoreGraphics.digits[ts[i] - '0'], new Rectangle((int)pos.X + i * 8, (int)pos.Y, 8, 10), Color.White);
            }
            renderer.Draw(Graphics.CoreGraphics.percent, new Rectangle((int)pos.X + ts.Length * 8, (int)pos.Y, 12, 10), Color.White);
        }
        #endregion

        public void UpdateProgressBar()
        {
            Components.Logics.CoreLogics l = (Components.Logics.CoreLogics)selectedCore.Logics;

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
                    for (y = 20; y < 37; y++)
                    {
                        fbobuffer[y * progressbar.Width + i] = Shortcuts.BG_COLOR;
                    }
                }
            }

            //if (l.lasttarget != l.target)
            {
                bool v = false;
                for (int x = 0; x < progressbar.Width; x++)
                {
                    v = l.target[x * l.target.Length / progressbar.Width];
                    if (v)
                    {
                        for (int y = 0; y < 18; y++)
                        {
                            fbobuffer[y * progressbar.Width + x] = Color.White;
                        }
                    }
                    else
                    {
                        for (int y = 0; y < 18; y++)
                        {
                            fbobuffer[y * progressbar.Width + x] = Shortcuts.BG_COLOR;
                        }
                    }
                }
                l.lasttarget = l.target;
            }

            //if (l.lastresult != l.result)
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
                        for (int y = 20; y < 37; y++)
                        {
                            fbobuffer[y * progressbar.Width + x] = Color.White;
                        }
                    }
                    else
                    {
                        for (int y = 20; y < 37; y++)
                        {
                            fbobuffer[y * progressbar.Width + x] = Shortcuts.BG_COLOR;
                        }
                    }
                }
                l.lasttarget = l.target;
            }

            progressbar.SetData<Color>(fbobuffer);
        }
        #endregion

        int OffsetX = 0;
        float fadeopacity = 0f;
        public override void Draw(Renderer renderer)
        {
            renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight), Shortcuts.BG_COLOR * fadeopacity);

            Vector2 position = new Vector2((Main.WindowWidth - 602) / 2, (Main.WindowHeight - 338) / 2);
            Vector2 size = new Vector2(602, 338);
            bool b = OffsetX != 0;
            if (b)
            {
                renderer.SetScissorRectangle(position.X, position.Y, size.X, size.Y, false);
                renderer.End();
                renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, null,
                    null, MicroWorld.Graphics.GraphicsEngine.s_ScissorsOn, null, Matrix.CreateTranslation(OffsetX, 0, 0));
            }

            renderer.Draw(bg, position, Color.White);
            base.Draw(renderer);

            float p = (selectedCore.Logics as Logics.CoreLogics).CorrectPercent;
            if (Settings.GameState == Settings.GameStates.Stopped)
                p = 0;

            renderer.End();
            DrawCircle(renderer, p);
            renderer.BeginUnscaled();
            if (b)
            {
                renderer.SetScissorRectangle(position.X, position.Y, size.X, size.Y, false);
                renderer.End();
                renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, null,
                    null, MicroWorld.Graphics.GraphicsEngine.s_ScissorsOn, null, Matrix.CreateTranslation(OffsetX, 0, 0));
            }
            renderer.Draw(progressbar, new Vector2(position.X+22, position.Y+280), Color.White);
            DrawDigits(renderer, p);

            if (b)
                renderer.ResetScissorRectangle();
        }

        #region Fades
        public override void onShow()
        {
            OffsetX = -602;
            fadeopacity = 0f;
            base.onShow();
        }
        #endregion

        #region inputs
        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            base.onButtonClick(e);
            //Close();
            e.Handled = true;
        }

        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            base.onButtonDown(e);
            e.Handled = true;
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            base.onButtonUp(e);
            e.Handled = true;
        }

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
            base.onKeyDown(e);
            e.Handled = true;
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            base.onKeyPressed(e);
            Close();
            e.Handled = true;
        }

        public override void onKeyUp(InputEngine.KeyboardArgs e)
        {
            base.onKeyUp(e);
            e.Handled = true;
        }
        #endregion
    }
}
