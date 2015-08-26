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
using System.IO;

namespace MicroWorld
{
    public static class Settings
    {
        public static readonly Color BGCOLOR = new Color(45, 57, 107);

        public enum GameStates
        {
            Paused = 0,
            Running = 1,
            Stopped = 2
        }

        public enum Maps
        {
            Normal = 0,
            Luminosity = 1,
            Magnet = 2
        }

        #region Constants
        public const String VERSION = "0.110 e";//When changed, change IsCompatibleSave

        public const double MAX_RESISTANCE = 10000000d;//10mOhm
        public const double MAX_CURRENT = 1000d;
        public const double MAX_VOLTAGE = 5000d;
        public const double MAX_LUMINOSITY = 800d;
        public const double MAX_MAGNETIC_FIELD = 800d;
        public const double MAX_CAPACITANCE = 1000d;
        #endregion

        public static bool startAsEncEdit = false;

        #region CameraImplementations
        public static float GameScale
        {
            get { return Graphics.GraphicsEngine.camera.Scale; }
            set
            {
                Graphics.GraphicsEngine.camera.Scale = value;
            }
        }
        public static Vector2 GameOffset
        {
            get { return Graphics.GraphicsEngine.camera.BottomRight; }
        }
        #endregion

        internal static GameStates gameState = GameStates.Stopped;
        public static GameStates GameState
        {
            get { return Settings.gameState; }
            set
            {
                if (Settings.gameState != value)
                {
                    var old = Settings.gameState;
                    Settings.gameState = value;
                    if (gameState == GameStates.Stopped || old == GameStates.Stopped)
                        simulationTicks = 0;
                    if (onGameStateChanged != null)
                    {
                        onGameStateChanged.Invoke(old, Settings.gameState);
                    }
                }
            }
        }
        internal static int simulationTicks = 0;
        public static int SimulationTicks
        {
            get { return simulationTicks; }
        }
        public delegate void GameStateEventHandler(GameStates prevState, GameStates curState);
        public static event GameStateEventHandler onGameStateChanged;
        public static Maps CurrentMap = Maps.Normal;

        #region Debug vars
        public static bool Debug = false;
        public static bool LogInput = false;
        public static bool DrawInvisibleWires = false;
        #endregion

        #region Volume vars
        public static float MasterVolume
        {
            get
            {
                return Sound.SoundManager.MasterVolume;
            }
            set
            {
                Sound.SoundManager.MasterVolume = value;
            }
        }
        private static float musicVolume = 0f;
        public static float MusicVolume
        {
            get { return Settings.musicVolume; }
            set
            {
                if (value > 1) value = 1f;
                if (value < 0) value = 0;
                Settings.musicVolume = value;
            }
        }
        private static float effectsVolume = 1f;
        public static float EffectsVolume
        {
            get { return Settings.effectsVolume; }
            set
            {
                if (value > 1) value = 1f;
                if (value < 0) value = 0; 
                Settings.effectsVolume = value;
            }
        }
        #endregion

        #region Graphics vars
        public static String AspectRatio = "16*9";
        public static String Resolution = "1280*720";
        public static bool IsFullscreen = false;//TODO change to true
        #endregion

        #region General vars
        internal static bool IntroWarningSkip_File = false;
        internal static bool IntroWarningShow = true;
        #endregion

