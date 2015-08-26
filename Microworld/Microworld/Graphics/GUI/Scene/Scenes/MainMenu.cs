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
    class MainMenu : HUDScene
    {
        internal class MenuState
        {
            public String caption = "";
            public List<Elements.MenuButton> items = new List<Elements.MenuButton>();
        }

        String[] tip = new String[]{
            "Tip: You can merge joints by holding <Ctrl> and dragging one joint to another.",
            "Tip: You can connect joints by selecting one joint and shift-clicking to another joint."
        };
        String currentTip = "";

        #region Items
        public Elements.MenuButton[] items = new Elements.MenuButton[]{
            new Elements.MenuButton("Campaign"){Children = new Elements.MenuButton[]{
                new Elements.MenuButton("New"){Children = new Elements.MenuButton[]{
                    new Elements.MenuButton("Tutorial"){frameScene = GUIEngine.s_levelSelection, tag = new object[]{6, "Levels/Tut/"}},/*
                    new Elements.MenuButton("Input Core"){frameScene = GUIEngine.s_levelSelection, tag = new object[]{5, "Levels/B1/"}},
                    new Elements.MenuButton("Logics Core"){frameScene = GUIEngine.s_levelSelection, tag = new object[]{4, "Levels/B2/"}},
                    new Elements.MenuButton("Output Core"){frameScene = GUIEngine.s_levelSelection, tag = new object[]{3, "Levels/B3/"}},//*/
                    new Elements.MenuButton("Back")
                }},
                new Elements.MenuButton("Load"){frameScene = GUIEngine.s_levelLoad},
                new Elements.MenuButton("Back")
            }},
            new Elements.MenuButton("Sandbox"){Children = new Elements.MenuButton[]{
                new Elements.MenuButton("New Sandbox"),
                new Elements.MenuButton("New Level"),
                new Elements.MenuButton("Load Sandbox"){frameScene = GUIEngine.s_sandboxLoad, ShouldStaySelectedAfterClick = true},
                new Elements.MenuButton("Load Level"){frameScene = GUIEngine.s_levelDesignerLoad, ShouldStaySelectedAfterClick = true},
                new Elements.MenuButton("Back")
            }},
            new Elements.MenuButton("Handbook"),
            new Elements.MenuButton("Options"){Children = new Elements.MenuButton[]{
                new Elements.MenuButton("Video"){frameScene = GUIEngine.s_optionsGraphics},
                new Elements.MenuButton("Audio"){frameScene = GUIEngine.s_optionsAudio},
                new Elements.MenuButton("Controls"){frameScene = GUIEngine.s_optionsControls},
                new Elements.MenuButton("Accept"),
                new Elements.MenuButton("Back")
            }},
            new Elements.MenuButton("Statistics"){frameScene = GUIEngine.s_statistics},
            new Elements.MenuButton("Credits"){frameScene = GUIEngine.s_credits},
            new Elements.MenuButton("Quit")//,
            //new Elements.MenuButton("Feedback")
        };
        public Elements.MenuButton[] inGameItems = new Elements.MenuButton[]{
            new Elements.MenuButton("Resume"),
            new Elements.MenuButton("Save"){frameScene = GUIEngine.s_save},
            new Elements.MenuButton("Restart"),
            new Elements.MenuButton("Handbook"),
            new Elements.MenuButton("Options"){Children = new Elements.MenuButton[]{
                new Elements.MenuButton("Video"){frameScene = GUIEngine.s_optionsGraphics},
                new Elements.MenuButton("Audio"){frameScene = GUIEngine.s_optionsAudio},
                new Elements.MenuButton("Controls"){frameScene = GUIEngine.s_optionsControls},
                new Elements.MenuButton("Accept"),
                new Elements.MenuButton("Back")
            }},
            new Elements.MenuButton("Main Menu"),
            new Elements.MenuButton("Quit")//,
            //new Elements.MenuButton("Feedback")
        };
        #endregion

        #region Animations
        Animation.MainMenu.GlobalAnimation animGlob = new Animation.MainMenu.GlobalAnimation();
        Animation.MainMenu.ItemsAnimation animItems = new Animation.MainMenu.ItemsAnimation();
        Animation.MainMenu.FrameAnimation animFrame = new Animation.MainMenu.FrameAnimation();
        #endregion

        internal MenuFrameScene nextFrame = null;
        internal MenuFrameScene currentFrame = null;

        internal MenuState currentState = new MenuState();
        internal MenuState nextState = new MenuState();

        YesNoMessageBox resChangeMb;
        int TicksSinceResChange = 0;

        public RenderTarget2D fboback, fbofront, fboresult;

        public SpriteFont fontCurrent;

        internal static float FrameOffset = 0;

        internal Vector2 CaptionPosition = new Vector2();
        #region lines
        //line 1 == main horizontal
        internal Vector2 line1p1 = new Vector2();//start
        internal Vector2 line1p2 = new Vector2();//end
        //line 2 == main vertical
        internal Vector2 line2p1 = new Vector2();//start
        internal Vector2 line2p2 = new Vector2();//end
        //line 3 == frame horizontal
        internal Vector2 line3p1 = new Vector2();//start
        internal Vector2 line3p2 = new Vector2();//end
        //line 4 == frame vertical
        internal Vector2 line4p1 = new Vector2();//start
        internal Vector2 line4p2 = new Vector2();//end
        #endregion

        public override void Initialize()
        {
            Layer = 950;
            animGlob.Init(this);

            CalculateLocations(items);
            CalculateLocations(inGameItems);
            RegisterEvents();

            fboback = Main.renderer.CreateFBO(Main.WindowWidth, Main.WindowHeight);
            fbofront = Main.renderer.CreateFBO(Main.WindowWidth, Main.WindowHeight);
            fboresult = Main.renderer.CreateFBOWStencil(Main.WindowWidth, Main.WindowHeight);

            background = new Background.ParallaxBackground();

            currentFrame = null;
            InitMenuFor(items, "MENU");

            base.Initialize();
        }

        public override void onShow()
        {
            line3p2.X = line3p1.X;
            line4p2.Y = line4p1.Y;
            gameBGOpacity = 0;
            animGlob.FadeIn();
            animItems.FadeIn();
            if (currentFrame != null)
                animFrame.FadeIn();
            currentTip = tip[new Random().Next(tip.Length * 10) % tip.Length];//TODO rm???
            UpdateLevelPackAvalability();
            base.onShow();
        }

        public override void onClose()
        {
            base.onClose();
            line3p1 = new Vector2();
            line3p2 = new Vector2();
            line4p1 = new Vector2();
            line4p2 = new Vector2();

            if (items[0] == inGameItems[0])
                Logics.GameLogicsHelper._gameResume();
        }

        #region Items stuff
        public void CalculateLocations(Elements.MenuButton[] el)
        {
            for (int i = 0; i < el.Length; i++)
            {
                el[i].Position = new Vector2(126 * Main.WindowWidth / 1920 + 1, 587 * Main.WindowHeight / 1080 + 30 * i);
                el[i].Size = new Vector2(236, 30);
                CalculateLocations(el[i].Children);
            }
        }

        public void InitMenuFor(Elements.MenuButton[] el, String caption, bool instant = false)
        {
            nextState.caption = caption;

            for (int i = 0; i < el.Length; i++)
            {
                nextState.items.Add(el[i]);
                nextState.items[i].onClicked -= new Elements.Button.ClickedEventHandler(AnyItem_onClicked);
                nextState.items[i].onClicked += new Elements.Button.ClickedEventHandler(AnyItem_onClicked);
                nextState.items[i].isVisible = false;
                nextState.items[i].Initialize();
                nextState.items[i].ResetMouseOverAnimation();
                nextState.items[i].WasInitiallyDrawn = false;
            }

            animItems.onFadeOutFinish += new Animation.SceneAnimation.FadeEventHandler(animItems_onFadeOutFinish);
            if (animItems.Scene != null)
            {
                animItems.StopFadeIn();
                animItems.FadeOut();
            }
            else
            {
                animItems_onFadeOutFinish(animItems);
            }

            if (instant)
            {
                if (animItems.IsFadeOut)
                    animItems.StopFade();
                animFrame.StopFade();
                System.Threading.Thread.Sleep(20);

                if (!animItems.IsFadeIn)
                    animItems_onFadeOutFinish(animFrame);
                animFrame_onFadeOutFinish(animItems);

                animItems.onFadeOutFinish -= new Animation.SceneAnimation.FadeEventHandler(animItems_onFadeOutFinish);
                animFrame.onFadeOutFinish -= new Animation.SceneAnimation.FadeEventHandler(animFrame_onFadeOutFinish);
            }
        }

        void animItems_onFadeOutFinish(object sender)
        {
            lock (controls)
                controls.Clear();

            currentState = nextState;
            nextState = new MenuState();

            lock (controls)
            {
                for (int i = 0; i < currentState.items.Count; i++)
                {
                    controls.Add(currentState.items[i]);
                }
            }

            animItems.onFadeOutFinish -= new Animation.SceneAnimation.FadeEventHandler(animItems_onFadeOutFinish);

            animItems = new Animation.MainMenu.ItemsAnimation();
            animItems.Init(this);
            animItems.FadeIn();
        }

        public void RegisterEvents()
        {
            #region MainMenu
            //campaign
            //items[0].onClicked += new Elements.Button.ClickedEventHandler(MainMenu_onClicked);//TODO change back, reenable UpdateLevelPackAvalability()
            //*
            items[0].Children[0].onClicked += new Elements.Button.ClickedEventHandler(CampaignNew_onClicked);
            items[0].Children[2].onClicked += new Elements.Button.ClickedEventHandler(BackToMainMenu_onClicked);
            {
                //level selection
                items[0].Children[0].Children[0].onClicked += new Elements.Button.ClickedEventHandler(Campaign_New_Category_onClicked);
                //items[0].Children[0].Children[1].onClicked += new Elements.Button.ClickedEventHandler(Campaign_New_Category_onClicked);
                //items[0].Children[0].Children[2].onClicked += new Elements.Button.ClickedEventHandler(Campaign_New_Category_onClicked);
                //items[0].Children[0].Children[3].onClicked += new Elements.Button.ClickedEventHandler(Campaign_New_Category_onClicked);
                items[0].Children[0].Children[1].onClicked += new Elements.Button.ClickedEventHandler(BackToCampaign_onClicked);
            }//*/
            //sandbox
            items[1].Children[0].onClicked += new Elements.Button.ClickedEventHandler(Sandbox_NewBox_onClicked);
            items[1].Children[1].onClicked += new Elements.Button.ClickedEventHandler(Sandbox_NewLevel_onClicked);
            items[1].Children[4].onClicked += new Elements.Button.ClickedEventHandler(BackToMainMenu_onClicked);
            //handbook
            items[2].onClicked += new Elements.Button.ClickedEventHandler(Handbook_onClicked);
            //options
            items[3].onClicked += new Elements.Button.ClickedEventHandler(Options_onClicked);
            items[3].Children[3].onClicked += new Elements.Button.ClickedEventHandler(Options_Save_onClicked);
            items[3].Children[4].onClicked += new Elements.Button.ClickedEventHandler(Options_Back_onClicked);
            //statistics
            //items[4].onClicked += new Elements.Button.ClickedEventHandler(Statistics_onClicked);
            //credits
            //items[5].onClicked += new Elements.Button.ClickedEventHandler(Credits_onClicked);
            //quit
            items[6].onClicked += new Elements.Button.ClickedEventHandler(Quit_onClicked);
            //feedback
            //items[7].onClicked += new Elements.Button.ClickedEventHandler(Feedback_onClicked);
            #endregion

            #region InGameMenu
            //resume
            inGameItems[0].onClicked+=new Elements.Button.ClickedEventHandler(igResume_onClicked);
            //save
            inGameItems[1].onClicked += new Elements.Button.ClickedEventHandler(igSave_onClicked);
            //restart
            inGameItems[2].onClicked += new Elements.Button.ClickedEventHandler(igRestart_onClicked);
            //handbook
            inGameItems[3].onClicked += new Elements.Button.ClickedEventHandler(Handbook_onClicked);
            //options
            inGameItems[4].onClicked += new Elements.Button.ClickedEventHandler(Options_onClicked);
            inGameItems[4].Children[3].onClicked += new Elements.Button.ClickedEventHandler(ig_Options_Save_onClicked);
            inGameItems[4].Children[4].onClicked += new Elements.Button.ClickedEventHandler(igOptions_Back_onClicked);
            //mainmenu
            inGameItems[5].onClicked += new Elements.Button.ClickedEventHandler(igMainMenu_onClicked);
            //Exit
            inGameItems[6].onClicked += new Elements.Button.ClickedEventHandler(Quit_onClicked);
            //feedback
            //inGameItems[7].onClicked += new Elements.Button.ClickedEventHandler(Feedback_onClicked);
            #endregion
        }

        void MainMenu_onClicked(object sender, InputEngine.MouseArgs e)
        {
            OKMessageBox.Show("Not implemented yet");
        }

        void CampaignNew_onClicked(object sender, InputEngine.MouseArgs e)
        {
            UpdateLevelPackAvalability();
        }

        public void InitForInGame()
        {
            InitMenuFor(inGameItems, "");

            if (animItems.IsFadeOut)
                animItems.StopFade();
            animFrame.StopFade();

            System.Threading.Thread.Sleep(20);

            if (!animItems.IsFadeIn)
                animItems_onFadeOutFinish(animFrame);
            animFrame_onFadeOutFinish(animItems);

            animItems.onFadeOutFinish -= new Animation.SceneAnimation.FadeEventHandler(animItems_onFadeOutFinish);
            animFrame.onFadeOutFinish -= new Animation.SceneAnimation.FadeEventHandler(animFrame_onFadeOutFinish);
        }

        public void CloseFrame()
        {
            nextFrame = null;

            animFrame.Init(this);
            animFrame.onFadeOutFinish += new Animation.SceneAnimation.FadeEventHandler(animFrame_onFadeOutFinish);
            animFrame.FadeOut();
        }

        public void UpdateLevelPackAvalability()
        {/*
            items[0].Children[0].Children[2].isEnabled = Logics.CampaingProgress.IsComplete(LevelSelection.TABS_NAMES[1], LevelSelection.TABS_LEVELS_COUNT[1] - 1);
            items[0].Children[0].Children[2].WasInitiallyDrawn = false;//*/
        }

        #region Events
        void Feedback_onClicked(object sender, InputEngine.MouseArgs e)
        {
            if (Settings.IsFullscreen)
            {
                var form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Main.game.Window.Handle);
                var aasd = form.WindowState;
                form.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            }
            var a = new Debug.ReportSender();
            a.InitForFeedback();
            a.ShowDialog();
            a.Activate();
            a.BringToFront();
        }

        /*
        void Credits_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Graphics.GUI.GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_credits2, "GUICredits");
        }
        
        void Statistics_onClicked(object sender, InputEngine.MouseArgs e)
        {
            GUIEngine.s_statistics2.CameFromState = Main.curState;
            GUIEngine.s_statistics2.CameFromScene = GUIEngine.curScene;
            GUIEngine.ChangeScene(GUIEngine.s_statistics2, "GUIStatistics");
        }
        //*/
        void Handbook_onClicked(object sender, InputEngine.MouseArgs e)
        {
            GUIEngine.s_handbook.InitForFolder("");
            InitMenuFor(GetButtonsForHandbook(false), "HANDBOOK");
        }

        #region HandbookStuff
        public void InitForHandbook(bool tutorial)
        {
            GUIEngine.s_handbook.InitForFolder("");
            InitMenuFor(GetButtonsForHandbook(tutorial), "HANDBOOK");

            animFrame.Init(this);
            animFrame.onFadeOutFinish += new Animation.SceneAnimation.FadeEventHandler(animFrame_onFadeOutFinish);
            animFrame.FadeOut();

            if (animItems.IsFadeOut)
                animItems.StopFadeOut();
            animFrame.StopFade();

            System.Threading.Thread.Sleep(13);

            nextFrame = GUIEngine.s_handbook;
            animFrame_onFadeOutFinish(animFrame);
            if (!animItems.IsFadeIn)
                animItems_onFadeOutFinish(animItems);

            //animItems.onFadeOutFinish -= new Animation.SceneAnimation.FadeEventHandler(animItems_onFadeOutFinish);
            //animFrame.onFadeOutFinish -= new Animation.SceneAnimation.FadeEventHandler(animFrame_onFadeOutFinish);
        }

        public Elements.MenuButton[] GetButtonsForHandbook(bool tutorial)
        {
            Elements.MenuButton[] m;
            var a = System.IO.Directory.GetDirectories("Content/Encyclopedia/");
            m = new Elements.MenuButton[a.Length];
            int curind = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (!a[i].EndsWith("/Resources"))
                {
                    m[curind] = new Elements.MenuButton(GetDirnameForPath(a[i])) { frameScene = GUIEngine.s_handbook };
                    m[curind].tag = a[i];
                    m[curind].onClicked += new Elements.Button.ClickedEventHandler(Handbook_Topic_onClicked);
                    curind++;
                }
            }
            m[a.Length - 1] = new Elements.MenuButton("Back");
            m[a.Length - 1].onClicked += new Elements.Button.ClickedEventHandler(Handbook_Back_onClicked);
            m[a.Length - 1].tag = (byte)(tutorial ? 0 : currentState.items[0] == items[0] ? 1 : 2);
            CalculateLocations(m);
            return m;
        }

        void Handbook_Back_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            byte t = (byte)((sender as Elements.MenuButton).tag);
            switch (t)
            {
                case 0:
                    Close();
                    break;
                case 1:
                    BackToMainMenu_onClicked(null, null);
                    break;
                case 2:
                    CloseFrame();
                    InitMenuFor(inGameItems, "");
                    break;
                default:
                    break;
            }
        }

        void Handbook_Topic_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.s_handbook.InitForFolder((sender as Elements.MenuButton).tag as string);
        }

        private String GetDirnameForPath(String s)
        {
            s = s.Substring(s.LastIndexOf('/') + 1);
            return s;
        }
        #endregion

        void Options_onClicked(object sender, InputEngine.MouseArgs e)
        {
            GUIEngine.s_optionsGraphics.origAR = Settings.AspectRatio;
            GUIEngine.s_optionsGraphics.origRes = Settings.Resolution;
            GUIEngine.s_optionsGraphics.origFS = Settings.IsFullscreen;
        }

        void Options_Back_onClicked(object sender, InputEngine.MouseArgs e)
        {
            if (!animItems.IsFadeOut)
            {
                InitMenuFor(items, "MENU");
                Settings.Load();
                Settings.ChangeResolution();
            }
        }

        void Options_Save_onClicked(object sender, InputEngine.MouseArgs e)
        {
            if (!animItems.IsFadeOut)
            {
                InitMenuFor(items, "MENU");

                Settings.ChangeResolution();

                if (Settings.Resolution != GUIEngine.s_optionsGraphics.origRes || GUIEngine.s_optionsGraphics.origFS != Settings.IsFullscreen)
                {
                    resChangeMb = YesNoMessageBox.Show("Do you want to keep this resolution?\r\nChanges will be reverted in 15 second(s)");
                    resChangeMb.onButtonClicked += new YesNoMessageBox.ButtonClickedEventHandler(mb_onButtonClicked);
                    TicksSinceResChange = 0;
                }

                Sound.SoundPlayer.PlayButtonClick();
                Settings.Save();
            }
        }

        void mb_onButtonClicked(object sender, YesNoMessageBox.ButtonClickedArgs e)
        {
            if (e.button == 0)
            {
                Settings.Resolution = GUIEngine.s_optionsGraphics.origRes;
                Settings.AspectRatio = GUIEngine.s_optionsGraphics.origAR;
                Settings.IsFullscreen = GUIEngine.s_optionsGraphics.origFS;
                Settings.ChangeResolution();
                Settings.Save();
            }
            resChangeMb.Close();
        }

        void Campaign_New_Category_onClicked(object sender, InputEngine.MouseArgs e)
        {
            GUIEngine.s_levelSelection.folder = ((sender as Elements.Button).tag as object[])[1] as String;
            animFrame.Tag = (int)(((sender as Elements.Button).tag as object[])[0]);
            //GUIEngine.s_levelSelection.InitForItemsCount((int)(((sender as Elements.Button).tag as object[])[0]));
        }

        void BackToMainMenu_onClicked(object sender, InputEngine.MouseArgs e)
        {
            if (!animItems.IsFadeOut)
            {
                InitMenuFor(items, "MENU");
            }
        }

        void BackToCampaign_onClicked(object sender, InputEngine.MouseArgs e)
        {
            if (!animItems.IsFadeOut)
            {
                InitMenuFor(items[0].Children, items[0].Text);
            }
        }

        #region Sandbox
        void Sandbox_NewLevel_onClicked(object sender, InputEngine.MouseArgs e)
        {
            GUIEngine.s_componentSelector.ClearCount();
            Logics.GameLogicsHelper.InitForGame();

            Main.curState = "GAMElvlDesign";
            Logics.GameLogicsHelper.InitScenesForGame();
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_placableAreasCreator);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_scriptEditor);
            GUIEngine.curScene = GUIEngine.s_game;
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_componentSelector);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_runControl);
        }

        void Sandbox_NewBox_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Logics.GameLogicsHelper.InitForGame();
            GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_game, "GAMESandbox");
            GUIEngine.s_componentSelector.ClearCount();
            Logics.GameLogicsHelper.InitScenesForGame();
        }
        #endregion

        void Quit_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Main.Close();
        }

        #region InGameMenu
        void igMainMenu_onClicked(object sender, InputEngine.MouseArgs e)
        {
            if (!animItems.IsFadeOut)
            {
                InitMenuFor(items, "MENU");
                animItems.onFadeOutFinish += new Animation.SceneAnimation.FadeEventHandler(animItems_onFadeOutFinish2);
            }
        }

        void animItems_onFadeOutFinish2(object sender)
        {
            GUIEngine.RemoveHUDScene(this);
            GUIEngine.ChangeScene(this, "GUIMainMenu");
            GUIEngine.ClearHUDs();
            Logics.LevelEngine.Stop();
            Graphics.GraphicsEngine.camera.Center = new Vector2();
        }

        void igOptions_Back_onClicked(object sender, InputEngine.MouseArgs e)
        {
            if (!animItems.IsFadeOut)
            {
                InitMenuFor(inGameItems, "");
                Settings.Load();
                Settings.ChangeResolution();
            }
        }

        void ig_Options_Save_onClicked(object sender, InputEngine.MouseArgs e)
        {
            if (!animItems.IsFadeOut)
            {
                InitMenuFor(inGameItems, "");

                Settings.ChangeResolution();

                if (Settings.Resolution != GUIEngine.s_optionsGraphics.origRes || GUIEngine.s_optionsGraphics.origFS != Settings.IsFullscreen)
                {
                    resChangeMb = YesNoMessageBox.Show("Do you want to keep this resolution?\r\nChanges will be reverted in 15 second(s)");
                    resChangeMb.onButtonClicked += new YesNoMessageBox.ButtonClickedEventHandler(mb_onButtonClicked);
                    TicksSinceResChange = 0;
                }

                Sound.SoundPlayer.PlayButtonClick();
                Settings.Save();
            }
        }

        internal void igRestart_onClicked(object sender, InputEngine.MouseArgs e)
        {
            if (Main.curState == "GAMELevels")
            {
                Logics.GameLogicsHelper.GlobalEvents_onLevelExited();
                GUIEngine.s_levelSelection.StartLevel();
                GUIEngine.RemoveHUDScene(this);
            }
            else
            {
                Components.ComponentsManager.Clear();
                Logics.CircuitManager.Clear();
                GUIEngine.RemoveHUDScene(this);
            }
        }

        void igSave_onClicked(object sender, InputEngine.MouseArgs e)
        {
            if (Main.curState == "GAMESandbox")
            {
                GUIEngine.s_save.folder = "Sandbox";
                GUIEngine.s_save.mask = "*.sav";
            }
            else if (Main.curState == "GAMELevels")
            {
                GUIEngine.s_save.folder = "Levels";
                GUIEngine.s_save.mask = "*.sav";
            }
            else if (Main.curState == "GAMElvlDesign")
            {
                GUIEngine.s_save.folder = "Levels";
                GUIEngine.s_save.mask = "*.lvl";
            }
        }

        void igResume_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Close();
        }
        #endregion

        void AnyItem_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();

            for (int i = 0; i < currentState.items.Count; i++)
            {
                currentState.items[i].StaySelected = false;
            }
            if ((sender as Elements.MenuButton).ShouldStaySelectedAfterClick)
            {
                (sender as Elements.MenuButton).StaySelected = true;
            }

            if ((sender as Elements.MenuButton).Children.Length != 0)
            {
                if (!animItems.IsFadeOut)
                {
                    (sender as Elements.MenuButton).StaySelected = false;
                    InitMenuFor((sender as Elements.MenuButton).Children, (sender as Elements.MenuButton).Text.ToUpper());
                }
            }
            var a = (sender as Elements.MenuButton).frameScene;
            nextFrame = a;

            if (!animFrame.IsFadeOut)
            {
                animFrame.Init(this);
                animFrame.onFadeOutFinish += new Animation.SceneAnimation.FadeEventHandler(animFrame_onFadeOutFinish);
                animFrame.FadeOut();
            }
        }

        void animFrame_onFadeOutFinish(object sender)
        {
            animFrame.onFadeOutFinish -= new Animation.SceneAnimation.FadeEventHandler(animFrame_onFadeOutFinish);
            if (nextFrame != null)
                nextFrame.isVisible = false;
            if (nextFrame != null && nextFrame == GUIEngine.s_levelSelection)
            {
                GUIEngine.s_levelSelection.InitForItemsCount((int)animFrame.Tag);
                UpdateLevelPackAvalability();
            }
            if (currentFrame != null)
                currentFrame.onClose();
            currentFrame = nextFrame;
            nextFrame = null;
            if (currentFrame != null)
                currentFrame.onShow();
            animFrame = new Animation.MainMenu.FrameAnimation();
            animFrame.Init(this);
            animFrame.FadeIn();
        }
        #endregion
        #endregion

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            line1p1 = new Microsoft.Xna.Framework.Vector2(0, 126f * h / 1080f);
            line1p2 = new Microsoft.Xna.Framework.Vector2(501f * w / 1920f, 126f * h / 1080f);
            line2p1 = new Microsoft.Xna.Framework.Vector2(126f * w / 1920f, 126f * h / 1080f);
            line2p2 = new Microsoft.Xna.Framework.Vector2(126f * w / 1920f, (126f + 863f) * h / 1080f);

            Microsoft.Xna.Framework.Vector2 lp2 = new Microsoft.Xna.Framework.Vector2(501f * w / 1920f, 126f * h / 1080f);

            line3p1 = lp2;
            line3p2 = lp2;
            line4p1 = lp2;
            line4p2 = lp2;

            line3p2.X = line3p1.X + 1263f * w / 1920f;
            line4p2.Y = line4p1.Y + 863f * h / 1080f;

            CaptionPosition = new Vector2(CaptionPosition.X * w / oldw, CaptionPosition.Y * w / oldw);
            
            fboback.Dispose();
            fbofront.Dispose();
            fboresult.Dispose();
            fboback = Main.renderer.CreateFBO(w, h);
            fbofront = Main.renderer.CreateFBO(w, h);
            fboresult = Main.renderer.CreateFBOWStencil(w, h);

            CalculateLocations(items);
            CalculateLocations(inGameItems);

            for (int i = 0; i < currentState.items.Count; i++)
            {
                currentState.items[i].WasInitiallyDrawn = false;
            }

            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void OnGraphicsDeviceReset()
        {
            for (int i = 0; i < currentState.items.Count; i++)
            {
                currentState.items[i].WasInitiallyDrawn = false;
            }
            base.OnGraphicsDeviceReset();
        }

        public override void LoadContent()
        {
            fontCurrent = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_30");

            base.LoadContent();
        }

        public override void Update()
        {
            #region ResChangeMB
            if (resChangeMb != null && resChangeMb.isVisible)
            {
                TicksSinceResChange++;
                if (TicksSinceResChange >= 15 * 60)
                {
                    TicksSinceResChange = 0;
                    resChangeMb.Close();
                    mb_onButtonClicked(null, new YesNoMessageBox.ButtonClickedArgs() { button = 0 });
                }
                else
                {
                    resChangeMb.Text = "Do you want to keep this resolution?\r\nChanges will be reverted in " +
                        ((int)((15 * 60 - TicksSinceResChange) / 60)).ToString() + " second(s)";
                }
            }
            #endregion

            if (currentFrame!=null)
                currentFrame.Update();

            base.Update();
        }

        float gameBGOpacity = 0f;
        public override void Draw(Renderer renderer)
        {
            if (background != null && (Main.CurState.StartsWith("GUI") || 
                (currentState.items.Count > 0 && currentState.items[0] != inGameItems[0] && currentState.items[0] != inGameItems[4].Children[0] &&
                currentState.caption != "HANDBOOK")))
            {
                gameBGOpacity = 0;
                background.Draw(renderer);
                var atr = GUIEngine.font.MeasureString(currentTip);
                renderer.DrawString(GUIEngine.font, ">> " + currentTip + " <<", new Rectangle(0, (int)(Main.WindowHeight - atr.Y), Main.WindowWidth, (int)atr.Y), 
                    new Color(180, 255, 180), Renderer.TextAlignment.Center);
            }
            else
            {
                if (gameBGOpacity < 0.5f)
                    gameBGOpacity += 0.02f;
                renderer.Draw(MicroWorld.Graphics.GraphicsEngine.pixel, new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight),
                    new Color(45, 57, 107) * gameBGOpacity);
            }

            renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, 0, 0, 0), Color.White);
            renderer.DrawLinesList(new VertexPositionColorTexture[]{
                new VertexPositionColorTexture(new Vector3(line1p1.X, line1p1.Y,0), Color.White, new Vector2()),
                new VertexPositionColorTexture(new Vector3(line1p2.X, line1p2.Y, 0), Color.White, new Vector2()),
                new VertexPositionColorTexture(new Vector3(line2p1.X, line2p1.Y,0), Color.White, new Vector2()),
                new VertexPositionColorTexture(new Vector3(line2p2.X, line2p2.Y, 0), Color.White, new Vector2()),
                new VertexPositionColorTexture(new Vector3(line3p1.X, line3p1.Y,0), Color.White, new Vector2()),
                new VertexPositionColorTexture(new Vector3(line3p2.X, line3p2.Y, 0), Color.White, new Vector2()),
                new VertexPositionColorTexture(new Vector3(line4p1.X, line4p1.Y,0), Color.White, new Vector2()),
                new VertexPositionColorTexture(new Vector3(line4p2.X, line4p2.Y, 0), Color.White, new Vector2())
            });

            renderer.SetScissorRectangle(line2p1.X+1, line1p1.Y, Main.WindowWidth, Main.WindowHeight, false);
            lock (controls)
            {
                foreach (Elements.Control c in controls)
                {
                    c.Draw(renderer);
                }
            }

            renderer.DrawString(fontCurrent, currentState.caption, new Vector2(CaptionPosition.X + 10, CaptionPosition.Y + 10), Color.White,
                Renderer.TextAlignment.Left);

            if (currentFrame != null && currentFrame.isVisible)
            {
                renderer.SetScissorRectangle(line3p1.X, line3p1.Y, Main.WindowWidth, Main.WindowHeight, false);
                renderer.End();
                renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, null,
                    null, Graphics.GraphicsEngine.s_ScissorsOn, null, Matrix.CreateTranslation(FrameOffset, 0, 0));
                currentFrame.Draw(renderer);
            }
            renderer.ResetScissorRectangle();
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (currentFrame != null)
                currentFrame.PostDraw();
        }

        #region IO
        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            base.onButtonClick(e);
            if (!e.Handled && currentFrame != null)
                currentFrame.onButtonClick(e);
            e.Handled = true;
        }

        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            base.onButtonDown(e);
            if (!e.Handled && currentFrame != null)
                currentFrame.onButtonDown(e);
            e.Handled = true;
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            base.onButtonUp(e);
            if (!e.Handled && currentFrame != null)
                currentFrame.onButtonUp(e);
            //e.Handled = true;
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.onMouseMove(e);
            if (!e.Handled && currentFrame != null)
                currentFrame.onMouseMove(e);
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            base.onMouseWheelMove(e);
            if (!e.Handled && currentFrame != null)
                currentFrame.onMouseWheelMove(e);
            e.Handled = true;
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            base.onKeyPressed(e);
            if (!e.Handled && e.key == Keys.Escape.GetHashCode() && Main.curState.StartsWith("GAME") && !animItems.IsFadeOut)
            {
                e.Handled = true;
                igResume_onClicked(null, null);
            }
            if (!e.Handled && currentFrame != null)
                currentFrame.onKeyPressed(e);
            e.Handled = true;
        }

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
            base.onKeyDown(e);
            if (!e.Handled && currentFrame != null)
                currentFrame.onKeyDown(e);
            e.Handled = true;
        }

        public override void onKeyUp(InputEngine.KeyboardArgs e)
        {
            base.onKeyUp(e);
            if (!e.Handled && currentFrame != null)
                currentFrame.onKeyUp(e);
            e.Handled = true;
        }
        #endregion
    }
}
