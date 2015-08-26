using System;
using System.Collections.Generic;
using System.Linq;
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
    public sealed class Main : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Graphics.Renderer renderer
        {
            get
            {
                return MicroWorld.Graphics.GraphicsEngine.Renderer;
            }
        }
        private static String _curState = "GUIGlobalLoad";
        internal static String curState
        {
            get { return Main._curState; }
            set
            {
                if (_curState.StartsWith("GAME") && value.StartsWith("GUI") && value != "GUIOptions" && value != "GUIEncyclopedia")
                {
                    GlobalEvents.OnLevelExited();
                }
                Main._curState = value;
                Sound.SoundPlayer.PlayBackground();
            }
        }
        public static String CurState
        {
            get { return _curState; }
        }
        public static bool isActive = true;
        public static GameWindow window;
        internal static Main game;

        public static String LoadState = "", LoadingDetails = "";
        public static SpriteFont LoadingFont, LoadingSmallFont;
        private RenderTarget2D globalFBO;
        private Effect shader;

        public static CircuitDebug cd;
        public static Debug.ComponentsList cs = new Debug.ComponentsList();
        public static MicroWorld.Debug.MatrixViewer mv;
        public static void showmv() { mv.Show(); }
        public static Debug.ViewVisMap vvm = new Debug.ViewVisMap();
        //public static Debug.LiquidsSystems ls = new Debug.LiquidsSystems();
        public static Debug.CircuitPartBuilder cpb = new Debug.CircuitPartBuilder();

        internal static int windowWidth = 0, windowHeight = 0;
        public static int WindowHeight
        {
            get { return Main.windowHeight; }
        }
        public static int WindowWidth
        {
            get { return Main.windowWidth; }
        }

        System.Threading.Thread LoadingThread;

        internal static Graphics.GUI.Background.GlobalLoad glbg = new Graphics.GUI.Background.GlobalLoad();

        int lastSecond = 0;
        int tps = 0;//ticks
        int fps = 0;//frames

        static long _ticks = 0;
        public static long Ticks
        {
            get { return _ticks; }
        }

        static int _startMS = 0;
        public static int StartMS
        {
            get { return _startMS; }
        }


        public Main()
        {
            _startMS = DateTime.Now.Millisecond;
            
            Settings.CheckVersionFormat();
            Settings.Load();
            IO.Log.Initialize();

            Settings.CheckFolders();

            IO.Log.Write("Entering constructor");

            IO.Log.Write("Setting graphics device");
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IO.Log.Write("Initializing resource manager");
            ResourceManager.Initialize(Content);
            window = Window;
            //Window.Title = "MicroWorld    Version " + Settings.VERSION;
            Window.Title = "MicroWorld";//    Version " + Settings.VERSION;

            //this.IsMouseVisible = true;
            game = this;

            cd = new CircuitDebug();
            //cd.Show();
            mv = new Debug.MatrixViewer();
            //mv.Show();

            
            //graphics.SynchronizeWithVerticalRetrace = true;
            //graphics.PreferredBackBufferWidth = 1024;
            //graphics.PreferredBackBufferHeight = 768;
            //graphics.PreferredBackBufferWidth = 800;
            //graphics.PreferredBackBufferHeight = 480;
            IO.Log.Write("Setting graphics stencil format");
            Main.graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            //graphics.PreferredBackBufferWidth = 1920;
            //graphics.PreferredBackBufferHeight = 1080;
            //graphics.IsFullScreen = true;
            windowWidth = graphics.PreferredBackBufferWidth;
            windowHeight = graphics.PreferredBackBufferHeight;

            Settings.ChangeResolution();

            Utilities.Reflection.RegisterAssembly(System.Reflection.Assembly.GetExecutingAssembly());
            GlobalEvents.onResolutionChanged += new GlobalEvents.ResolutionEventHandler(GlobalEvents_onResolutionChanged);

            lastSecond = DateTime.Now.Second;

            IO.Log.Write("Leaving constructor");
            //vvm.Show();

            Logics.CircuitPart.ttt();
        }

        void GlobalEvents_onResolutionChanged(int w, int h, int oldw, int oldh)
        {
            if (globalFBO != null)
                globalFBO.Dispose();
            globalFBO = new RenderTarget2D(GraphicsDevice, WindowWidth, WindowHeight);
        }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            MicroWorld.Graphics.GraphicsEngine.Renderer = new Graphics.Renderer(spriteBatch);
            IO.Log.Write("Entering initialization");
            glbg.Initialize();
            Settings.ChangeResolution();

            LoadingThread = new System.Threading.Thread(new System.Threading.ThreadStart(InitLoadThread));
            LoadingThread.SetApartmentState(System.Threading.ApartmentState.STA);
            LoadingThread.Start();

            IO.Log.Write("Leaving initialization");
            base.Initialize();
        }

        bool hasSaves = false;
        public void InitLoadThread()
        {
            IO.Log.Write("Entering initialization thread");
            //INIT
            LoadState = "Initializing...";

            Logics.GameLogicsHelper.Init();

            LoadingDetails = "Initializing components...";
            IO.Log.Write("Initializing ComponentManager");
            MicroWorld.Components.ComponentsManager.Initialize();

            //LoadingDetails = "Initializing liquids...";
            //IO.Log.Write("Initializing LiquidsManager");
            //MicroWorld.Logics.Liquids.LiquidsManager.Initialize();

            LoadingDetails = "Initializing graphics...";
            IO.Log.Write("Initializing GraphicsEngine");
            Graphics.GraphicsEngine.Initialize();

            LoadingDetails = "Initializing sound...";
            IO.Log.Write("Initializing SoundManager");
            Sound.SoundManager.Initialize();

            LoadingDetails = "Initializing Levels Engine...";
            IO.Log.Write("Initializing Levels Engine");
            Logics.LevelEngine.Init();

            LoadingDetails = "Initializing mods...";
            IO.Log.Write("Initializing mods");
            Modding.ModdingLogics.Initialize();

            hasSaves = System.IO.File.Exists("Saves/progress.lpg");
            LoadingDetails = "Loading save data...";
            IO.Log.Write("Loading save data");
            Logics.CampaingProgress.Initialize();
            //Sound.SoundManager.MasterVolume = 0.0f;
            Logics.GameInputHandler.Initialize();

            //CONTENT
            LoadState = "Loading Content...";
            IO.Log.Write("Loading content...");

            LoadingDetails = "Loading statistics...";
            IO.Log.Write("    Loading statistics");
            Statistics.Load();
            Statistics.GameStarts++;

            LoadingDetails = "Loading graphics...";
            IO.Log.Write("    Loading graphics");
            Graphics.GraphicsEngine.LoadContent();

            LoadingDetails = "Loading sounds...";
            IO.Log.Write("    Loading souns");
            Sound.Sounds.LoadContent();

            LoadingDetails = "Loading content for mods...";
            IO.Log.Write("    Loading content for mods");
            Modding.ModdingLogics.LoadContent();

            LoadState = "Post initialization...";
            LoadingDetails = "GUI...";
            var a = Settings.Resolution.Split('*');
            GlobalEvents.OnResolutionChanged(Convert.ToInt32(a[0]), Convert.ToInt32(a[1]), 800, 480);

            if (Utilities.Tools.IsRunningOnMono())
                Settings.ChangeResolution();

            IO.Log.Write("Leaving initialization thread");

            if (Utilities.Tools.IsRunningOnMono())
                LoadingThread.Abort();
        }

        protected override void LoadContent()
        {
            LoadingFont = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_15");
            LoadingSmallFont = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_11");
            globalFBO = new RenderTarget2D(GraphicsDevice, WindowWidth, WindowHeight);
            shader = ResourceManager.Load<Effect>("Shaders/StereoDistortion");
        }

        protected override void UnloadContent()
        {
            try
            {
                if (LoadingThread != null && LoadingThread.ThreadState == System.Threading.ThreadState.Running)
                    LoadingThread.Abort();
                System.Threading.Thread.Sleep(5);
            }
            catch { }
            try
            {
                Graphics.GUI.GUIEngine.s_componentSelector.SaveStructure();
            }
            catch { }
            try
            {
                IO.Log.UnloadContent();
            }
            catch { }
            try
            {
                Logics.LUA.VMWatcher.Unload();
            }
            catch { }
            try
            {
                Sound.SoundManager.UnloadContent();
            }
            catch { }
            Statistics.Save();
            Logics.CampaingProgress.Save();
            Settings.Save();
        }

        internal static int updateExcaprionsInRow = 0;
        protected override void Update(GameTime gameTime)
        {
            if (InputEngine.IsKeyPressed(Keys.F10) && !InputEngine.IsKeyDown(Keys.F11))
            {
                InputEngine.Update();
                return;
            }

            Debug.Initializer.Update();
            Debug.ChartDebug.Step();

            _ticks++;
            isActive = IsActive;

            try//TODO rm?
            {
                if (curState == "GUIGlobalLoad")
                {
                    glbg.Update();
                    if (LoadingThread.ThreadState == System.Threading.ThreadState.Stopped || !LoadingThread.IsAlive)
                    {
                        if (Settings.startAsEncEdit)
                        {
                            Main.curState = "GUIEncyclopediaEditor";
                            Graphics.GUI.GUIEngine.curScene = Graphics.GUI.GUIEngine.s_encyclopediaEditor;
                        }
                        else
                        {
                            glbg.OnContentLoaded();
                            //Graphics.GUI.GUIEngine.ChangeScene(Graphics.GUI.GUIEngine.s_mainMenu, "GUIMain");
                        }
                    }

                    base.Update(gameTime);
                    return;
                }

                this.IsMouseVisible = !this.IsActive;

                var tt1 = DateTime.Now;

                InputEngine.Update();

                Modding.ModdingLogics.PreUpdate();
                Logics.GameLogicsHelper.Update();

                Graphics.GraphicsEngine.Update();

                Logics.LevelEngine.Update();
                Graphics.GUI.ClickabilityOverlay.Update();

                Debug.DebugInfo.MatrixRecalculateTime = 0;
                Debug.DebugInfo.ComponentUpdateTime = 0;
                if (curState.StartsWith("GAME"))
                {
                    MicroWorld.Components.ComponentsManager.NonGameUpdate();
                    if (Settings.GameState == Settings.GameStates.Running)
                    {
                        Logics.GameLogicsHelper.SimulationStep();
                    }
                }

                Sound.SoundManager.Update();
                Sound.SoundPlayer.Update();

                Modding.ModdingLogics.PostUpdate();

                var tt5 = DateTime.Now;
                var t2 = tt5.Subtract(tt1);
                Debug.DebugInfo.UpdateTime = (int)t2.TotalMilliseconds;

                //Window.Title = (Graphics.GUI.GUIEngine.s_mainMenu.background as Graphics.GUI.Background.MainMenu).seed.ToString();
                tps++;
                if (lastSecond != DateTime.Now.Second)
                {
                    lastSecond = DateTime.Now.Second;
                    Debug.DebugInfo.UpdatesPerSecond = tps;
                    Debug.DebugInfo.FramesPerSecond = fps;
                    tps = 0;
                    fps = 0;
                }
                updateExcaprionsInRow = 0;
            }
            //catch (Exception e)
            catch (DivideByZeroException e)//TODO
            {
                throw e;//TODO rm in releases
                updateExcaprionsInRow++;
                IO.Log.Write(e, updateExcaprionsInRow, false, true);
                if (updateExcaprionsInRow >= 5)
                {
                    IO.Log.Write(IO.Log.State.SEVERE, "Maximum number of exceptions in row was reached in Update. Terminating...");
                    Close();
                }
            }

            //Window.Title = InputEngine.curMouse.X.ToString() + "  ;  " + InputEngine.curMouse.Y.ToString();

            base.Update(gameTime);
        }

        public static void Close()
        {
            game.Exit();
        }

        internal static int debugExcaprionsInRow = 0;
        protected override void Draw(GameTime gameTime)
        {
            try//TODO rm?
            {
                if (globalFBO == null)
                    return;

                if (InputEngine.IsKeyPressed(Keys.F10) && !InputEngine.IsKeyDown(Keys.F11))
                {
                    goto PostDraw;
                }

                renderer.EnableFBO(globalFBO);
                if (curState == "GUIGlobalLoad")
                {
                    GraphicsDevice.Clear(new Color(45, 57, 107));
                    renderer.BeginUnscaled();
                    glbg.Draw(renderer);
                    renderer.End();
                    goto PostDraw;
                }

                var t1 = DateTime.Now;

                GraphicsDevice.Clear(new Color(45, 57, 107));
                renderer.Begin();

                Modding.ModdingLogics.PreDraw();

                Graphics.GraphicsEngine.Draw();
                Debug.DebugInfo.Draw();
                Debug.InputInfo.Draw();
                Debug.CollidersOverlay.Draw();
                Debug.VisMapOverlay.Draw();

                Modding.ModdingLogics.PostDraw();

                renderer.End();

                var t2 = DateTime.Now;
                var t = t2.Subtract(t1);
                Debug.DebugInfo.DrawTime = (int)t.TotalMilliseconds;

                fps++;
                debugExcaprionsInRow = 0;

            PostDraw:
                renderer.DisableFBO();
                if (InputEngine.curKeyboard.IsKeyDown(Keys.F11) && !InputEngine.lastKeyboard.IsKeyDown(Keys.F11))
                {
                    var fs = new System.IO.FileStream("screenshot.png", FileMode.Create);
                    globalFBO.SaveAsPng(fs, globalFBO.Width, globalFBO.Height);
                    fs.Close();
                }
                GraphicsDevice.Clear(new Color(45, 57, 107));
                updateStereoShader();
                renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone,
                    shader);
                renderer.Draw(globalFBO, new Vector2(), Color.White);
                renderer.End();
                //*
            }
            //catch (Exception e)
            catch (DivideByZeroException e)//TODO
            {
                throw e;//TODO rm in releases
                debugExcaprionsInRow++;
                IO.Log.Write(e, debugExcaprionsInRow, true, false);
                if (debugExcaprionsInRow >= 5)
                {
                    IO.Log.Write(IO.Log.State.SEVERE, "Maximum number of exceptions in row was reached in Draw. Terminating...");
                    Close();
                }
            }//*/

            base.Draw(gameTime);
        }

        bool dist = false;
        int distlength = 0;
        public static int MaxDistPower = 10;//TODO private const
        public static int DistortionLevel = 1;
        private void updateStereoShader()
        {
            if (shader == null)
                return;
            Random r = new Random();
            if (distlength < 0)
            {
                dist = !dist;
                distlength = r.Next(dist ? 10 : 100, dist ? 30 : 3000);
            }
            distlength--;

            Matrix projection = Matrix.CreateOrthographicOffCenter(0,
                Main.WindowWidth, Main.WindowHeight, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            shader.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);
            float pw = 1f / Main.WindowWidth;
            shader.Parameters["halfpixel"].SetValue(new float[] { pw / 2f, 0.5f / Main.WindowHeight });
            shader.Parameters["sinTime"].SetValue(Ticks);
            if (dist)//TODO
            {
                shader.Parameters["power"].SetValue(0);
                shader.Parameters["brightnessDistortion"].SetValue(0);
                shader.Parameters["sinDistForce"].SetValue(0);
                shader.Parameters["inversionForce"].SetValue(0);

                for (int i = 0; i < DistortionLevel; i++)
                {
                    switch (r.Next(4))
                    {
                        case 0:
                            shader.Parameters["power"].SetValue((float)(r.NextDouble() * MaxDistPower * pw));
                            break;
                        case 1:
                            //shader.Parameters["brightnessDistortion"].SetValue((float)((r.NextDouble() - 0.5) / 8));
                            break;
                        case 2:
                            //shader.Parameters["sinDistForce"].SetValue((float)(r.NextDouble() / 2));
                            break;
                        case 3:
                            //shader.Parameters["inversionForce"].SetValue((float)(r.NextDouble()) / 3);
                            break;
                        default:
                            break;
                    }
                }
                shader.Parameters["power"].SetValue((float)(r.NextDouble() * MaxDistPower * pw));
            }
            else
            {
                shader.Parameters["power"].SetValue(0);
                shader.Parameters["brightnessDistortion"].SetValue(0);
                shader.Parameters["sinDistForce"].SetValue(0);
                shader.Parameters["inversionForce"].SetValue(0);
            }
        }
    }
}
