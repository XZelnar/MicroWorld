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
using MicroWorld.Graphics.GUI.Scene;

namespace MicroWorld.Graphics.GUI
{
    public static class GUIEngine
    {
        private static Scene.Scene curScene_;
        internal static Scene.Scene curScene
        {
            get { return GUIEngine.curScene_; }
            set
            {
                if (curScene_ != null)
                {
                    curScene_.onClose();
                    Scenes.Remove(curScene_);
                }
                GUIEngine.curScene_ = value;
                if (value == null)
                    curScene_ = s_empty;
                Scenes.Add(curScene_);
                SortScenes();
                curScene_.onShow();
            }
        }
        public static Scene.Scene CurScene
        {
            get { return curScene_; }
        }
        private static List<Scene.Scene> Scenes = new List<Scene.Scene>();
        private static List<Scene.HUDScene> HUDScenes = new List<Scene.HUDScene>();
        public static SpriteFont font;

        #region Events
        public delegate void HUDSceneEventHandler(String name);
        public static event HUDSceneEventHandler onHUDSceneAdded;
        public static event HUDSceneEventHandler onHUDSceneRemoved;
        public static event HUDSceneEventHandler onHUDSceneOpened;
        public static event HUDSceneEventHandler onHUDSceneClosed;
        #endregion

        #region Scenes
        internal static Scene.Empty s_empty = new Empty();
        //global
        internal static IntroSequence s_intro;
        internal static MainMenu2 s_mainMenu2;
        internal static Options s_options;
        internal static StatisticsScreen s_statistics2;
        internal static SandboxMenu s_sandboxMenu;
        internal static SandboxLoadMenu s_sandboxLoadMenu;
        internal static SandboxSaveMenu s_sandboxSaveMenu;
        internal static Loading s_loading;
        internal static LevelsMenu s_levelsMenu;
        internal static LevelsSaveMenu s_levelsSaveMenu;
        internal static LevelsLoadMenu s_levelsLoadMenu;
        internal static EncyclopediaPage s_encyclopediaPage;
        internal static EncyclopediaEditor s_encyclopediaEditor;
        internal static Credits s_credits2;
        internal static EncyclopediaBrowser s_encyclopediaBrowser;
        internal static LevelDesignerMenu s_lvlDesignerMenu;
        internal static LevelDesignerLoadMenu s_lvlDesignerLoad;
        internal static LevelDesignerSaveMenu s_lvlDesignerSave;
        internal static GameScene s_game;
        internal static MainMenu s_mainMenu;
        internal static MenuBGOnly s_menuBG;
        //MenuFrames
        internal static LevelSelection s_levelSelection;
        internal static SaveFrameScene s_save;
        internal static LoadFrameScene s_sandboxLoad;
        internal static LoadLevelDesigner s_levelDesignerLoad;
        internal static LoadLevel s_levelLoad;
        internal static OptionsAudio s_optionsAudio;
        internal static OptionsGraphics s_optionsGraphics;
        internal static OptionsControls s_optionsControls;
        internal static StatisticsFrame s_statistics;
        internal static CreditsFrame s_credits;
        internal static HandbookFrame s_handbook;
        //HUD
        internal static Scene.Console s_console;
        public static Scene.CodeEditor s_code;
        public static Scene.ComponentSelector.ComponentSelector s_componentSelector;
        internal static Scene.CircuitRunningControl s_runControl;
        public static Scene.SubComponentButtons s_subComponentButtons;
        internal static Scene.PlacableAreasCreator s_placableAreasCreator;
        internal static Scene.Tutorial s_tutorial;
        internal static Scene.ScriptEditor s_scriptEditor;
        internal static Scene.VictoryMessage s_victoryMessage;
        internal static Scene.ClickableAreas s_clickableAreas;
        internal static Scene.ZoomBar s_zoombar;
        internal static Scene.StatusStrip s_statusStrip;
        internal static Scene.Purpose s_purpose;
        internal static Scene.VisibilityMapOverlay s_visMapOverlay;
        internal static Scene.ToolTip s_toolTip;
        internal static Scene.InfoPanel s_infoPanel;
        internal static Scene.Ruller s_ruller;
        internal static Scene.MapOverlays.LightOverlay s_lightOverlay;
        internal static Scene.MapOverlays.MagnetOverlay s_magneticOverlay;
        internal static Scene.MapsHUD s_maphud;
        internal static Scene.Dialog s_dialog;
        #endregion

