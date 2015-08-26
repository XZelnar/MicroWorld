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
    public class ZoomBar : HUDScene
    {
        Texture2D bar, scroll;

        Vector2 position;

        public override void Initialize()
        {
            Layer = 550;
            ShouldBeScaled = false;
            GlobalEvents.onCameraScaled += new GlobalEvents.CameraEvent(GlobalEvents_onCameraScaled);
            base.Initialize();
        }

        void GlobalEvents_onCameraScaled(float scale, float oldscale, Vector2 deltaOffset)
        {
            if (!isVisible) return;
            if (IsIn(InputEngine.curMouse.X, InputEngine.curMouse.Y))
            {
                String s = (Math.Round(Graphics.GraphicsEngine.camera.Scale * 10) / 20f).ToString() + ")";
                s = "X " + s;
                Shortcuts.SetInGameStatus("Grid zoom (" + s, "<Ctrl> + Mouse Wheel");
            }
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            position.X = w - 196;
            position.Y = h - 29;

            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void LoadContent()
        {
            bar = ResourceManager.Load<Texture2D>("GUI/HUD/ZoomBar/Bar");
            scroll = ResourceManager.Load<Texture2D>("GUI/HUD/ZoomBar/Scroll");

            base.LoadContent();
        }

        public override void Draw(Renderer renderer)
        {
            renderer.Draw(bar, position, Color.White);
            float scrollx = (196 - 17) * (GraphicsEngine.camera.Scale - Camera.ZOOM_MIN) / (Camera.ZOOM_MAX - Camera.ZOOM_MIN);
            renderer.Draw(scroll, new Vector2(position.X + scrollx, Main.WindowHeight - 23), Color.White);
        }

        public override bool IsIn(int x, int y)
        {
            return x >= position.X && x < position.X + 196
                && y >= position.Y && y < position.Y + 29;
        }

        public override Vector2 GetPosition()
        {
            return position;
        }

        public override Vector2 GetSize()
        {
            return new Vector2(196, 29);
        }

        #region io
        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y))
            {
                ChangeScale(e.curState.X);
                e.Handled = true;
            }
        }

        bool isdnd = false;
        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y) && e.button == 0)
            {
                isdnd = true;
                ChangeScale(e.curState.X);
                e.Handled = true;
            }
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            if (e.button == 0 && isdnd)
            {
                isdnd = false;
                e.Handled = true;
            }
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            if (isdnd)
                ChangeScale(e.curState.X);
            if (IsIn(e.curState.X, e.curState.Y))
            {
                String s = (Math.Round(Graphics.GraphicsEngine.camera.Scale * 10) / 10f).ToString() + ")";
                s = "X " + s;
                float scrollx = (196 - 17) * (GraphicsEngine.camera.Scale - Camera.ZOOM_MIN) / (Camera.ZOOM_MAX - Camera.ZOOM_MIN);
                Shortcuts.SetInGameStatus("Grid zoom (" + s, "<Ctrl> + Mouse Wheel");
                if (GUIEngine.s_toolTip.isVisible)
                    Shortcuts.ShowToolTipNoAnimation(position + new Vector2(scrollx, -1), "Zoom (" + s + ")", ArrowLineDirection.LeftUp);
                else
                    if (!GUIEngine.s_toolTip.Text.StartsWith("Zoom ("))
                        Shortcuts.ShowToolTip(position + new Vector2(scrollx, -1), "Zoom (" + s + ")", ArrowLineDirection.LeftUp);
            }
            else
            {
                if (GUIEngine.s_toolTip.isVisible &&
                    GUIEngine.s_toolTip.Text.StartsWith("Zoom ("))
                {
                    GUIEngine.s_toolTip.Close();
                }
            }
        }

        private void ChangeScale(int x)
        {
            x -= (int)position.X + 17 - 9;
            x = x < 0 ? 0 : x > 179 ? 179 : x;
            float s = (float)x / 179 * (Camera.ZOOM_MAX - Camera.ZOOM_MIN) + Camera.ZOOM_MIN;
            s = (float)(Math.Round(s * 10)) / 10f;
            GraphicsEngine.camera.Scale = s;
        }
        #endregion
    }
}
