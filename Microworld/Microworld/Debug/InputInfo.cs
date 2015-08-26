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

namespace MicroWorld.Debug
{
    internal static class InputInfo
    {
        public static bool IsVisible = false;

        public static void Draw()
        {
            if (!IsVisible) return;
            Main.renderer.End();
            Main.renderer.BeginUnscaled();

            String s =
                "Cursor position: (" + InputEngine.curMouse.X.ToString() + ";" + InputEngine.curMouse.Y.ToString() + ")\r\n" +
                "Grid cursor position: (" + Graphics.GUI.GridDraw.CursorX.ToString() + ";" + Graphics.GUI.GridDraw.CursorY.ToString() + ")\r\n" +
                " \r\n" +
                "Camera cenetr: (" + Shortcuts.camera.Center.X.ToString() + ";" + Shortcuts.camera.Center.Y.ToString() + ")\r\n" +
                " \r\n" + 
                "WasWirePathFound: " + Logics.PathFinding.PathFinder.WasPathFound.ToString() + "\r\n" + 
                "LastSearch: " + (new TimeSpan(DateTime.Now.Ticks) - Logics.PathFinding.PathFinder.LastSearch).ToString() + "\r\n" + 
                " \r\n" + 
                "Components: " + Components.ComponentsManager.Components.Count.ToString() + "\r\n" +
                "MouseOver component ID: " + (Logics.GameInputHandler.MouseOverComponent == null ? "null" : Logics.GameInputHandler.MouseOverComponent.ID.ToString());

            Main.renderer.DrawStringRight(Graphics.GUI.GUIEngine.font, s,
                new Rectangle(0, -2, Main.graphics.PreferredBackBufferWidth - 2, Main.graphics.PreferredBackBufferHeight),
                Color.Black);
            Main.renderer.DrawStringRight(Graphics.GUI.GUIEngine.font, s,
                new Rectangle(0, 0, Main.graphics.PreferredBackBufferWidth, Main.graphics.PreferredBackBufferHeight),
                Color.White);
        }
    }
}
