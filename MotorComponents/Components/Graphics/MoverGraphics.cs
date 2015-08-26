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
    class MoverGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, texture90cw, texture180cw, texture270cw, cog;

        private float cogRotation = 0;
        internal float CogRotation
        {
            get { return cogRotation; }
            set
            {
                cogRotation = value;
                if (cogRotation < 0)
                    cogRotation += (float)Math.PI * 2;
                if (cogRotation > Math.PI * 2)
                {
                    cogRotation -= (float)Math.PI * 2;
                }
            }
        }

        public MoverGraphics()
        {
            Size = new Vector2(32, 32);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            cog = ComponentsManager.LoadTexture("Components/Mover/Cog");
            texture0cw = ComponentsManager.LoadTexture("Components/Mover/Base0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/Mover/Base90cw");
            texture180cw = ComponentsManager.LoadTexture("Components/Mover/Base180cw");
            texture270cw = ComponentsManager.LoadTexture("Components/Mover/Base270cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/Mover";
        }

        public override string GetCSToolTip()
        {
            return "Mover";
        }

        public override string GetComponentSelectorPath()
        {
            return "Motors";
        }

        public override string GetHandbookFile()
        {
            return "Components/Mover.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            return new Vector2(16, 16);
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(32, 32);
            else
                return new Vector2(32, 32);
        }

        public override void Reset()
        {
            cogRotation = 0;

            base.Reset();
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            Motor p = parent as Motor;
            //Components.Logics.MotorLogics l = (Components.Logics.MotorLogics)parent.Logics;

            var s = GetSizeRotated(parent.ComponentRotation);
            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(cog, new Rectangle((int)Position.X + 16, (int)Position.Y, 32, 32), null, Color.White, cogRotation, new Vector2(64.5f, 64.5f));
                    renderer.Draw(texture0cw,
                        new Rectangle((int)Position.X, (int)Position.Y - 16,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y + 16), null,
                            Color.White);
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(cog, new Rectangle((int)Position.X + 32, (int)Position.Y + 16, 32, 32), null, Color.White, cogRotation, new Vector2(64.5f, 64.5f));
                    renderer.Draw(texture90cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X + 16, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    break;
                case Component.Rotation.cw180:
                    renderer.Draw(cog, new Rectangle((int)Position.X + 16, (int)Position.Y + 32, 32, 32), null, Color.White, cogRotation, new Vector2(64.5f, 64.5f));
                    renderer.Draw(texture180cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y + 16), null,
                            Color.White);
                    break;
                case Component.Rotation.cw270:
                    renderer.Draw(cog, new Rectangle((int)Position.X, (int)Position.Y + 16, 32, 32), null, Color.White, cogRotation, new Vector2(64.5f, 64.5f));
                    renderer.Draw(texture270cw,
                        new Rectangle((int)Position.X - 16, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X + 16, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    break;
                default:
                    break;
            }
        }

        public override void DrawBorder(MicroWorld.Graphics.Renderer renderer)
        {
            base.DrawBorder(renderer);
            DrawTrajectory(renderer);
        }

        public void DrawTrajectory(MicroWorld.Graphics.Renderer renderer)
        {
        }

        public override void DrawGhost(int x, int y, MicroWorld.Graphics.Renderer renderer, Component.Rotation rotation)
        {
            if (texture0cw == null) return;
            switch (rotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw, new Rectangle(x, y - 16, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y + 16),
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(cog, new Rectangle(x, y - 16, 32, 32), new Color(1f, 1f, 1f, 0.5f));
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X + 16, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(cog, new Rectangle(x + 16, y, 32, 32), new Color(1f, 1f, 1f, 0.5f));
                    break;
                case Component.Rotation.cw180:
                    renderer.Draw(texture180cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y + 16),
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(cog, new Rectangle(x, y + 16, 32, 32), new Color(1f, 1f, 1f, 0.5f));
                    break;
                case Component.Rotation.cw270:
                    renderer.Draw(texture270cw, new Rectangle(x - 16, y, (int)GetSizeRotated(rotation).X + 16, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(cog, new Rectangle(x - 16, y, 32, 32), new Color(1f, 1f, 1f, 0.5f));
                    break;
                default:
                    break;
            }
        }

        public override bool CanDraw()
        {
            Rectangle r1 = new Rectangle(),// = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y),
                      r2 = new Rectangle(-(int)Settings.GameOffset.X, -(int)Settings.GameOffset.Y,
                          (int)(Main.WindowWidth / Settings.GameScale), (int)(Main.WindowHeight / Settings.GameScale));
            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    r1 = new Rectangle((int)Position.X, (int)Position.Y - 16, (int)Size.X, (int)Size.Y + 16);
                    break;
                case Component.Rotation.cw90:
                    r1 = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X + 16, (int)Size.Y);
                    break;
                case Component.Rotation.cw180:
                    r1 = new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y + 16);
                    break;
                case Component.Rotation.cw270:
                    r1 = new Rectangle((int)Position.X - 16, (int)Position.Y, (int)Size.X + 16, (int)Size.Y);
                    break;
                default:
                    break;
            }
            return r1.Intersects(r2) || r2.Contains(r1);
        }
    }
}
