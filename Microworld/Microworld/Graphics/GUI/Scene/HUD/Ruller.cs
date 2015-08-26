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
    class Ruller : HUDScene
    {
        public new void Initialize()
        {
            ShouldBeScaled = false;
            Layer = 400;
            isVisible = true;
            
            base.Initialize();
        }

        static int xt, yt;
        static float tx = 0, ty = 0;
        static float MaxLineSize, Pixel, tml, st;
        public static Color ColorLines = new Color(1f, 1f, 1f);
        public override void Draw(Renderer renderer)
        {
            int Step = GridDraw.Step;
            List<VertexPositionColorTexture> t = new List<VertexPositionColorTexture>();
            Vector2 offset = -new Vector2((int)(Settings.GameOffset.X / Step) * Step, (int)(Settings.GameOffset.Y / Step) * Step);

            MaxLineSize = 80f * Main.WindowWidth / 1920f;
            Pixel = 2f;
            tml = 0;

            st = Step * Settings.GameScale;
            Vector2 RightBottom = GraphicsEngine.camera.BottomRight;
            Rectangle rect = GraphicsEngine.camera.VisibleRectangle;
            //xt
            if (RightBottom.X > 0)
                xt = -(int)((-RightBottom.X - Step) % (Step * 4));
            else
                xt = -(int)(RightBottom.X % (Step * 4));
            xt /= Step;
            if (RightBottom.X > 0)
            {
                xt -= 2;
                if (xt < 0)
                    xt += 4;
                if (xt == 3)
                    xt = 1;
                else if (xt == 1)
                    xt = 3;
            }
            if (RightBottom.X > 0) xt += 3;
            //yt
            if (RightBottom.Y > 0)
                yt = -(int)((-RightBottom.Y - Step) % (Step * 4));
            else
                yt = -(int)(RightBottom.Y % (Step * 4));
            yt /= Step;
            if (RightBottom.Y > 0)
            {
                yt -= 2;
                if (yt < 0)
                    yt += 4;
                if (yt == 3)
                    yt = 1;
                else if (yt == 1)
                    yt = 3;
            }
            if (RightBottom.Y > 0) yt += 3;

            tx = rect.X - rect.X % Step;
            ty = rect.Y - rect.Y % Step;
            Utilities.Tools.GameToScreenCoords(ref tx, ref ty);

            for (float x = 0; x + tx < Main.WindowWidth - 196; x += st)
            {
                if (xt % 2 == 0)
                    if (xt % 4 == 0)
                        tml = MaxLineSize;
                    else
                        tml = MaxLineSize / 2;
                else
                    tml = MaxLineSize / 4;
                xt++;
                t.Add(new VertexPositionColorTexture(new Vector3((int)(tx + x), (int)(Main.WindowHeight - Pixel), 0), ColorLines,
                    new Vector2()));
                t.Add(new VertexPositionColorTexture(new Vector3((int)(tx + x), (int)(Main.WindowHeight - Pixel - tml), 0), ColorLines,
                    new Vector2()));
            }
            t.Add(new VertexPositionColorTexture(new Vector3(Main.WindowWidth - 196, (int)(Main.WindowHeight - Pixel), 0), ColorLines,
                new Vector2()));
            t.Add(new VertexPositionColorTexture(new Vector3(Main.WindowWidth - 196, (int)(Main.WindowHeight - Pixel - MaxLineSize), 0), ColorLines,
                new Vector2()));

            for (float y = 0; y + ty < Main.WindowHeight - 69; y += st)
            {
                if (yt % 2 == 0)
                    if (yt % 4 == 0)
                        tml = MaxLineSize;
                    else
                        tml = MaxLineSize / 2;
                else
                    tml = MaxLineSize / 4;
                yt++;
                t.Add(new VertexPositionColorTexture(new Vector3((int)(Main.WindowWidth - Pixel), (int)(ty + y), 0), ColorLines,
                    new Vector2()));
                t.Add(new VertexPositionColorTexture(new Vector3((int)(Main.WindowWidth - Pixel - tml), (int)(ty + y), 0), ColorLines,
                    new Vector2()));
            }
            t.Add(new VertexPositionColorTexture(new Vector3((int)(Main.WindowWidth - Pixel), Main.WindowHeight - 69, 0), ColorLines,
                new Vector2()));
            t.Add(new VertexPositionColorTexture(new Vector3((int)(Main.WindowWidth - Pixel - MaxLineSize), Main.WindowHeight - 69, 0), ColorLines,
                new Vector2()));

            Main.renderer.Draw(Graphics.GraphicsEngine.pixel, new Rectangle(0, 0, 0, 0), Color.White);
            Main.renderer.DrawLinesList(t.ToArray());

            base.Draw(renderer);
        }
    }
}
