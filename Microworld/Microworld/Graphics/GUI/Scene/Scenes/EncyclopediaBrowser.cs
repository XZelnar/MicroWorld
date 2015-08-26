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
using System.IO;

namespace MicroWorld.Graphics.GUI.Scene
{
    class EncyclopediaBrowser : Scene
    {
        const int ELEMENTS_PER_PAGE = 10;//not counting "folder up"

        Texture2D bg;
        Texture2D file, folder;

        public List<Button> buttons;
        EncyclopediaBrowserButton back, bnext, bprev, folderup;
        Label lcurFolder;

        public Scene lastScene;
        public String lastState;

        int folderElementsCount = 0;

        String curFolder = "Content\\Encyclopedia\\";
        int curPage = 0;
        public int CurPage
        {
            get { return curPage; }
            set { curPage = value; }
        }

        List<bool> arefiles;

        public override void Initialize()
        {
            buttons = new List<Button>();
            arefiles = new List<bool>();

            lcurFolder = new Label(10, 10, curFolder);
            lcurFolder.size.X = Main.WindowWidth - 20;
            lcurFolder.foreground = Color.White;
            lcurFolder.TextAlignment = Renderer.TextAlignment.Center;
            controls.Add(lcurFolder);

            folderup = new EncyclopediaBrowserButton(20, (int)(lcurFolder.position.Y + lcurFolder.size.Y + 5), 400, 28, "    ..");
            folderup.onClicked += new Button.ClickedEventHandler(folderup_onClicked);
            folderup.background = Color.Transparent;
            folderup.foreground = Color.White;
            folderup.textAlignment = Renderer.TextAlignment.Left;
            controls.Add(folderup);

            bnext = new EncyclopediaBrowserButton(Main.WindowWidth - 85, Main.WindowHeight - 38, 80, 28, ">>");
            (bnext as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            bnext.foreground = Color.White;
            //bnext.font = ResourceManager.Load<SpriteFont>("Fonts/MenuFont");
            bnext.onClicked += new Button.ClickedEventHandler(bnext_onClicked);
            controls.Add(bnext);

            bprev = new EncyclopediaBrowserButton(5, Main.WindowHeight - 38, 80, 28, "<<");
            (bprev as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            bprev.foreground = Color.White;
            //bprev.font = ResourceManager.Load<SpriteFont>("Fonts/MenuFont");
            bprev.onClicked += new Button.ClickedEventHandler(bprev_onClicked);
            controls.Add(bprev);

            back = new EncyclopediaBrowserButton((Main.WindowWidth - 80) / 2, Main.WindowHeight - 38, 80, 28, "Back");
            (back as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            back.foreground = Color.White;
            //back.font = ResourceManager.Load<SpriteFont>("Fonts/MenuFont");
            back.onClicked += new Button.ClickedEventHandler(back_onClicked);
            controls.Add(back);

            base.Initialize();

            background = GUIEngine.s_mainMenu.background;
            ResetState();
        }

        void folderup_onClicked(object sender, InputEngine.MouseArgs e)
        {
            var a = curFolder.Substring(0, curFolder.Length - 1);
            curFolder = a.Substring(0, a.LastIndexOf("\\") + 1);
            curPage = 0;
            InitForCurState();
        }

        public override void LoadContent()
        {
            bg = ResourceManager.Load<Texture2D>("GUI/BackgroundBlack");
            lcurFolder.font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_16");
            file = ResourceManager.Load<Texture2D>("GUI/Encyclopedia/file");
            folder = ResourceManager.Load<Texture2D>("GUI/Encyclopedia/folder");
            base.LoadContent();
        }

        void back_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.ChangeScene(lastScene, lastState);
            //Main.curState = lastState;
            //GUIEngine.curScene = lastScene;
        }

        void bprev_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            CurPage--;
            InitForCurState();
        }

        void bnext_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            CurPage++;
            InitForCurState();
        }

        public void CheckCurPageBorders()
        {
            if (curPage == 0)
            {
                bprev.isEnabled = false;
            }
            else
            {
                bprev.isEnabled = true;
            }

            if (curPage == (folderElementsCount - 1) / ELEMENTS_PER_PAGE)
            {
                bnext.isEnabled = false;
            }
            else
            {
                bnext.isEnabled = true;
            }
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            bnext.position = new Vector2(Main.WindowWidth - 85, Main.WindowHeight - 38);
            bprev.position = new Vector2(5, Main.WindowHeight - 38);
            back.position = new Vector2((Main.WindowWidth - 80) / 2, Main.WindowHeight - 38);
            lcurFolder.size.X = Main.WindowWidth - 20;

            InitForCurState();

            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public void ResetState()
        {
            curFolder = "Content\\Encyclopedia\\";
            curPage = 0;
            InitForCurState();
        }

        public void InitForCurState()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                controls.Remove(buttons[i]);
                buttons[i].Dispose();
            }
            buttons.Clear();
            arefiles.Clear();
            if (!Directory.Exists(curFolder)) return;

