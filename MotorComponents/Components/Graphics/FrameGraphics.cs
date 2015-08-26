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
    class FrameGraphics : GraphicalComponent
    {
        public override Vector2 Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
                if (!IO.SaveEngine.IsLoading)
                    (parent as Frame).UpdateComponentsList();
            }
        }

        public static Texture2D bg, left, right, top, bottom;

        public FrameGraphics()
        {
            base.Size = new Vector2(0, 0);
            Layer = -1;
        }

        public static void LoadContentStatic()
        {
            bg = ResourceManager.Load<Texture2D>("Components/Frame/FrameBG");
            left = ResourceManager.Load<Texture2D>("Components/Frame/FrameLeft");
            right = ResourceManager.Load<Texture2D>("Components/Frame/FrameRight");
            top = ResourceManager.Load<Texture2D>("Components/Frame/FrameTop");
            bottom = ResourceManager.Load<Texture2D>("Components/Frame/FrameBottom");
        }

        public override string GetIconName()
        {
            return "Components/Icons/Frame";
        }

        public override string GetCSToolTip()
        {
            return "Frame";
        }

        public override string GetComponentSelectorPath()
        {
            return "Motors";
        }

        public override string GetHandbookFile()
        {
            return "Components/Frame.edf";
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
            var p = parent as Frame;

            if (Settings.GameState != Settings.GameStates.Stopped && p.origPos.HasValue)
                renderer.Draw(p.fbo, new Rectangle((int)p.origPos.Value.X, (int)p.origPos.Value.Y, (int)(Size.X), (int)(Size.Y)), Color.White * 0.4f);

            DrawFrame((int)Position.X, (int)Position.Y, (int)(Position.X + Size.X), (int)(Position.Y + Size.Y), renderer, 1f, true);

            for (int i = 0; i < p.containsComponents.Length; i++)
            {
                p.containsComponents[i].Graphics.IgnoreNextDraw = true;
                ComponentsManager.DrawComponent(p.containsComponents[i]);
            }
        }

        public void DrawToFBO(MicroWorld.Graphics.Renderer renderer)
        {
            var p = parent as Frame;

            renderer.EnableFBO(p.fbo);
            renderer.GraphicsDevice.Clear(Color.Transparent);
            var s = Settings.GameScale;
            Settings.GameScale = MicroWorld.Graphics.Camera.ZOOM_MAX;
            var t = Shortcuts.camera.Center;
            var a = Shortcuts.camera.VisibleRectangle;
            Shortcuts.camera.Center = Position + new Vector2(a.Width / 2, a.Height / 2);

            renderer.Begin();

            DrawFrame((int)Position.X, (int)Position.Y, (int)(Position.X + Size.X), (int)(Position.Y + Size.Y), renderer, 1f, true);

            for (int i = 0; i < p.containsComponents.Length; i++)
            {
                if (!renderer.IsDrawing)
                    renderer.Begin();
                p.containsComponents[i].Graphics.IgnoreNextDraw = true;
                p.containsComponents[i].Graphics.Draw(renderer);
            }

            renderer.End();
            Shortcuts.camera.Center = t;
            Settings.GameScale = s;
            renderer.DisableFBO();
        }

        public void DrawFrame(int x1, int y1, int x2, int y2, MicroWorld.Graphics.Renderer renderer, float opacity, bool safe = false)
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
                if (x2 - x1 < 32 || y2 - y1 < 32)
                    return;
            }

            bool b = renderer.IsDrawing;
            if (!b)
                renderer.Begin(SpriteSortMode.Immediate, null, SamplerState.PointWrap, null, null);
            Color c = Color.White * opacity;
            int sx = x2 - x1;
            int sy = y2 - y1;

            renderer.Draw(bg, new Rectangle(x1 + 8, y1 + 8, sx - 16, sy - 16), new Rectangle(0, 0, sx * 4 - 64, sy * 4 - 64), c * 0.6f);

            renderer.Draw(top, new Rectangle(x1 + 8, y1, sx - 16, 8), new Rectangle(0, 0, sx * 4 - 64, 32), c);
            renderer.Draw(left, new Rectangle(x1, y1 + 8, 8, sy - 16), new Rectangle(0, 0, 32, sy * 4 - 64), c);
            renderer.Draw(bottom, new Rectangle(x1 + 8, y2 - 8, sx - 16, 8), new Rectangle(0, 0, sx * 4 - 64, 32), c);
            renderer.Draw(right, new Rectangle(x2 - 8, y1 + 8, 8, sy - 16), new Rectangle(0, 0, 32, sy * 4 - 64), c);

            if (!b)
                renderer.End();
        }

        public override void DrawGhost(int x, int y, MicroWorld.Graphics.Renderer renderer, Component.Rotation rotation)
        {
        }

    }
}
