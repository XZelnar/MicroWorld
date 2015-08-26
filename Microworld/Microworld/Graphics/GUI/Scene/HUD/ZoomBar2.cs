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
    public class ZoomBar2 : HUDScene
    {
        Texture2D bar, scroll;
        SpriteFont font;

        Vector2 position;

        public override void Initialize()
        {
            Layer = 500;
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
            position.X = w - 40;
            position.Y = (h - 200) / 2;

            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void LoadContent()
        {
            bar = ResourceManager.Load<Texture2D>("GUI/HUD/ZoomBar/Bar");
            scroll = ResourceManager.Load<Texture2D>("GUI/HUD/ZoomBar/Scroll");
            font = ResourceManager.Load<SpriteFont>("Fonts/ZoomBarFont");

            base.LoadContent();
        }

        public override void Draw(Renderer renderer)
        {
            float opacity = 0;
            Vector2 p = new Vector2(InputEngine.curMouse.X, InputEngine.curMouse.Y);
            Rectangle r = new Rectangle((int)position.X, (int)position.Y, (int)40, (int)200);
            var d = Math.Abs(Utilities.Tools.DistancePointToRectangle(p, r));
            if (d > 100)
            {
                opacity = 0.6f;
            }
            else
            {
                opacity = (float)(1f - 0.4 * d / 100f);
            }
            //base.Draw(renderer);

            renderer.Draw(bar, position, Color.White * opacity);
            float scrolly = 200 - 
                ((200 - 24) * (GraphicsEngine.camera.Scale - Camera.ZOOM_MIN) / (Camera.ZOOM_MAX - Camera.ZOOM_MIN) + 20);
            renderer.Draw(scroll, new Vector2(Main.WindowWidth - 40, position.Y + scrolly), Color.White * opacity);
            String s = "X " + (Math.Round(Graphics.GraphicsEngine.camera.Scale * 10) / 20f).ToString();
            renderer.DrawStringCentered(font, s, new Rectangle(Main.WindowWidth - 40, (int)(position.Y + scrolly + 1), 40, 16),
                Color.Black * opacity);
        }

        public override bool IsIn(int x, int y)
        {
            return x >= position.X && x < position.X + 40
                && y >= position.Y && y < position.Y + 200;
        }

        public override Vector2 GetPosition()
        {
            return position;
        }

        public override Vector2 GetSize()
        {
            return new Vector2(40, 200);
        }

        #region io
        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y))
            {
                ChangeScale(e.curState.Y);
                e.Handled = true;
            }
        }

        bool isdnd = false;
        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y) && e.button == 0)
            {
                isdnd = true;
                ChangeScale(e.curState.Y);
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
                ChangeScale(e.curState.Y);
            if (IsIn(e.curState.X, e.curState.Y))
            {
                String s = (Math.Round(Graphics.GraphicsEngine.camera.Scale * 10) / 20f).ToString() + ")";
                s = "X " + s;
                float scrolly = 200 -
                    ((200 - 24) * (GraphicsEngine.camera.Scale - Camera.ZOOM_MIN) / (Camera.ZOOM_MAX - Camera.ZOOM_MIN) + 20);
                Shortcuts.SetInGameStatus("Grid zoom (" + s, "<Ctrl> + Mouse Wheel");
                if (GUIEngine.s_toolTip.isVisible)
                    Shortcuts.ShowToolTipNoAnimation(position + new Vector2(-1, scrolly), "Zoom (" + s + ")");
                else
                    if (!GUIEngine.s_toolTip.Text.StartsWith("Zoom ("))
                        Shortcuts.ShowToolTip(position + new Vector2(-1, scrolly), "Zoom (" + s + ")");
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

        private void ChangeScale(int y)
        {
            y -= (int)position.Y + 4 + 8;
            y = y < 0 ? 0 : y > 176 ? 176 : y;
            y = Math.Abs(y - 176);
            float s = (float)y / 176 * (Camera.ZOOM_MAX - Camera.ZOOM_MIN) + Camera.ZOOM_MIN;
            s = (float)(Math.Round(s * 10)) / 10f;
            GraphicsEngine.camera.Scale = s;
        }
        #endregion
    }
}
