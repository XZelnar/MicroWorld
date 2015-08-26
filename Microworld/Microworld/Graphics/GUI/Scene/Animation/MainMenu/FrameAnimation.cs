using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Graphics.GUI.Scene.Animation.MainMenu
{
    class FrameAnimation : SceneAnimation
    {
        public override void Init(Scene s)
        {
            base.Init(s);
        }

        protected override void fadeIn()
        {
            while (Main.curState == "GUIGlobalLoad")
                System.Threading.Thread.Sleep(2);

            var a = Scene as GUI.Scene.MainMenu;
            if (a == null || a.currentFrame == null)
            {
                base.fadeIn();
                return;
            }
            GUI.Scene.MainMenu.FrameOffset = -a.currentFrame.Size.X;
            a.currentFrame.isVisible = true;

            Microsoft.Xna.Framework.Vector2 lp2 = new Microsoft.Xna.Framework.Vector2(501f * Main.WindowWidth / 1920f, 126 * Main.WindowHeight / 1080);

            a.line3p1 = lp2;
            a.line3p2 = lp2;
            a.line4p1 = lp2;
            a.line4p2 = lp2;

            for (int i = 0; i <= 30; i++)
            {
                if (ForceStop)
                {
                    ForceStop = false;
                    return;
                }
                a.line3p2.X = a.line3p1.X + 1263f * Main.WindowWidth / 1920f * i / 30f;
                a.line4p2.Y = a.line4p1.Y + 863 * Main.WindowHeight / 1080 * i / 30f;
                GUI.Scene.MainMenu.FrameOffset = -a.currentFrame.Size.X * (float)(30 - i) / 30f;
                System.Threading.Thread.Sleep(10);
            }

            base.fadeIn();
        }

        protected override void fadeOut()
        {
            var a = Scene as GUI.Scene.MainMenu;
            if (a == null || a.currentFrame == null)
            {
                base.fadeOut();
                return;
            }

            for (int i = 29; i >= 0; i--)
            {
                if (ForceStop)
                {
                    ForceStop = false;
                    return;
                }
                a.line3p2.X = a.line3p1.X + 1263f * Main.WindowWidth / 1920f * i / 30f;
                a.line4p2.Y = a.line4p1.Y + 863 * Main.WindowHeight / 1080 * i / 30f;
                GUI.Scene.MainMenu.FrameOffset = -a.currentFrame.Size.X * (float)(30 - i) / 30f;
                System.Threading.Thread.Sleep(10);
            }

            base.fadeOut();
        }
    }
}
