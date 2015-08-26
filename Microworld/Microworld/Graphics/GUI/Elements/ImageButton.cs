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
    public class ImageButton : Button
    {
        Texture2D image, imageover, imagepressed, bg;
        public Color pressedColor = Color.White;
        public Color overrideColor = Color.Black;
        public bool drawbg = false;

        public ImageButton(int x, int y, int w, int h, String txt) : base(x, y, w, h, txt) { }
        public ImageButton(int x, int y, int w, int h) : base(x, y, w, h, "") { }

        public void LoadImages(String img)
        {
            image = ResourceManager.Load<Texture2D>(img);

            bg = ResourceManager.Load<Texture2D>("GUI/Menus/ButtonBackground");
        }

        public void LoadImages(String img, String imgover, String imgpressed)
        {
            image = ResourceManager.Load<Texture2D>(img);
            imageover = ResourceManager.Load<Texture2D>(imgover);
            imagepressed = ResourceManager.Load<Texture2D>(imgpressed);

            bg = ResourceManager.Load<Texture2D>("GUI/Menus/ButtonBackground");
        }

        public override void Draw(Renderer renderer)
        {
            if (!isVisible) return;
            if (image == null)
            {
                base.Draw(renderer);
                return;
            }

            if (overrideColor != Color.Black)
            {
                renderer.Draw(image, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                    overrideColor);
                return;
            }

            if (drawbg && bg != null)
            {
                renderer.Draw(bg, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                    Color.White);
            }

            if (!isEnabled)
            {
                renderer.Draw(image, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                    new Color(0.4f, 0.4f, 0.4f));
                return;
            }

            if (isPressed)
            {
                if (imagepressed != null)
                {
                    renderer.Draw(imagepressed, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                        pressedColor);
                }
                else
                {
                    renderer.Draw(image, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                        pressedColor);
                }
            }
            else if (isMouseOver)
            {
                if (imageover != null)
                {
                    renderer.Draw(imageover, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                        Color.White);
                }
                else
                {
                    renderer.Draw(image, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                        new Color(0.9f, 0.9f, 0.9f));
                }
            }
            else
            {
                if (imageover != null)
                {
                    renderer.Draw(image, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                        new Color(1f, 1f, 1f));
                }
                else
                {
                    renderer.Draw(image, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                        new Color(0.8f, 0.8f, 0.8f));
                }
            }
        }


    }
}