        #region HotKeys
        public static IO.InputSequence k_SimulationStart = new IO.InputSequence(false, Keys.F5);
        public static IO.InputSequence k_SimulationPause = new IO.InputSequence(false, Keys.F6);
        public static IO.InputSequence k_SimulationStop = new IO.InputSequence(false, Keys.F7);
        public static IO.InputSequence k_Undo = new IO.InputSequence(false, true, false, false, Keys.Z);
        public static IO.InputSequence k_ComponentRemove = new IO.InputSequence(false, false, false, false, false, true, false, 0);
        public static IO.InputSequence k_Eraser = new IO.InputSequence(true, Keys.Delete);
        public static IO.InputSequence k_ZoomIn = new IO.InputSequence(true, true, false, false, false, false, false, 1);
        public static IO.InputSequence k_ZoomOut = new IO.InputSequence(true, true, false, false, false, false, false, -1);
        public static IO.InputSequence k_ToggleGrid = new IO.InputSequence(false, Keys.G);
        public static IO.InputSequence k_DragScene = new IO.InputSequence(true, false, false, false, false, false, true, 0);
        public static IO.InputSequence k_ComponentRotateCW = new IO.InputSequence(false, false, false, false, false, false, false, -1);
        public static IO.InputSequence k_ComponentRotateCCW = new IO.InputSequence(false, false, false, false, false, false, false, 1);

        public static bool IsAnyInputSequenceMet()
        {
            return
                k_SimulationStart.IsMatched() ||
                k_SimulationPause.IsMatched() ||
                k_SimulationStop.IsMatched() ||
                k_Undo.IsMatched() ||
                k_ComponentRemove.IsMatched() ||
                k_Eraser.IsMatched() ||
                k_ZoomIn.IsMatched() ||
                k_ZoomOut.IsMatched() ||
                k_ToggleGrid.IsMatched() ||
                k_DragScene.IsMatched() ||
                k_ComponentRotateCW.IsMatched() ||
                k_ComponentRotateCCW.IsMatched();
        }
        #endregion

        public static void CheckVersionFormat()
        {
            try
            {
                var a = VERSION.Split(' ');
                var b = a[0].Split('.');
                int t = Convert.ToInt32(b[0]);
                t = Convert.ToInt32(b[1]);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Invalid version format!");
                Main.Close();
            }
        }

        public static bool IsCompatibleSave(String version)
        {
            if (version == "0.090 CAT" || 
                version == "0.100 CBT" || version == "0.101 CBT" || version == "0.102 CBT" ||
                version == "0.103 OBT" ||
                version == "0.104 CBT" ||
                version == "0.105" ||
                version == "0.106 CBT" || version == "0.107 CBT" ||
                version == "0.108 OBT" || 
                version == "0.109" ||
                version == "0.110 b" || version == "0.110 c" || version == "0.110 d" || version == "0.110 e")
                return true;
            return false;
        }

        public static void Save()
        {
            BinaryWriter bw = new BinaryWriter(new FileStream("settings.dat", FileMode.Create), Encoding.Unicode);
            bw.Write(VERSION);
            bw.Write((double)MasterVolume);
            bw.Write((double)musicVolume);
            bw.Write((double)effectsVolume);
            bw.Write(AspectRatio);
            bw.Write(Resolution);
            bw.Write(IsFullscreen);
            bw.Write(Graphics.GUI.GridDraw.ShouldDrawGrid);
            bw.Write(IntroWarningShow);
            //hotkeys
            bw.Write(k_SimulationStart.Save());
            bw.Write(k_SimulationStop.Save());
            bw.Write(k_SimulationPause.Save());
            bw.Write(k_Undo.Save());
            bw.Write(k_ComponentRemove.Save());
            bw.Write(k_Eraser.Save());
            bw.Write(k_ZoomIn.Save());
            bw.Write(k_ZoomOut.Save());
            bw.Write(k_ToggleGrid.Save());
            bw.Write(k_DragScene.Save());
            bw.Write(k_ComponentRotateCW.Save());
            bw.Write(k_ComponentRotateCW.Save());
            bw.Close();
        }

