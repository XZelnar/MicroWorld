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
    public class NewStyleButton : Button
    {
        static Texture2D overPattern;
        Texture2D image;
        public bool IsActivated = false;

        public NewStyleButton(int x, int y, int w, int h, String txt) : base(x, y, w, h, txt) { }
        public NewStyleButton(int x, int y, int w, int h) : base(x, y, w, h, "") { }

        public void LoadImages(String img)
        {
            image = ResourceManager.Load<Texture2D>(img);

            if (overPattern == null)
                overPattern = ResourceManager.Load<Texture2D>("GUI/HUD/HighlightPattern");
        }

        float HighlightForce = 0f;
        public override void Update()
        {
            if (isMouseOver || IsActivated)
            {
                if (HighlightForce < 1)
                {
                    HighlightForce += 0.05f;
                }
            }
            else if (!isMouseOver && !IsActivated)
            {
                if (HighlightForce > 0)
                {
                    HighlightForce -= 0.05f;
                }
            }
            base.Update();
        }

        public override void Draw(Renderer renderer)
        {
            if (!isVisible) return;
            if (image == null)
            {
                base.Draw(renderer);
                return;
            }

            renderer.Draw(image, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                Color.White);

            if (!isEnabled)
            {
                renderer.Draw(GraphicsEngine.pixel, 
                    new Rectangle((int)position.X + 2, (int)position.Y + 2, (int)size.X - 4, (int)size.Y - 4),
                    Shortcuts.BG_COLOR * 0.5f);
            }

            renderer.End();
            renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap,
                DepthStencilState.Default, RasterizerState.CullNone);
            renderer.Draw(overPattern,
                new Rectangle((int)position.X + 2, (int)position.Y + 2, (int)size.X - 4, (int)size.Y - 4),
                new Rectangle((int)position.X + 2, (int)position.Y + 2, (int)size.X - 4, (int)size.Y - 4),
                Color.White * HighlightForce);
            renderer.End();
            renderer.BeginUnscaled();
        }


    }
}
