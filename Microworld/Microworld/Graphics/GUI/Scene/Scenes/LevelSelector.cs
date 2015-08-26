using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MicroWorld.Graphics.GUI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MicroWorld.Graphics.GUI.Scene
{
    class LevelSelector2 : Scene
    {
        internal static String[] TABS_NAMES = new String[] { "Tutorial", "Input core", "Logics core", "Output core" };
        internal static int[] TABS_LEVELS_COUNT = new int[] { 5, 8, 8, 4 };
        private static String[] TABS_FOLDERS = new String[] { "Levels/Tutorial/", "Levels/B1/", "Levels/B2/", "Levels/B3/" };
        static Texture2D bgtexture;
        static Texture2D[] wires = new Texture2D[32];
        static Color wireIdle = new Color(199, 85, 0),
                wireMouseOver = new Color(150, 150, 0),
                 wireSelected = new Color(0, 214, 0),
                wireCompleted = new Color(0, 224, 206),
                 wireDisabled = new Color(128, 128, 128);

        Button[] tabs = new Button[4];
        LevelSelectorButton[,] levels = new LevelSelectorButton[8, 4];
        Button back;
        private int curTab = 0;
        internal int CurTab
        {
            get { return curTab; }
            set
            {
                (tabs[curTab] as LevelSelectorTabsButton).IsSelected = false;
                curTab = value;
                if (curTab < 0) curTab = 0;
                if (curTab >= tabs.Length) curTab = tabs.Length - 1;
                (tabs[curTab] as LevelSelectorTabsButton).IsSelected = true;
            }
        }
        private int _selectedLevel = 0;
        internal int SelectedTab = 0;

        internal int selectedLevel
        {
            get { return _selectedLevel; }
            set { _selectedLevel = value; }
        }
        String selectedFile = "Tutorial";

        public override void Initialize()
        {
            ShouldBeScaled = false;

            back = new EncyclopediaBrowserButton(670, 15, 120, 30, "Back");
            back.onClicked += new Button.ClickedEventHandler(back_onClicked);
            back.foreground = Color.White;
            (back as EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
            controls.Add(back);

            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i] = new LevelSelectorTabsButton(15 + i * 130, 15, 120, 30, TABS_NAMES[i]);
                tabs[i].onClicked += new Button.ClickedEventHandler(Tabs_onClicked);
                tabs[i].foreground = Color.White;
                (tabs[i] as EncyclopediaBrowserButton).OverrideTexture("GUI/Menus/ButtonBackground2");
                controls.Add(tabs[i]);
            }

            //int w = (int)((Main.WindowWidth - 1) / levels.GetLength(0)),//why 1?
            //    h = (int)((Main.WindowHeight - 50) / levels.GetLength(1));
            int d = 0;
            for (int x = 0; x < levels.GetLength(0); x++)
            {
                if (x > 3) d = 16;
                for (int y = 0; y < levels.GetLength(1); y++)
                {
                    levels[x, y] = new LevelSelectorButton(16 + x * 96 + d, 54 + y * 90, 80, 80, (y * levels.GetLength(0) + x).ToString());
                    levels[x, y].Font = ResourceManager.Load<SpriteFont>("Fonts/LevelSelectorButtonFont");
                    levels[x, y].LoadImages("GUI/Menus/LevelSelector/Button", "GUI/Menus/LevelSelector/ButtonOver", "GUI/Menus/LevelSelector/ButtonSelected");
                    levels[x, y].onClicked += new Button.ClickedEventHandler(Levels_onClicked);
                    controls.Add(levels[x, y]);
                }
            }

            CurTab = curTab;

            base.Initialize();

            background = GUIEngine.s_mainMenu.background;
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);

            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i].position = new Microsoft.Xna.Framework.Vector2(tabs[i].position.X / oldw * w,
                    tabs[i].position.Y / oldh * h);
            }
            back.position = new Microsoft.Xna.Framework.Vector2(Main.WindowWidth - 120 - 15f / 800f * w,
                15f / 480f * h);


            for (int x = 0; x < levels.GetLength(0); x++)
            {
                for (int y = 0; y < levels.GetLength(1); y++)
                {
                    levels[x, y].position = new Microsoft.Xna.Framework.Vector2(levels[x, y].position.X / oldw * w+0.5f,
                        levels[x, y].position.Y / oldh * h);
                    //levels[x, y].position = new Microsoft.Xna.Framework.Vector2(16 + x * 96 + d, 54 + y * 90);
                    //levels[x, y].size = new Microsoft.Xna.Framework.Vector2(90 - 10, 90 - 10);
                    levels[x, y].size = new Microsoft.Xna.Framework.Vector2(80f / 800f * w, 80f / 480f * h);
                }
            }
        }

        public override void LoadContent()
        {
            bgtexture = ResourceManager.Load<Texture2D>("GUI/Menus/LevelSelector/Bg");
            for (int i = 0; i < 32; i++)
            {
                wires[i] = ResourceManager.Load<Texture2D>("GUI/Menus/LevelSelector/Wires/w" + i.ToString());
            }
            base.LoadContent();
        }

        void back_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.ChangeScene(GUIEngine.s_levelsMenu, "GUILevels");
        }

        public override void onShow()
        {
            InitForTab();
            base.onShow();
        }

        void Tabs_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            for (int i = 0; i < tabs.Length; i++)
            {
                if (sender == tabs[i])
                {
                    CurTab = i;
                    InitForTab();
                    return;
                }
            }
        }

        public void InitForTab()
        {
            for (int x = 0; x < levels.GetLength(0); x++)
            {
                for (int y = 0; y < levels.GetLength(1); y++)
                {
                    if (y * levels.GetLength(0) + x < TABS_LEVELS_COUNT[CurTab])
                    {
                        levels[x, y].isVisible = true;
                        levels[x, y].isEnabled = Logics.CampaingProgress.IsOpened(TABS_NAMES[CurTab],
                            y * levels.GetLength(0) + x);
                    }
                    else
                    {
                        levels[x, y].isVisible = false;
                    }
                }
            }
        }

        void Levels_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            selectedFile = TABS_FOLDERS[CurTab] + (sender as Button).Text;
            if (System.IO.File.Exists(selectedFile + ".lvl"))
            {
                selectedLevel = Convert.ToInt32((sender as Button).Text);
                SelectedTab = CurTab;

                StartLevel();

                levelSelected = true;
                //multithreading not working correctly. Dunno why.
                //GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_game, "GAMELevels");
                
                PostFadeOut();
                GUIEngine.curScene = Graphics.GUI.GUIEngine.s_game;
                Main.curState = "GAMELevels";
            }
        }

        public void StartLevel()
        {
            //Logics.LevelEngine.Stop();
            //Logics.GameInputHandler.PlacableAreas.Clear();
            //Settings.ResetInGameSettings();
            //Logics.CircuitManager.Clear();
            //Components.ComponentsManager.Clear();
            Logics.LevelEngine.Stop();
            Logics.GameLogicsHelper.InitForGame();

            IO.SaveEngine.LoadAll(selectedFile + ".lvl", IO.SaveEngine.SaveType.Levels);
            Logics.LevelEngine.LoadLevelScript(selectedFile + ".lua");
            Logics.GameLogicsHelper.InitScenesForGame();
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_purpose);
        }

        public void StartNextLevel()
        {
            selectedLevel++;
            if (selectedLevel >= TABS_LEVELS_COUNT[SelectedTab])
            {
                Sound.SoundPlayer.PlayButtonClick();
                GUIEngine.ChangeScene(GUIEngine.s_levelSelection, "GUILevelSelector");
            }
            else
            {
                selectedFile = selectedFile.Substring(0, selectedFile.LastIndexOf("/") + 1) + selectedLevel.ToString();
                StartLevel();
            }
        }

        System.Threading.Thread levelStarter;
        bool levelSelected = false;
        public override void PostFadeOut()
        {
            base.PostFadeOut();
            if (levelSelected)
            {
                try
                {
                    if (levelStarter != null) levelStarter.Abort();
                }
                catch { }
                levelStarter = new System.Threading.Thread(new System.Threading.ThreadStart(StartLevel));
                levelStarter.Start();
                while (levelStarter.IsAlive) System.Threading.Thread.Sleep(1);

                Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_componentSelector);
                Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_runControl);
                //StartLevel();
                levelSelected = false;
            }
        }

        internal void WriteSaveInfo(ref IO.SaveWriter sw)
        {
            sw.WriteLine(TABS_NAMES[SelectedTab]);
            sw.WriteLine(selectedLevel.ToString());
        }

        internal bool ReadSaveInfo(ref IO.SaveReader sr, bool start)
        {
            String t = sr.ReadLine();
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
            }
            //selectedLevel = Convert.ToInt32(sr.ReadLine());
            sr.ReadLine();
            if (!IsLevelOpened(t, selectedLevel))
            {
                CurTab = tct;
                return false;
            }
            String s = TABS_FOLDERS[CurTab] + selectedLevel.ToString();
            if (System.IO.File.Exists(s + ".lua") && start)
                Logics.LevelEngine.LoadLevelScript(s + ".lua");
            else
            {
                CurTab = tct;
                return !start;
            }
            //selectedTab = curTab;
            return true;
        }

        internal bool BlankReadSaveInfo(ref IO.SaveReader sr, bool start)
        {
            String t = sr.ReadLine();
            sr.ReadLine();
            return true;
        }

        public bool IsLevelOpened(String tabName, int level)
        {
            return Logics.CampaingProgress.IsOpened(tabName, level);
        }






        Vector2 fadeSize = new Vector2();
        public override void Draw(Renderer renderer)
        {
            //tabs[selectedTab].isMouseOver = true;

            if (background != null) background.Draw(renderer);
            if (fadeSize.Y != Main.WindowHeight)
                renderer.SetScissorRectangle((int)(Main.WindowWidth - fadeSize.X) / 2, (int)(Main.WindowHeight - fadeSize.Y) / 2,
                    (int)fadeSize.X, (int)fadeSize.Y, false);
            renderer.Draw(bgtexture, new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight), Color.White);
            foreach (Control c in controls)
            {
                c.Draw(renderer);
            }
            for (int x = 0; x < levels.GetLength(0); x++)
            {
                for (int y = 0; y < levels.GetLength(1); y++)
                {
                    if (levels[x, y].isVisible)
                    {
                        renderer.Draw(wires[y * levels.GetLength(0) + x], new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight),
                            !levels[x, y].isEnabled ? wireDisabled :
                            levels[x, y].isPressed ? wireSelected :
                            levels[x,y].isMouseOver ? wireMouseOver :
                            Logics.CampaingProgress.IsComplete(TABS_NAMES[CurTab], y * levels.GetLength(0) + x) ? wireCompleted :
                            wireIdle);
                    }
                    //levels[x, y] = new ImageButton(16 + x * 96 + d, 54 + y * 90, 80, 80, (y * levels.GetLength(0) + x).ToString());
                }
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





    }
}
