using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Logics
{
    public static class GameLogicsHelper
    {
        public static bool CanStart = true, CanPause = true, CanStep = true, CanStop = true;

        public static void Init()
        {
            GlobalEvents.onLevelExited += new GlobalEvents.LevelEventHandler(GlobalEvents_onLevelExited);
        }

        public static void Update()
        {
        }

        public static void OnLevelChange()
        {
            Sound.SoundPlayer.ChangeGameBG();
            Graphics.GUI.GUIEngine.RemoveHUDScene<Components.GUI.GeneralProperties>();
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_subComponentButtons);
            Graphics.GUI.GUIEngine.RemoveHUDScene<Graphics.GUI.Scene.ScriptedInfoPanel>();
            Graphics.GraphicsEngine.camera.Center = new Microsoft.Xna.Framework.Vector2();
            Graphics.GUI.GUIEngine.s_componentSelector.Collapse();
            Graphics.OverlayManager.Clear();

            CanStart = true;
            CanPause = true;
            CanStep = true;
            CanStop = true;
        }

        public static void InitForGame()
        {
            Logics.LevelEngine.Stop();
            Logics.PlacableAreasManager.Clear();
            Settings.ResetInGameSettings();
            Logics.CircuitManager.Clear();
            Components.ComponentsManager.Clear();
            Logics.ChangeHistory.Reset();
            Graphics.GraphicsEngine.camera.Center = new Microsoft.Xna.Framework.Vector2();

            CanStart = true;
            CanPause = true;
            CanStep = true;
            CanStop = true;
        }

        public static void InitScenesForGame()
        {
            Graphics.GUI.GUIEngine.s_componentSelector.ResetSelection();
            Graphics.GUI.GUIEngine.s_componentSelector.Collapse();
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_componentSelector);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_runControl);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_clickableAreas);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_maphud);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_zoombar);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_visMapOverlay);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_clickableAreas);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_infoPanel);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_ruller);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_lightOverlay);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_magneticOverlay);
        }

        internal static void GlobalEvents_onLevelExited()
        {
            Logics.LevelEngine.Stop();

            CanStart = true;
            CanPause = true;
            CanStep = true;
            CanStop = true;
            GameStop();

            Graphics.GUI.GUIEngine.s_componentSelector.isVisible = true;
            Graphics.GUI.GUIEngine.s_runControl.isVisible = true;
            Graphics.GUI.GUIEngine.s_maphud.isVisible = true;

            Logics.PlacableAreasManager.Clear();
            Graphics.GUI.ClickabilityOverlay.allowedClear();
            Graphics.GUI.GUIEngine.s_componentSelector.ClearCount();
            Graphics.GUI.GUIEngine.RemoveHUDScene<Components.GUI.GeneralProperties>();
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_subComponentButtons);
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_componentSelector);
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_code);
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_placableAreasCreator);
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_runControl);
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_scriptEditor);
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_tutorial);
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_victoryMessage);
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_maphud);
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_zoombar);
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_purpose);
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_visMapOverlay);
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_clickableAreas);
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_infoPanel);
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_ruller);
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_lightOverlay);
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_magneticOverlay);
            Graphics.GUI.GUIEngine.RemoveHUDScene<Components.GUI.CoreCloseup>();
            Graphics.GUI.GUIEngine.RemoveHUDScene<MicroWorld.Graphics.GUI.Scene.ToolTip>();
            Graphics.GUI.GUIEngine.RemoveHUDScene<Graphics.GUI.Scene.ScriptedInfoPanel>();
            Graphics.GUI.GUIEngine.ClearHUDs();
            Graphics.GUI.GUIEngine.s_toolTip.Close();
            Graphics.OverlayManager.Clear();
            Graphics.GraphicsEngine.camera.Center = new Microsoft.Xna.Framework.Vector2();
            Settings.GameScale = 2f;
            if (Settings.GameState != Settings.GameStates.Stopped)
                Graphics.GUI.GUIEngine.s_runControl.stpClick(null, null);
        }

        #region Simulation
        static bool abortPauseStart = false;
        public static void GameStart()
        {
            if (!CanStart)
                return;
            if (Settings.GameState == Settings.GameStates.Running) 
                return;
            if (Settings.GameState == Settings.GameStates.Stopped)
            {
                Logics.CircuitManager.Clear();
                //Components.ComponentsManager.Reset();
                Logics.CircuitManager.InitializeCircuit();
                //Liquids.LiquidsManager.Start();
                Components.ComponentsManager.Start();
            }
            Settings.GameState = Settings.GameStates.Running;

            Statistics.TimesSimulationStarted++;
            //if (Graphics.GUI.GUIEngine.s_subComponentButtons.isVisible)
            //{
            //    Graphics.GUI.GUIEngine.s_subComponentButtons.Close();
            //}
        }

        public static void GameStop()
        {
            if (!CanStop)
                return;
            if (Settings.GameState == Settings.GameStates.Stopped) 
                return;
            Logics.CircuitManager.Reset();
            Components.ComponentsManager.Reset();
            Settings.GameState = Settings.GameStates.Stopped;
            abortPauseStart = true;
        }

        public static void GamePause()
        {
            if (Settings.GameState == Settings.GameStates.Stopped)
            {
                if (!CanStart)
                    return;
                abortPauseStart = false;
                GameStart();
                if (abortPauseStart) return;
                GamePause();
            }
            if (Settings.GameState == Settings.GameStates.Paused)
            {
                if (CanStep)
                    SimulationStep();
            }
            else
            {
                if (CanPause)
                    Settings.GameState = Settings.GameStates.Paused;
            }
        }

        internal static Settings.GameStates prePauseState;
        internal static void _gamePause()
        {
            prePauseState = Settings.gameState;
            Settings.gameState = Settings.GameStates.Paused;
        }

        internal static void _gameResume()
        {
            Settings.gameState = prePauseState;
        }

        public static void SimulationStep()
        {
            var tt4 = DateTime.Now;
            MicroWorld.Logics.CircuitManager.Update();
            var tt5 = DateTime.Now;
            var tt2 = DateTime.Now;
            MicroWorld.Components.ComponentsManager.Update();
            var tt3 = DateTime.Now;
            //MicroWorld.Logics.Liquids.LiquidsManager.Update();

            var t = tt5.Subtract(tt4);
            Debug.DebugInfo.MatrixRecalculateTime = (int)t.TotalMilliseconds;
            t = tt4.Subtract(tt3);
            Debug.DebugInfo.LiquidsUpdateTime = (int)t.TotalMilliseconds;
            t = tt3.Subtract(tt2);
            Debug.DebugInfo.ComponentUpdateTime = (int)t.TotalMilliseconds;

            Debug.ChartDebug.StepGame();
        }
        #endregion
    }
}