        public static void Init()
        {
            ClickabilityOverlay.RegisterExtension(Logics.ClickablePlacableAreas.Instance);

            IO.Log.Write("        Initializing events");
            InputEngine.onButtonDown += new InputEngine.MouseEventHandler(onButtonDown);
            InputEngine.onButtonUp += new InputEngine.MouseEventHandler(onButtonUp);
            InputEngine.onButtonClick += new InputEngine.MouseEventHandler(onButtonClick);
            InputEngine.onMouseMove += new InputEngine.MouseMoveEventHandler(onMouseMove);
            InputEngine.onMouseWheelMove += new InputEngine.MouseWheelMoveEventHandler(onMouseWheelMove);
            InputEngine.onMouseWheelClick += new InputEngine.MouseEventHandler(onMouseWheelClick);

            InputEngine.onKeyDown += new InputEngine.KeyboardEventHandler(onKeyDown);
            InputEngine.onKeyUp += new InputEngine.KeyboardEventHandler(onKeyUp);
            InputEngine.onKeyPressed += new InputEngine.KeyboardEventHandler(onKeyPressed);

            GlobalEvents.onResolutionChanged += new GlobalEvents.ResolutionEventHandler(GlobalEvents_onResolutionChanged);
            GlobalEvents.onGraphicsDeviceReset += new GlobalEvents.GraphicDeviceEventHandler(GlobalEvents_onGraphicsDeviceReset);

            if (Scene.MenuFrameScene.ButtonFont == null)
                Scene.MenuFrameScene.buttonFont = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_19");

            IO.Log.Write("        Loading base fonts");
            font = ResourceManager.Load<SpriteFont>("Fonts/CourierNew_11");
            //Graphics.GUI.Elements.TextBox.defaultFont = ResourceManager.Load<SpriteFont>("Fonts/Standart");//do not move!
            Graphics.GUI.Elements.Label.DefaultFont = font;//do not move!

            Scene.ComponentSelector.ComponentSelector.StaticInit();

            IO.Log.Write("        TextBox static initialization");
            //Elements.TextBox.InitStatic();

            #region ScenesInit
            IO.Log.Write("            LevelSelection");
            s_levelSelection = new LevelSelection();
            s_levelSelection.Initialize();
            IO.Log.Write("            SandboxLoad");
            s_sandboxLoad = new LoadFrameScene();
            s_sandboxLoad.Initialize();
            IO.Log.Write("            LevelDesignerLoad");
            s_levelDesignerLoad = new LoadLevelDesigner();
            s_levelDesignerLoad.Initialize();
            IO.Log.Write("            LevelLoad");
            s_levelLoad = new LoadLevel();
            s_levelLoad.Initialize();
            IO.Log.Write("            OptionsAudio");
            s_optionsAudio = new OptionsAudio();
            s_optionsAudio.Initialize();
            IO.Log.Write("            OptionsGraphics");
            s_optionsGraphics = new OptionsGraphics();
            s_optionsGraphics.Initialize();
            IO.Log.Write("            OptionsControls");
            s_optionsControls = new OptionsControls();
            s_optionsControls.Initialize();
            IO.Log.Write("            Save");
            s_save = new SaveFrameScene();
            s_save.Initialize();
            IO.Log.Write("            Statistics");
            s_statistics = new StatisticsFrame();
            s_statistics.Initialize();
            IO.Log.Write("            Credits");
            s_credits = new CreditsFrame();
            s_credits.Initialize();
            IO.Log.Write("            Handbook");
            s_handbook = new HandbookFrame();
            s_handbook.Initialize();
            IO.Log.Write("            MenuBG");
            s_menuBG = new MenuBGOnly();
            s_menuBG.Initialize();
            
            //
            IO.Log.Write("        Initializing scenes");
            Main.LoadingDetails = "Initializing scenes...";
            IO.Log.Write("            Intro");
            s_intro = new IntroSequence();
            s_intro.Initialize();
            IO.Log.Write("            MainMenu");
            s_mainMenu = new MainMenu();
            s_mainMenu.Initialize();
            IO.Log.Write("            Options");
            s_options = new Options();
            s_options.Initialize();
            IO.Log.Write("            Statistics");
            s_statistics2 = new StatisticsScreen();
            s_statistics2.Initialize();
            IO.Log.Write("            Sandbox");
            s_sandboxMenu = new SandboxMenu();
            s_sandboxMenu.Initialize();
            IO.Log.Write("            SandboxLoad");
            s_sandboxLoadMenu = new SandboxLoadMenu();
            s_sandboxLoadMenu.Initialize();
            IO.Log.Write("            SandboxSave");
            s_sandboxSaveMenu = new SandboxSaveMenu();
            s_sandboxSaveMenu.Initialize();
            IO.Log.Write("            Loading");
            s_loading = new Loading();
            s_loading.Initialize();
            IO.Log.Write("            LevelsMenu");
            s_levelsMenu = new LevelsMenu();
            s_levelsMenu.Initialize();
            //IO.Log.Write("            LevelSelector");
            //s_levelSelector = new LevelSelector();
            //s_levelSelector.Initialize();
            IO.Log.Write("            LevelsSaveMenu");
            s_levelsSaveMenu = new LevelsSaveMenu();
            s_levelsSaveMenu.Initialize();
            IO.Log.Write("            LevelsLoadMenu");
            s_levelsLoadMenu = new LevelsLoadMenu();
            s_levelsLoadMenu.Initialize();
            IO.Log.Write("            EncyclopediaPage");
            s_encyclopediaPage = new EncyclopediaPage();
            s_encyclopediaPage.Initialize();
            IO.Log.Write("            EncyclopediaEditor");
            s_encyclopediaEditor = new EncyclopediaEditor();
            s_encyclopediaEditor.Initialize();
            IO.Log.Write("            Credits");
            s_credits2 = new Credits();
            s_credits2.Initialize();
            IO.Log.Write("            EncyclopediaBrowser");
            s_encyclopediaBrowser = new EncyclopediaBrowser();
            s_encyclopediaBrowser.Initialize();
            IO.Log.Write("            LevelDesignerMenu");
            s_lvlDesignerMenu = new LevelDesignerMenu();
            s_lvlDesignerMenu.Initialize();
            IO.Log.Write("            LevelDesignerLoadMenu");
            s_lvlDesignerLoad = new LevelDesignerLoadMenu();
            s_lvlDesignerLoad.Initialize();
            IO.Log.Write("            LevelDesignerSaveMenu");
            s_lvlDesignerSave = new LevelDesignerSaveMenu();
            s_lvlDesignerSave.Initialize();
            IO.Log.Write("            GameScene");
            s_game = new GameScene();
            s_game.Initialize();
            IO.Log.Write("            MainMenu2");
            s_mainMenu2 = new MainMenu2();
            s_mainMenu2.Initialize();
            

            //
            IO.Log.Write("        Initializing HUD scenes");
            Main.LoadingDetails = "Initializing HUD scenes...";
            IO.Log.Write("            Console");
            s_console = new Scene.Console();
            s_console.Initialize();
            IO.Log.Write("            CodeEditor");
            s_code = new Scene.CodeEditor();
            s_code.Initialize();
            IO.Log.Write("            CircuitRunningControl");
            s_runControl = new CircuitRunningControl();
            s_runControl.Initialize();
            //IO.Log.Write("            InGameMenu");
            //s_inGameMenu = new InGameMenu();
            //s_inGameMenu.Initialize();
            IO.Log.Write("            SubComponentButtons");
            s_subComponentButtons = new SubComponentButtons();
            s_subComponentButtons.Initialize();
            IO.Log.Write("            PlacableAreasManager");
            s_placableAreasCreator = new PlacableAreasCreator();
            s_placableAreasCreator.Initialize();
            IO.Log.Write("            Tutorial");
            s_tutorial = new Tutorial();
            s_tutorial.Initialize();
            IO.Log.Write("            ScriptEditor");
            s_scriptEditor = new ScriptEditor();
            s_scriptEditor.Initialize();
            IO.Log.Write("            VictoryScreen");
            s_victoryMessage = new VictoryMessage();
            s_victoryMessage.Initialize();
            IO.Log.Write("            ClickableAreas");
            s_clickableAreas = new ClickableAreas();
            s_clickableAreas.Initialize();
            IO.Log.Write("            MapHUD");
            s_maphud = new MapsHUD();
            s_maphud.Initialize();
            IO.Log.Write("            ZoomBar");
            s_zoombar = new ZoomBar();
            s_zoombar.Initialize();
            IO.Log.Write("            StatusStrip");
            s_statusStrip = new StatusStrip();
            s_statusStrip.Initialize();
            IO.Log.Write("            Purpose");
            s_purpose = new Purpose();
            s_purpose.Initialize();
            IO.Log.Write("            ComponentSelector");
            s_componentSelector = new Scene.ComponentSelector.ComponentSelector();
            s_componentSelector.Initialize();
            IO.Log.Write("            VisibilityMapOverlay");
            s_visMapOverlay = new VisibilityMapOverlay();
            s_visMapOverlay.Initialize();
            IO.Log.Write("            ToolTip");
            s_toolTip = new ToolTip();
            s_toolTip.Initialize();
            IO.Log.Write("            InfoPanel");
            s_infoPanel = new InfoPanel();
            s_infoPanel.Initialize();
            IO.Log.Write("            Ruller");
            s_ruller = new Ruller();
            s_ruller.Initialize();
            IO.Log.Write("            LightOverlay");
            s_lightOverlay = new Scene.MapOverlays.LightOverlay();
            s_lightOverlay.Initialize();
            IO.Log.Write("            MagneticOverlay");
            s_magneticOverlay = new Scene.MapOverlays.MagnetOverlay();
            s_magneticOverlay.Initialize();
            IO.Log.Write("            Dialog");
            s_dialog = new Dialog();
            s_dialog.Initialize();
            
            #endregion
            #region HUDInit
            s_code.isVisible = true;
            s_runControl.isVisible = true;
            s_placableAreasCreator.isVisible = true;
            s_tutorial.isVisible = true;
            s_scriptEditor.isVisible = true;
            s_victoryMessage.isVisible = true;
            s_clickableAreas.isVisible = true;
            s_maphud.isVisible = true;
            s_console.isVisible = true;
            s_zoombar.isVisible = true;
            s_statusStrip.isVisible = true;
            s_purpose.isVisible = true;
            s_componentSelector.isVisible = true;
            s_visMapOverlay.isVisible = true;
            s_mainMenu.isVisible = true;
            s_infoPanel.isVisible = true;
            s_lightOverlay.isVisible = true;
            s_magneticOverlay.isVisible = true;
            s_dialog.isVisible = true;
            
            #endregion

            //curScene = s_mainMenu;
        }

