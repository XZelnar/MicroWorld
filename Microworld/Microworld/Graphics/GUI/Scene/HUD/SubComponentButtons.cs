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
using MicroWorld.Graphics.GUI.Elements;

namespace MicroWorld.Graphics.GUI.Scene
{
    public class SubComponentButtons : HUDScene
    {
        public Components.Component SelectedComponent;
        private Vector2 position = new Vector2(), Size = new Vector2(50, 20);
        public Vector2 Position
        {
            get { return position; }
            set
            {
                value += new Vector2(0, 5);
                position = value;
                UpdatePos();
            }
        }
        public bool AutoUpdatePosition = true;

        private bool? lastRemovable = null, lastProp = null;

        public Elements.ImageButton bRemove;
        public override void Initialize()
        {
            ShouldBeScaled = false;
            CanOffset = true;
            Layer = -10;

            bRemove = new Elements.ImageButton(15, 0, 20, 20, "Rm");
            bRemove.LoadImages("GUI/HUD/SubButtons/Remove");
            bRemove.drawbg = true;
            bRemove.onClicked += new Elements.Button.ClickedEventHandler(bRemove_onClicked);
            //controls.Add(bRemove);

            base.Initialize();

            GlobalEvents.onCameraScaled += new GlobalEvents.CameraEvent(GlobalEvents_onCameraScaled);
        }

        void GlobalEvents_onCameraScaled(float scale, float oldscale, Vector2 deltaOffset)
        {
            if (isVisible && SelectedComponent != null && SelectedComponent is Components.Wire)
            {
                float x = position.X + Size.X / 2;
                float y = position.Y + Size.Y / 2;
                Utilities.Tools.ScreenToGameCoords(ref x, ref y, oldscale, GraphicsEngine.camera.BottomRight - deltaOffset);
                Utilities.Tools.GameToScreenCoords(ref x, ref y, scale);
                position.X = x - Size.X / 2;
                position.Y = y - Size.Y / 2;
                UpdatePos();
            }
        }

        public void UpdatePos()
        {
            bRemove.position = Position + new Vector2(15, 0);
        }

        public override void onShow()
        {
            lastRemovable = null;
            lastProp = null;
            if (SelectedComponent.IsRemovable)
            {
                bRemove.isEnabled = true;
            }
            else
            {
                bRemove.isEnabled = false;
            }
            base.onShow();
        }

        public override void onClose()
        {
            base.onClose();
            AutoUpdatePosition = true;
            bRemove.isEnabled = true;
            //GUIEngine.RemoveHUDScene(GUIEngine.s_code);
        }

        public override void Update()
        {
            base.Update();

            if (SelectedComponent == null) return;
            lock (SelectedComponent)
            {
                if (AutoUpdatePosition)
                {
                    if (SelectedComponent is Components.Wire || SelectedComponent is Components.RotatableConnector)
                    {
                        //var a = (SelectedComponent.Graphics as Components.Graphics.WireGraphics).GetPosSubButtons();
                        //float x = a.X, y = a.Y;
                        //Utilities.Tools.GameToScreenCoords(ref x, ref y);
                        //a = new Vector2(x, y);
                        var a = new Vector2(InputEngine.curMouse.X, InputEngine.curMouse.Y);
                        Position = a + new Vector2(-Size.X / 2, 10);
                    }
                    else
                    {
                        Position = (SelectedComponent.Graphics.Position + Settings.GameOffset) * Settings.GameScale +
                            new Vector2((SelectedComponent.Graphics.Size.X * Settings.GameScale - Size.X) / 2,
                                SelectedComponent.Graphics.Size.Y * Settings.GameScale + 10);
                    }
                }
                if (lastRemovable.HasValue)
                {
                    if (Settings.GameState == Settings.GameStates.Stopped)
                    {
                        bRemove.isEnabled = lastRemovable.Value;
                        lastRemovable = null;
                        lastProp = null;
                    }
                }
                else
                {
                    if (Settings.GameState != Settings.GameStates.Stopped)
                    {
                        lastRemovable = bRemove.isEnabled;
                        bRemove.isEnabled = false;
                    }
                }
                //Position *= Settings.GameScale;
            }
        }

        void bRemove_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            if (sender != null)
            {
                InputEngine.blockClick = true;
            }
            isVisible = false;
            Graphics.GUI.GUIEngine.RemoveHUDScene(this);
            SelectedComponent.OnButtonClickedRemove();
            SelectedComponent = null;
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            if (e.key == Keys.Delete.GetHashCode() && bRemove.isEnabled)
            {
                bRemove_onClicked(null, null);
                e.Handled = true;
                return;
            }
            base.onKeyPressed(e);
        }

        public override bool IsIn(int x, int y)
        {
            x = (int)(Settings.GameScale * x);
            y = (int)(Settings.GameScale * y);
            return GUIEngine.ContainsHUDScene(this) && isVisible &&
                bRemove.IsIn(x - (int)Position.X, y - (int)Position.Y);
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.onMouseMove(e);
            if (bRemove.IsIn(e.curState.X, e.curState.Y))
                Shortcuts.SetInGameStatus("Remove component", "");
        }
    }
}
