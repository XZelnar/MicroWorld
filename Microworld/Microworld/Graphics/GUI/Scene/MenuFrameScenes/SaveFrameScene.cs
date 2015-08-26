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
    class SaveFrameScene : MenuFrameScene
    {
        static SpriteFont overlayFont;
        Texture2D tfolder;

        internal String folder = "Sandbox";
        internal String mask = "*.sav";

        protected Elements.SavesLoadsList saves;
        protected YesNoMessageBox mb;
        protected Elements.TextBox tb;
        protected Elements.MenuButton bdelete, bsave;
        protected Elements.Label lTitle;

        public override void Initialize()
        {
            ButtonsCount = FrameButtonsCount.Two;
            ShouldBeScaled = false;

            saves = new Elements.SavesLoadsList((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y - 35);
            saves.onElementSelected += new Elements.SavesLoadsList.SaveLoadEvent(saves_onElementSelected);
            saves.onSelectedIndexChanged += new Elements.SavesLoadsList.SelectionEvent(saves_onSelectedIndexChanged);
            controls.Add(saves);

            lTitle = new Elements.Label((int)Position.X + 92, (int)Position.Y + 31, "Save");
            lTitle.foreground = Color.White;
            controls.Add(lTitle);

            bdelete = new Elements.MenuButton((int)(Position.X + Size.X) - 240, (int)(Position.Y + Size.Y) - 23,
                120, 23, "Delete");
            bdelete.Font = ButtonFont;
            bdelete.onClicked += new Elements.Button.ClickedEventHandler(deleteClick);
            controls.Add(bdelete);

            bsave = new Elements.MenuButton((int)(Position.X + Size.X) - 120, (int)(Position.Y + Size.Y) - 23,
                120, 23, "Save");
            bsave.Font = ButtonFont;
            bsave.onClicked += new Elements.Button.ClickedEventHandler(saveClick);
            controls.Add(bsave);

            tb = new Elements.TextBox((int)saves.position.X, (int)bsave.position.Y, (int)saves.size.X, 23, "");
            tb.onTextChanged += new Elements.TextBox.TextChangedEventHandler(tb_onTextChanged);
            tb.BackgroundColor = Color.White * 0.2f;
            tb.ForegroundColor = Color.White;
            tb.CursorColor = Color.White;
            tb.Multiline = false;
            tb.BGAsColor = true;
            controls.Add(tb);

            base.Initialize();
        }

        public override void LoadContent()
        {
            if (overlayFont == null)
                overlayFont = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_30");
            tfolder = ResourceManager.Load<Texture2D>("GUI/Encyclopedia/folder");
            lTitle.font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_22");
            base.LoadContent();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);
            bdelete.position = new Vector2((int)(GetPosForWH(w, h).X + GetSizeForWH(w, h).X) - 240,
                (int)(GetPosForWH(w, h).Y + GetSizeForWH(w, h).Y) - 23);
            bsave.position = new Vector2((int)(GetPosForWH(w, h).X + GetSizeForWH(w, h).X) - 120,
                (int)(GetPosForWH(w, h).Y + GetSizeForWH(w, h).Y) - 23);

            saves.Position = GetPosForWH(w, h);
            saves.size.X = (int)GetSizeForWH(w, h).X;
            saves.size.Y = (int)GetSizeForWH(w, h).Y - 28;
            tb.Position = new Vector2((int)saves.position.X, (int)bsave.position.Y);
            tb.size.X = saves.Size.X - 245;
        }

        public override void OnGraphicsDeviceReset()
        {
            bdelete.WasInitiallyDrawn = false;
            bsave.WasInitiallyDrawn = false;
            base.OnGraphicsDeviceReset();
        }

        void saves_onElementSelected(object sender, int index, bool saveload, bool delete)
        {
            if (saveload)
                saveClick(sender, null);
            if (delete)
                deleteClick(sender, null);
        }

        bool ignoreLBEvent = false;
        protected void tb_onTextChanged(object sender, Elements.TextBox.TextChangedArgs e)
        {
            int i = DoesAlreadyExist();
            ignoreLBEvent = true;
            saves.SelectedIndex = i;
            ignoreLBEvent = false;
        }

        protected void saves_onSelectedIndexChanged(object sender, int selectedIndex)
        {
            if (!ignoreLBEvent && selectedIndex != -1)
            {
                tb.Text = (sender as Elements.SavesLoadsList).GetSelected();
            }
        }

        public override void onShow()
        {
            //if (Settings.GameState != Settings.GameStates.Stopped)
            //    Logics.GameLogicsHelper.GameStop();

            base.onShow();

            FillListBox();
            tb.Text = "";
        }

        protected void FillListBox()
        {
            saves.Clear();

            if (System.IO.Directory.Exists("Saves/" + folder))
            {
                var a = System.IO.Directory.GetFiles("Saves/" + folder + "/", mask);
                int t = 0, p = 0;
                for (int i = 0; i < a.Length; i++)
                {
                    t = a[i].LastIndexOf('/') + 1;
                    p = a[i].LastIndexOf('.');
                    saves.ItemsAdd(a[i].Substring(t, p - t), new System.IO.FileInfo(a[i]).LastWriteTime);
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
            GUIEngine.s_mainMenu.CloseFrame();
            GUIEngine.s_mainMenu.Close();
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
                catch (Exception ee)
                {
                }
            }
            mb.Dispose();
            mb = null;
            FillListBox();
            tb_onTextChanged(null, null);
        }

        protected void a_onButtonClicked(object sender, OKMessageBox.ButtonClickedArgs e)
        {
            tb.isFocused = true;
        }

        protected virtual void Save()
        {
            if (Main.curState == "GAMESandbox")
            {
                IO.SaveEngine.SaveAll("Saves/" + folder + "/" + tb.Text + mask.Substring(1));
            }
            if (Main.curState == "GAMELevels")
            {
                IO.SaveEngine.SaveAll("Saves/" + folder + "/" + tb.Text + ".sav", IO.SaveEngine.SaveType.Levels);
            }
            if (Main.curState == "GAMElvlDesign")
            {
                IO.SaveEngine.SaveAll("Saves/" + folder + "/" + tb.Text + ".lvl", IO.SaveEngine.SaveType.LevelDesigner);
            }
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

        public void deleteClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            if (saves.SelectedIndex == -1) return;
            mb = YesNoMessageBox.Show("Are you sure you want\r\nto delete the following file: \r\n" +
                saves.GetSelected() + "?");
            mb.onButtonClicked += new YesNoMessageBox.ButtonClickedEventHandler(mb_onButtonClicked);
            GUIEngine.AddHUDScene(mb);
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

        public override void Draw(Renderer renderer)
        {
            base.Draw(renderer);

            renderer.Draw(tfolder, Position + new Vector2(26, 26), Color.White);
            renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)Position.X - 9, (int)Position.Y + 95, (int)Size.X + 9, 1), Color.White);
            renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)Position.X - 10 + (int)Size.X + 9, (int)Position.Y + 95, 1, 5), Color.White);
            if (saves.ItemsCount() == 0)
            {
                var a = overlayFont.MeasureString("No save files found");
                renderer.DrawString(overlayFont, "No save files found",
                    new Rectangle((int)saves.Position.X, (int)(saves.Position.Y + (saves.Size.Y - a.Y) / 2), (int)saves.Size.X, (int)a.Y),
                    Color.Gray, Renderer.TextAlignment.Center);
            }
        }
    }
}
