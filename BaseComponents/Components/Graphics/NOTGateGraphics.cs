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
    class NOTGateGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, texture90cw, texture180cw, texture270cw;
        public static Texture2D textureConnection1_0cw, textureConnection1_90cw, textureConnection1_180cw, textureConnection1_270cw;
        public static Texture2D textureConnection2_0cw, textureConnection2_90cw, textureConnection2_180cw, textureConnection2_270cw;

        public NOTGateGraphics()
        {
            Size = new Vector2(64, 40);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/NOTGate/NOTGate0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/NOTGate/NOTGate90cw");
            texture180cw = ComponentsManager.LoadTexture("Components/NOTGate/NOTGate180cw");
            texture270cw = ComponentsManager.LoadTexture("Components/NOTGate/NOTGate270cw");

            textureConnection1_0cw = ComponentsManager.LoadTexture("Components/NOTGate/NOTGateConnection1_0cw");
            textureConnection1_90cw = ComponentsManager.LoadTexture("Components/NOTGate/NOTGateConnection1_90cw");
            textureConnection1_180cw = ComponentsManager.LoadTexture("Components/NOTGate/NOTGateConnection1_180cw");
            textureConnection1_270cw = ComponentsManager.LoadTexture("Components/NOTGate/NOTGateConnection1_270cw");

            textureConnection2_0cw = ComponentsManager.LoadTexture("Components/NOTGate/NOTGateConnection2_0cw");
            textureConnection2_90cw = ComponentsManager.LoadTexture("Components/NOTGate/NOTGateConnection2_90cw");
            textureConnection2_180cw = ComponentsManager.LoadTexture("Components/NOTGate/NOTGateConnection2_180cw");
            textureConnection2_270cw = ComponentsManager.LoadTexture("Components/NOTGate/NOTGateConnection2_270cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/NOTGate";
        }

        public override string GetCSToolTip()
        {
            return "NOT Gate";
        }

        public override string GetComponentSelectorPath()
        {
            return "Logics";
        }

        public override string GetHandbookFile()
        {
            return "Components/NOT Gate.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            switch (rotation)
            {
                case Component.Rotation.cw0:
                    return new Vector2(32, 16);
                case Component.Rotation.cw90:
                    return new Vector2(16, 32);
                case Component.Rotation.cw180:
                    return new Vector2(32, 16);
                case Component.Rotation.cw270:
                    return new Vector2(16, 32);
                default:
                    return new Vector2();
            }
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
            Components.NOTGate p = (Components.NOTGate)parent;
            Components.Logics.NOTGateLogics l = (Components.Logics.NOTGateLogics)parent.Logics;

            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(textureConnection1_0cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            l.v1 > 2.5 ? Color.Red : Color.DarkRed);
                    renderer.Draw(textureConnection2_0cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            p.Joints[1].SendingVoltage > 2.5 ? Color.Red : Color.DarkRed);
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(textureConnection1_90cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            l.v1 > 2.5 ? Color.Red : Color.DarkRed);
                    renderer.Draw(textureConnection2_90cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            p.Joints[1].SendingVoltage > 2.5 ? Color.Red : Color.DarkRed);
                    break;
                case Component.Rotation.cw180:
                    renderer.Draw(texture180cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(textureConnection1_180cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            l.v1 > 2.5 ? Color.Red : Color.DarkRed);
                    renderer.Draw(textureConnection2_180cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            p.Joints[1].SendingVoltage > 2.5 ? Color.Red : Color.DarkRed);
                    break;
                case Component.Rotation.cw270:
                    renderer.Draw(texture270cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(textureConnection1_270cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            l.v1 > 2.5 ? Color.Red : Color.DarkRed);
                    renderer.Draw(textureConnection2_270cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            p.Joints[1].SendingVoltage > 2.5 ? Color.Red : Color.DarkRed);
                    break;
                default:
                    break;
            }
        }

        public override void DrawGhost(int x, int y, MicroWorld.Graphics.Renderer renderer, Component.Rotation rotation)
        {
            if (texture0cw == null) return;
            switch (rotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(textureConnection1_0cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        Color.DarkRed * 0.5f);
                    renderer.Draw(textureConnection2_0cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        Color.DarkRed * 0.5f);
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(textureConnection1_90cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        Color.DarkRed * 0.5f);
                    renderer.Draw(textureConnection2_90cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        Color.DarkRed * 0.5f);
                    break;
                case Component.Rotation.cw180:
                    renderer.Draw(texture180cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(textureConnection1_180cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        Color.DarkRed * 0.5f);
                    renderer.Draw(textureConnection2_180cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        Color.DarkRed * 0.5f);
                    break;
                case Component.Rotation.cw270:
                    renderer.Draw(texture270cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(textureConnection1_270cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        Color.DarkRed * 0.5f);
                    renderer.Draw(textureConnection2_270cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y), 
                        Color.DarkRed * 0.5f);
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
