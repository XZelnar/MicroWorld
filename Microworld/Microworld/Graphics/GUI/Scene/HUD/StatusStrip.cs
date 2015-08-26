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

namespace MicroWorld.Graphics.GUI.Scene
{
    public class StatusStrip : HUDScene
    {
        Texture2D bg;

        Vector2 position;
        Vector2 size;

        SpriteFont font;

        public String TextLeft = "", TextRight = "";

        public override void Initialize()
        {
            Layer = 800;
            ShouldBeScaled = false;
            base.Initialize();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            position.X = 43;
            position.Y = h - 20;
            size.X = w - 43 - 180;
            size.Y = 20;

            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void LoadContent()
        {
            bg = ResourceManager.Load<Texture2D>("GUI/BackgroundBlack");
            font = ResourceManager.Load<SpriteFont>("Fonts/CourierNew_14");

            base.LoadContent();
        }

        public override void Draw(Renderer renderer)
        {
            RenderHelper.SmartDrawRectangle(bg, 6, (int)position.X, (int)position.Y, (int)size.X, (int)size.Y, Color.White, renderer);
            renderer.DrawString(font, TextLeft, new Rectangle((int)position.X + 4, (int)position.Y, (int)size.X, (int)size.Y), Color.White,
                Renderer.TextAlignment.Left);
            renderer.DrawString(font, TextRight, new Rectangle((int)position.X + 4, (int)position.Y, (int)size.X - 7, (int)size.Y), Color.White,
                Renderer.TextAlignment.Right);
        }

        #region io
        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y))
            {
                e.Handled = true;
            }
        }
        #endregion
    }
}