        static void GlobalEvents_onResolutionChanged(int w, int h, int oldw, int oldh)
        {
            GridDraw.OnResolutionChange(w, h, oldw, oldh);

            s_intro.OnResolutionChanged(w, h, oldw, oldh);
            s_mainMenu.OnResolutionChanged(w, h, oldw, oldh);
            s_options.OnResolutionChanged(w, h, oldw, oldh);
            s_statistics2.OnResolutionChanged(w, h, oldw, oldh);
            s_sandboxMenu.OnResolutionChanged(w, h, oldw, oldh);
            s_sandboxLoadMenu.OnResolutionChanged(w, h, oldw, oldh);
            s_sandboxSaveMenu.OnResolutionChanged(w, h, oldw, oldh);
            s_loading.OnResolutionChanged(w, h, oldw, oldh);
            s_levelsMenu.OnResolutionChanged(w, h, oldw, oldh);
            s_levelsSaveMenu.OnResolutionChanged(w, h, oldw, oldh);
            s_levelsLoadMenu.OnResolutionChanged(w, h, oldw, oldh);
            s_encyclopediaPage.OnResolutionChanged(w, h, oldw, oldh);
            s_encyclopediaEditor.OnResolutionChanged(w, h, oldw, oldh);
            s_credits2.OnResolutionChanged(w, h, oldw, oldh);
            s_encyclopediaBrowser.OnResolutionChanged(w, h, oldw, oldh);
            s_lvlDesignerMenu.OnResolutionChanged(w, h, oldw, oldh);
            s_lvlDesignerLoad.OnResolutionChanged(w, h, oldw, oldh);
            s_lvlDesignerSave.OnResolutionChanged(w, h, oldw, oldh);
            s_game.OnResolutionChanged(w, h, oldw, oldh);
            s_mainMenu2.OnResolutionChanged(w, h, oldw, oldh);
            s_menuBG.OnResolutionChanged(w, h, oldw, oldh);
            
            s_levelSelection.OnResolutionChanged(w, h, oldw, oldh);
            s_sandboxLoad.OnResolutionChanged(w, h, oldw, oldh);
            s_levelDesignerLoad.OnResolutionChanged(w, h, oldw, oldh);
            s_levelLoad.OnResolutionChanged(w, h, oldw, oldh);
            s_optionsAudio.OnResolutionChanged(w, h, oldw, oldh);
            s_optionsGraphics.OnResolutionChanged(w, h, oldw, oldh);
            s_optionsControls.OnResolutionChanged(w, h, oldw, oldh);
            s_save.OnResolutionChanged(w, h, oldw, oldh);
            s_statistics.OnResolutionChanged(w, h, oldw, oldh);
            s_credits.OnResolutionChanged(w, h, oldw, oldh);
            s_handbook.OnResolutionChanged(w, h, oldw, oldh);
            
            s_console.OnResolutionChanged(w, h, oldw, oldh);
            s_code.OnResolutionChanged(w, h, oldw, oldh);
            s_runControl.OnResolutionChanged(w, h, oldw, oldh);
            //s_inGameMenu.OnResolutionChanged(w, h, oldw, oldh);
            s_subComponentButtons.OnResolutionChanged(w, h, oldw, oldh);
            s_placableAreasCreator.OnResolutionChanged(w, h, oldw, oldh);
            s_tutorial.OnResolutionChanged(w, h, oldw, oldh);
            s_scriptEditor.OnResolutionChanged(w, h, oldw, oldh);
            s_victoryMessage.OnResolutionChanged(w, h, oldw, oldh);
            s_clickableAreas.OnResolutionChanged(w, h, oldw, oldh);
            s_maphud.OnResolutionChanged(w, h, oldw, oldh);
            s_zoombar.OnResolutionChanged(w, h, oldw, oldh);
            s_statusStrip.OnResolutionChanged(w, h, oldw, oldh);
            s_purpose.OnResolutionChanged(w, h, oldw, oldh);
            s_componentSelector.OnResolutionChanged(w, h, oldw, oldh);
            s_visMapOverlay.OnResolutionChanged(w, h, oldw, oldh);
            s_toolTip.OnResolutionChanged(w, h, oldw, oldh);
            s_infoPanel.OnResolutionChanged(w, h, oldw, oldh);
            s_ruller.OnResolutionChanged(w, h, oldw, oldh);
            s_lightOverlay.OnResolutionChanged(w, h, oldw, oldh);
            s_magneticOverlay.OnResolutionChanged(w, h, oldw, oldh);
            s_dialog.OnResolutionChanged(w, h, oldw, oldh);
            
        }

