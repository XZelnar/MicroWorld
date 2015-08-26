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
    class AccumulatorGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, texture90cw, texture180cw, texture270cw;
        public static Texture2D arrow0cw, arrow90cw, arrow180cw, arrow270cw;
        
        RenderTarget2D fbo;
        Color[] fboarr = new Color[48];//48*26

        public AccumulatorGraphics()
        {
            Size = new Vector2(64, 24);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/Accumulator/Accumulator0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/Accumulator/Accumulator90cw");
            texture180cw = ComponentsManager.LoadTexture("Components/Accumulator/Accumulator180cw");
            texture270cw = ComponentsManager.LoadTexture("Components/Accumulator/Accumulator270cw");

            arrow0cw = ComponentsManager.LoadTexture("Components/Accumulator/AccumulatorArrow0cw");
            arrow90cw = ComponentsManager.LoadTexture("Components/Accumulator/AccumulatorArrow90cw");
            arrow180cw = ComponentsManager.LoadTexture("Components/Accumulator/AccumulatorArrow180cw");
            arrow270cw = ComponentsManager.LoadTexture("Components/Accumulator/AccumulatorArrow270cw");
        }

        public override void Initialize()
        {
            fbo = new RenderTarget2D(Shortcuts.renderer.GraphicsDevice, 48, 1);
            
            base.Initialize();
        }

        public override string GetIconName()
        {
            return "Components/Icons/Accumulator";
        }

        public override string GetCSToolTip()
        {
            return "Accumulator";
        }

        public override string GetComponentSelectorPath()
        {
            return "Advanced";
        }

        public override string GetHandbookFile()
        {
            return "Components/Accumulator.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(32, 8);
            else
                return new Vector2(8, 32);
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(64, 24);
            else
                return new Vector2(24, 64);
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            Components.Logics.AccumulatorLogics l = (Components.Logics.AccumulatorLogics)parent.Logics;

            UpdateFBO();

            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            Color.White);
                    renderer.Draw(fbo, new Rectangle((int)Position.X + 8, (int)Position.Y + 4, 48, 16), null, Color.White);
                    renderer.Draw(arrow0cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            Color.White);
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            Color.White);
                    renderer.Draw(fbo, new Rectangle((int)Position.X + 20, (int)Position.Y + 8, 48, 16), null, Color.White, (float)Math.PI / 2, Vector2.Zero);
                    renderer.Draw(arrow90cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            Color.White);
                    break;
                case Component.Rotation.cw180:
                    renderer.Draw(texture180cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(fbo, new Rectangle((int)Position.X + 56, (int)Position.Y + 20, 48, 16), null, Color.White, (float)Math.PI, Vector2.Zero);
                    renderer.Draw(arrow180cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    break;
                case Component.Rotation.cw270:
                    renderer.Draw(texture270cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(fbo, new Rectangle((int)Position.X + 4, (int)Position.Y + 56, 48, 16), null, Color.White, 3 * (float)Math.PI / 2, Vector2.Zero);
                    renderer.Draw(arrow270cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    break;
                default:
                    break;
            }
        }

        internal unsafe void UpdateFBO()
        {
            if (Shortcuts.renderer.GraphicsDevice.Textures[0] == fbo)
                Shortcuts.renderer.GraphicsDevice.Textures[0] = null;

            Components.Logics.AccumulatorLogics l = (Components.Logics.AccumulatorLogics)parent.Logics;
            int w = (int)(l.Charge / l.MaxCharge * fbo.Width);

            fixed (Color* a = fboarr)
            {
                for (int x = 0; x < w; x++)
                    *(a + x) = Color.Red;
                for (int x = w; x < fbo.Width; x++)
                    *(a + x) = Color.Transparent;
            }

            fbo.SetData<Color>(fboarr);
        }

        public override void DrawGhost(int x, int y, MicroWorld.Graphics.Renderer renderer, Component.Rotation rotation)
        {
            if (texture0cw == null) return;
            switch (rotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(arrow0cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        new Color(1f, 1f, 1f, 0.5f));
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(arrow90cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        new Color(1f, 1f, 1f, 0.5f));
                    break;
                case Component.Rotation.cw180:
                    renderer.Draw(texture180cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(arrow180cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        new Color(1f, 1f, 1f, 0.5f));
                    break;
                case Component.Rotation.cw270:
                    renderer.Draw(texture270cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(arrow270cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        new Color(1f, 1f, 1f, 0.5f));
                    break;
                default:
                    break;
            }
        }
    }
}