        public static void Load()
        {
            #region Debug
            if (System.IO.File.Exists("debug.txt"))
            {
                Settings.Debug = true;
                try
                {
                    System.IO.File.Delete("debug.txt");
                }
                catch { }
            }
            #endregion

            #region Startup
            bool skipload = false;
            if (System.IO.File.Exists("startup.txt"))
            {
                try
                {
                    System.IO.StreamReader sr = new StreamReader("startup.txt");
                    bool delete = true;
                    while (sr.Peek() > -1)
                    {
                        String s = sr.ReadLine().ToLower();
                        if (s == "keepfile") delete = false;
                        if (s == "debug") Debug = true;
                        if (s == "defaultsettings") skipload = true;
                        if (s == "loginput") LogInput = true;
                        if (s == "resetsettings")
                            try
                            {
                                System.IO.File.Delete("settings.dat");
                            }
                            catch { }
                        if (s == "skipwarning") IntroWarningSkip_File = true;
                    }
                    sr.Close();
                    if (delete) System.IO.File.Delete("startup.txt");
                }
                catch { }
            }
            #endregion

            #region Settings
            if (!skipload && File.Exists("settings.dat"))
            {
                try
                {
                    BinaryReader br = new BinaryReader(new FileStream("settings.dat", FileMode.Open), Encoding.Unicode);
                    if (!IsCompatibleSave(br.ReadString()))
                    {
                        br.Close();
                        MasterVolume = 0.8f;
                        MusicVolume = 0f;
                        EffectsVolume = 0.8f;
                        return;
                    }
                    MasterVolume = (float)br.ReadDouble();
                    MusicVolume = (float)br.ReadDouble();
                    EffectsVolume = (float)br.ReadDouble();
                    AspectRatio = br.ReadString();
                    Resolution = br.ReadString();
                    IsFullscreen = br.ReadBoolean();
                    Graphics.GUI.GridDraw.ShouldDrawGrid = br.ReadBoolean();
                    IntroWarningShow = br.ReadBoolean();
                    //hotkeys
                    if (br.PeekChar() > 0)
                    {
                        k_SimulationStart.Load(br.ReadString());
                        k_SimulationStop.Load(br.ReadString());
                        k_SimulationPause.Load(br.ReadString());
                        k_Undo.Load(br.ReadString());
                        k_ComponentRemove.Load(br.ReadString());
                        k_Eraser.Load(br.ReadString());
                        k_ZoomIn.Load(br.ReadString());
                        k_ZoomOut.Load(br.ReadString());
                        k_ToggleGrid.Load(br.ReadString());
                        k_DragScene.Load(br.ReadString());
                        k_ComponentRotateCW.Load(br.ReadString());
                        k_ComponentRotateCW.Load(br.ReadString());
                    }
                    br.Close();
                }
                catch { }
                //ChangeResolution();
            }
            else
            {
                MasterVolume = 0.8f;
                MusicVolume = 0f;
                EffectsVolume = 0.8f;
            }
            #endregion
        }

        public static void ResetInGameSettings()
        {
            Graphics.GraphicsEngine.camera.AllowedVisibleRectangle = null;
        }

        public static void CheckFolders()
        {
            if (!Directory.Exists("Content"))
                Directory.CreateDirectory("Content");
            if (!Directory.Exists("Content/Encyclopedia"))
                Directory.CreateDirectory("Content/Encyclopedia");
            if (!Directory.Exists("Saves"))
                Directory.CreateDirectory("Saves");
            if (!Directory.Exists("Saves/Levels"))
                Directory.CreateDirectory("Saves/Levels");
            if (!Directory.Exists("Saves/Sandbox"))
                Directory.CreateDirectory("Saves/Sandbox");
        }

