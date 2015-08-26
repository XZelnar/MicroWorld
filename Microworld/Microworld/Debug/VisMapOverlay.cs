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
    internal static class VisMapOverlay
    {
        public static bool IsVisible = false;

        public static void Draw()
        {
            if (!IsVisible) return;
            if (!Main.CurState.StartsWith("GAME"))
                return;
            Main.renderer.End();
            Main.renderer.Begin();

            var a = Shortcuts.camera.VisibleRectangle;
            int x = (a.Left / 8) * 8;
            int y = (a.Top / 8) * 8;
            bool t;

            for (; x < a.Right; x += 8)
            {
                for (int yy = y; yy < a.Bottom; yy += 8)
                {
                    t = Components.ComponentsManager.VisibilityMap.IsFree(x + 2, yy + 2);
                    if (!t)
                        Shortcuts.renderer.Draw(Shortcuts.pixel, new Rectangle(x, yy, 8, 8), Color.Red * 0.4f);
                }
            }
        }
    }
}
