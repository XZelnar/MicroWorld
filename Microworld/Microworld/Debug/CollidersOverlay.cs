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
    internal static class CollidersOverlay
    {
        public static bool IsVisible = false;

        public static void Draw()
        {
            if (!IsVisible) return;
            if (!Main.CurState.StartsWith("GAME"))
                return;
            Main.renderer.End();
            Main.renderer.Begin();

            Components.Colliders.AABB a;
            for (int i = 0; i < Components.ComponentsManager.CollidersManager.colliders.Count; i++)
            {
                a = Components.ComponentsManager.CollidersManager.colliders[i].GetAABB();
                Shortcuts.renderer.Draw(Shortcuts.pixel, new Rectangle((int)a.X1, (int)a.Y1, 1, (int)a.Size.Y), Color.Cyan);
                Shortcuts.renderer.Draw(Shortcuts.pixel, new Rectangle((int)a.X1, (int)a.Y1, (int)a.Size.X, 1), Color.Cyan);
                Shortcuts.renderer.Draw(Shortcuts.pixel, new Rectangle((int)a.X2 - 1, (int)a.Y1, 1, (int)a.Size.Y), Color.Cyan);
                Shortcuts.renderer.Draw(Shortcuts.pixel, new Rectangle((int)a.X1, (int)a.Y2 - 1, (int)a.Size.X, 1), Color.Cyan);
            }
        }
    }
}
