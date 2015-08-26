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

namespace MicroWorld.Graphics.GUI.Elements
{
    public class MenuAnimatedButton : Button
    {
        public Texture2D bg;
        public Texture2D AnimatedTexture, Glow, SelectedTexture;
        public Texture2D CurTexture
        {
            get
            {
                return isPressed ? SelectedTexture : AnimatedTexture;
            }
        }
        private int _curTick = 0;
        public int EndDelay = 0;
        public int Speed = 7;

        private RenderTarget2D mono_fbo;

        public override Vector2 Size
        {
            get { return base.Size; }
            set
            {
                base.Size = value;
                if (Utilities.Tools.IsRunningOnMono() && !IsFading)
                {
                    try
                    {
                        if (mono_fbo != null)
                            mono_fbo.Dispose();
                        mono_fbo = new RenderTarget2D(Main.renderer.GraphicsDevice, Main.WindowWidth, Main.WindowHeight, false,
                        SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
                    }
                    catch { }
                }
            }
        }

        public MenuAnimatedButton(int x, int y, int w, int h, String txt) : base(x, y, w, h, txt)
        {
        }

        public void InitAnimation(String textName, String textSelectedName, int endDelay, int speed, int offset)
        {
            AnimatedTexture = ResourceManager.Load<Texture2D>(textName);
            SelectedTexture = ResourceManager.Load<Texture2D>(textSelectedName);
            Glow = ResourceManager.Load<Texture2D>("GUI/Menus/glow");
            bg = ResourceManager.Load<Texture2D>("GUI/Menus/ButtonBackground");
            EndDelay = endDelay;
            Speed = speed;
            _curTick = offset;

            if (Utilities.Tools.IsRunningOnMono())
            {
                if (mono_fbo != null)
                    mono_fbo.Dispose();
                mono_fbo = new RenderTarget2D(Main.renderer.GraphicsDevice, Main.WindowWidth, Main.WindowHeight, false,
                SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            }
        }

        #region animation
        System.Threading.Thread fadethread;
        float textopacity = 1f;
        float globalopacity = 1f;
        bool fadeoverride = false;
        public bool IsFading
        {
            get
            {
                return (fadethread != null && fadethread.ThreadState != System.Threading.ThreadState.Stopped) || fadeoverride;
            }
        }
        internal void setIsFade(bool b)
        {
            fadeoverride = b;
        }
        public override void FadeIn()
        {
            while (IsFading)
                System.Threading.Thread.Sleep(1);
            fadethread = new System.Threading.Thread(new System.Threading.ThreadStart(_fadein));
            fadethread.Start();
        }

        private void _fadein()
        {
            globalopacity = 0f;
            isVisible = false;
            var a = position;
            var b = Size;
            int tt = 5;

            textopacity = 0;

            position = position + Size / 2;
            Size = new Vector2(4, 4);
            isVisible = true;
            globalopacity = 1;
            //x
            for (int i = 0; i < 10; i++)
            {
                position.X -= b.X / 200;
                base.size.X += b.X / 100;
                System.Threading.Thread.Sleep(tt);
            }

            for (int i = 0; i < 40; i++)
            {
                position.X -= b.X / 100;
                base.size.X += b.X / 50;
                System.Threading.Thread.Sleep(tt);
            }

            for (int i = 0; i < 10; i++)
            {
                position.X -= b.X / 200;
                base.size.X += b.X / 100;
                System.Threading.Thread.Sleep(tt);
            }
            base.size.X = b.X;
            position.X = a.X;
            //y
            for (int i = 0; i < 10; i++)
            {
                position.Y -= b.Y / 200;
                base.size.Y += b.Y / 100;
                System.Threading.Thread.Sleep(tt);
            }

            for (int i = 0; i < 40; i++)
            {
                position.Y -= b.Y / 100;
                base.size.Y += b.Y / 50;
                System.Threading.Thread.Sleep(tt);
            }

            for (int i = 0; i < 10; i++)
            {
                position.Y -= b.Y / 200;
                base.size.Y += b.Y / 100;
                System.Threading.Thread.Sleep(tt);
            }
            base.size.Y = b.Y;
            position.Y = a.Y;
            //text
            for (int i = 0; i < 100; i++)
            {
                textopacity += 0.01f;
                System.Threading.Thread.Sleep(tt);
            }
        }

        public override void FadeOut()
        {
            while (IsFading)
                System.Threading.Thread.Sleep(1);
            fadethread = new System.Threading.Thread(new System.Threading.ThreadStart(_fadeout));
            fadethread.Start();
        }

        private void _fadeout()
        {
            var a = position;
            var b = Size;
            int tt = 5;

            //textopacity = 0;

            //position = position + size / 2;
            //size = new Vector2(4, 4);
            //text
            for (int i = 0; textopacity > 0; i++)
            {
                textopacity -= 0.01f;
                System.Threading.Thread.Sleep(tt);
            }
            //y
            for (int i = 0; i < 10; i++)
            {
                position.Y += b.Y / 200;
                base.size.Y -= b.Y / 100;
                System.Threading.Thread.Sleep(tt);
            }

            for (int i = 0; i < 40; i++)
            {
                position.Y += b.Y / 100;
                base.size.Y -= b.Y / 50;
                System.Threading.Thread.Sleep(tt);
            }

            for (int i = 0; i < 10; i++)
            {
                position.Y += b.Y / 200;
                base.size.Y -= b.Y / 100;
                System.Threading.Thread.Sleep(tt);
            }
            base.size.Y = 4;
            position.Y = a.Y + b.Y / 2 - 2;
            //x
            for (int i = 0; i < 10; i++)
            {
                position.X += b.X / 200;
                base.size.X -= b.X / 100;
                System.Threading.Thread.Sleep(tt);
            }

            for (int i = 0; i < 40; i++)
            {
                position.X += b.X / 100;
                base.size.X -= b.X / 50;
                System.Threading.Thread.Sleep(tt);
            }

            for (int i = 0; i < 10; i++)
            {
                position.X += b.X / 200;
                base.size.X -= b.X / 100;
                System.Threading.Thread.Sleep(tt);
            }
            globalopacity = 0f;
            isVisible = false;
            base.size.X = b.X;
            position.X = a.X;
            base.size.Y = b.Y;
            position.Y = a.Y;
            isVisible = true;
        }
        #endregion

        public override void Update()
        {
            _curTick++;
            if (_curTick > Size.X / Speed + EndDelay) _curTick = 0;
            if (!IsIn(InputEngine.curMouse.X, InputEngine.curMouse.Y)) _curTick = 0;

            if (Utilities.Tools.IsRunningOnMono() && IsIn(InputEngine.curMouse.X, InputEngine.curMouse.Y) && !IsFading)
            {
                Main.renderer.GraphicsDevice.SetRenderTarget(mono_fbo);
                Main.renderer.GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil | ClearOptions.Target,
                    Color.FromNonPremultiplied(0, 0, 0, 0), 1f, 0);
                Main.renderer.BeginUnscaled();
                DrawStencilView();
                Main.renderer.GraphicsDevice.SetRenderTarget(null);
            }

            base.Update();
        }

        public override void Draw(Renderer renderer)
        {
            if (!isVisible) return;
            if (AnimatedTexture == null)
            {
                base.Draw(renderer);
                return;
            }

            Main.renderer.Draw(bg, new Rectangle((int)position.X, (int)position.Y, (int)Size.X, (int)Size.Y),
                Color.White * 0.95f * globalopacity);
            if (IsIn(InputEngine.curMouse.X,InputEngine.curMouse.Y) && !IsFading)
            {
                if (Utilities.Tools.IsRunningOnMono())
                {
                    Main.renderer.Draw(mono_fbo, new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight), Color.White);
                }
                else
                {
                    DrawStencilView();
                    Main.renderer.BeginUnscaled();
                }
            }
            else
            {
                Main.renderer.Draw(CurTexture, new Rectangle((int)position.X, (int)position.Y, (int)Size.X, (int)Size.Y),
                    new Color(255, 255, 175) * textopacity * globalopacity);
            }
        }

