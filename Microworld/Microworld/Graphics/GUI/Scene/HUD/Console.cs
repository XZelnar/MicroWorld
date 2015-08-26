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
using System.Reflection;
using System.IO;

namespace MicroWorld.Graphics.GUI.Scene
{
    unsafe class Console : HUDScene//TODO bigger history
    {
        Elements.TextBox tbLog, tbinput;
        string[] values = new string[0];
        int logoffset = 0;
        int CurInputHistorySelected = -1;

        String[] InputHistory = new String[20];

        internal static ScripterNet.ScripterVM vm;
        
        public static void writeln(object o)
        {
            OutputEngine.WriteLine(">> " + o == null ? "null" : o.ToString());
        }

        public static void debuggraph()
        {
            Debug.Initializer.ShowForm(new Debug.ChartDebug());
        }

        public new void Initialize()
        {
            vm = new ScripterNet.ScripterVM();
            vm.RegisterFunction("print", GetType().GetMethod("writeln"));
            vm.RegisterFunction("graph", GetType().GetMethod("debuggraph"));

            ShouldBeScaled = false;
            Layer = 90000;

            tbLog = new Elements.TextBox(0, 0, 800, 20 * 20);
            tbLog.Editable = false;
            tbLog.ShouldOffset = false;
            tbLog.Font = ResourceManager.Load<SpriteFont>("Fonts/CourierNew_14");
            tbLog.BackgroundColor = tbLog.BackgroundColor * 0.7f;
            controls.Add(tbLog);

            tbinput = new Elements.TextBox(0, OutputEngine.LOG_LENGTH * 20, 800, 30);
            tbinput.Font = ResourceManager.Load<SpriteFont>("Fonts/CourierNew_14");
            tbinput.BackgroundColor = tbinput.BackgroundColor * 0.7f;
            tbinput.Multiline = true;
            controls.Add(tbinput);

            logoffset = 1000;
            CheckLogRange();

            base.Initialize();

            if (File.Exists("console.txt"))
            {
                string s = File.ReadAllText("console.txt");
                try
                {
                    DoCommand(s);
                }
                catch (Exception e) { }
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();

            var a = tbLog.Font.MeasureString("w");
            tbLog.size.Y = 20 * a.Y;
            tbinput.size.Y = a.Y;
            tbinput.position.Y = tbLog.size.Y;
        }

        public override void onShow()
        {
            ticks = 60;
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(FadeIn));
            t.IsBackground = true;
            t.Start();
            tbinput.isFocused = true;
            base.onShow();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            tbLog.size.X = w;
            tbinput.size.X = w;
            tbinput.size.Y = h - tbinput.position.X;
        }

        int ticks = 0;
        public override void Update()
        {
            if (!isVisible) return;
            if (!tbinput.isFocused) tbinput.isFocused = true;
            //tbLog.size = new Vector2(Main.window.ClientBounds.Width, OutputEngine.LOG_LENGTH * 20);

            ticks++;
            if (ticks >= 20)//reupdate hints and logs every second or so
            {
                updateLog();
                ticks = 0;
            }

            base.Update();
        }

        private void updateLog()
        {
            String t = "";
            for (int i = OutputEngine.LOG_LENGTH - 1; i >= 0; i--)
            {
                if (i != 0)
                    t += "\r\n";
                t += OutputEngine.log[i];
            }
            tbLog.Text = t;
            tbLog.TextOffset = new Vector2(0, logoffset * tbLog.Font.MeasureString("w").Y);
        }

        float opacity = 0f;
        public override void Draw(Renderer renderer)
        {
            //bg
            RenderHelper.SmartDrawRectangle(GraphicsEngine.pixel, 6, 0, 0, Main.WindowWidth, Main.WindowHeight, Color.Black * opacity,
                renderer);
            //tbs
            base.Draw(renderer);
            //separator
            renderer.Draw(Shortcuts.pixel, new Rectangle(0, (int)tbinput.position.Y, Main.windowWidth, 1), Color.White * opacity);
            //hints
            if (!isfading && tbinput.position.Y > 0)
            {
                //bg
                renderer.Draw(GraphicsEngine.pixel,
                    new Rectangle(0, (int)(tbinput.position.Y + tbinput.size.Y), Main.WindowWidth, Main.WindowHeight),
                    new Color(0, 0, 50) * 0.8f);

                var s = tbLog.Font.MeasureString("w");
                int hc = (int)((Main.WindowHeight - tbLog.size.Y - tbinput.size.Y) / s.Y);
            }
        }

