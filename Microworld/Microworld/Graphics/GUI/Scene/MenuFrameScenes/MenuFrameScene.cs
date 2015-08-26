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
    public class MenuFrameScene : HUDScene
    {
        internal static SpriteFont buttonFont;
        public static SpriteFont ButtonFont
        {
            get { return MenuFrameScene.buttonFont; }
        }

        public enum FrameButtonsCount
        {
            Zero = 0,
            One = 1,
            Two = 2
        }

        public FrameButtonsCount ButtonsCount = FrameButtonsCount.Zero;
        protected bool ShortUpperLine = false;

        public Vector2 Position
        {
            get { return new Vector2(515 * Main.WindowWidth / 1920 + 1, 126 * Main.WindowHeight / 1080 + 1); }
        }

        public Vector2 Size
        {
            get { return new Vector2(1262 * Main.WindowWidth / 1920, 862 * Main.WindowHeight / 1080); }
        }

        public Vector2 GetPosForWH(int w, int h)
        {
            return new Vector2(515 * w / 1920 + 1, 126 * h / 1080 + 1);
        }

        public Vector2 GetSizeForWH(int w, int h)
        {
            return new Vector2(1262 * w / 1920, 862 * h / 1080);
        }

        public override void Draw(Renderer renderer)
        {
            base.Draw(renderer);

            renderer.SetScissorRectangle(GUIEngine.s_mainMenu.line3p1.X, GUIEngine.s_mainMenu.line3p1.Y, Main.WindowWidth, Main.WindowHeight, false);
            renderer.End();
            renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, null,
                null, Graphics.GraphicsEngine.s_ScissorsOn, null, Matrix.CreateTranslation(MainMenu.FrameOffset, 0, 0));

            Vector2 corner = Position + Size;
            renderer.Draw(GraphicsEngine.pixel,
                new Rectangle((int)(corner.X - 333), (int)(corner.Y - 1),
                    333, 1), Color.White);
            renderer.Draw(GraphicsEngine.pixel,
                new Rectangle((int)(corner.X - 1), (int)(corner.Y - (ShortUpperLine ? 29 : 96)),
                    1, (int)((ShortUpperLine ? 29 : 96) - (ButtonsCount == FrameButtonsCount.Zero ? 0 : 23))), Color.White);

            if (ButtonsCount != FrameButtonsCount.Zero)
            {
                renderer.Draw(GraphicsEngine.pixel,
                    new Rectangle((int)(corner.X - 120), (int)(corner.Y - 23),
                        1, 23), Color.White);
                int linew = ButtonsCount == FrameButtonsCount.One ? 120 : 240;
                renderer.Draw(GraphicsEngine.pixel,
                    new Rectangle((int)(corner.X - linew), (int)(corner.Y - 24),
                        linew, 1), Color.White);
            }
        }

        public override bool IsIn(int x, int y)
        {
            return x >= Position.X && x <= Position.X + Size.X &&
                   y >= Position.Y && y <= Position.Y + Size.Y;
        }
    }
}
