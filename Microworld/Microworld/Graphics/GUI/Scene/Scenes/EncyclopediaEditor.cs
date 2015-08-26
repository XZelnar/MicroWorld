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
    class EncyclopediaEditor : Scene
    {
        Texture2D bg;

        Encyclopedia.Article article;

        public String LastState = "";
        public Scene LastScene = null;

        String savefn = "";

        EncyclopediaBrowserButton goToLink, exit, loadhtml;
        EncyclopediaBrowserButton load, save, saveas, clear;
        TextBox tbFile, tbLink;
        public override void Initialize()
        {
            #region NececarryStuff
            article = new Encyclopedia.Article();
            article.position = new Vector2(10, 10);
            article.Size = new Vector2(Main.WindowWidth - 20, Main.WindowHeight - 110);

            loadhtml = new EncyclopediaBrowserButton(Main.WindowWidth - 325, Main.WindowHeight - 25, 100, 20, "View page");
            (loadhtml as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            loadhtml.foreground = Color.White;
            loadhtml.onClicked += new Button.ClickedEventHandler(loadhtml_onClicked);
            controls.Add(loadhtml);

            goToLink = new EncyclopediaBrowserButton(Main.WindowWidth - 215, Main.WindowHeight - 25, 100, 20, "Go to link");
            (goToLink as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            goToLink.foreground = Color.White;
            goToLink.onClicked += new Button.ClickedEventHandler(goToLink_onClicked);
            controls.Add(goToLink);

            exit = new EncyclopediaBrowserButton(Main.WindowWidth - 105, Main.WindowHeight - 25, 100, 20, "Exit");
            (exit as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            exit.foreground = Color.White;
            exit.onClicked += new Button.ClickedEventHandler(exit_onClicked);
            controls.Add(exit);

            Label l1 = new Label(11, Main.WindowHeight - 80, "File:");
            l1.foreground = Color.White;
            controls.Add(l1);

            tbFile = new TextBox(60, Main.WindowHeight - 80,
                Main.WindowWidth - 20 - 50, 18);
            tbFile.BackgroundColor = new Color(32, 32, 32, 192);
            tbFile.ForegroundColor = Color.White;
            tbFile.CursorColor = new Color(200, 200, 200);
            tbFile.Multiline = false;
            controls.Add(tbFile);

            Label l = new Label(11, Main.WindowHeight - 50, "Link:");
            l.foreground = Color.White;
            controls.Add(l);

            tbLink = new TextBox(60, Main.WindowHeight - 50,
                Main.WindowWidth - 20 - 50, 20);
            tbLink.BackgroundColor = new Color(32, 32, 32, 192);
            tbLink.ForegroundColor = Color.White;
            tbLink.CursorColor = new Color(200, 200, 200);
            tbLink.Multiline = false;
            tbLink.onTextChanged += new TextBox.TextChangedEventHandler(tbLink_onTextChanged);
            controls.Add(tbLink);
            #endregion

            #region FileManagement
            save = new EncyclopediaBrowserButton(5, Main.WindowHeight - 25, 100, 20, "Save");
            (save as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            save.foreground = Color.White;
            save.onClicked += new Button.ClickedEventHandler(save_onClicked);
            controls.Add(save);

            saveas = new EncyclopediaBrowserButton(115, Main.WindowHeight - 25, 100, 20, "Save As");
            (saveas as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            saveas.foreground = Color.White;
            saveas.onClicked += new Button.ClickedEventHandler(saveas_onClicked);
            controls.Add(saveas);

            load = new EncyclopediaBrowserButton(225, Main.WindowHeight - 25, 100, 20, "Load");
            (load as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            load.foreground = Color.White;
            load.onClicked += new Button.ClickedEventHandler(load_onClicked);
            controls.Add(load);

            clear = new EncyclopediaBrowserButton(335, Main.WindowHeight - 25, 100, 20, "Clear");
            (clear as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            clear.foreground = Color.White;
            clear.onClicked += new Button.ClickedEventHandler(clear_onClicked);
            controls.Add(clear);
            #endregion

            base.Initialize();

            background = GUIEngine.s_mainMenu.background;
        }

        public override void LoadContent()
        {
            bg = ResourceManager.Load<Texture2D>("GUI/BackgroundBlack");
            base.LoadContent();
        }

        void loadhtml_onClicked(object sender, InputEngine.MouseArgs e)
        {
            if (System.IO.File.Exists(tbFile.Text))
                article.LoadHTML(tbFile.Text);
        }

        void clear_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Clear();
        }

        void saveas_onClicked(object sender, InputEngine.MouseArgs e)
        {
            System.Windows.Forms.SaveFileDialog od = new System.Windows.Forms.SaveFileDialog();
            od.InitialDirectory = System.IO.Path.GetFullPath(".") + "Content\\Encyclopedia";
            od.Filter = "Encyclopedia files|*.edf";
            savefn = "";
            if (od.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                article.Save(od.FileName);
                savefn = od.FileName;
            }
        }

        void save_onClicked(object sender, InputEngine.MouseArgs e)
        {
            if (savefn == "")
            {
                saveas_onClicked(null, null);
                return;
            }
            article.Save(savefn);
        }

        void load_onClicked(object sender, InputEngine.MouseArgs e)
        {
            System.Windows.Forms.OpenFileDialog od = new System.Windows.Forms.OpenFileDialog();
            od.InitialDirectory = System.IO.Path.GetFullPath(".") + "Content\\Encyclopedia";
            od.Filter = "Encyclopedia files|*.edf";
            savefn = "";
            if (od.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                article.Load(od.FileName);
                tbLink.Text = article.Link;
                savefn = od.FileName;
            }
        }

        void tbLink_onTextChanged(object sender, TextBox.TextChangedArgs e)
        {
            article.Link = e.NewText;
            goToLink.isEnabled = article.HasLink;
        }

        public void OpenPage(String page)
        {
            article.Load(page);
        }

        void exit_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            Main.Close();
            //Main.curState = LastState;
            //GUIEngine.curScene = LastScene;
        }

        void goToLink_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            article.GoToLink();
        }

        public override void onShow()
        {
            Clear();
            goToLink.isEnabled = article.HasLink;
            base.onShow();
        }

        public void Clear()
        {
            tbFile.Text = "";
            tbLink.Text = "";
            article.Clear();
            savefn = "";
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            article.OnResolutionChanged(w, h - 50, oldw, oldh);
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
            article.Draw(renderer);
        }
    }
}
