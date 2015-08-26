using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Graphics.GUI.Scene.Animation.MainMenu
{
    class GlobalAnimation : SceneAnimation
    {
        public override void Init(Scene s)
        {
            base.Init(s);
        }

        protected override void fadeIn()
        {
            var a = Scene as GUI.Scene.MainMenu;

            a.line1p1 = new Microsoft.Xna.Framework.Vector2(0, 126 * Main.WindowHeight / 1080);
            a.line1p2 = new Microsoft.Xna.Framework.Vector2(0, 126 * Main.WindowHeight / 1080);
            a.line2p1 = new Microsoft.Xna.Framework.Vector2(126 * Main.WindowWidth / 1920, 126 * Main.WindowHeight / 1080);
            a.line2p2 = new Microsoft.Xna.Framework.Vector2(126 * Main.WindowWidth / 1920, 126 * Main.WindowHeight / 1080);
            //horizontal
            for (int i = 0; i <= 30; i++)
            {
                a.line1p2.X = 501f * Main.WindowWidth / 1920f * i / 30f;
                System.Threading.Thread.Sleep(10);
            }
            //vertical
            for (int i = 0; i <= 30; i++)
            {
                a.line2p2.Y = a.line2p1.Y + 863 * Main.WindowHeight / 1080 * i / 30f;
                System.Threading.Thread.Sleep(10);
            }

            base.fadeIn();
        }

        protected override void fadeOut()
        {
            base.fadeOut();
        }
    }
}
