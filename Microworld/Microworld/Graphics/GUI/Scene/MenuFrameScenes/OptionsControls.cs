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
    class OptionsControls : MenuFrameScene
    {
        Elements.HotkeyControl hcSimStart;
        Elements.HotkeyControl hcSimStop;
        Elements.HotkeyControl hcSimPause;
        Elements.HotkeyControl hcUndo;
        Elements.HotkeyControl hcCompRem;
        Elements.HotkeyControl hcEraser;
        Elements.HotkeyControl hcZoomIn;
        Elements.HotkeyControl hcZoomOut;
        Elements.HotkeyControl hcDragScene;
        Elements.HotkeyControl hcGhostRotateCW;
        Elements.HotkeyControl hcGhostRotateCCW;

        public override void Initialize()
        {
            int offset = 350;
            #region Hotkeys
            hcSimStart = new Elements.HotkeyControl((int)Position.X + 5, (int)Position.Y + 5, offset, "Simulation start", Settings.k_SimulationStart);
            controls.Add(hcSimStart);
            hcSimStop = new Elements.HotkeyControl((int)Position.X + 5, (int)Position.Y + 30, offset, "Simulation stop", Settings.k_SimulationStop);
            controls.Add(hcSimStop);
            hcSimPause = new Elements.HotkeyControl((int)Position.X + 5, (int)Position.Y + 55, offset, "Simulation pause", Settings.k_SimulationPause);
            controls.Add(hcSimPause);
            hcUndo = new Elements.HotkeyControl((int)Position.X + 5, (int)Position.Y + 80, offset, "Undo", Settings.k_Undo);
            controls.Add(hcUndo);
            hcCompRem = new Elements.HotkeyControl((int)Position.X + 5, (int)Position.Y + 105, offset, "Remove single component", Settings.k_ComponentRemove);
            controls.Add(hcCompRem);
            hcEraser = new Elements.HotkeyControl((int)Position.X + 5, (int)Position.Y + 130, offset, "Eraser", Settings.k_Eraser);
            controls.Add(hcEraser);
            hcZoomIn = new Elements.HotkeyControl((int)Position.X + 5, (int)Position.Y + 155, offset, "Zoom In", Settings.k_ZoomIn);
            controls.Add(hcZoomIn);
            hcZoomOut = new Elements.HotkeyControl((int)Position.X + 5, (int)Position.Y + 180, offset, "Zoom Out", Settings.k_ZoomOut);
            controls.Add(hcZoomOut);
            hcDragScene = new Elements.HotkeyControl((int)Position.X + 5, (int)Position.Y + 205, offset, "Drag Scene", Settings.k_DragScene);
            controls.Add(hcDragScene);
            hcGhostRotateCW = new Elements.HotkeyControl((int)Position.X + 5, (int)Position.Y + 230, offset, "Rotate component clockwise", Settings.k_ComponentRotateCW);
            controls.Add(hcGhostRotateCW);
            hcGhostRotateCCW = new Elements.HotkeyControl((int)Position.X + 5, (int)Position.Y + 255, offset, "Rotate component counterclockwise", Settings.k_ComponentRotateCCW);
            controls.Add(hcGhostRotateCCW);
            #endregion

            base.Initialize();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            hcSimStart.Position = new Vector2((int)GetPosForWH(w, h).X + 5, (int)GetPosForWH(w, h).Y + 5);
            hcSimStop.Position = new Vector2((int)GetPosForWH(w, h).X + 5, (int)GetPosForWH(w, h).Y + 30);
            hcSimPause.Position = new Vector2((int)GetPosForWH(w, h).X + 5, (int)GetPosForWH(w, h).Y + 55);
            hcUndo.Position = new Vector2((int)GetPosForWH(w, h).X + 5, (int)GetPosForWH(w, h).Y + 80);
            hcCompRem.Position = new Vector2((int)GetPosForWH(w, h).X + 5, (int)GetPosForWH(w, h).Y + 105);
            hcEraser.Position = new Vector2((int)GetPosForWH(w, h).X + 5, (int)GetPosForWH(w, h).Y + 130);
            hcZoomIn.Position = new Vector2((int)GetPosForWH(w, h).X + 5, (int)GetPosForWH(w, h).Y + 155);
            hcZoomOut.Position = new Vector2((int)GetPosForWH(w, h).X + 5, (int)GetPosForWH(w, h).Y + 180);
            hcDragScene.Position = new Vector2((int)GetPosForWH(w, h).X + 5, (int)GetPosForWH(w, h).Y + 205);
            hcGhostRotateCW.Position = new Vector2((int)GetPosForWH(w, h).X + 5, (int)GetPosForWH(w, h).Y + 230);
            hcGhostRotateCCW.Position = new Vector2((int)GetPosForWH(w, h).X + 5, (int)GetPosForWH(w, h).Y + 255);

            hcSimStart.Size = new Vector2(GetSizeForWH(w, h).X - 10, 20);
            hcSimStop.Size = hcSimStart.Size;
            hcSimPause.Size = hcSimStart.Size;
            hcUndo.Size = hcSimStart.Size;
            hcCompRem.Size = hcSimStart.Size;
            hcEraser.Size = hcSimStart.Size;
            hcZoomIn.Size = hcSimStart.Size;
            hcZoomOut.Size = hcSimStart.Size;
            hcDragScene.Size = hcSimStart.Size;
            hcGhostRotateCW.Size = hcSimStart.Size;
            hcGhostRotateCCW.Size = hcSimStart.Size;

            

            base.OnResolutionChanged(w, h, oldw, oldh);
        }
    }
}
