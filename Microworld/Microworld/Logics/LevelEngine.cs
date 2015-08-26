using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Logics
{
    public static class LevelEngine
    {
        public const int NO_TIMEOUT = -1;//TODO MOVE!!!

        public static LUA.LUALevelEngine scripts = new LUA.LUALevelEngine();
        private static bool IsRunning = false;
        public static int GameUpdates = 0;

        public static void Init()
        {
            Graphics.GUI.Scene.ComponentSelector.ComponentSelector.onSelectionChanged += 
                new Graphics.GUI.Scene.ComponentSelector.ComponentSelector.SelectionHandler(ComponentSelector_onSelectionChanged);
            GlobalEvents.onComponentSelected += new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentSelected);
            GlobalEvents.onComponentPlacedByPlayer += new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentPlacedByPlayer);
            GlobalEvents.onComponentRemovedByPlayer += new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentRemovedByPlayer);
            GlobalEvents.onResolutionChanged += new GlobalEvents.ResolutionEventHandler(GlobalEvents_onResolutionChanged);
            InputEngine.onButtonClick += new InputEngine.MouseEventHandler(InputEngine_onButtonClick);
            InputEngine.onMouseWheelClick += new InputEngine.MouseEventHandler(InputEngine_onMouseWheelClick);
            Graphics.GUI.GUIEngine.onHUDSceneOpened += new Graphics.GUI.GUIEngine.HUDSceneEventHandler(GUIEngine_onHUDSceneOpened);
            Graphics.GUI.GUIEngine.onHUDSceneClosed += new Graphics.GUI.GUIEngine.HUDSceneEventHandler(GUIEngine_onHUDSceneClosed);
            Graphics.GUI.Scene.Tutorial.OnTutorialClicked += new Graphics.GUI.Scene.Tutorial.TutorialClickedEventHandler(Tutorial_onTutorialClicked);
            Graphics.GUI.Scene.Dialog.OnDialogClicked += new Graphics.GUI.Scene.Dialog.DialogClickedEventHandler(Dialog_onDialogClicked);
            Settings.onGameStateChanged += new Settings.GameStateEventHandler(Settings_onGameStateChanged);
        }

        public static void Update()
        {
            if (IsRunning)
            {
                if (Main.curState.StartsWith("GUI") && Main.curState != "GUIOptions" && Main.curState != "GUILoading" && 
                    Main.curState != "GUIEncyclopedia")
                {
                    //scripts.CallFunction("OnGUIUpdate");
                    Stop();
                }
                else if (Main.curState.StartsWith("GAME"))
                {
                    scripts.CallFunction("OnGameUpdate");
                    if (!MicroWorld.Graphics.GUI.GUIEngine.ContainsHUDScene(MicroWorld.Graphics.GUI.GUIEngine.s_mainMenu))
                        GameUpdates++;
                }
                var a = scripts.GetNumber("powerJoint");
                var b = scripts.GetNumber("groundJoint");
                var c = scripts.GetString("joints");
            }
        }

        public static void LoadLevelScript(String luaFileName)
        {
            GameUpdates = 0;
            Main.LoadingDetails = "Loading LUA script...";
            if (IsRunning)
            {
                Stop();
            }
            if (!System.IO.File.Exists(luaFileName))
            {
                return;
            }
            scripts.Init();
            SetConstants();
            scripts.DoFileAsync(luaFileName);
            Main.LoadingDetails = "Waiting for script initialization...";
            int t = 0;
            while (t < 100)
            {
                t++;
                System.Threading.Thread.Sleep(10);
                if (!scripts.IsExecuting) break;
            }
            if (scripts.IsExecuting)
            {
                scripts.Dispose();
                throw new Exception("Given LUA script appears to have an infinete loop. Aborting loading process.\r\n" +
                    "File name: " + luaFileName);
            }
            else
            {
                System.Threading.Thread.Sleep(100);
                IsRunning = true;
            }
        }

        public static void Stop()
        {
            if (IsRunning)
            {
                scripts.Dispose();
                IsRunning = false;
            }
            GameUpdates = 0;
        }

        internal static void Save(ref IO.SaveWriter sw)
        {
            if (IsRunning)
            {
                sw.WriteLine("ir");
                scripts.CallFunction("SaveState");
                String s = scripts.GetString("saveInfo");
                if (s == null || s.Length == 0) s = "";
                sw.WriteLine(s.Length.ToString());
                sw.WriteLine(s);
            }
            else
            {
                sw.WriteLine("nr");
            }
        }

        internal static void Load(ref IO.SaveReader sr)
        {
            if (sr.ReadLine() == "ir")
            {
                int l = Convert.ToInt32(sr.ReadLine());
                var s = IO.SaveEngine.ReadBlock(sr, l);
                System.Threading.Thread.Sleep(100);
                if (IsRunning)
                    scripts.CallFunction("LoadState", s);
                sr.ReadLine();
            }
        }

        internal static void SetConstants()
        {
            scripts.DoString(
                "noTimeOut = " + NO_TIMEOUT.ToString() + "\r\n" +
                "windowWidth = " + Main.WindowWidth.ToString() + "\r\n" + 
                "windowHeight = " + Main.WindowHeight.ToString() + "\r\n"
                );
        }

        #region Events
        static void Tutorial_onTutorialClicked(object sender, Graphics.GUI.Scene.Tutorial.TutorialClickedArgs e)
        {
            if (IsRunning)
                scripts.CallFunction("GUI_OnTutorialClicked", e.button);
        }

        static void Dialog_onDialogClicked(object sender, Graphics.GUI.Scene.Dialog.DialogClickedArgs e)
        {
            if (IsRunning)
                scripts.CallFunction("GUI_OnDialogClicked", e.button);
        }

        static void GUIEngine_onHUDSceneClosed(string name)
        {
            if (IsRunning)
                scripts.CallFunction("GUI_HUDSceneClose", name);
        }

        static void GUIEngine_onHUDSceneOpened(string name)
        {
            if (IsRunning)
                scripts.CallFunction("GUI_HUDSceneOpen", name);
        }

        static void InputEngine_onMouseWheelClick(InputEngine.MouseArgs e)
        {
            if (IsRunning)
            {
                int gamex = (int)(e.curState.X / Settings.GameScale),
                    gamey = (int)(e.curState.Y / Settings.GameScale);
                GridHelper.GridCoordsOffset(ref gamex, ref gamey);
                scripts.CallFunction("OnMouseClick", e.curState.X, e.curState.Y, gamex, gamey, e.button);
            }
        }

        static void InputEngine_onButtonClick(InputEngine.MouseArgs e)
        {
            if (IsRunning)
            {
                int gamex = (int)(e.curState.X / Settings.GameScale),
                    gamey = (int)(e.curState.Y / Settings.GameScale);
                GridHelper.GridCoordsOffset(ref gamex, ref gamey);
                scripts.CallFunction("OnMouseClick", e.curState.X, e.curState.Y, gamex, gamey, e.button);
            }
        }

        static void GlobalEvents_onComponentSelected(Components.Component sender)
        {
            if (IsRunning)
                scripts.CallFunction("OnComponentSelected", sender.ID);
        }

        static void GlobalEvents_onComponentRemovedByPlayer(Components.Component sender)
        {
            if (IsRunning)
                scripts.CallFunction("OnComponentRemoved", sender.ID);
        }

        static void GlobalEvents_onComponentPlacedByPlayer(Components.Component sender)
        {
            if (IsRunning)
                scripts.CallFunction("OnComponentPlaced", sender.ID);
        }

        static void GlobalEvents_onResolutionChanged(int w, int h, int oldw, int oldh)
        {
            scripts.DoString("windowWidth = " + Main.WindowWidth.ToString() + "\r\n" +
                "windowHeight = " + Main.WindowHeight.ToString() + "\r\n");
        }

        static void ComponentSelector_onSelectionChanged(Graphics.GUI.Scene.ComponentSelector.CSComponentCopy current, 
            Graphics.GUI.Scene.ComponentSelector.CSComponentCopy last)
        {
            if (IsRunning)
                scripts.CallFunction("OnSelectedComponentChanged", current.GetFullPath());
        }

        static void Settings_onGameStateChanged(Settings.GameStates prevState, Settings.GameStates curState)
        {
            if (IsRunning)
                scripts.CallFunction("OnGameStateChanged", prevState.GetHashCode(), curState.GetHashCode());
        }
        #endregion
    }
}