        public static void LoadContent()
        {
            //Graphics.GUI.Elements.Button.textureGlobal = ResourceManager.Load<Texture2D>("GUI/button");
            Graphics.GUI.Elements.Button.textureGlobal = ResourceManager.Load<Texture2D>("pixel");
            Graphics.GUI.Elements.Button.border = ResourceManager.Load<Texture2D>("GUI/BackgroundTransparent");
            Graphics.GUI.Elements.Button.textureSelected = ResourceManager.Load<Texture2D>("GUI/buttonSelection");
            //Graphics.GUI.Elements.TextBox.texture = ResourceManager.Load<Texture2D>("GUI/button");
            //Graphics.GUI.Elements.TextBox.texture = ResourceManager.Load<Texture2D>("GUI/BackgroundBlack");
            Graphics.GUI.Elements.ScrollBar.defaultTexture = ResourceManager.Load<Texture2D>("GUI/scrollBarDefault");
            Graphics.GUI.Elements.ListBox.font = ResourceManager.Load<SpriteFont>("Fonts/CourierNew_11");
            Graphics.GUI.Scene.YesNoMessageBox.font = ResourceManager.Load<SpriteFont>("Fonts/CourierNew_11");
            Graphics.GUI.Elements.Label.DefaultFont = ResourceManager.Load<SpriteFont>("Fonts/CourierNew_11");
            Graphics.GUI.Scene.ScriptedInfoPanel.font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_24");

            Components.Core.StaticInit();

            s_intro.LoadContent();
            s_mainMenu.LoadContent();
            s_options.LoadContent();
            s_statistics2.LoadContent();
            s_sandboxMenu.LoadContent();
            s_sandboxLoadMenu.LoadContent();
            s_sandboxSaveMenu.LoadContent();
            s_loading.LoadContent();
            s_levelsMenu.LoadContent();
            s_levelsSaveMenu.LoadContent();
            s_levelsLoadMenu.LoadContent();
            s_encyclopediaPage.LoadContent();
            s_encyclopediaEditor.LoadContent();
            s_credits2.LoadContent();
            s_encyclopediaBrowser.LoadContent();
            s_lvlDesignerMenu.LoadContent();
            s_lvlDesignerLoad.LoadContent();
            s_lvlDesignerSave.LoadContent();
            s_game.LoadContent();
            s_mainMenu2.LoadContent();
            s_menuBG.LoadContent();

            s_levelSelection.LoadContent();
            s_sandboxLoad.LoadContent();
            s_levelDesignerLoad.LoadContent();
            s_levelLoad.LoadContent();
            s_optionsAudio.LoadContent();
            s_optionsGraphics.LoadContent();
            s_optionsControls.LoadContent();
            s_save.LoadContent();
            s_statistics.LoadContent();
            s_credits.LoadContent();
            s_handbook.LoadContent();
            
            s_console.LoadContent();
            s_code.LoadContent();
            s_runControl.LoadContent();
            //s_inGameMenu.LoadContent();
            s_subComponentButtons.LoadContent();
            s_placableAreasCreator.LoadContent();
            s_tutorial.LoadContent();
            s_scriptEditor.LoadContent();
            s_victoryMessage.LoadContent();
            s_clickableAreas.LoadContent();
            s_maphud.LoadContent();
            s_zoombar.LoadContent();
            s_statusStrip.LoadContent();
            s_purpose.LoadContent();
            s_componentSelector.LoadContent();
            s_visMapOverlay.LoadContent();
            s_toolTip.LoadContent();
            s_infoPanel.LoadContent();
            s_ruller.LoadContent();
            s_lightOverlay.LoadContent();
            s_magneticOverlay.LoadContent();
            s_dialog.LoadContent();
        }

        static void GlobalEvents_onGraphicsDeviceReset()
        {
            s_intro.OnGraphicsDeviceReset();
            s_mainMenu.OnGraphicsDeviceReset();
            s_options.OnGraphicsDeviceReset();
            s_statistics2.OnGraphicsDeviceReset();
            s_sandboxMenu.OnGraphicsDeviceReset();
            s_sandboxLoadMenu.OnGraphicsDeviceReset();
            s_sandboxSaveMenu.OnGraphicsDeviceReset();
            s_loading.OnGraphicsDeviceReset();
            s_levelsMenu.OnGraphicsDeviceReset();
            s_levelsSaveMenu.OnGraphicsDeviceReset();
            s_levelsLoadMenu.OnGraphicsDeviceReset();
            s_encyclopediaPage.OnGraphicsDeviceReset();
            s_encyclopediaEditor.OnGraphicsDeviceReset();
            s_credits2.OnGraphicsDeviceReset();
            s_encyclopediaBrowser.OnGraphicsDeviceReset();
            s_lvlDesignerMenu.OnGraphicsDeviceReset();
            s_lvlDesignerLoad.OnGraphicsDeviceReset();
            s_lvlDesignerSave.OnGraphicsDeviceReset();
            s_game.OnGraphicsDeviceReset();
            s_mainMenu2.OnGraphicsDeviceReset();
            s_menuBG.OnGraphicsDeviceReset();

            s_levelSelection.OnGraphicsDeviceReset();
            s_sandboxLoad.OnGraphicsDeviceReset();
            s_levelDesignerLoad.OnGraphicsDeviceReset();
            s_levelLoad.OnGraphicsDeviceReset();
            s_optionsAudio.OnGraphicsDeviceReset();
            s_optionsGraphics.OnGraphicsDeviceReset();
            s_optionsControls.OnGraphicsDeviceReset();
            s_save.OnGraphicsDeviceReset();
            s_statistics.OnGraphicsDeviceReset();
            s_credits.OnGraphicsDeviceReset();
            s_handbook.OnGraphicsDeviceReset();

            s_console.OnGraphicsDeviceReset();
            s_code.OnGraphicsDeviceReset();
            s_runControl.OnGraphicsDeviceReset();
            //s_inGameMenu.OnGraphicsDeviceReset();
            s_subComponentButtons.OnGraphicsDeviceReset();
            s_placableAreasCreator.OnGraphicsDeviceReset();
            s_tutorial.OnGraphicsDeviceReset();
            s_scriptEditor.OnGraphicsDeviceReset();
            s_victoryMessage.OnGraphicsDeviceReset();
            s_clickableAreas.OnGraphicsDeviceReset();
            s_maphud.OnGraphicsDeviceReset();
            s_zoombar.OnGraphicsDeviceReset();
            s_statusStrip.OnGraphicsDeviceReset();
            s_purpose.OnGraphicsDeviceReset();
            s_componentSelector.OnGraphicsDeviceReset();
            s_visMapOverlay.OnGraphicsDeviceReset();
            s_toolTip.OnGraphicsDeviceReset();
            s_infoPanel.OnGraphicsDeviceReset();
            s_ruller.OnGraphicsDeviceReset();
            s_lightOverlay.OnGraphicsDeviceReset();
            s_magneticOverlay.OnGraphicsDeviceReset();
            s_dialog.OnGraphicsDeviceReset();

            for (int i = 0; i < HUDScenes.Count; i++)
                HUDScenes[i].OnGraphicsDeviceReset();
        }

