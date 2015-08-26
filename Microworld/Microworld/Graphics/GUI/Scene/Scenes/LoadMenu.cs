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
    class LoadMenu : Scene
    {
        Texture2D bg;
        static SpriteFont overlayFont;

        protected String folder = "Sandbox";

        protected Elements.ListBox saves;
        protected YesNoMessageBox mb;
        protected Elements.EncyclopediaBrowserButton bback, bref, bdelete, bload;

        protected String mask = "*.sav";

        public override void Initialize()
        {
            ShouldBeScaled = false;

            saves = new Elements.ListBox(10, 20, 780, 410);
            saves.onDoubleClicked += new Elements.ListBox.DoubleClickedEventHandler(saves_onDoubleClicked);
            controls.Add(saves);

            bback = new Elements.EncyclopediaBrowserButton(20, 440, 80, 30, "Back");
            (bback as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            bback.foreground = Color.White;
            //bback.font = ResourceManager.Load<SpriteFont>("Fonts/MenuFont");
            bback.onClicked += new Elements.Button.ClickedEventHandler(backClick);
            controls.Add(bback);

            bref = new Elements.EncyclopediaBrowserButton(500, 440, 80, 30, "Refresh");
            (bref as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            bref.foreground = Color.White;
            //bref.font = ResourceManager.Load<SpriteFont>("Fonts/MenuFont");
            bref.onClicked += new Elements.Button.ClickedEventHandler(refClick);
            controls.Add(bref);

            bdelete = new Elements.EncyclopediaBrowserButton(600, 440, 80, 30, "Delete");
            (bdelete as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            bdelete.foreground = Color.White;
            //bdelete.font = ResourceManager.Load<SpriteFont>("Fonts/MenuFont");
            bdelete.onClicked += new Elements.Button.ClickedEventHandler(deleteClick);
            controls.Add(bdelete);

            bload = new Elements.EncyclopediaBrowserButton(700, 440, 80, 30, "Load");
            (bload as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            bload.foreground = Color.White;
            //bload.font = ResourceManager.Load<SpriteFont>("Fonts/MenuFont");
            bload.onClicked += new Elements.Button.ClickedEventHandler(loadClick);
            controls.Add(bload);

            base.Initialize();

            background = GUIEngine.s_mainMenu.background;
        }

        public override void LoadContent()
        {
            if (overlayFont == null)
                overlayFont = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_30");
            bg = ResourceManager.Load<Texture2D>("GUI/BackgroundBlack");
            base.LoadContent();
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            base.onKeyPressed(e);
            if (e.key == Keys.Enter.GetHashCode())
            {
                loadClick(null, null);
                e.Handled = true;
                return;
            }
            if (!e.Handled && e.key == Keys.R.GetHashCode())
            {
                refClick(null, null);
                e.Handled = true;
                return;
            }
            if (!e.Handled && e.key == Keys.Delete.GetHashCode())
            {
                deleteClick(null, null);
                e.Handled = true;
                return;
            }
            if (!e.Handled && e.key == Keys.Down.GetHashCode())
            {
                saves.SelectedIndex++;
                saves.CheckSelectedBounds();
                e.Handled = true;
                return;
            }
            if (!e.Handled && e.key == Keys.Up.GetHashCode())
            {
                saves.SelectedIndex--;
                saves.CheckSelectedBounds();
                e.Handled = true;
                return;
            }
            if (!e.Handled && e.key == Keys.Escape.GetHashCode())
            {
                backClick(null, null);
                e.Handled = true;
                return;
            }
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);
            bback.position.Y += h - oldh;
            bref.position.Y += h - oldh;
            bdelete.position.Y += h - oldh;
            bload.position.Y += h - oldh;

            bref.position.X += w - oldw;
            bdelete.position.X += w - oldw;
            bload.position.X += w - oldw;
            saves.scrollbar.position.X -= saves.size.X;

            saves.size.X = w - 20;

            saves.scrollbar.position.X += saves.size.X;

            saves.size.Y = h - 70;
            saves.scrollbar.size.Y = saves.size.Y;
        }

        protected void saves_onDoubleClicked(object sender, Elements.ListBox.SelectedIndexArgs e)
        {
            loadClick(sender, null);
        }

        public override void onShow()
        {
            base.onShow();

            FillListBox();
        }

        protected void FillListBox()
        {
            saves.Clear();
            saves.Reset();

            if (System.IO.Directory.Exists("Saves/" + folder))
            {
                var a = System.IO.Directory.GetFiles("Saves/" + folder + "/", mask);
                int t = 0, p = 0;
                for (int i = 0; i < a.Length; i++)
                {
                    t = a[i].LastIndexOf('/') + 1;
                    p = a[i].LastIndexOf('.');
                    saves.ItemsAdd(a[i].Substring(t, p - t));
                }
            }
        }

        public void loadClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            if (saves.SelectedIndex == -1) return;

            loadLevel = true;
            GUIEngine.ChangeScene(GUIEngine.s_loading, "GUILoading");
            //GUIEngine.curScene = GUIEngine.s_loading;
            //Main.curState = "GUILoading";

            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(_load));
            t.Start();
        }

        protected virtual void _load()
        {
            //Logics.LevelEngine.Stop();
            //Logics.GameInputHandler.PlacableAreas.Clear();
            //Settings.ResetInGameSettings();
            Logics.GameLogicsHelper.InitForGame();

            IO.SaveEngine.LoadAll("Saves/" + folder + "/" + saves.GetSelected() + mask.Substring(1));

            GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_game, "GAMESandbox");
            //GUIEngine.curScene = null;
            //Main.curState = "GAMESandbox";
        }
        bool loadLevel = false;
        public override void PostFadeOut()
        {
            base.PostFadeOut();
            if (loadLevel)
            {
                Logics.GameLogicsHelper.InitScenesForGame();
                loadLevel = false;
            }
        }

        public void deleteClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            if (saves.SelectedIndex == -1) return;
            mb = YesNoMessageBox.Show("Are you sure you want\r\nto delete the following file: \r\n" + 
                saves.GetSelected() + "?");
            mb.onButtonClicked += new YesNoMessageBox.ButtonClickedEventHandler(mb_onButtonClicked);
            GUIEngine.AddHUDScene(mb);
        }

        public void refClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            FillListBox();
        }

        public virtual void backClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.ChangeScene(GUIEngine.s_sandboxMenu, "GUISandbox");
            //Main.curState = "GUISandbox";
            //GUIEngine.curScene = GUIEngine.s_sandboxMenu;
        }

        protected void mb_onButtonClicked(object sender, YesNoMessageBox.ButtonClickedArgs e)
        {
            if (e.button == 1)
            {
                try
                {
                    System.IO.File.Delete("Saves/" + folder + "/" + saves.GetSelected() + mask.Substring(1));
                    System.IO.File.Delete("Saves/" + folder + "/" + saves.GetSelected() + ".lua");
                }
                catch { }
            }
            //GUIEngine.RemoveHUDScene(mb);
            mb.Dispose();
            mb = null;
            FillListBox();
        }


        Vector2 fadeSize = new Vector2();
        public override void Draw(Renderer renderer)
        {
            if (background != null) background.Draw(renderer);
            if (fadeSize.Y != Main.WindowHeight)
                renderer.SetScissorRectangle((int)(Main.WindowWidth - fadeSize.X) / 2, (int)(Main.WindowHeight - fadeSize.Y) / 2,
                    (int)fadeSize.X, (int)fadeSize.Y, false);
            RenderHelper.SmartDrawRectangle(bg, 5, 0, 0, Main.WindowWidth, Main.WindowHeight, Color.White, renderer);
            foreach (Elements.Control c in controls)
            {
                c.Draw(renderer);
            }
            if (fadeSize.X != Main.WindowWidth)
            {
                renderer.ResetScissorRectangle();
                renderer.SetScissorRectangle((int)(Main.WindowWidth - fadeSize.X) / 2, (int)(Main.WindowHeight - fadeSize.Y) / 2,
                    (int)fadeSize.X, (int)fadeSize.Y, false);
            }
            if (saves.ItemsCount() == 0)
            {
                var a = overlayFont.MeasureString("No save files found");
                renderer.DrawString(overlayFont, "No save files found",
                    new Rectangle(0, (int)(saves.Position.Y + (saves.Size.Y - a.Y) / 2), Main.WindowWidth, (int)a.Y),
                    Color.Gray, Renderer.TextAlignment.Center);
            }
            if (fadeSize.X != Main.WindowWidth)
                renderer.ResetScissorRectangle();
        }

        public override void FadeIn()
        {
            fadeSize = new Vector2();
            Vector2 delta = new Vector2((float)Main.WindowWidth / 50f, (float)Main.WindowHeight / 50f);
            fadeSize.Y += delta.Y;
            //x
            for (int i = 0; i < 10; i++)
            {
                fadeSize.X += delta.X / 2f;
                System.Threading.Thread.Sleep(5);
            }
            for (int i = 0; i < 40; i++)
            {
                fadeSize.X += delta.X;
                System.Threading.Thread.Sleep(5);
            }
            for (int i = 0; i < 10; i++)
            {
                fadeSize.X += delta.X / 2f;
                System.Threading.Thread.Sleep(5);
            }
            //y
            for (int i = 0; i < 10; i++)
            {
                fadeSize.Y += delta.Y / 2f;
                System.Threading.Thread.Sleep(5);
            }
            for (int i = 0; i < 39; i++)
            {
                fadeSize.Y += delta.Y;
                System.Threading.Thread.Sleep(5);
            }
            for (int i = 0; i < 10; i++)
            {
                fadeSize.Y += delta.Y / 2f;
                System.Threading.Thread.Sleep(5);
            }
        }

        public override void FadeOut()
        {
            if (loadLevel) return;
            Vector2 delta = new Vector2((float)Main.WindowWidth / 50f, (float)Main.WindowHeight / 50f);
            //y
            for (int i = 0; i < 10; i++)
            {
                fadeSize.Y -= delta.Y / 2f;
                System.Threading.Thread.Sleep(5);
            }
            for (int i = 0; i < 39; i++)
            {
                fadeSize.Y -= delta.Y;
                System.Threading.Thread.Sleep(5);
            }
            for (int i = 0; i < 10; i++)
            {
                fadeSize.Y -= delta.Y / 2f;
                System.Threading.Thread.Sleep(5);
            }
            //x
            for (int i = 0; i < 10; i++)
            {
                fadeSize.X -= delta.X / 2f;
                System.Threading.Thread.Sleep(5);
            }
            for (int i = 0; i < 40; i++)
            {
                fadeSize.X -= delta.X;
                System.Threading.Thread.Sleep(5);
            }
            for (int i = 0; i < 10; i++)
            {
                fadeSize.X -= delta.X / 2f;
                System.Threading.Thread.Sleep(5);
            }
        }

    }
}
