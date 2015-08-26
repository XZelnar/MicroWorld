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
    class DoubleSwitchGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, texture90cw, texture180cw, texture270cw;
        public static Texture2D textureConnected1_0cw, textureConnected1_90cw, textureConnected1_180cw, textureConnected1_270cw;
        public static Texture2D textureConnected2_0cw, textureConnected2_90cw, textureConnected2_180cw, textureConnected2_270cw;

        public DoubleSwitchGraphics()
        {
            Size = new Vector2(32, 24);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/DoubleSwitch/DoubleSwitchBase0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/DoubleSwitch/DoubleSwitchBase90cw");
            texture180cw = ComponentsManager.LoadTexture("Components/DoubleSwitch/DoubleSwitchBase180cw");
            texture270cw = ComponentsManager.LoadTexture("Components/DoubleSwitch/DoubleSwitchBase270cw");

            textureConnected1_0cw = ComponentsManager.LoadTexture("Components/DoubleSwitch/DoubleSwitchConnection1_0cw");
            textureConnected1_90cw = ComponentsManager.LoadTexture("Components/DoubleSwitch/DoubleSwitchConnection1_90cw");
            textureConnected1_180cw = ComponentsManager.LoadTexture("Components/DoubleSwitch/DoubleSwitchConnection1_180cw");
            textureConnected1_270cw = ComponentsManager.LoadTexture("Components/DoubleSwitch/DoubleSwitchConnection1_270cw");

            textureConnected2_0cw = ComponentsManager.LoadTexture("Components/DoubleSwitch/DoubleSwitchConnection2_0cw");
            textureConnected2_90cw = ComponentsManager.LoadTexture("Components/DoubleSwitch/DoubleSwitchConnection2_90cw");
            textureConnected2_180cw = ComponentsManager.LoadTexture("Components/DoubleSwitch/DoubleSwitchConnection2_180cw");
            textureConnected2_270cw = ComponentsManager.LoadTexture("Components/DoubleSwitch/DoubleSwitchConnection2_270cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/DoubleSwitch";
        }

        public override string GetCSToolTip()
        {
            return "Double Switch";
        }

        public override string GetComponentSelectorPath()
        {
            return "Basic:Interactive";
        }

        public override string GetHandbookFile()
        {
            return "Components/DoubleSwitch.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(16, 8);
            else
                return new Vector2(8, 16);
        }

        public override Vector2 GetSize()
        {
            if (parent.ComponentRotation == Component.Rotation.cw0 || parent.ComponentRotation == Component.Rotation.cw180)
                return new Vector2(32, 24);
            else
                return new Vector2(24, 32);
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(32, 24);
            else
                return new Vector2(24, 32);
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            DoubleSwitch d = parent as DoubleSwitch;

            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(d.connection == DoubleSwitch.Connection.Connection1 ? textureConnected1_0cw : textureConnected2_0cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(d.connection == DoubleSwitch.Connection.Connection1 ? textureConnected1_90cw : textureConnected2_90cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    break;
                case Component.Rotation.cw180:
                    renderer.Draw(texture180cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(d.connection == DoubleSwitch.Connection.Connection1 ? textureConnected1_180cw : textureConnected2_180cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    break;
                case Component.Rotation.cw270:
                    renderer.Draw(texture270cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(d.connection == DoubleSwitch.Connection.Connection1 ? textureConnected1_270cw : textureConnected2_270cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
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
                    renderer.Draw(textureConnected1_0cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(textureConnected1_90cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    break;
                case Component.Rotation.cw180:
                    renderer.Draw(texture180cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(textureConnected1_180cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    break;
                case Component.Rotation.cw270:
                    renderer.Draw(texture270cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(textureConnected1_270cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                        new Color(1f, 1f, 1f, 0.5f));
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