        /// <summary>
        /// Changes scene
        /// </summary>
        /// <param name="scene">New scene</param>
        /// <param name="state">New state</param>
        public static void ChangeScene(Scene.Scene scene, String state)
        {
            if (state == null) state = Main.curState;
            try
            {
                if (animationThread != null) animationThread.Abort();
            }
            catch { }
            nextScene = scene;
            nextState = state;
            _changeScene();
            //animationThread = new System.Threading.Thread(new System.Threading.ThreadStart(_changeScene));
            //animationThread.IsBackground = true;
            //animationThread.Start();
        }
        static Scene.Scene nextScene;
        static String nextState;
        static System.Threading.Thread animationThread;
        private static void _changeScene()
        {
            if (curScene != null)
            {
                lock (curScene)
                {
                    curScene.blockInput = true;
                    curScene.FadeOut();
                    curScene.PostFadeOut();
                    curScene.blockInput = false;
                }
            }
            if (curScene != null)
            {
                lock (curScene)
                    curScene = nextScene;
            }
            else
                curScene = nextScene;
            Main.curState = nextState;
            if (curScene != null)
            {
                lock (curScene)
                {
                    curScene.blockInput = true;
                    curScene.FadeIn();
                    curScene.blockInput = false;
                }
            }
            SortScenes();
        }

        public static void AddHUDScene(Scene.HUDScene s)
        {
            RemoveHUDScene(s);
            lock(Scenes)
                Scenes.Remove(s);
            lock(HUDScenes)
                HUDScenes.Add(s);
            lock(Scenes)
                Scenes.Add(s);
            s.onShow();
            SortScenes();
            if (onHUDSceneOpened != null)
            {
                onHUDSceneOpened.Invoke(s.ToString());
            }
        }

        public static String GetHUDSceneName(int id)
        {
            lock (HUDScenes)
            {
                if (id < 0 || id >= HUDScenes.Count) return "";
                return HUDScenes[id].ToString();
            }
        }

        public static bool ContainsHUDScene(Scene.HUDScene s)
        {
            lock(HUDScenes)
                return HUDScenes.Contains(s);
        }

        public static bool ContainsHUDScene(String s)
        {
            lock (HUDScenes)
            {
                for (int i = 0; i < HUDScenes.Count; i++)
                {
                    if (HUDScenes[i].ToString() == s && HUDScenes[i].isVisible) return true;
                }
                return false;
            }
        }

        public static Vector2 GetHUDSceneSize(String s)
        {
            lock (HUDScenes)
            {
                for (int i = 0; i < HUDScenes.Count; i++)
                {
                    if (HUDScenes[i].ToString() == s && HUDScenes[i].isVisible)
                        return HUDScenes[i].GetSize();
                }
                return new Vector2();
            }
        }

        public static double[] GetHUDSceneRectangle(String s)
        {
            lock (HUDScenes)
            {
                for (int i = 0; i < HUDScenes.Count; i++)
                {
                    if (HUDScenes[i].ToString() == s && HUDScenes[i].isVisible)
                    {
                        var a = HUDScenes[i].GetSize();
                        var b = HUDScenes[i].GetPosition();
                        return new double[] { b.X, b.Y, a.X, a.Y };
                    }
                }
                return new double[] { 0, 0, 0, 0 };
            }
        }