            var dirs = Directory.GetDirectories(curFolder);
            var files = Directory.GetFiles(curFolder, "*.edf");
            folderElementsCount = dirs.Length + files.Length;
            int count = 0;
            if (ELEMENTS_PER_PAGE * curPage < dirs.Length)
            {
                for (int i = ELEMENTS_PER_PAGE * curPage; i < dirs.Length && count < ELEMENTS_PER_PAGE; i++)
                {
                    AddButton(dirs[i], false);
                    count++;
                }
            }
            if (count < ELEMENTS_PER_PAGE)
            {
                for (int i = ELEMENTS_PER_PAGE * curPage + count - dirs.Length; i < files.Length && count < ELEMENTS_PER_PAGE; i++)
                {
                    AddButton(files[i], true);
                    count++;
                }
            }
            CheckCurPageBorders();
            lcurFolder.text = ".\\" + curFolder.Substring(21);
            lcurFolder.size.X = Main.WindowWidth - 20;
            if (curFolder == "Content\\Encyclopedia\\") folderup.isVisible = false;
            else folderup.isVisible = true;
        }

        public void AddButton(String name, bool file)
        {
            float h = (Main.WindowHeight - folderup.position.Y - 48 - 38) / ELEMENTS_PER_PAGE;
            String txt = name.Substring(name.LastIndexOf("\\") + 1);
            if (file)
                txt = txt.Substring(0, txt.LastIndexOf("."));
            int x = curFolder == "Content\\Encyclopedia\\" ? 20 : 40;
            Button b = new EncyclopediaBrowserButton(x, (int)(buttons.Count * h + folderup.position.Y + 38), 
                Main.WindowWidth / 2 - x, (int)(h - 10), "     " + txt);
            b.onClicked += new Button.ClickedEventHandler(b_onClicked);
            b.tag = new object[] { name, file };
            b.Initialize();
            b.textAlignment = Renderer.TextAlignment.Left;
            b.background = Color.Transparent;
            b.foreground = Color.White;
            buttons.Add(b);
            arefiles.Add(file);
            controls.Add(b);
        }

        void b_onClicked(object sender, InputEngine.MouseArgs e)
        {
            var tag = (sender as Button).tag as object[];
            if ((bool)(tag)[1])//this is file
            {
                GUIEngine.s_encyclopediaPage.LastState = Main.curState;
                GUIEngine.s_encyclopediaPage.LastScene = GUIEngine.curScene;
                GUIEngine.ChangeScene(GUIEngine.s_encyclopediaPage, "GUIEncyclopedia");
                //Main.curState = "GUIEncyclopedia";
                //GUIEngine.curScene = GUIEngine.s_encyclopediaPage;
                GUIEngine.s_encyclopediaPage.OpenPage(tag[0] as String);
            }
            else//this is folder
            {
                curFolder = tag[0] as String;
                curPage = 0;
                InitForCurState();
            }
        }

        public override void Draw(Renderer renderer)
        {
            if (background != null) background.Draw(renderer);
            RenderHelper.SmartDrawRectangle(bg, 5, 0, 0, Main.WindowWidth, Main.WindowHeight, Color.White, renderer);
            foreach (Elements.Control c in controls)
            {
                c.Draw(renderer);
            }

            float h = (Main.WindowHeight - folderup.position.Y - 48 - 38) / ELEMENTS_PER_PAGE - 10;
            for (int i = 0; i < arefiles.Count; i++)
            {
                if (arefiles[i])
                    renderer.Draw(file, buttons[i].position + new Vector2(4, (h - file.Height) / 2), Color.White);
                else
                    renderer.Draw(folder, buttons[i].position + new Vector2(4, (h - file.Height) / 2), Color.White);
            }
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            if (e.key == Keys.Right.GetHashCode())
            {
                if (bnext.isEnabled) bnext_onClicked(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.Left.GetHashCode())
            {
                if (bprev.isEnabled) bprev_onClicked(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.Escape.GetHashCode())
            {
                back_onClicked(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.Back.GetHashCode())
            {
                if (folderup.isVisible) folderup_onClicked(null, null);
                e.Handled = true;
                return;
            }
            base.onKeyPressed(e);
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
