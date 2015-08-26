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
    class LevelSelectorTabsButton : EncyclopediaBrowserButton
    {
        public bool IsSelected = false;

        public LevelSelectorTabsButton(int x, int y, int w, int h, String txt)
            : base(x, y, w, h, txt)
        {
        }

        public override void Draw(Renderer renderer)
        {
            if (!isVisible) return;
            if (texture == null) texture = textureGlobal;
            if (Font == null) Font = GUIEngine.font;

            if (IsSelected)
            {
                renderer.Draw(selectedbg, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                    Color.White * IdleOpacity);
                renderer.Draw(selectedbg, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), Color.White);
                Main.renderer.DrawString(Font, Text, new Rectangle((int)position.X,
                    (int)(position.Y + (size.Y - stringSize.Y) / 2), (int)size.X, (int)stringSize.Y), foreground * (isEnabled ? 1f : 0.4f),
                    textAlignment);
                return;
            }

            if (isEnabled)
            {
                renderer.Draw(selectedbg, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                    Color.White * IdleOpacity);
                if (tpsize.Y != 0)
                {
                    renderer.Draw(selectedbg, new Rectangle((int)tppos.X, (int)tppos.Y, (int)tpsize.X, (int)tpsize.Y), Color.White);
                }
            }
            else
            {
                renderer.Draw(selectedbg, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                    new Color(80, 80, 80) * IdleOpacity);
            }

            if (textOld != Text)
            {
                textOld = Text;
                stringSize = GUIEngine.font.MeasureString(Text);
            }

            Main.renderer.DrawString(Font, Text, new Rectangle((int)position.X,
                (int)(position.Y + (size.Y - stringSize.Y) / 2), (int)size.X, (int)stringSize.Y), foreground * (isEnabled ? 1f : 0.4f),
                textAlignment);
        }
    }
}
