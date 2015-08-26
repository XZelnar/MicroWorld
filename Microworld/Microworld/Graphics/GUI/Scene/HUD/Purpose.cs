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

namespace MicroWorld.Graphics.GUI.Scene
{
    class Purpose : HUDScene
    {
        SpriteFont font;

        Elements.MenuButton bok;
        Elements.Label lAvalable, lPurpose;

        Vector2 pos = new Vector2(),
            size = new Vector2(401, 200);
        Texture2D bg;

        bool ignoreNextClick = false;
        public new void Initialize()
        {
            font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_18");
            ShouldBeScaled = false;
            Layer = 650;

            bok = new Elements.MenuButton(680, 0, 400, 30, "Got it!");
            bok.Font = font;
            bok.TextOffset = new Vector2(0, 3);
            bok.onClicked += new Elements.Button.ClickedEventHandler(bokClick);
            controls.Add(bok);

            lAvalable = new Elements.Label(0, 0, "Avalable components:");
            lAvalable.foreground = Color.White;
            controls.Add(lAvalable);

            lPurpose = new Elements.Label(0, 0, "Objective:");
            lPurpose.foreground = Color.White;
            controls.Add(lPurpose);

            base.Initialize();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);
            pos.X = (w - size.X) / 2;
            pos.Y = (h - size.Y) / 2;
            lAvalable.position = pos + new Vector2(5, 5);
            bok.position = pos + new Vector2(0, size.Y - bok.Size.Y);
        }

        public override void LoadContent()
        {
            bg = ResourceManager.Load<Texture2D>("GUI/BackgroundBlack");
            base.LoadContent();
        }

        public void bokClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            if (isclosing) return;
            isclosing = true;
        }

        public void initOnShow()
        {
            lock (compToDraw)
            {
                compToDraw.Clear();
                ComponentSelector.CSComponent c;
                for (int i = 3; i < GUIEngine.s_componentSelector.components.Count; i++)
                {
                    c = GUIEngine.s_componentSelector.components[i];
                    if (c.Avalable != 0)
                        compToDraw.Add(c);
                }
                base.onShow();
                ignoreNextClick = true;
                //purpose
                int x = 5;
                int y = 30;
                for (int i = 0; i < compToDraw.Count; i++)
                {
                    if (x >= size.X - 40)
                    {
                        x = 0;
                        y += 50;
                    }
                    x += 50;
                }
                y += 40;
                lPurpose.position = pos + new Vector2(5, y + 10);
                int count = 0;
                for (int i = 0; i < Components.ComponentsManager.Components.Count; i++)
                {
                    if (Components.ComponentsManager.Components[i] is Components.Properties.ICore)
                        count++;
                }
                lPurpose.text = "Objective:\r\n  Overload " + count.ToString() + " core(s)";
                //bump up size and position
                size.Y = y + 20 + 40 + 30;
                pos.Y = (Main.WindowHeight - size.Y) / 2;
                lAvalable.position = pos + new Vector2(5, 5);
                lPurpose.position = pos + new Vector2(0, y + 10);
                bok.position.Y = pos.Y + size.Y - bok.Size.Y;
            }
        }

        int tupdates = 0;
        public override void Update()
        {
            tupdates++;
            if (tupdates >= 20)
            {
                tupdates = 0;
                initOnShow();
            }

            base.Update();
        }

        List<ComponentSelector.CSComponent> compToDraw = new List<ComponentSelector.CSComponent>();
        int OffsetX = 0;
        public override void Draw(Renderer renderer)
        {
            #region fadeUpdate
            if (isclosing)
            {
                if (OffsetX > -size.X)
                    OffsetX -= 15;
                if (OffsetX <= -size.X)
                {
                    OffsetX = -(int)size.X;
                    Close();
                }
            }
            else
            {
                if (OffsetX < 0)
                    OffsetX += 15;
            }
            if (OffsetX > 0)
                OffsetX = 0;

            fadeopacity = (float)(OffsetX + size.X) / size.X / 2f;
            #endregion

            lock (compToDraw)
            {
                renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight), Shortcuts.BG_COLOR * fadeopacity);
                bool b = OffsetX != 0;
                if (b)
                {
                    renderer.SetScissorRectangle(pos.X, pos.Y, size.X, size.Y, false);
                    renderer.End();
                    renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, null,
                        null, Graphics.GraphicsEngine.s_ScissorsOn, null, Matrix.CreateTranslation(OffsetX, 0, 0));
                }

                RenderHelper.SmartDrawRectangle(GraphicsEngine.bg, 5,
                    (int)pos.X - 2, (int)pos.Y - 2, (int)size.X + 4, (int)size.Y - 30 + 4,
                    Color.White, renderer);

                int x = 5;
                int y = 30;
                for (int i = 0; i < compToDraw.Count; i++)
                {
                    compToDraw[i].DrawAt((int)pos.X + x, (int)pos.Y + y, renderer);
                    x += 50;
                    if (x + 50 >= size.X)
                    {
                        x = 0;
                        y += 50;
                    }
                }
                base.Draw(renderer);
                if (b)
                    renderer.ResetScissorRectangle();
            }
        }

        #region fades
        float fadeopacity = 0f;
        bool isclosing = false;

        public override void onShow()
        {
            initOnShow();
            OffsetX = -(int)size.X;
            isclosing = false;
        }
        #endregion


        #region io
        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            base.onButtonClick(e);
            e.Handled = true;
            if (!IsIn(e.curState.X, e.curState.Y) && !ignoreNextClick)
                Close();
            ignoreNextClick = false;
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

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.onMouseMove(e);
            e.Handled = true;
            int x = 5;
            int y = 30;
            for (int i = 0; i < compToDraw.Count; i++)
            {
                //compToDraw[i].DrawAt((int)pos.X + x, (int)pos.Y + y);
                Rectangle r = new Rectangle((int)pos.X + x, (int)pos.Y + y, 40, 40);
                Point p = new Point(e.curState.X, e.curState.Y);
                if (r.Contains(p))
                {
                    Shortcuts.SetInGameStatus(
                        compToDraw[i].ComponentGraphics.GetCSToolTip() + " (" + compToDraw[i].Avalable.ToString() + " avalable)", "");
                    return;
                }
                x += 50;
                if (x >= size.X)
                {
                    x = 0;
                    y += 50;
                }
            }
        }

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
            base.onKeyDown(e);
            e.Handled = true;
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            base.onKeyPressed(e);
            e.Handled = true;
            if (e.key == Keys.Enter.GetHashCode() || e.key == Keys.Escape.GetHashCode())
            {
                if (isclosing) return;
                isclosing = true;
            }
        }

        public override void onKeyUp(InputEngine.KeyboardArgs e)
        {
            base.onKeyUp(e);
            e.Handled = true;
        }
        #endregion

    }
}
