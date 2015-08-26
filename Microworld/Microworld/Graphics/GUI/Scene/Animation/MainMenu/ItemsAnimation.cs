using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Graphics.GUI.Scene.Animation.MainMenu
{
    class ItemsAnimation : SceneAnimation
    {
        List<Elements.MenuButton> buttons = new List<Elements.MenuButton>();

        public override void Init(Scene s)
        {
            var a = s as GUI.Scene.MainMenu;
            a.CaptionPosition = new Microsoft.Xna.Framework.Vector2();

            buttons.Clear();
            for (int i = 0; i < a.controls.Count; i++)
            {
                if (a.controls[i] is Elements.MenuButton)
                {
                    a.controls[i].isVisible = false;
                    buttons.Add(a.controls[i] as Elements.MenuButton);
                }
            }

            base.Init(s);
        }

        protected override void fadeIn()
        {
            while (Main.curState == "GUIGlobalLoad")
                System.Threading.Thread.Sleep(2);

            var a = Scene as GUI.Scene.MainMenu;
            int x = 126 * Main.WindowWidth / 1920 + 1;
            int y = 126 * Main.WindowHeight / 1080;
            a.CaptionPosition.X = x;
            a.CaptionPosition.Y = y - 100;
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].position.X = x - buttons[i].Size.X;
                buttons[i].isVisible = true;
            }

            ticks = 0;
            while (buttons.Count != 0 && buttons[buttons.Count - 1].position.X < x)
            {
                if (ForceStop)
                {
                    ForceStop = false;
                    return;
                }
                for (int i = 0; i < ticks / 20 && i < buttons.Count; i++)
                {
                    if (ticks == (i + 1) * 20)
                        Sound.SoundPlayer.ItemFadeIn();
                    if (buttons[i].position.X < x)
                        buttons[i].position.X += buttons[i].size.X / 40f;
                }

                if (a.CaptionPosition.Y < y)
                {
                    a.CaptionPosition.Y += 2;
                }

                ticks++;
                System.Threading.Thread.Sleep(5);
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].position.X = x;
            }

            base.fadeIn();
        }

        protected override void fadeOut()
        {
            var a = Scene as GUI.Scene.MainMenu;
            int x = 126 * Main.WindowWidth / 1920 + 1;
            int y = 126 * Main.WindowHeight / 1080;

            ticks = 0;
            while (buttons.Count != 0 && buttons[buttons.Count - 1].position.X > x - buttons[buttons.Count - 1].Size.X)
            {
                if (ForceStop)
                {
                    ForceStop = false;
                    return;
                }
                for (int i = 0; i < ticks / 20 && i < buttons.Count; i++)
                {
                    if (ticks == (i + 1) * 20)
                        Sound.SoundPlayer.ItemFadeOut();
                    if (buttons[i].position.X > x - buttons[i].Size.X)
                        buttons[i].position.X -= buttons[i].size.X / 40f;
                }

                if (a.CaptionPosition.Y > y - 100)
                {
                    a.CaptionPosition.Y -= 2;
                }

                ticks++;
                System.Threading.Thread.Sleep(5);
            }

            base.fadeOut();
        }
    }
}
