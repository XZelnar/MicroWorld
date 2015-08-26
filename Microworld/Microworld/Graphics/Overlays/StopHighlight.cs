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

namespace MicroWorld.Graphics.Overlays
{
    class StopHighlight : Overlay
    {
        internal static Texture2D texture;

        float opacity = 0f;
        bool t = true;
        public override void Update()
        {
            Position = new Vector2(Main.WindowWidth / 2 + 52 - 128, 22 - 128);
            Size = new Vector2(256, 256);
            base.Update();
            if (t)
            {
                opacity += 0.05f;
                if (opacity >= 0.6f)
                {
                    opacity = 0.6f;
                    t = false;
                }
            }
            else
            {
                opacity -= 0.05f;
                if (opacity <= 0f)
                {
                    opacity = 0f;
                    IsDead = true;
                }
            }
        }

        public override void Draw(Renderer r)
        {
            base.Draw(r);

            r.Draw(texture, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y),
                Color.White * opacity);
        }
    }
}