        System.Threading.Thread fadeThread;
        public override void Close()
        {
            if (fadeThread != null && fadeThread.ThreadState != System.Threading.ThreadState.Stopped)
                fadeThread.Abort();
            fadeThread = new System.Threading.Thread(new System.Threading.ThreadStart(FadeOut));
            fadeThread.IsBackground = true;
            fadeThread.Start();
        }

        #region Fades
        bool isfading = false;
        public override void FadeIn()
        {
            isfading = true;
            tbLog.position.Y = -tbLog.size.Y - tbinput.size.Y;
            tbinput.position.Y = -tbinput.size.Y;
            var d = tbLog.size.Y + tbinput.size.Y;
            d /= 100;
            opacity = 0f;

            for (int i = 0; i < 100; i++)
            {
                tbLog.position.Y += d;
                tbinput.position.Y += d;
                if (opacity < 0.5f) opacity += 0.01f;
                System.Threading.Thread.Sleep(1);
            }
            isfading = false;
        }

        public override void FadeOut()
        {
            isfading = true;
            var d = tbLog.size.Y + tbinput.size.Y;
            d /= 100;
            opacity = 0f;

            for (int i = 0; i < 100; i++)
            {
                tbLog.position.Y -= d;
                tbinput.position.Y -= d;
                if (opacity > 0f) opacity -= 0.01f;
                System.Threading.Thread.Sleep(1);
            }
            GUIEngine.RemoveHUDScene(this);
            isfading = false;
        }
        #endregion

        public void CheckLogRange()
        {
            if (logoffset < 1) logoffset = 1;
            if (logoffset > OutputEngine.LOG_LENGTH - 24)
            {
                logoffset = OutputEngine.LOG_LENGTH - 24;
            }
        }

        public void DoCommand(String s, bool output = true)
        {
            try
            {
                vm.Execute(s);
            }
            catch (Exception e)
            {
                OutputEngine.WriteLine(e.Message);
            }
        }

        #region IOBlock
        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            base.onButtonClick(e);
            e.Handled = true;
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.onMouseMove(e);
            e.Handled = true;
        }

        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            base.onButtonDown(e);
            e.Handled = true;
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            base.onButtonUp(e);
            e.Handled = true;
        }

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
            base.onKeyDown(e);
            e.Handled = true;
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            e.Handled = true;
            if (e.key == Keys.Enter.GetHashCode() && InputEngine.Control)
            {
                DoCommand(tbinput.Text);
                for (int i = InputHistory.Length - 1; i > 0; i--)
                {
                    InputHistory[i] = InputHistory[i - 1];
                }
                InputHistory[0] = tbinput.Text;
                tbinput.Text = "";
                CurInputHistorySelected = -1;
                logoffset = OutputEngine.LOG_LENGTH;
                CheckLogRange();
                updateLog();
                return;
            }
            if (e.key == Keys.Up.GetHashCode())
            {
                CurInputHistorySelected++;
                if (CurInputHistorySelected >= InputHistory.Length)
                    CurInputHistorySelected = InputHistory.Length - 1;

                if (InputHistory[CurInputHistorySelected] == null)
                    CurInputHistorySelected--;

                if (CurInputHistorySelected == -1)
                    tbinput.Text = "";
                else
                    tbinput.Text = InputHistory[CurInputHistorySelected];
                tbinput.Cursor = tbinput.Text.Length;
                return;
            }
            if (e.key == Keys.Down.GetHashCode())
            {
                CurInputHistorySelected--;
                if (CurInputHistorySelected < -1)
                    CurInputHistorySelected = -1;

                if (CurInputHistorySelected == -1)
                    tbinput.Text = "";
                else
                    tbinput.Text = InputHistory[CurInputHistorySelected];
                tbinput.Cursor = tbinput.Text.Length;
                return;
            }
            InputEngine.eventHandled = false;
            CurInputHistorySelected = -1;
            base.onKeyPressed(e);
            e.Handled = true;
        }

        public override void onKeyUp(InputEngine.KeyboardArgs e)
        {
            base.onKeyUp(e);
            e.Handled = true;
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            if (!e.Handled)
            {
                logoffset -= e.delta / 120 * 4;
                CheckLogRange();
                updateLog();
            }
            e.Handled = true;
        }
        #endregion

    }
}
