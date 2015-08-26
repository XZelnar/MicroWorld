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

namespace MicroWorld.Graphics.GUI.Scene
{
    class YesNoMessageBox : HUDScene
    {
        public static SpriteFont font;
        //public static Texture2D bgtexture = null;

        //public Color background = Color.Black;

        public Vector2 position = new Vector2(), size = new Vector2();

        public String NoText
        {
            get { return no.Text; }
            set { no.Text = value == null ? "No" : value; }
        }
        public String YesText
        {
            get { return yes.Text; }
            set { yes.Text = value == null ? "Yes" : value; }
        }

        public String LUAVarToSet = null;

        MenuButton yes, no;
        Label text;
        public String Text
        {
            get { return text.text; }
            set
            {
                text.text = value;
                position = new Vector2((Main.graphics.PreferredBackBufferWidth - text.size.X) / 2 - 5,
                    (Main.graphics.PreferredBackBufferHeight - size.Y) / 2 - 15);
                size = new Vector2(text.size.X + 10, text.size.Y + 30);
                text.position = position + new Vector2(5, 5);
                no.Position = new Vector2((int)position.X, (int)(position.Y + size.Y) - 20);
                yes.Position = new Vector2((int)(position.X + size.X / 2), (int)(position.Y + size.Y) - 20);
            }
        }

        #region Events
        public class ButtonClickedArgs
        {
            public int button;//yes == 1, no == 0
        }
        public delegate void ButtonClickedEventHandler(object sender, ButtonClickedArgs e);
        public event ButtonClickedEventHandler onButtonClicked;
        #endregion

        private YesNoMessageBox(String txt)
        {
            text = new Label(0, 0, txt);
            text.TextAlignment = Graphics.Renderer.TextAlignment.Center;
            text.foreground = Color.White;
            position = new Vector2((Main.graphics.PreferredBackBufferWidth - text.size.X) / 2 - 5,
                (Main.graphics.PreferredBackBufferHeight - size.Y) / 2 - 15);
            size = new Vector2(text.size.X + 10, text.size.Y + 30);
            text.position = position + new Vector2(5, 5);
            no = new MenuButton((int)position.X, (int)(position.Y + size.Y) - 23, (int)(size.X / 2), 23, "No");
            yes = new MenuButton((int)(position.X + size.X / 2), (int)(position.Y + size.Y) - 23, (int)(size.X / 2), 23, "Yes");
            yes.onClicked += new Button.ClickedEventHandler(yesClick);
            no.onClicked += new Button.ClickedEventHandler(noClick);
            yes.Font = MenuFrameScene.ButtonFont;
            no.Font = MenuFrameScene.ButtonFont;
            yes.TextOffset = new Vector2(5, 2);
            no.TextOffset = yes.TextOffset;
            no.Initialize();
            yes.Initialize();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            position = new Vector2((Main.graphics.PreferredBackBufferWidth - text.size.X) / 2 - 5,
                (Main.graphics.PreferredBackBufferHeight - size.Y) / 2 - 15);
            size = new Vector2(text.size.X + 10, text.size.Y + 30);
            text.position = position + new Vector2(5, 5);
            no.Position = new Vector2((int)position.X, (int)(position.Y + size.Y) - 23);
            yes.Position = new Vector2((int)(position.X + size.X / 2), (int)(position.Y + size.Y) - 23);
            yes.Size = new Vector2(size.X / 2, 23);
            no.Size = yes.Size;

            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void Dispose()
        {
            no.Dispose();
            yes.Dispose();
            text.Dispose();
            no = null;
            yes = null;
            text = null;
            base.Dispose();
        }

        public void yesClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            if (onButtonClicked != null) onButtonClicked(this, new ButtonClickedArgs() { button = 1 });

            if (LUAVarToSet != null && LUAVarToSet != "")
                Logics.LevelEngine.scripts.DoString(LUAVarToSet + "=1");

            GUIEngine.RemoveHUDScene(this);
        }

        public void noClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            if (onButtonClicked != null) onButtonClicked(this, new ButtonClickedArgs() { button = 0 });

            if (LUAVarToSet != null && LUAVarToSet != "")
                Logics.LevelEngine.scripts.DoString(LUAVarToSet + "=0");

            GUIEngine.RemoveHUDScene(this);
        }

        public static YesNoMessageBox Show(String txt)
        {
            var a = new YesNoMessageBox(txt);
            a.Initialize();
            a.isVisible = true;
            GUIEngine.AddHUDScene(a);
            return a;
        }

        public override void Initialize()
        {
            Layer = 1000;

            yes.Initialize();
            no.Initialize();
            text.Initialize();
        }

        public override void Update()
        {
            yes.Update();
            no.Update();
            if (bgopacity < 0.5f)
                bgopacity += 0.02f;
        }

        float bgopacity = 0f;
        public override void Draw(Renderer renderer)
        {
            renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight), Shortcuts.BG_COLOR * bgopacity);

            RenderHelper.SmartDrawRectangle(GraphicsEngine.bg, 5,
                (int)position.X - 2, (int)position.Y - 2, (int)size.X + 4, (int)size.Y + 4, Color.White,
                renderer);

            text.Draw(renderer);
            yes.Draw(renderer);
            no.Draw(renderer);

            renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)no.position.X, (int)no.position.Y, (int)size.X, 1), Color.White);
            renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)yes.position.X, (int)yes.position.Y, 1, 23), Color.White);
        }

        public override void onShow()
        {
            bgopacity = 0f;
            base.onShow();
        }

        #region IO
        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            if (yes == null || no == null) return;
            try
            {
                //if (IsIn(e.curState.X, e.curState.Y))
                {
                    yes.onButtonDown(e);
                    no.onButtonDown(e);
                }
            }
            catch { }
            e.Handled = true;
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            if (yes == null || no == null) return;
            try
            {
                //if (IsIn(e.curState.X, e.curState.Y))
                {
                    yes.onButtonUp(e);
                    no.onButtonUp(e);
                }
            }
            catch { }
            e.Handled = true;
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            if (yes == null || no == null) return;
            try
            {
                //if (IsIn(e.curState.X, e.curState.Y))
                {
                    yes.onButtonClick(e);
                    no.onButtonClick(e);
                }
            }
            catch { }
            e.Handled = true;
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            if (yes == null || no == null) return;
            try
            {
                //if (IsIn(e.curState.X, e.curState.Y))
                //{
                    yes.onMouseMove(e);
                    no.onMouseMove(e);
                //}
            }
            catch { }
            e.Handled = true;
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y))
            {
                yes.onMouseWheelMove(e);
                no.onMouseWheelMove(e);
            }
            e.Handled = true;
        }

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
            yes.onKeyDown(e);
            no.onKeyDown(e);
            e.Handled = true;
        }

        public override void onKeyUp(InputEngine.KeyboardArgs e)
        {
            yes.onKeyDown(e);
            no.onKeyDown(e);
            e.Handled = true;
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            if (e.key == Keys.Escape.GetHashCode())
            {
                noClick(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.N.GetHashCode())
            {
                noClick(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.Y.GetHashCode())
            {
                yesClick(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.Enter.GetHashCode())
            {
                yesClick(null, null);
                e.Handled = true;
                return;
            }
            yes.onKeyDown(e);
            no.onKeyDown(e);
            e.Handled = true;
        }
        #endregion

        public override bool IsIn(int x, int y)
        {
            return x >= position.X && x < position.X + size.X
                && y >= position.Y && y < position.Y + size.Y;
        }

    }
}
