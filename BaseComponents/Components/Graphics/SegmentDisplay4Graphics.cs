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
    class SegmentDisplay4Graphics : GraphicalComponent
    {
        public static int[] PORTS_STATE_POSITION = new int[] { 
            //Top
            10, -8,
            18, -8,
            26, -8,
            34, -8,
            42, -8,
            50, -8,
            //Down
            10, 36,
            18, 36,
            26, 36,
            34, 36,
            42, 36,
            50, 36
        };
        public static int[] DigitsPositions = new int[]{
            4,
            19,
            35,
            50
        };
        public static Vector2 DigitSize = new Vector2(10, 18);
        public static Texture2D texture, mask;
        public static Texture2D LECAOE;

        public Color DisplayColor = Color.Blue;

        public SegmentDisplay4Graphics()
        {
            Size = new Vector2(64, 32);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture = ComponentsManager.LoadTexture("Components/4SegmentDisplay");
            mask = ComponentsManager.LoadTexture("Components/4SegmentDisplayMask");

            LECAOE = ComponentsManager.LoadTexture("Components/AOE");
        }

        public override Component.Rotation[] GetPossibleRotations()
        {
            return null;
        }

        public override string GetIconName()
        {
            return "Components/Icons/4SegmentDisplay";
        }

        public override string GetCSToolTip()
        {
            return "Display";
        }

        public override string GetComponentSelectorPath()
        {
            return "Light-Emitting";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            return new Vector2(28, 16);
        }

        public override Vector2 GetSize()
        {
            return new Vector2(64, 32);
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            return GetSize();
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture == null) return;
            if (!CanDraw()) return;
            Components.old.SegmentDisplay4 p = (Components.old.SegmentDisplay4)parent;
            Logics.SegmentDisplay4Logics l = p.Logics as Logics.SegmentDisplay4Logics;

            if (!wasAOEDrawn && AOEOpacity > 0 && !MicroWorld.Graphics.GraphicsEngine.IsSelectedGlowPass)
            {
                DrawAOE(renderer, 1f);
                wasAOEDrawn = false;
            }

            renderer.Draw(texture, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y),
                new Rectangle(0, 0, (int)Size.X*4, (int)Size.Y*4), Color.White);
            //curnumbers
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    if ((l.digits[j] & (1 << i)) != 0)
                        renderer.Draw(mask,
                            new Rectangle((int)Position.X + DigitsPositions[j], (int)Position.Y + 7, (int)DigitSize.X, (int)DigitSize.Y),
                            new Rectangle(i * 44, 0, (int)DigitSize.X*4, (int)DigitSize.Y*4), 
                            DisplayColor * ((float)p.W[i*4+j].VoltageDropAbs / 5f));
                }
            }
            //last
            for (int c = 0; c < l.digitsold.Length; c++)
            {
                if (l.digitsold[c] == null) break;
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if ((l.digitsold[c][j] & (1 << i)) != 0)
                            renderer.Draw(mask,
                                new Rectangle((int)Position.X + DigitsPositions[j], (int)Position.Y + 7, (int)DigitSize.X, (int)DigitSize.Y),
                                new Rectangle(i * 44, 0, (int)DigitSize.X*4, (int)DigitSize.Y*4),
                                DisplayColor * 0.1f);
                    }
                }
            }
        }

        public override void DrawBorder(MicroWorld.Graphics.Renderer renderer)
        {
            DrawAOE(renderer, 1f);
            base.DrawBorder(renderer);
        }

        internal float AOEOpacity = 0f;
        internal bool wasAOEDrawn = false;
        public void DrawAOE(MicroWorld.Graphics.Renderer renderer, float opacityMultiplier)
        {
            var p = parent as old.SegmentDisplay4;
            MicroWorld.Graphics.RenderHelper.DrawDottedCircle(p.Luminosity, Position + GetSize() / 2, (int)(p.Luminosity / 2),
                (float)((Main.Ticks % 40) * Math.PI / 40f / 15f), renderer, Color.White);
            return;
            if (wasAOEDrawn) return;
            wasAOEDrawn = true;
            var s = GetSizeRotated(parent.ComponentRotation) / 2;
            renderer.Draw(LECAOE, new Rectangle((int)(Position.X + s.X - 100), (int)(Position.Y + s.Y - 100), 200, 200),
                Color.Yellow * 0.6f * opacityMultiplier * AOEOpacity);
        }

        public override void DrawGhost(int x, int y, MicroWorld.Graphics.Renderer renderer, Component.Rotation rotation)
        {
            if (texture == null) return;
            renderer.Draw(texture, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                new Color(1f, 1f, 1f, 0.5f));
            //renderer.Draw(texture, new Vector2(x, y) + GetCenter(rotation), null, new Color(1f, 1f, 1f, 0.5f),
            //    rotation.GetHashCode() * (float)Math.PI / 2f, GetCenter(rotation), 1);
        }
    }
}
