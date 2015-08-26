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
    class CircuitRunningControl : HUDScene
    {
        Elements.NewStyleButton strt;
        Elements.NewStyleButton stp;
        Elements.NewStyleButton ps;
        Texture2D bg;
        int bgx = 0;
        public new void Initialize()
        {
            ShouldBeScaled = false;
            Layer = 500;

            strt = new Elements.NewStyleButton(620, 4, 52, 35, "Start");
            strt.LoadImages("GUI/HUD/RunningControls/Start");
            strt.onClicked += new Elements.Button.ClickedEventHandler(strtClick);
            controls.Add(strt);

            stp = new Elements.NewStyleButton(680, 4, 52, 35, "Stop");
            stp.LoadImages("GUI/HUD/RunningControls/Stop");
            stp.onClicked += new Elements.Button.ClickedEventHandler(stpClick);
            controls.Add(stp);
            stp.isEnabled = false;

            ps = new Elements.NewStyleButton(740, 4, 52, 35, "Pause");
            ps.LoadImages("GUI/HUD/RunningControls/Pause");
            ps.onClicked += new Elements.Button.ClickedEventHandler(psClick);
            controls.Add(ps);
            //ps.isEnabled = false;

            base.Initialize();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);
            strt.position.X = w / 2 - 52 - 26;
            ps.position.X = w / 2 - 26;
            stp.position.X = w / 2 + 26;
            bgx = w / 2 - 52 - 26 - 4;
        }

        public void strtClick(object sender, InputEngine.MouseArgs e)
        {
            if (!strt.isEnabled) return;
            Sound.SoundPlayer.PlayButtonClick();
            Logics.GameLogicsHelper.GameStart();
        }

        public void stpClick(object sender, InputEngine.MouseArgs e)
        {
            if (!stp.isEnabled) return;
            Sound.SoundPlayer.PlayButtonClick();
            Logics.GameLogicsHelper.GameStop();
        }

        public void psClick(object sender, InputEngine.MouseArgs e)
        {
            if (!ps.isEnabled) return;
            Sound.SoundPlayer.PlayButtonClick();
            Logics.GameLogicsHelper.GamePause();
        }


        public override void LoadContent()
        {
            bg = ResourceManager.Load<Texture2D>("GUI/HUD/RunningControls/bg");

            base.LoadContent();
        }

        public override void Update()
        {
            ps.isEnabled = (Settings.gameState == Settings.GameStates.Paused ? MicroWorld.Logics.GameLogicsHelper.CanStep : MicroWorld.Logics.GameLogicsHelper.CanPause);
            strt.isEnabled = (Settings.GameState != Settings.GameStates.Running) && MicroWorld.Logics.GameLogicsHelper.CanStart;
            stp.isEnabled = (Settings.GameState != Settings.GameStates.Stopped) && MicroWorld.Logics.GameLogicsHelper.CanStop;

            strt.IsActivated = (Settings.GameState != Settings.GameStates.Stopped);
            ps.IsActivated = (Settings.GameState == Settings.GameStates.Paused);
            base.Update();
        }

        public override void Draw(Renderer renderer)
        {
            RenderHelper.SmartDrawRectangle(bg, 5, bgx, 0, 164, 43, Color.White, renderer);

            base.Draw(renderer);
        }



        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.onMouseMove(e);
            CheckSetToolTip(e.curState.X, e.curState.Y);
        }

        public void CheckSetToolTip(int x, int y)
        {
            if (strt.IsIn(x, y))
            {
                Shortcuts.SetInGameStatus("Start simulation", "<F5>");
                if (!GUIEngine.s_toolTip.isVisible || GUIEngine.s_toolTip.Text != "Start simulation")
                    Shortcuts.ShowToolTip(strt.position + new Vector2(strt.size.X / 2, strt.size.Y), "Start simulation",
                        ArrowLineDirection.RightDown);
            }
            else if (stp.IsIn(x, y))
            {
                Shortcuts.SetInGameStatus("Stop simulation", "<F6>");
                if (!GUIEngine.s_toolTip.isVisible || GUIEngine.s_toolTip.Text != "Stop simulation")
                    Shortcuts.ShowToolTip(stp.position + new Vector2(stp.size.X / 2, stp.size.Y), "Stop simulation",
                        ArrowLineDirection.RightDown);
            }
            else if (ps.IsIn(x, y))
            {
                if (ps.Text == "Pause")
                {
                    Shortcuts.SetInGameStatus("Pause simulation", "<F7>");
                    if (!GUIEngine.s_toolTip.isVisible || GUIEngine.s_toolTip.Text != "Pause simulation")
                        Shortcuts.ShowToolTip(ps.position + new Vector2(ps.size.X / 2, ps.size.Y), "Pause simulation",
                        ArrowLineDirection.RightDown);
                }
                else
                {
                    Shortcuts.SetInGameStatus("Simulation step", "<F7>");
                    if (!GUIEngine.s_toolTip.isVisible || GUIEngine.s_toolTip.Text != "Simulation step")
                        Shortcuts.ShowToolTip(ps.position + new Vector2(ps.size.X / 2, ps.size.Y), "Simulation step",
                        ArrowLineDirection.RightDown);
                }
            }
            else
            {
                if (GUIEngine.s_toolTip.isVisible && (
                    GUIEngine.s_toolTip.Text == "Simulation step" ||
                    GUIEngine.s_toolTip.Text == "Pause simulation" ||
                    GUIEngine.s_toolTip.Text == "Stop simulation" ||
                    GUIEngine.s_toolTip.Text == "Start simulation"))
                {
                    GUIEngine.s_toolTip.Close();
                }
            }
        }


    }
}
