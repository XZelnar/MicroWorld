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
    class VoltageGraphGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, texture90cw, texture180cw, texture270cw;
        static SpriteFont font;
        
        RenderTarget2D fbo;
        Color[] fboarr = new Color[156*92];

        public VoltageGraphGraphics()
        {
            Size = new Vector2(64, 40);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/VoltageGraph/VoltageGraph0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/VoltageGraph/VoltageGraph90cw");
            texture180cw = ComponentsManager.LoadTexture("Components/VoltageGraph/VoltageGraph180cw");
            texture270cw = ComponentsManager.LoadTexture("Components/VoltageGraph/VoltageGraph270cw");

            font = ResourceManager.LoadSpriteFont("Fonts/LiberationSans_7");
        }

        public override void Initialize()
        {
            fbo = new RenderTarget2D(Shortcuts.renderer.GraphicsDevice, 156, 92);
            
            base.Initialize();
        }

        public override string GetIconName()
        {
            return "Components/Icons/VoltageGraph";
        }

        public override string GetCSToolTip()
        {
            return "Voltage Graph";
        }

        public override string GetComponentSelectorPath()
        {
            return "Information";
        }

        public override string GetHandbookFile()
        {
            return "Components/Voltage Graph.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(32, 16);
            else
                return new Vector2(16, 32);
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(64, 40);
            else
                return new Vector2(40, 64);
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            Components.Logics.VoltageGraphLogics l = (Components.Logics.VoltageGraphLogics)parent.Logics;

            UpdateFBO();

            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            Color.White);
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            Color.White);
                    break;
                case Component.Rotation.cw180:
                    renderer.Draw(texture180cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    break;
                case Component.Rotation.cw270:
                    renderer.Draw(texture270cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    break;
                default:
                    break;
            }

            if (parent.ComponentRotation == Component.Rotation.cw0 || parent.ComponentRotation == Component.Rotation.cw180)
                renderer.Draw(fbo, new Rectangle((int)Position.X + 15, (int)Position.Y + 11, 39, 23), null, Color.White);
            else
                renderer.Draw(fbo, new Rectangle((int)Position.X + 29, (int)Position.Y + 15, 39, 23), null, Color.White, (float)Math.PI / 2, Vector2.Zero);

            renderer.End();
            renderer.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone,
                null, Matrix.CreateScale(0.25f), true);
            if (parent.ComponentRotation == Component.Rotation.cw0 || parent.ComponentRotation == Component.Rotation.cw180)
            {
                renderer.DrawStringRight(font, l.max.ToString(), new Rectangle((int)Position.X * 4 + 42, (int)Position.Y * 4 + 47, 12, 8), Color.Gray);
                renderer.DrawStringRight(font, l.min.ToString(), new Rectangle((int)Position.X * 4 + 42, (int)Position.Y * 4 + 125, 12, 8), Color.Gray);
            }
            else
            {
                renderer.DrawStringLeft(font, l.min.ToString(), Position * 4 + new Vector2(26, 44), Color.Gray);
                renderer.DrawStringRight(font, l.max.ToString(), new Rectangle((int)Position.X * 4 + 102, (int)Position.Y * 4 + 44, 12, 8), Color.Gray);
            }
            renderer.End();
            renderer.Begin();
        }

        internal unsafe void UpdateFBO()
        {
            //Shortcuts.renderer.Push();
            //Shortcuts.renderer.EnableFBO(fbo);
            //Shortcuts.renderer.Clear(Color.Transparent);
            //Shortcuts.renderer.DisableFBO();
            //Shortcuts.renderer.Pop();
            Shortcuts.renderer.GraphicsDevice.Textures[0] = null;

            Components.Logics.VoltageGraphLogics l = (Components.Logics.VoltageGraphLogics)parent.Logics;

            double y = 0;
            double d = l.max - l.min;
            fixed (Color* a = fboarr)
            {
                for (int i = 0; i < fboarr.Length; i++)
                    *(a + i) = Color.Transparent;

                for (int x = 0; x < fbo.Width; x++)
                {
                    y = l.values[x];
                    if (y > l.max)
                        *(a + x) = Color.White;
                    else if (y < l.min)
                        *(a + x + fbo.Width * (fbo.Height - 1)) = Color.White;
                    else
                    {
                        y = (1 - (y - l.min) / d) * (fbo.Height - 2);
                        *(a + x + fbo.Width * (int)y) = Color.White;
                        *(a + x + fbo.Width * (int)(y + 1)) = Color.White;
                    }
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
