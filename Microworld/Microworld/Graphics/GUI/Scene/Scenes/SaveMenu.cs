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
    class SaveMenu : Scene
    {
        Texture2D bg;
        static SpriteFont overlayFont;

        public String folder = "Sandbox";

        protected Elements.ListBox saves;
        protected YesNoMessageBox mb;
        protected Elements.TextBox tb;
        protected Elements.EncyclopediaBrowserButton bback, bref, bdelete, bsave;

        protected String mask = "*.sav";

        public override void Initialize()
        {
            ShouldBeScaled = false;
            Layer = 990;

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

            bsave = new Elements.EncyclopediaBrowserButton(700, 440, 80, 30, "Save");
            (bsave as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            bsave.foreground = Color.White;
            //bsave.font = ResourceManager.Load<SpriteFont>("Fonts/MenuFont");
            bsave.onClicked += new Elements.Button.ClickedEventHandler(saveClick);
            controls.Add(bsave);

            saves = new Elements.ListBox(10, 20, 780, 370);
            saves.onDoubleClicked += new Elements.ListBox.DoubleClickedEventHandler(saves_onDoubleClicked);
            saves.onSelectedIndexChanged += new Elements.ListBox.SelectedIndexChangedEventHandler(saves_onSelectedIndexChanged);
            controls.Add(saves);

            tb = new Elements.TextBox(10, 400, 780, 30, "");
            tb.onTextChanged += new Elements.TextBox.TextChangedEventHandler(tb_onTextChanged);
            tb.BackgroundColor = Color.Black;
            tb.ForegroundColor = Color.White;
            tb.CursorColor = Color.White;
            tb.Multiline = false;
            controls.Add(tb);

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

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);
            tb.position.Y += h - oldh;
            bback.position.Y += h - oldh;
            bref.position.Y += h - oldh;
            bdelete.position.Y += h - oldh;
            bsave.position.Y += h - oldh;

            bref.position.X += w - oldw;
            bdelete.position.X += w - oldw;
            bsave.position.X += w - oldw;
            saves.scrollbar.position.X -= saves.size.X;

            saves.size.X = w - 20;
            tb.size.X = saves.size.X;

            saves.scrollbar.position.X += saves.size.X;

            saves.size.Y = h - 110;
            saves.scrollbar.size.Y = saves.size.Y;
        }

        protected void saves_onDoubleClicked(object sender, Elements.ListBox.SelectedIndexArgs e)
        {
            saveClick(sender, null);
        }

        bool ignoreLBEvent = false;
        protected void tb_onTextChanged(object sender, Elements.TextBox.TextChangedArgs e)
        {
            int i = DoesAlreadyExist();
            ignoreLBEvent = true;
            saves.SelectedIndex = i;
            ignoreLBEvent = false;
        }

        protected void saves_onSelectedIndexChanged(object sender, Elements.ListBox.SelectedIndexArgs e)
        {
            if (!ignoreLBEvent && e.selectedIndex != -1)
            {
                tb.Text = (sender as Elements.ListBox).GetSelected();
            }
        }

        Vector2 fadeSize = new Vector2();
        public override void Draw(Renderer renderer)
        {
            if (background != null) background.Draw(renderer);
            if (fadeSize.Y != Main.WindowHeight)
                renderer.SetScissorRectangle((int)(Main.WindowWidth - fadeSize.X) / 2, (int)(Main.WindowHeight - fadeSize.Y) / 2,
                    (int)fadeSize.X, (int)fadeSize.Y, false);
            RenderHelper.SmartDrawRectangle(bg, 5, 0, 0, Main.WindowWidth, Main.WindowHeight, Color.White, renderer);
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].Draw(renderer);
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

        public override void onShow()
        {
            tb.Text = "";

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

        protected virtual void saveClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            if (tb.Text == "") return;
            try
            {
                if (saves.SelectedIndex != -1)
                {
                    mb = YesNoMessageBox.Show("Are you sure you want to\r\nreplace existing file: \r\n" + saves.GetSelected() + "?");
                    mb.onButtonClicked += new YesNoMessageBox.ButtonClickedEventHandler(mbReplace_onButtonClicked);
                }
                else
                {
                    Save();
                    GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_game, null);
                    //GUIEngine.curScene = null;
                }
            }
            catch
            {
                var a = OKMessageBox.Show("Wrong filename!");
                a.onButtonClicked += new OKMessageBox.ButtonClickedEventHandler(a_onButtonClicked);
            }
        }

        protected void a_onButtonClicked(object sender, OKMessageBox.ButtonClickedArgs e)
        {
            tb.isFocused = true;
        }

        protected virtual void mbReplace_onButtonClicked(object sender, YesNoMessageBox.ButtonClickedArgs e)
        {
            if (e.button == 1)
            {
                try
                {
                    Save();
                    GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_game, null);
                    //GUIEngine.curScene = null;
                }
                catch { }
            }
            mb.Dispose();
            mb = null;
            FillListBox();
            tb_onTextChanged(null, null);
        }

        protected virtual void Save()
        {
            IO.SaveEngine.SaveAll("Saves/" + folder + "/" + tb.Text + mask.Substring(1));
        }

        protected int DoesAlreadyExist()
        {
            for (int i = 0; i < saves.ItemsCount(); i++)
            {
                if (saves.ItemsGetAt(i) == tb.Text)
                {
                    return i;
                }
            }
            return -1;
        }

        protected void deleteClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            if (saves.SelectedIndex == -1) return;
            mb = YesNoMessageBox.Show("Are you sure you want\r\nto delete the following file: \r\n" + 
                saves.GetSelected() + "?");
            mb.onButtonClicked += new YesNoMessageBox.ButtonClickedEventHandler(mb_onButtonClicked);
            GUIEngine.AddHUDScene(mb);
        }

        protected void refClick(object sender, InputEngine.MouseArgs e)
        {
            FillListBox();
        }

        protected void backClick(object sender, InputEngine.MouseArgs e)
        {
            GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_game, null);
            //Main.curState = "GUISandbox";
            //GUIEngine.curScene = null;
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
            mb.Dispose();
            mb = null;
            FillListBox();
            tb_onTextChanged(null, null);
        }

        #region InputOverrides
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

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
            base.onKeyDown(e);
            e.Handled = true;
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            base.onKeyPressed(e);
            if (e.key == Keys.Enter.GetHashCode())
            {
                saveClick(null, null);
                e.Handled = true;
                return;
            }
            if (!e.Handled && e.key == Keys.Escape.GetHashCode())
            {
                backClick(null, null);
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
            e.Handled = true;
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.onMouseMove(e);
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
