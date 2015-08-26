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
using MicroWorld.Graphics.GUI.Elements;

namespace MicroWorld.Graphics.GUI.Scene
{
    public class VisibilityMapOverlay : HUDScene
    {
        public static bool DrawOverlay = false;

        public override void Initialize()
        {
            ShouldBeScaled = true;
            Layer = 10;
            base.Initialize();
        }

        public override void Draw(Renderer renderer)
        {
            if (!DrawOverlay) return;
            if (!Main.curState.StartsWith("GAME")) return;
            int Step = GridDraw.Step;
            Vector2 offset;

            int HighlightLineDivider = Step * 5;

            int w = (int)(Main.graphics.PreferredBackBufferWidth / Settings.GameScale) + Step;
            int h = (int)(Main.graphics.PreferredBackBufferHeight / Settings.GameScale) + Step;
            offset = -new Vector2((int)(Settings.GameOffset.X / Step) * Step, (int)(Settings.GameOffset.Y / Step) * Step);

            int t = 0;
            for (int x = (int)offset.X; x < offset.X + w; x += Step)
            {
                for (int y = (int)offset.Y; y < offset.Y + h; y += Step)
                {
                    t = Components.ComponentsManager.VisibilityMap.GetAStarValue(x, y);
                    renderer.Draw(GraphicsEngine.pixel, new Rectangle(x - 2, y - 2, 4, 4), 
                        (t == 0 ? Color.Red : t == Components.VisibilityMap.WIRE ? Color.Violet : Color.Green) * 0.4f);
                }
            }
        }
    }
}
