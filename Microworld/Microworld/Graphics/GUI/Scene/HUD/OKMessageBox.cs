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
    class OKMessageBox : HUDScene
    {
        public Vector2 position = new Vector2(), size = new Vector2();

        MenuButton ok;
        Label text;

        #region Events
        public class ButtonClickedArgs
        {
            public int button;//yes == 1, no == 0
        }
        public delegate void ButtonClickedEventHandler(object sender, ButtonClickedArgs e);
        public event ButtonClickedEventHandler onButtonClicked;
        #endregion

        private OKMessageBox(String txt)
        {
            text = new Label(0, 0, txt);
            text.TextAlignment = Graphics.Renderer.TextAlignment.Center;
            text.foreground = Color.White;
            position = new Vector2((Main.graphics.PreferredBackBufferWidth - text.size.X) / 2 - 5,
                (Main.graphics.PreferredBackBufferHeight - size.Y) / 2 - 15);
            size = new Vector2(text.size.X + 10, text.size.Y + 30);
            text.position = position + new Vector2(5, 5);
            ok = new MenuButton((int)position.X, (int)(position.Y + size.Y) - 20, (int)(size.X), 20, "OK");
            ok.Font = MenuFrameScene.ButtonFont;
            ok.TextOffset = new Vector2(5, 2);
            ok.onClicked += new Button.ClickedEventHandler(okClick);

            controls.Add(ok);
            controls.Add(text);
        }

        public override void Dispose()
        {
            ok.Dispose();
            text.Dispose();
            ok = null;
            text = null;
            base.Dispose();
        }

        public void okClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            if (onButtonClicked != null) onButtonClicked(this, new ButtonClickedArgs() { button = 1 });
            GUIEngine.RemoveHUDScene(this);
        }

        public static OKMessageBox Show(String txt)
        {
            var a = new OKMessageBox(txt);
            a.Initialize();
            a.isVisible = true;
            GUIEngine.AddHUDScene(a);
            return a;
        }

        public override void Initialize()
        {
            Layer = 1000;
            base.Initialize();
        }

        public override void Update()
        {
            base.Update();
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

            base.Draw(renderer);

            renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)ok.position.X, (int)ok.position.Y, (int)size.X, 1), Color.White);
        }

        public override void onShow()
        {
            bgopacity = 0f;
            base.onShow();
        }

        #region IO
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

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            base.onButtonClick(e);
            e.Handled = true;
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.onMouseMove(e);
            e.Handled = true;
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            base.onMouseWheelMove(e);
            e.Handled = true;
        }

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
            base.onKeyDown(e);
            e.Handled = true;
        }

        public override void onKeyUp(InputEngine.KeyboardArgs e)
        {
            base.onKeyUp(e);
            e.Handled = true;
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            if (e.key == Keys.Escape.GetHashCode())
            {
                okClick(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.Enter.GetHashCode())
            {
                okClick(null, null);
                e.Handled = true;
                return;
            }
            base.onKeyPressed(e);
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
