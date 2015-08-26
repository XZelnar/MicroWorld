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
    class EncyclopediaPage : Scene
    {
        Texture2D bg;

        Encyclopedia.Article article;

        public String LastState = "";
        public Scene LastScene = null;

        EncyclopediaBrowserButton goToLink, back;
        public override void Initialize()
        {
            Layer = 100000;

            article = new Encyclopedia.Article();
            article.position = new Vector2(10, 10);
            article.Size = new Vector2(Main.WindowWidth - 20, Main.WindowHeight - 50);

            goToLink = new EncyclopediaBrowserButton(Main.WindowWidth - 215, Main.WindowHeight - 25, 100, 20, "Go to link");
            (goToLink as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            goToLink.foreground = Color.White;
            goToLink.onClicked += new Button.ClickedEventHandler(goToLink_onClicked);
            controls.Add(goToLink);

            back = new EncyclopediaBrowserButton(Main.WindowWidth - 105, Main.WindowHeight - 25, 100, 20, "Back");
            (back as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            back.foreground = Color.White;
            back.onClicked += new Button.ClickedEventHandler(back_onClicked);
            controls.Add(back);

            base.Initialize();

            background = GUIEngine.s_mainMenu.background;
        }

        public override void LoadContent()
        {
            bg = ResourceManager.Load<Texture2D>("GUI/BackgroundBlack");
            base.LoadContent();
        }

        public void OpenPage(String page)
        {
            article.Load(page);
            goToLink.isEnabled = article.HasLink;
        }

        void back_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.ChangeScene(LastScene, LastState);
        }

        void goToLink_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            article.GoToLink();
        }

        public override void onShow()
        {
            goToLink.isEnabled = article.HasLink;
            base.onShow();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            goToLink.position = new Vector2(Main.WindowWidth - 215, Main.WindowHeight - 25);
            back.position = new Vector2(Main.WindowWidth - 105, Main.WindowHeight - 25);

            article.OnResolutionChanged(w, h, oldw, oldh);
            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void Draw(Renderer renderer)
        {
            if (background != null) background.Draw(renderer);
            RenderHelper.SmartDrawRectangle(bg, 5, 0, 0, Main.WindowWidth, Main.WindowHeight, Color.White, renderer);
            foreach (Elements.Control c in controls)
            {
                c.Draw(renderer);
            }
            article.Draw(Main.renderer);
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            base.onKeyPressed(e);
            if (e.key == Keys.Escape.GetHashCode())
            {
                back_onClicked(null, null);
                e.Handled = true;
                return;
            }
        }

        #region ioblock
        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            base.onButtonClick(e);
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

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.onMouseMove(e);
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

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            base.onMouseWheelMove(e);
            e.Handled = true;
        }
        #endregion
    }
}