        public void DrawStencilView()
        {
            Main.renderer.MaskClear();
            Main.renderer.MaskBeginDrawOnUnScaled();
            Main.renderer.Draw(CurTexture, new Rectangle((int)position.X + 1, (int)position.Y + 1, (int)Size.X, (int)Size.Y - 1),
                Color.White * textopacity * globalopacity);
            //Main.renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, 0, 600, 600), Color.White);
            //Main.renderer.GraphicsDevice.Clear(ClearOptions.Target, Color.FromNonPremultiplied(0, 0, 0, 0), 1f, 0);
            Main.renderer.End();

            Main.renderer.MaskUse();
            if (textopacity != 0f && Speed > 0)
                Main.renderer.Draw(Glow, new Rectangle((int)(position.X + _curTick * Speed), (int)position.Y, (int)20, (int)Size.Y),
                    Color.Black * textopacity * globalopacity * 0.5f);
            Main.renderer.End();
        }

        #region IO
        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            if (IsFading)
            {
                e.Handled = true;
                return;
            }
            base.onButtonDown(e);
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            if (IsFading)
            {
                e.Handled = true;
                return;
            }
            base.onButtonUp(e);
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            if (IsFading)
            {
                e.Handled = true;
                return;
            }
            base.onButtonClick(e);
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            bool wasover = isMouseOver;
            base.onMouseMove(e);
            if (isMouseOver && !wasover)
            {
                Sound.SoundPlayer.MenuMouseOver();
            }
        }
        #endregion
    }
}
