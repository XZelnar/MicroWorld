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
    class LoadFrameScene2 : MenuFrameScene
    {
        static SpriteFont overlayFont;

        protected String folder = "Sandbox";

        protected Elements.ListBox saves;
        protected YesNoMessageBox mb;
        protected Elements.MenuButton bdelete, bload;

        protected String mask = "*.sav";

        public override void Initialize()
        {
            ShouldBeScaled = false;
            ButtonsCount = FrameButtonsCount.Two;

            saves = new Elements.ListBox((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y - 35);
            saves.onDoubleClicked += new Elements.ListBox.DoubleClickedEventHandler(saves_onDoubleClicked);
            controls.Add(saves);

            bdelete = new Elements.MenuButton((int)(Position.X + Size.X) - 240, (int)(Position.Y + Size.Y) - 23, 
                120, 23, "Delete");
            bdelete.Font = ButtonFont;
            bdelete.onClicked += new Elements.Button.ClickedEventHandler(deleteClick);
            controls.Add(bdelete);

            bload = new Elements.MenuButton((int)(Position.X + Size.X) - 120, (int)(Position.Y + Size.Y) - 23, 
                120, 23, "Load");
            bload.Font = ButtonFont;
            bload.onClicked += new Elements.Button.ClickedEventHandler(loadClick);
            controls.Add(bload);

            base.Initialize();
        }

        public override void LoadContent()
        {
            if (overlayFont == null)
                overlayFont = ResourceManager.Load<SpriteFont>("Fonts/SaveLoadOverlay");
            base.LoadContent();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);
            bdelete.position = new Vector2((int)(GetPosForWH(w, h).X + GetSizeForWH(w, h).X) - 240,
                (int)(GetPosForWH(w, h).Y + GetSizeForWH(w, h).Y) - 23);
            bload.position = new Vector2((int)(GetPosForWH(w, h).X + GetSizeForWH(w, h).X) - 120,
                (int)(GetPosForWH(w, h).Y + GetSizeForWH(w, h).Y) - 23);

            saves.Position = GetPosForWH(w, h);
            saves.size.X = (int)GetSizeForWH(w, h).X;
            saves.size.Y = (int)GetSizeForWH(w, h).Y - 35;
            saves.scrollbar.position.X = saves.position.X + saves.size.X - saves.scrollbar.size.X;
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

            Logics.GameLogicsHelper.InitScenesForGame();
            Graphics.GUI.GUIEngine.s_componentSelector.ClearCount();
            GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_game, "GAMESandbox");
            //GUIEngine.curScene = null;
            //Main.curState = "GAMESandbox";
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
