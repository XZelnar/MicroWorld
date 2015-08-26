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
    class DelayerGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, texture90cw, texture180cw, texture270cw;
        
        RenderTarget2D fbo;
        Color[] fboarr = new Color[96];

        public DelayerGraphics()
        {
            Size = new Vector2(48, 24);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/Delayer/Delayer0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/Delayer/Delayer90cw");
            texture180cw = ComponentsManager.LoadTexture("Components/Delayer/Delayer180cw");
            texture270cw = ComponentsManager.LoadTexture("Components/Delayer/Delayer270cw");
        }

        public override void Initialize()
        {
            fbo = new RenderTarget2D(Shortcuts.renderer.GraphicsDevice, 96, 1);
            
            base.Initialize();
        }

        public override string GetIconName()
        {
            return "Components/Icons/Delayer";
        }

        public override string GetCSToolTip()
        {
            return "Delayer";
        }

        public override string GetComponentSelectorPath()
        {
            return "Logics";
        }

        public override string GetHandbookFile()
        {
            return "Components/Delayer.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(24, 8);
            else
                return new Vector2(8, 24);
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(48, 24);
            else
                return new Vector2(24, 48);
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            Components.Logics.DelayerLogics l = (Components.Logics.DelayerLogics)parent.Logics;

            UpdateFBO();

            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            Color.White);
                    renderer.Draw(fbo, new Rectangle((int)Position.X + 12, (int)Position.Y + 7, 24, 10), null, Color.White);
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            Color.White);
                    renderer.Draw(fbo, new Rectangle((int)Position.X + 17, (int)Position.Y + 12, 24, 10), null, Color.White, (float)Math.PI / 2, Vector2.Zero);
                    break;
                case Component.Rotation.cw180:
                    renderer.Draw(texture180cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            Color.White);
                    renderer.Draw(fbo, new Rectangle((int)Position.X + 36, (int)Position.Y + 17, 24, 10), null, Color.White, (float)Math.PI, Vector2.Zero);
                    break;
                case Component.Rotation.cw270:
                    renderer.Draw(texture270cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            Color.White);
                    renderer.Draw(fbo, new Rectangle((int)Position.X + 7, (int)Position.Y + 36, 24, 10), null, Color.White, (float)-Math.PI / 2, Vector2.Zero);
                    break;
                default:
                    break;
            }
        }

        private unsafe void UpdateFBO()
        {
            Components.Logics.DelayerLogics l = (Components.Logics.DelayerLogics)parent.Logics;
            bool b = false;

            fixed (Color* a = fboarr)
            {
                for (int x = 0; x < fbo.Width; x++)
                {
                    b = l.signals[x * l.Delay / fbo.Width] > 2.5f;

                    *(a + x) = b ? Color.Red : Color.DarkRed;
                }
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
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        new Color(1f, 1f, 1f, 0.5f));
                    break;
                case Component.Rotation.cw180:
                    renderer.Draw(texture180cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        new Color(1f, 1f, 1f, 0.5f));
                    break;
                case Component.Rotation.cw270:
                    renderer.Draw(texture270cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        new Color(1f, 1f, 1f, 0.5f));
                    break;
                default:
                    break;
            }
        }
    }
}
