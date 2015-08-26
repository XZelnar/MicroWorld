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
    class PlacableAreasCreator : HUDScene
    {
        internal static bool create = false, delete = false;

        Elements.NewStyleButton bnew;
        Elements.NewStyleButton bdel;
        Elements.NewStyleButton bclr;
        public new void Initialize()
        {
            ShouldBeScaled = false;
            Layer = 500;

            bnew = new Elements.NewStyleButton(680, 4 + 55, 52, 35, "Add");
            bnew.LoadImages("GUI/HUD/PlacableAreas/Add");
            bnew.onClicked += new Elements.Button.ClickedEventHandler(newClick);
            controls.Add(bnew);

            bdel = new Elements.NewStyleButton(720, 4 + 55, 52, 35, "Del");
            bdel.LoadImages("GUI/HUD/PlacableAreas/Remove");
            bdel.onClicked += new Elements.Button.ClickedEventHandler(delClick);
            controls.Add(bdel);

            bclr = new Elements.NewStyleButton(760, 4 + 55, 52, 35, "Clr");
            bclr.LoadImages("GUI/HUD/PlacableAreas/Clear");
            bclr.onClicked += new Elements.Button.ClickedEventHandler(clrClick);
            controls.Add(bclr);

            base.Initialize();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);
            bclr.position.X = w - bclr.size.X  - 4;
            bdel.position.X = w - bclr.size.X - bdel.size.X - 4;
            bnew.position.X = w - bdel.size.X - bclr.size.X - bnew.size.X - 4;
        }

        public void newClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.s_componentSelector.ResetSelection();
            create = true;
            delete = false;
        }

        public void delClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.s_componentSelector.ResetSelection();
            delete = true;
            create = false;
        }

        public void clrClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            Logics.PlacableAreasManager.Clear();
            delete = false;
            create = false;
        }

        public override void Draw(Renderer renderer)
        {
            bnew.IsActivated = create;
            bdel.IsActivated = delete;

            RenderHelper.SmartDrawRectangle(GraphicsEngine.bg, 5, Main.WindowWidth - 164, 55, 164, 43, Color.White, renderer);

            base.Draw(renderer);
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.onMouseMove(e);
            if (bnew.IsIn(e.curState.X, e.curState.Y))
            {
                Shortcuts.SetInGameStatus("Add placable area", "");
                if (!GUIEngine.s_toolTip.isVisible || GUIEngine.s_toolTip.Text != "Add placable area")
                    Shortcuts.ShowToolTip(bnew.position + new Vector2(bnew.size.X / 2, bnew.size.Y + 1), "Add placable area", 
                        ArrowLineDirection.LeftDown);
            }
            else if (bdel.IsIn(e.curState.X, e.curState.Y))
            {
                Shortcuts.SetInGameStatus("Remove placable area", "");
                if (!GUIEngine.s_toolTip.isVisible || GUIEngine.s_toolTip.Text != "Remove placable area")
                    Shortcuts.ShowToolTip(bdel.position + new Vector2(bdel.size.X / 2, bdel.size.Y + 1), "Remove placable area",
                        ArrowLineDirection.LeftDown);
            }
            else if (bclr.IsIn(e.curState.X, e.curState.Y))
            {
                Shortcuts.SetInGameStatus("Reset placable areas", "");
                if (!GUIEngine.s_toolTip.isVisible || GUIEngine.s_toolTip.Text != "Reset placable areas")
                    Shortcuts.ShowToolTip(bclr.position + new Vector2(bclr.size.X / 2, bclr.size.Y + 1), "Reset placable areas",
                        ArrowLineDirection.LeftDown);
            }
            else
            {
                if (GUIEngine.s_toolTip.isVisible && (
                    GUIEngine.s_toolTip.Text == "Add placable area" ||
                    GUIEngine.s_toolTip.Text == "Remove placable area" ||
                    GUIEngine.s_toolTip.Text == "Reset placable areas"))
                {
                    GUIEngine.s_toolTip.Close();
                }
            }
        }

    }
}
