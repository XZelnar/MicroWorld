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
    internal static class DebugInfo
    {
        public static int UpdateTime = 0;
        public static int MatrixRecalculateTime = 0;
        public static int ComponentUpdateTime = 0;
        public static int LiquidsUpdateTime = 0;
        public static int DrawTime = 0;
        public static int UpdatesPerSecond = 0;
        public static int FramesPerSecond = 0;
        public static long MemoryUsed = 0;

        public static bool IsVisible = false;

        public static void Draw()
        {
            if (Main.Ticks % 120 == 0)
                MemoryUsed = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024;

            if (!IsVisible) return;
            Main.renderer.End();
            Main.renderer.BeginUnscaled();

            String s =
                "Matrix recalculate time: " + MatrixRecalculateTime.ToString() + "ms\r\n" +
                "Component update time: " + ComponentUpdateTime.ToString() + "ms\r\n" +
                //"Liquids update time: " + LiquidsUpdateTime.ToString() + "ms\r\n" +
                "\r\n" +
                "Update time: " + UpdateTime.ToString() + "ms\r\n" +
                "Render time: " + DrawTime.ToString() + "ms\r\n \r\n" +
                "TPS: " + UpdatesPerSecond.ToString() + "\r\n" +
                "FPS: " + FramesPerSecond.ToString() + "\r\n \r\n" +
                "Memory usage: " + MemoryUsed.ToString() + "mb";

            Main.renderer.DrawStringRight(Graphics.GUI.GUIEngine.font, s,
                new Rectangle(0, -2, Main.graphics.PreferredBackBufferWidth - 2, Main.graphics.PreferredBackBufferHeight),
                Color.Black);
            Main.renderer.DrawStringRight(Graphics.GUI.GUIEngine.font, s,
                new Rectangle(0, 0, Main.graphics.PreferredBackBufferWidth, Main.graphics.PreferredBackBufferHeight),
                Color.White);
        }
    }
}
