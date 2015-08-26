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
    unsafe class PulseFormerCloseup : HUDScene
    {
        Texture2D progressbar, bg;
        Components.PulseFormer selectedPF;
        internal Components.PulseFormer SelectedPF
        {
            get { return selectedPF; }
            set
            {
                selectedPF = value;
                if (selectedPF != null) updateFBO = true;
            }
        }
        bool updateFBO = false;
        Color[] fbobuffer;
        MicroWorld.Graphics.GUI.Elements.MenuButton close;

        public override void Initialize()
        {
            Layer = 550;
            ShouldBeScaled = false;

            close = new MenuButton((Main.WindowWidth - 602) / 2 + 555, (Main.WindowHeight - 317) / 2, 41, 17, "x");
            close.foreground = Color.White;
            close.background = Shortcuts.BG_COLOR;
            close.TextOffset = new Vector2(6, 3);
            close.MoveOnMouseOver = false;
            close.onClicked += new MicroWorld.Graphics.GUI.Elements.Button.ClickedEventHandler(close_onClicked);
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
            bg = ResourceManager.Load<Texture2D>("GUI/HUD/PulseFormerCloseup/bg");
            close.Font = ResourceManager.Load<SpriteFont>("Fonts/CourierNew_10");
            progressbar = new Texture2D(GraphicsEngine.Renderer.GraphicsDevice, 558, 37);
            fbobuffer = new Color[progressbar.Width * progressbar.Height];
            base.LoadContent();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            close.Position = new Vector2((w - 602) / 2 + 555, (h - 317) / 2);
            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void Update()
        {
            if (selectedPF == null)
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
        public void UpdateProgressBar()
        {
            Components.Logics.PulseFormerLogics l = (Components.Logics.PulseFormerLogics)selectedPF.Logics;
            //if (l.pulsesOld == l.pulses)
            //    return;
            if (l.pulses.Length == 0)
            {
                for (int i = 0; i < fbobuffer.Length; i++)
                {
                    fbobuffer[i] = Shortcuts.BG_COLOR;
                }
            }
            else
            {
                float v = 0;
                Color c = Color.White;
                for (int x = 0; x < progressbar.Width; x++)
                {
                    v = l.pulses[x * l.pulses.Length / progressbar.Width];
                    c = Color.White * v;
                    for (int y = 0; y < progressbar.Height; y++)
                    {
                        fbobuffer[y * progressbar.Width + x] = c;
                    }
                }
            }
            progressbar.SetData<Color>(fbobuffer);

            l.pulsesOld = l.pulses;
        }
        #endregion

        int OffsetX = 0;
        float fadeopacity = 0f;
        public override void Draw(Renderer renderer)
        {
            renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight), Shortcuts.BG_COLOR * fadeopacity);

            Vector2 position = new Vector2((Main.WindowWidth - 602) / 2, (Main.WindowHeight - 317) / 2);
            Vector2 size = new Vector2(602, 317);
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
            renderer.Draw(progressbar, new Vector2(position.X + 22, position.Y + 197), Color.White);

            if (b)
                renderer.ResetScissorRectangle();
        }

        #region Fades
        public override void onShow()
        {
            OffsetX = -602;
            fadeopacity = 0f;
            updateFBO = true;
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