        public static void ChangeResolution(int w, int h, bool fs)
        {
            IO.Log.Write("Attempting to change resolution:");
            IO.Log.Write("    Old resolution: " + Main.WindowWidth + "*" + Main.WindowHeight + "; Fullscreen = " + Main.graphics.IsFullScreen.ToString());
            IO.Log.Write("    New resolution: " + w.ToString() + "*" + h.ToString() + "; Fullscreen = " + fs.ToString());
            if (Main.graphics.GraphicsDevice == null)
            {
                IO.Log.Write(IO.Log.State.WARNING, " Graphics device is null");
                int oldw = Main.WindowWidth;
                int oldh = Main.WindowHeight;
                Main.graphics.PreferredBackBufferWidth = w;
                Main.graphics.PreferredBackBufferHeight = h;
                Main.graphics.IsFullScreen = fs;
                Main.graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
                IO.Log.Write("    Attempting to apply changes...");
                Main.graphics.ApplyChanges();
                IO.Log.Write("    Changes applied successfully");
                Main.windowWidth = Main.graphics.PreferredBackBufferWidth;
                Main.windowHeight = Main.graphics.PreferredBackBufferHeight;
                IO.Log.Write("    Current resolution: " + Main.WindowWidth + "*" + Main.WindowHeight + "; Fullscreen = " + Main.graphics.IsFullScreen.ToString());
                GlobalEvents.OnResolutionChanged(w, h, oldw, oldh);

                if (Utilities.Tools.IsRunningOnMono())
                {
                    Utilities.FrameworkSpecificTools.SetWindowPosition(
                        (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - Main.WindowWidth) / 2,
                        (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - Main.WindowHeight) / 2);
                    if (IsFullscreen)
                        Utilities.FrameworkSpecificTools.SetWindowPosition(0, 0);
                    Utilities.FrameworkSpecificTools.SetWindowBordered(!IsFullscreen);
                }
            }
            else
            {
                int oldw = Main.graphics.GraphicsDevice.Viewport.Width;
                int oldh = Main.graphics.GraphicsDevice.Viewport.Height;
                Main.graphics.PreferredBackBufferWidth = w;
                Main.graphics.PreferredBackBufferHeight = h;
                Main.graphics.IsFullScreen = fs;
                Main.graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
                IO.Log.Write("    Attempting to apply changes...");
                Main.graphics.ApplyChanges();
                IO.Log.Write("    Changes applied successfully");
                if (Utilities.Tools.IsRunningOnMono())
                {
                    Main.windowWidth = Main.graphics.PreferredBackBufferWidth;
                    Main.windowHeight = Main.graphics.PreferredBackBufferHeight;
                }
                else
                {
                    Main.windowWidth = Main.graphics.GraphicsDevice.Viewport.Width;
                    Main.windowHeight = Main.graphics.GraphicsDevice.Viewport.Height;
                }
                IO.Log.Write("    Current resolution: " + Main.WindowWidth + "*" + Main.WindowHeight + "; Fullscreen = " + Main.graphics.IsFullScreen.ToString());
                GlobalEvents.OnResolutionChanged(Main.WindowWidth, Main.WindowHeight, oldw, oldh);
                if (Main.renderer != null) Main.renderer.OnResolutionChanged();

                if (Utilities.Tools.IsRunningOnMono())
                {
                    Utilities.FrameworkSpecificTools.SetWindowPosition(
                        (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - Main.WindowWidth) / 2,
                        (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - Main.WindowHeight) / 2);
                    if (IsFullscreen)
                        Utilities.FrameworkSpecificTools.SetWindowPosition(0, 0);
                    Utilities.FrameworkSpecificTools.SetWindowBordered(!IsFullscreen);
                }
            }
        }

        public static void ChangeResolution(int w, int h)
        {
            ChangeResolution(w, h, IsFullscreen);
        }

        public static void ChangeResolution(String res)
        {
            var a = res.Split('*');
            ChangeResolution(Convert.ToInt32(a[0]), Convert.ToInt32(a[1]));
        }

        public static void ChangeResolution(String res, bool fs)
        {
            var a = res.Split('*');
            ChangeResolution(Convert.ToInt32(a[0]), Convert.ToInt32(a[1]), fs);
        }

        public static void ChangeResolution()
        {
            var a = Resolution.Split('*');
            ChangeResolution(Convert.ToInt32(a[0]), Convert.ToInt32(a[1]), IsFullscreen);
        }

    }
}
