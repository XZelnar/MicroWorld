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
    class LevelSelection : MenuFrameScene
    {
        internal static String[] TABS_NAMES = new String[] { "Levels/Tut/", "Levels/B1/", "Levels/B2/", "Levels/B3/" };
        internal static int[] TABS_LEVELS_COUNT = new int[] { 6, 5, 4, 3 };

        internal String folder = "";
        private String selectedFile = "";
        internal int selectedLevel = 0;
        List<Elements.LevelSelectorItem> items = new List<Elements.LevelSelectorItem>();

        public override void Initialize()
        {
            isVisible = true;

            base.Initialize();
        }

        public void Clear()
        {
            lock (items)
                lock (controls)
                    while (items.Count > 0)
                    {
                        controls.Remove(items[0]);
                        items[0].onClicked -= new Elements.Button.ClickedEventHandler(LevelSelection_onClicked);
                        items[0].Dispose();
                        items.RemoveAt(0);
                    }
        }

        public void InitForItemsCount(int c)
        {
            lock (items)
            {
                Clear();
                for (int i = 0; i < c; i++)
                {
                    items.Add(new Elements.LevelSelectorItem());
                    items[i].Initialize();
                    items[i].Text = "Level " + (i + 1).ToString();
                    items[i].Size = new Vector2(370 * Main.WindowWidth / 1920, 211 * Main.WindowHeight / 1080);
                    items[i].Position = new Vector2(Position.X + items[i].Size.X * (i % 3), Position.Y + items[i].size.Y * (i / 3));
                    items[i].onClicked += new Elements.Button.ClickedEventHandler(LevelSelection_onClicked);
                    items[i].isEnabled = IsLevelOpened(folder, i);
                    items[i].ResetMouseOverAnimation();
                    lock (controls)
                        controls.Add(items[i]);
                }
            }
        }

        public void RefreshCompletionState()
        {
            lock (items)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].isEnabled = IsLevelOpened(folder, i);
                    items[i].ResetMouseOverAnimation();
                    items[i].WasInitiallyDrawn = false;
                }
            }
        }

        void LevelSelection_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            selectedFile = folder + items.IndexOf(sender as Elements.LevelSelectorItem).ToString();
            if (System.IO.File.Exists(selectedFile + ".lvl"))
            {
                selectedLevel = items.IndexOf(sender as Elements.LevelSelectorItem);
                StartLevel();

                GUIEngine.curScene = Graphics.GUI.GUIEngine.s_game;
                Main.curState = "GAMELevels";
            }
        }

        public void StartLevel(int index)
        {
            //TODO safety
            selectedLevel = index;
            selectedFile = folder + index.ToString();
            StartLevel();
            GUIEngine.curScene = Graphics.GUI.GUIEngine.s_game;
            Main.curState = "GAMELevels";
        }

        public void StartLevel()
        {
            Logics.LevelEngine.Stop();
            Logics.GameLogicsHelper.InitForGame();

            IO.SaveEngine.LoadAll(selectedFile + ".lvl", IO.SaveEngine.SaveType.Levels);
            Logics.GameLogicsHelper.InitScenesForGame();
            Logics.LevelEngine.LoadLevelScript(selectedFile + ".lua");
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_purpose);
        }

        public void StartNextLevel()
        {
            GUIEngine.RemoveHUDScene(GUIEngine.s_tutorial);
            selectedLevel++;
            if (selectedLevel >= items.Count)
            {
                Sound.SoundPlayer.PlayButtonClick();
                GUIEngine.ChangeScene(GUIEngine.s_mainMenu, "GUIMainMenu");
            }
            else
            {
                selectedFile = selectedFile.Substring(0, selectedFile.LastIndexOf("/") + 1) + selectedLevel.ToString();
                StartLevel();
            }
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Position = GetPosForWH(w, h);
            }

            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void OnGraphicsDeviceReset()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].WasInitiallyDrawn = false;
            }
            base.OnGraphicsDeviceReset();
        }

        public override void Draw(Renderer renderer)
        {
            lock (items)
            {
                base.Draw(renderer);
            }
        }

        #region fromOld
        internal void WriteSaveInfo(ref IO.SaveWriter sw)
        {
            sw.WriteLine(folder);
            sw.WriteLine(selectedLevel.ToString());
        }

        internal bool ReadSaveInfo(ref IO.SaveReader sr, bool start)
        {
            String t = sr.ReadLine();
            folder = t;
            /*
            int tct = CurTab;
            CurTab = -1;
            for (int i = 0; i < TABS_NAMES.Length; i++)
            {
                if (TABS_NAMES[i] == t)
                {
                    CurTab = i;
                    break;
                }
            }
            if (CurTab == -1)
            {
                CurTab = tct;
                return false;
            }//*/
            selectedLevel = Convert.ToInt32(sr.ReadLine());
            //sr.ReadLine();
            if (!IsLevelOpened(t, selectedLevel))
            {
                //CurTab = tct;
                return false;
            }
            String s = folder + selectedLevel.ToString();
            if (System.IO.File.Exists(s + ".lua") && start)
                Logics.LevelEngine.LoadLevelScript(s + ".lua");
            else
            {
                //CurTab = tct;
                return !start;
            }
            //selectedTab = curTab;
            return true;
        }

        internal bool BlankReadSaveInfo(ref IO.SaveReader sr, bool start)
        {
            String t = sr.ReadLine();
            t = sr.ReadLine();
            return true;
        }

        public bool IsLevelOpened(String tabName, int level)
        {
            return Logics.CampaingProgress.IsOpened(tabName, level);
        }
        #endregion
    }
}