        public static bool ContainsHUDSceneType<T>()
        {
            lock (HUDScenes)
            {
                for (int i = 0; i < HUDScenes.Count; i++)
                {
                    if (HUDScenes[i] is T)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static Scene.HUDScene GetFirstHUDSceneType<T>()
        {
            lock (HUDScenes)
            {
                for (int i = 0; i < HUDScenes.Count; i++)
                {
                    if (HUDScenes[i] is T)
                    {
                        return HUDScenes[i];
                    }
                }
                return null;
            }
        }

        public static T[] GetAllHUDSceneType<T>()
        {
            List<T> s = new List<T>();
            lock (HUDScenes)
            {
                for (int i = 0; i < HUDScenes.Count; i++)
                {
                    if (HUDScenes[i] is T)
                    {
                        s.Add((T)((object)HUDScenes[i]));
                    }
                }
                return s.ToArray();
            }
        }

        public static HUDScene[] GetAllHUDSceneType(Type t)
        {
            List<HUDScene> s = new List<HUDScene>();
            lock (HUDScenes)
            {
                for (int i = 0; i < HUDScenes.Count; i++)
                {
                    if (HUDScenes[i].GetType() == t)
                    {
                        s.Add(HUDScenes[i]);
                    }
                }
                return s.ToArray();
            }
        }

        public static int GetHUDSceneTypeCount<T>()
        {
            lock (HUDScenes)
            {
                int r = 0;
                for (int i = 0; i < HUDScenes.Count; i++)
                {
                    if (HUDScenes[i] is T)
                    {
                        r++;
                    }
                }
                return r;
            }
        }

        public static void RemoveHUDScene(Scene.HUDScene s)
        {
            lock (HUDScenes)
            {
                Scene.HUDScene sc;
                for (int i = 0; i < HUDScenes.Count; i++)
                {
                    sc = HUDScenes[i];
                    if (sc == s)
                    {
                        try
                        {
                            lock (Scenes)
                            {
                                Scenes.Remove(sc);
                            }
                            HUDScenes.Remove(sc);
                            if (onHUDSceneRemoved != null)
                            {
                                onHUDSceneRemoved.Invoke(sc.ToString());
                            }
                            sc.onClose();
                            i--;
                        }
                        catch { }
                    }
                }
            }
        }

        public static void RemoveHUDScene<T>()
        {
            lock (HUDScenes)
            {
                Scene.HUDScene sc;
                for (int i = 0; i < HUDScenes.Count; i++)
                {
                    sc = HUDScenes[i];
                    if (sc is T)
                    {
                        try
                        {
                            lock (Scenes)
                            {
                                Scenes.Remove(sc);
                            }
                            lock (HUDScenes)
                            {
                                HUDScenes.Remove(sc);
                            }
                            if (onHUDSceneRemoved != null)
                            {
                                onHUDSceneRemoved.Invoke(sc.ToString());
                            }
                            sc.onClose();
                            i--;
                        }
                        catch { }
                    }
                }
            }
        }

        public static void RemoveHUDScene(String t)
        {
            lock (HUDScenes)
            {
                Scene.HUDScene sc;
                for (int i = 0; i < HUDScenes.Count; i++)
                {
                    sc = HUDScenes[i];
                    if (sc.ToString() == t)
                    {
                        try
                        {
                            lock (Scenes)
                            {
                                Scenes.Remove(sc);
                            }
                            lock (HUDScenes)
                            {
                                HUDScenes.Remove(sc);
                            }
                            if (onHUDSceneRemoved != null)
                            {
                                onHUDSceneRemoved.Invoke(sc.ToString());
                            }
                            sc.onClose();
                            i--;
                        }
                        catch { }
                    }
                }
            }
        }

        public static void ClearHUDs()
        {
            lock(HUDScenes)
                lock (Scenes)
                {
                    for (int i = 0; i < HUDScenes.Count; i++)
                    {
                        HUDScenes[i].onClose();
                        Scenes.Remove(HUDScenes[i]);
                        HUDScenes.RemoveAt(i);
                        i--;
                        if (onHUDSceneRemoved != null)
                        {
                            onHUDSceneRemoved.Invoke(HUDScenes[i].ToString());
                        }
                    }
                }
        }

        public static void InvokeSceneOpened(String name)
        {
            if (onHUDSceneOpened != null)
            {
                onHUDSceneOpened.Invoke(name);
            }
        }

        public static void InvokeSceneClosed(String name)
        {
            if (onHUDSceneClosed != null)
            {
                onHUDSceneClosed.Invoke(name);
            }
        }

        public static void SortScenes()
        {
            lock(Scenes)
                lock (HUDScenes)
                {
                    Scene.Scene t;
                    bool c = true;
                    Scenes.Remove(null);
                    HUDScenes.Remove(null);
                    for (int i = 0; i < Scenes.Count && c; i++)
                    {
                        //c = false;
                        for (int j = i + 1; j < Scenes.Count; j++)
                        {
                            if (Scenes[i].Layer > Scenes[j].Layer)
                            {
                                t = Scenes[i];
                                Scenes[i] = Scenes[j];
                                Scenes[j] = t;
                                //c = true;
                            }
                        }
                    }
                }
        }

        public static void DeFocusAll()
        {
            lock (Scenes)
            {
                for (int i = 0; i < Scenes.Count; i++)
                {
                    for (int j = 0; j < Scenes[i].controls.Count; j++)
                    {
                        Scenes[i].controls[j].isFocused = false;
                    }
                }
            }
            /*
            if (curScene_ != null)
            {
                for (int j = 0; j < curScene_.controls.Count; j++)
                {
                    curScene_.controls[j].isFocused = false;
                }
            }//*/
        }

        public static void Update()
        {
            //if (curScene != null)
            //    curScene.Update();
            if (Main.CurState.StartsWith("GAME"))
            {
                GridDraw.Update();
            }

            lock (Scenes)
            {
                Scenes.Remove(null);
                for (int i = 0; i < Scenes.Count; i++)
                {
                    Scenes[i].Update();
                }
            }
        }

        public static void Draw()
        {
            /*
            if (curScene != null)
            {
                if (curScene.ShouldBeScaled)
                {
                    Main.renderer.Begin();
                }
                else
                {
                    Main.renderer.BeginUnscaled();
                }
                curScene.Draw();
                Main.renderer.End();
            }//*/

            lock (Scenes)
            {
                for (int i = 0; i < Scenes.Count; i++)
                {
                    if (Scenes[i].isVisible)
                    {
                        if (Scenes[i].ShouldBeScaled)
                        {
                            Main.renderer.Begin();
                        }
                        else
                        {
                            Main.renderer.BeginUnscaled();
                        }
                        Scenes[i].Draw(Main.renderer);
                        Main.renderer.End();
                    }
                }
                for (int i = 0; i < Scenes.Count; i++)
                {
                    if (Scenes[i].isVisible)
                    {
                        if (Scenes[i].ShouldBeScaled)
                        {
                            Main.renderer.Begin();
                        }
                        else
                        {
                            Main.renderer.BeginUnscaled();
                        }
                        Scenes[i].PostDraw();
                        Main.renderer.End();
                    }
                }
            }
        }

        #region ScaleAndTranslate
        public static InputEngine.MouseArgs GetScaledCoords(ref InputEngine.MouseArgs e)
        {
            InputEngine.MouseArgs ee = new InputEngine.MouseArgs()
            {
                curState = new MouseState((int)(e.curState.X / Settings.GameScale), (int)(e.curState.Y / Settings.GameScale),
                    e.curState.ScrollWheelValue,
                    e.curState.LeftButton, e.curState.MiddleButton, e.curState.RightButton,
                    e.curState.XButton1, e.curState.XButton2),
                button = e.button
            };
            return ee;
        }

        public static InputEngine.MouseMoveArgs GetScaledCoords(ref InputEngine.MouseMoveArgs e)
        {
            InputEngine.MouseMoveArgs ee = new InputEngine.MouseMoveArgs()
            {
                curState = new MouseState((int)(e.curState.X / Settings.GameScale), (int)(e.curState.Y / Settings.GameScale),
                    e.curState.ScrollWheelValue,
                    e.curState.LeftButton, e.curState.MiddleButton, e.curState.RightButton,
                    e.curState.XButton1, e.curState.XButton2),
                dx = e.dx,
                dy = e.dy
            };
            return ee;
        }

        public static InputEngine.MouseWheelMoveArgs GetScaledCoords(ref InputEngine.MouseWheelMoveArgs e)
        {
            InputEngine.MouseWheelMoveArgs ee = new InputEngine.MouseWheelMoveArgs()
            {
                curState = new MouseState((int)(e.curState.X / Settings.GameScale), (int)(e.curState.Y / Settings.GameScale),
                    e.curState.ScrollWheelValue,
                    e.curState.LeftButton, e.curState.MiddleButton, e.curState.RightButton,
                    e.curState.XButton1, e.curState.XButton2),
                delta = e.delta
            };
            return ee;
        }

        public static InputEngine.MouseArgs GetScaledOffsettedCoords(ref InputEngine.MouseArgs e)
        {
            InputEngine.MouseArgs ee = new InputEngine.MouseArgs()
            {
                curState = new MouseState(
                    (int)(e.curState.X / Settings.GameScale - Settings.GameOffset.X),
                    (int)(e.curState.Y / Settings.GameScale - Settings.GameOffset.Y),
                    e.curState.ScrollWheelValue,
                    e.curState.LeftButton, e.curState.MiddleButton, e.curState.RightButton,
                    e.curState.XButton1, e.curState.XButton2),
                button = e.button
            };
            return ee;
        }

        public static InputEngine.MouseMoveArgs GetScaledOffsettedCoords(ref InputEngine.MouseMoveArgs e)
        {
            InputEngine.MouseMoveArgs ee = new InputEngine.MouseMoveArgs()
            {
                curState = new MouseState(
                    (int)(e.curState.X / Settings.GameScale - Settings.GameOffset.X),
                    (int)(e.curState.Y / Settings.GameScale - Settings.GameOffset.Y),
                    e.curState.ScrollWheelValue,
                    e.curState.LeftButton, e.curState.MiddleButton, e.curState.RightButton,
                    e.curState.XButton1, e.curState.XButton2),
                dx = (int)(e.dx / Settings.GameScale),
                dy = (int)(e.dy / Settings.GameScale)
            };
            return ee;
        }

        public static InputEngine.MouseWheelMoveArgs GetScaledOffsettedCoords(ref InputEngine.MouseWheelMoveArgs e)
        {
            InputEngine.MouseWheelMoveArgs ee = new InputEngine.MouseWheelMoveArgs()
            {
                curState = new MouseState(
                    (int)(e.curState.X / Settings.GameScale - Settings.GameOffset.X),
                    (int)(e.curState.Y / Settings.GameScale - Settings.GameOffset.Y),
                    e.curState.ScrollWheelValue,
                    e.curState.LeftButton, e.curState.MiddleButton, e.curState.RightButton,
                    e.curState.XButton1, e.curState.XButton2),
                delta = e.delta
            };
            return ee;
        }
        #endregion

        public static void onButtonDown(InputEngine.MouseArgs e)
        {
            if (!ClickabilityOverlay.IsClickable(e)) return;
            DeFocusAll();
            InputEngine.MouseArgs ee = GetScaledCoords(ref e);
            //lock(Scenes)
            for (int i = Scenes.Count - 1; i >= 0 && !InputEngine.eventHandled; i--)
            {
                if (!Scenes[i].blockInput) 
                    Scenes[i].onButtonDown(Scenes[i].ShouldBeScaled ? ee : e);
                else 
                    InputEngine.eventHandled = true;
            }
            if (Main.curState.StartsWith("GAME") && !InputEngine.eventHandled)
            {
                if (!InputEngine.eventHandled) 
                    Components.ComponentsManager.OnMouseDown(GetScaledOffsettedCoords(ref e));
                if (!InputEngine.eventHandled) 
                    Logics.GameInputHandler.onButtonDown(GetScaledOffsettedCoords(ref e));
            }
        }

        public static void onButtonUp(InputEngine.MouseArgs e)
        {
            if (!ClickabilityOverlay.IsClickable(e))
            {
                if (Logics.GameInputHandler.isLine || Logics.GameInputHandler.isComponentDnD)
                    Logics.GameInputHandler.onButtonUp(GetScaledOffsettedCoords(ref e));
                return;
            }
            InputEngine.MouseArgs ee = GetScaledCoords(ref e);
            //lock(Scenes)
                for (int i = Scenes.Count - 1; i >= 0 && !InputEngine.eventHandled; i--)
                {
                    if (!Scenes[i].blockInput) Scenes[i].onButtonUp(Scenes[i].ShouldBeScaled ? ee : e);
                    else InputEngine.eventHandled = true;
                }
            if (Main.curState.StartsWith("GAME") && !InputEngine.eventHandled)
            {
                if (!InputEngine.eventHandled) Components.ComponentsManager.OnMouseUp(GetScaledOffsettedCoords(ref e));
                if (!InputEngine.eventHandled) Logics.GameInputHandler.onButtonUp(GetScaledOffsettedCoords(ref e));
            }
        }

        static void onButtonClick(InputEngine.MouseArgs e)
        {
            if (!ClickabilityOverlay.IsClickable(e)) 
                return;
            DeFocusAll();
            InputEngine.MouseArgs ee = GetScaledCoords(ref e);
            //lock(Scenes)
                for (int i = Scenes.Count - 1; i >= 0 && !InputEngine.eventHandled; i--)
                {
                    if (!Scenes[i].blockInput) Scenes[i].onButtonClick(Scenes[i].ShouldBeScaled ? ee : e);
                    else InputEngine.eventHandled = true;
                }
            if (Main.curState.StartsWith("GAME") && !InputEngine.eventHandled && !ContainsHUDScene(s_mainMenu))
            {
                if (!InputEngine.eventHandled) Components.ComponentsManager.OnMouseClick(GetScaledOffsettedCoords(ref e));
                if (!InputEngine.eventHandled) Logics.GameInputHandler.onButtonClick(GetScaledOffsettedCoords(ref e));
            }
        }

        static void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            Shortcuts.SetInGameStatus("", "");
            InputEngine.MouseMoveArgs ee = GetScaledCoords(ref e);
            //lock(Scenes)
                for (int i = Scenes.Count - 1; i >= 0 && !InputEngine.eventHandled; i--)
                {
                    if (!Scenes[i].blockInput) Scenes[i].onMouseMove(Scenes[i].ShouldBeScaled ? ee : e);
                    else InputEngine.eventHandled = true;
                }
            if (Main.curState.StartsWith("GAME"))
            {
                if (Settings.k_DragScene.IsMatched() && InputEngine.WasActive && !ContainsHUDScene(s_mainMenu))
                {
                    Graphics.GraphicsEngine.camera.Center -= new Vector2(e.dx, e.dy) / Settings.GameScale;
                    InputEngine.eventHandled = true;
                }
                if (!InputEngine.eventHandled) Components.ComponentsManager.OnMouseMove(GetScaledOffsettedCoords(ref e));
                if (!InputEngine.eventHandled) Logics.GameInputHandler.onMouseMove(GetScaledOffsettedCoords(ref e));
            }
        }

        static void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            InputEngine.MouseWheelMoveArgs ee = GetScaledCoords(ref e);
            //lock(Scenes)
            for (int i = Scenes.Count - 1; i >= 0 && !InputEngine.eventHandled; i--)
            {
                if (!Scenes[i].blockInput) Scenes[i].onMouseWheelMove(Scenes[i].ShouldBeScaled ? ee : e);
                else InputEngine.eventHandled = true;
            }
            if (Main.curState.StartsWith("GAME") && !InputEngine.eventHandled && !ContainsHUDScene(s_mainMenu))
            {
                //if (!InputEngine.eventHandled && InputEngine.Control)
                //{
                //    InputEngine.eventHandled = true;
                //    Settings.GameScale += (float)e.delta / 1200;
                //}
                if (!InputEngine.eventHandled) Components.ComponentsManager.OnMouseWheel(GetScaledOffsettedCoords(ref e));
                if (!InputEngine.eventHandled) Logics.GameInputHandler.onMouseWheelMove(e);
            }
        }

        static void onMouseWheelClick(InputEngine.MouseArgs e)
        {
            if (!ClickabilityOverlay.IsClickable(e)) return;

            InputEngine.MouseArgs ee = GetScaledCoords(ref e);
            for (int i = Scenes.Count - 1; i >= 0 && !InputEngine.eventHandled; i--)
            {
                if (!Scenes[i].blockInput) Scenes[i].onButtonClick(Scenes[i].ShouldBeScaled ? ee : e);
                else InputEngine.eventHandled = true;
            }

            if (!InputEngine.eventHandled && !ContainsHUDScene(s_mainMenu)) Components.ComponentsManager.OnMouseClick(e);
            if (!InputEngine.eventHandled && !ContainsHUDScene(s_mainMenu)) Logics.GameInputHandler.OnMouseWheelClick(e);
        }

        public static void onKeyDown(InputEngine.KeyboardArgs e)
        {
            //lock(Scenes)
            for (int i = Scenes.Count - 1; i >= 0 && !InputEngine.eventHandled; i--)
            {
                if (!Scenes[i].blockInput) Scenes[i].onKeyDown(e);
                else InputEngine.eventHandled = true;
            }

            if (e.key == Keys.F12.GetHashCode())//TODO rm
            {
                for (int i = 0; i < HUDScenes.Count; i++)
                {
                    HUDScenes[i].Tag = HUDScenes[i].isVisible;
                    HUDScenes[i].isVisible = false;
                }
            }
        }

        public static void onKeyUp(InputEngine.KeyboardArgs e)
        {
            //lock(Scenes)
            for (int i = Scenes.Count - 1; i >= 0 && !InputEngine.eventHandled; i--)
            {
                if (!Scenes[i].blockInput) Scenes[i].onKeyUp(e);
                else InputEngine.eventHandled = true;
            }

            if (e.key == Keys.F12.GetHashCode())//TODO rm
            {
                for (int i = 0; i < HUDScenes.Count; i++)
                {
                    try
                    {
                        HUDScenes[i].isVisible = (bool)HUDScenes[i].Tag;
                    }
                    catch { }
                }
            }
        }

        public static void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            if (!InputEngine.eventHandled && e.key == Keys.OemTilde.GetHashCode() && InputEngine.Control && !InputEngine.Shift)
            {
                if (ContainsHUDScene(s_console))
                {
                    s_console.Close();
                }
                else
                {
                    s_console.Show();
                }
                //s_console.isVisible = !s_console.isVisible;
                InputEngine.eventHandled = true;
            }
            //lock(Scenes)
                for (int i = Scenes.Count - 1; i >= 0 && !InputEngine.eventHandled; i--)
                {
                    if (!Scenes[i].blockInput) Scenes[i].onKeyPressed(e);
                    else InputEngine.eventHandled = true;
                }
            if (!e.Handled && e.key == Keys.Escape.GetHashCode() && curScene_ == s_game)
            {
                if (!ContainsHUDScene(s_mainMenu))
                {
                    if (Settings.GameState == Settings.GameStates.Running)
                        Logics.GameLogicsHelper._gamePause();
                        //s_runControl.psClick(null, null);
                    AddHUDScene(s_mainMenu);
                    s_mainMenu.Show();
                    s_mainMenu.InitForInGame();
                    s_mainMenu.isVisible = true;
                    e.Handled = true;
                }
            }
            //if (!InputEngine.eventHandled)
            {
                //if (e.key == Keys.F2.GetHashCode())
                //{
                //    IO.SaveEngine.SaveAll("Saves/quicksave.sav");
                //}
                if (e.key == Keys.F2.GetHashCode())
                {
                    Debug.InputInfo.IsVisible = false;
                    Debug.DebugInfo.IsVisible = false;
                    Debug.VisMapOverlay.IsVisible = false;
                    Debug.CollidersOverlay.IsVisible = !Debug.CollidersOverlay.IsVisible;
                }
                if (e.key == Keys.F3.GetHashCode())
                {
                    Debug.CollidersOverlay.IsVisible = false;
                    Debug.InputInfo.IsVisible = false;
                    Debug.VisMapOverlay.IsVisible = false;
                    Debug.DebugInfo.IsVisible = !Debug.DebugInfo.IsVisible;
                }
                if (e.key == Keys.F4.GetHashCode())
                {
                    Debug.CollidersOverlay.IsVisible = false;
                    Debug.DebugInfo.IsVisible = false;
                    Debug.VisMapOverlay.IsVisible = false;
                    Debug.InputInfo.IsVisible = !Debug.InputInfo.IsVisible;
                }
                if (e.key == Keys.F9.GetHashCode())
                {
                    Debug.CollidersOverlay.IsVisible = false;
                    Debug.DebugInfo.IsVisible = false;
                    Debug.InputInfo.IsVisible = false;
                    Debug.VisMapOverlay.IsVisible = !Debug.VisMapOverlay.IsVisible;
                }
                if (e.key == Keys.F10.GetHashCode())
                {
                    Settings.DrawInvisibleWires = !Settings.DrawInvisibleWires;
                }
                /*
                if (e.key == Keys.F4.GetHashCode())
                {
                    var a = (Graphics.GUI.GUIEngine.s_mainMenu.background as Graphics.GUI.Background.MainMenu);
                    a.Generate();
                    Main.window.Title = a.cs.ToString() + "  ;  " + a.seed.ToString();
                }
                if (e.key == Keys.F5.GetHashCode())
                {
                    var a = (Graphics.GUI.GUIEngine.s_mainMenu.background as Graphics.GUI.Background.MainMenu);
                    //a.Generate();
                    //Main.window.Title = a.cs.ToString() + "  ;  " + a.seed.ToString();
                    a.aaaa.RemoveAt(a.cs);
                    a.cs--;
                }
                if (e.key == Keys.F6.GetHashCode())
                {
                    var a = (Graphics.GUI.GUIEngine.s_mainMenu.background as Graphics.GUI.Background.MainMenu);
                    System.IO.StreamWriter sw = new System.IO.StreamWriter("aaaa.txt");
                    for (int i = 0; i < a.aaaa.Count; i++)
                    {
                        sw.WriteLine(a.aaaa[i]);
                    }
                    sw.Close();
                }
                //*/
            }
        }

        public static bool IsIn(int x, int y)
        {
            lock(HUDScenes)
                for (int i = 0; i < HUDScenes.Count; i++)
                    if (HUDScenes[i].isVisible && HUDScenes[i].IsIn(x, y)) 
                        return true;
            return false;
        }
    }
}
