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

namespace MicroWorld.Components.Graphics
{
    class BlockerGraphics : GraphicalComponent
    {
        public static Texture2D bg;

        public BlockerGraphics()
        {
            base.Size = new Vector2(0, 0);
            Layer = -1;
        }

        public static void LoadContentStatic()
        {
            bg = ResourceManager.Load<Texture2D>("Components/Blocker/Blocker0cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/Blocker";
        }

        public override string GetCSToolTip()
        {
            return "Blocker";
        }

        public override string GetComponentSelectorPath()
        {
            return "Motors";
        }

        public override string GetHandbookFile()
        {
            return "Components/Blocker.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            return new Vector2();
        }

        public override Vector2 GetSize()
        {
            return Size;
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            return Size;
        }

        public override Component.Rotation[] GetPossibleRotations()
        {
            return new Component.Rotation[] { Component.Rotation.cw0 };
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (!CanDraw()) return;
            var p = parent as Blocker;

            if (p.fbo != null)
                renderer.Draw(p.fbo, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), Color.White);
            //DrawBlocker((int)Position.X, (int)Position.Y, (int)(Position.X + Size.X), (int)(Position.Y + Size.Y), renderer, 1f, true);
        }

        public void DrawToFBO(MicroWorld.Graphics.Renderer renderer)
        {
            var p = parent as Blocker;

            renderer.EnableFBO(p.fbo);
            renderer.GraphicsDevice.Clear(Color.Transparent);
            renderer.BeginUnscaled();

            renderer.Draw(bg, new Rectangle(0, 0, (int)Size.X * 4, (int)Size.Y * 4), new Rectangle(0, 0, (int)Size.X * 4, (int)Size.Y * 4), Color.White);
            renderer.Draw(Shortcuts.pixel, new Rectangle(0, 0, 2, (int)Size.Y * 4), Color.White);
            renderer.Draw(Shortcuts.pixel, new Rectangle(0, 0, (int)Size.X * 4, 2), Color.White);
            renderer.Draw(Shortcuts.pixel, new Rectangle((int)Size.X * 4 - 2, 0, 2, (int)Size.Y * 4), Color.White);
            renderer.Draw(Shortcuts.pixel, new Rectangle(0, (int)Size.Y * 4 - 2, (int)Size.X * 4, 2), Color.White);

            renderer.End();
            renderer.DisableFBO();
        }

        public void DrawBlocker(int x1, int y1, int x2, int y2, MicroWorld.Graphics.Renderer renderer, float opacity, bool safe = false)
        {
            if (!safe)
            {
                if (x1 > x2)
                {
                    int t = x1;
                    x1 = x2;
                    x2 = t;
                }
                if (y1 > y2)
                {
                    int t = y1;
                    y1 = y2;
                    y2 = t;
                }
                if (x2 - x1 < 8 || y2 - y1 < 8)
                    return;
            }

            bool b = renderer.IsDrawing;
            if (b)
                renderer.End();

            renderer.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, null, null);
            Color c = Color.White * opacity;
            int sx = x2 - x1;
            int sy = y2 - y1;

            renderer.Draw(bg, new Rectangle(x1, y1, sx, sy), new Rectangle(0, 0, sx * 4, sy * 4), c * 0.6f);
            renderer.Draw(Shortcuts.pixel, new Vector2(x1, y1), null, c, 0, Vector2.Zero, new Vector2(0.25f, sy), SpriteEffects.None, 0);

            renderer.End();
            if (b)
                renderer.Begin();
        }

        public override void DrawGhost(int x, int y, MicroWorld.Graphics.Renderer renderer, Component.Rotation rotation)
        {
        }

    }
}
