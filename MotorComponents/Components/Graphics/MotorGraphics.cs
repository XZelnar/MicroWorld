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
    class MotorGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, texture90cw, overlay0cw, overlay90cw;

        public MotorGraphics()
        {
            Size = new Vector2(48, 40);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/Motor/Motor0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/Motor/Motor90cw");
            overlay0cw = ComponentsManager.LoadTexture("Components/Motor/MotorOverlay0cw");
            overlay90cw = ComponentsManager.LoadTexture("Components/Motor/MotorOverlay90cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/Motor";
        }

        public override string GetCSToolTip()
        {
            return "Motor";
        }

        public override string GetComponentSelectorPath()
        {
            return "Motors";
        }

        public override string GetHandbookFile()
        {
            return "Components/Motor.edf";
        }

        public override Component.Rotation[] GetPossibleRotations()
        {
            return new Component.Rotation[] { 
                Component.Rotation.cw0, 
                Component.Rotation.cw90};
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            switch (rotation)
            {
                case Component.Rotation.cw0:
                    return new Vector2(24, 16);
                case Component.Rotation.cw90:
                    return new Vector2(16, 24);
                case Component.Rotation.cw180:
                    return new Vector2(24, 16);
                case Component.Rotation.cw270:
                    return new Vector2(16, 24);
                default:
                    return new Vector2();
            }
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(48, 40);
            else
                return new Vector2(40, 48);
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            Motor p = parent as Motor;
            Components.Logics.MotorLogics l = (Components.Logics.MotorLogics)parent.Logics;

            var s = GetSizeRotated(parent.ComponentRotation);
            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(overlay0cw,
                        new Rectangle((int)(Position.X + s.X / 2), (int)(Position.Y + s.Y / 2), (int)s.X, (int)s.Y), null,
                            Color.White, l.Angle, new Vector2(96, 80));
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(overlay90cw,
                        new Rectangle((int)(Position.X + s.X / 2), (int)(Position.Y + s.Y / 2), (int)s.X, (int)s.Y), null,
                            Color.White, l.Angle, new Vector2(80, 96));
                    break;
                case Component.Rotation.cw180:
                    break;
                case Component.Rotation.cw270:
                    break;
                default:
                    break;
            }


            //renderer.DrawStringLeft(MicroWorld.Graphics.GUI.Elements.TextBox.defaultFont, ((Resistor)parent).Resistance.ToString(),
            //    Position, Color.White);
        }

        public override void DrawBorder(MicroWorld.Graphics.Renderer renderer)
        {
            base.DrawBorder(renderer);
            DrawTrajectory(renderer);
        }

        public void DrawTrajectory(MicroWorld.Graphics.Renderer renderer)
        {
            float rad = 100f;
            var p = parent as Motor;
            if (p.connector != null)
            {
                var a1 = p.connector.ConnectedComponent1.Graphics.Position + p.connector.ConnectedComponent1.Graphics.GetSize() / 2;
                var a2 = p.connector.ConnectedComponent2.Graphics.Position + p.connector.ConnectedComponent2.Graphics.GetSize() / 2;
                rad = Math.Abs((a2 - a1).Length());
            }
            else return;

            MicroWorld.Graphics.RenderHelper.DrawDottedCircle(rad, Position + GetSize() / 2, 60, (float)((Main.Ticks % 40) * Math.PI / 40f / 15f), renderer, Color.White);
            return;

            float opacity = 1f;
            VertexPositionColorTexture[] arr = new VertexPositionColorTexture[60];

            double d = (float)((Main.Ticks % 40)) * Math.PI / 40f / 15f;
            int c = 0;
            Vector2 off = Position + GetSize() / 2;
            for (double i = 0; i <= Math.PI * 2 - 0.001d; i += Math.PI / 30d)
            {
                arr[c] = new VertexPositionColorTexture(
                    new Vector3((float)Math.Cos(i + d) * rad + off.X, (float)Math.Sin(i + d) * rad + off.Y, 0),
                    Color.White * opacity, new Vector2());
                c++;
            }

            Main.renderer.Draw(MicroWorld.Graphics.GraphicsEngine.pixel, new Rectangle(0, 0, 0, 0), Color.White);
            bool a = renderer.IsDrawing;
            bool b = renderer.IsScaeld;
            if (renderer.IsDrawing) renderer.End();

            renderer.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.LineList,
                arr, 0, arr.Length / 2);

            if (a) renderer.Begin(b);
        }

        public override void DrawGhost(int x, int y, MicroWorld.Graphics.Renderer renderer, Component.Rotation rotation)
        {
            if (texture0cw == null) return;
            switch (rotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(overlay0cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(overlay90cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
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
