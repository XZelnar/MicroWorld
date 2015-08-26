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

namespace MicroWorld.Graphics.GUI.Elements
{
    public class LevelSelectorButton : ImageButton
    {
        public LevelSelectorButton(int x, int y, int w, int h, String txt) : base(x, y, w, h, txt) { }

        public override void Draw(Renderer renderer)
        {
            if (!isVisible) return;

            base.Draw(renderer);

            var a = Font.MeasureString(Text);
            int y = (int)(size.Y - a.Y) / 2;
            Main.renderer.DrawString(Font, Text, new Rectangle((int)position.X, (int)position.Y + y, (int)size.X, (int)a.Y), 
                Color.White, Renderer.TextAlignment.Center);
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            bool wasover = isMouseOver;
            base.onMouseMove(e);
            if (isMouseOver && !wasover)
            {
                Sound.SoundPlayer.MenuMouseOver();
            }
        }
    }
}
