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
    class CoilGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, texture90cw;
        public static Texture2D MFAOE;
        public Color LEDColor = Color.Lime;

        public CoilGraphics()
        {
            Size = new Vector2(48, 24);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/Coil/Coil0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/Coil/Coil90cw");

            MFAOE = ComponentsManager.LoadTexture("Components/AOE");
        }

        public override string GetIconName()
        {
            return "Components/Icons/Coil";
        }

        public override string GetCSToolTip()
        {
            return "Coil";
        }

        public override string GetComponentSelectorPath()
        {
            return "Magnets";
        }

        public override string GetHandbookFile()
        {
            return "Components/Coil.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            switch (rotation)
            {
                case Component.Rotation.cw0:
                    return new Vector2(24, 16);
                case Component.Rotation.cw90:
                    return new Vector2(16, 24);
                default:
                    return new Vector2(26, 16);
            }
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(48, 24);
            else
                return new Vector2(24, 48);
        }

        public override Component.Rotation[] GetPossibleRotations()
        {
            return new Component.Rotation[] { Component.Rotation.cw0, Component.Rotation.cw90 };
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            Coil p = parent as Coil;
            Components.Logics.CoilLogics l = (Components.Logics.CoilLogics)parent.Logics;
            if (!wasAOEDrawn && AOEOpacity > 0 && !MicroWorld.Graphics.GraphicsEngine.IsSelectedGlowPass)
            {
                DrawAOE(renderer, 1f);
                wasAOEDrawn = false;
            }

            //renderer.DrawString(MicroWorld.Graphics.GUI.GUIEngine.font, p.W.Resistance.ToString(), Position, Color.Red, MicroWorld.Graphics.Renderer.TextAlignment.Left);

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
                    break;
                case Component.Rotation.cw270:
                    break;
                default:
                    break;
            }
            //renderer.DrawStringLeft(MicroWorld.Graphics.GUI.GUIEngine.font, l.Field.ToString() + "\r\n" + l.desiredField.ToString(), Position, Color.Red);
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
            var p = parent as Coil;
            float a = (float)((Main.Ticks % 1200) * Math.PI / 600f);
            float r = p.FieldRadius;
            float rabs = Math.Abs(r);
            float[] radiuses = new float[4];
            int d = (int)(Main.Ticks % 80) / 2;
            if (r < 0)
            {
                radiuses[0] = rabs - d;
                radiuses[1] = rabs - d - 40;
                radiuses[2] = rabs - d - 80;
                radiuses[3] = rabs - d - 120;
            }
            else
            {
                radiuses[0] = rabs + d - 160;
                radiuses[1] = rabs + d - 120;
                radiuses[2] = rabs + d - 80;
                radiuses[3] = rabs + d - 40;
            }
            if (radiuses[0] < 0) radiuses[0] = 0;
            if (radiuses[1] < 0) radiuses[1] = 0;
            if (radiuses[2] < 0) radiuses[2] = 0;
            if (radiuses[3] < 0) radiuses[3] = 0;

            MicroWorld.Graphics.RenderHelper.DrawDottedCircle(rabs, Position + GetSize() / 2, (int)(rabs / 2),
                a, renderer, Color.White * 0.4f);

            MicroWorld.Graphics.RenderHelper.DrawDottedCircle(radiuses[0], Position + GetSize() / 2, (int)(rabs / 2),
                a, renderer, Color.White * (float)((float)d / 40f));
            MicroWorld.Graphics.RenderHelper.DrawDottedCircle(radiuses[1], Position + GetSize() / 2, (int)(rabs / 2),
                a, renderer, Color.White);
            MicroWorld.Graphics.RenderHelper.DrawDottedCircle(radiuses[2], Position + GetSize() / 2, (int)(rabs / 2),
                a, renderer, Color.White);
            MicroWorld.Graphics.RenderHelper.DrawDottedCircle(radiuses[3], Position + GetSize() / 2, (int)(rabs / 2),
                a, renderer, Color.White * (float)((float)(40 - d) / 40f));
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
                    break;
                case Component.Rotation.cw270:
                    break;
                default:
                    renderer.Draw(texture0cw, new Vector2(x, y) + GetCenter(parent.ComponentRotation), null,
                        new Color(1f, 1f, 1f, 0.5f), rotation.GetHashCode() * (float)Math.PI / 2f,
                        GetCenter(parent.ComponentRotation), 1);
                    break;
            }
        }

    }
}
