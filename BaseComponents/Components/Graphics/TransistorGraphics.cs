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
    class TransistorGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, texture90cw, texture180cw, texture270cw;
        public static Texture2D textureBase0cw, textureBase90cw, textureBase180cw, textureBase270cw;
        public static Texture2D textureEmitter0cw, textureEmitter90cw, textureEmitter180cw, textureEmitter270cw;

        public TransistorGraphics()
        {
            Size = new Vector2(32, 40);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/Transistor/Transistor0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/Transistor/Transistor90cw");
            texture180cw = ComponentsManager.LoadTexture("Components/Transistor/Transistor180cw");
            texture270cw = ComponentsManager.LoadTexture("Components/Transistor/Transistor270cw");

            textureBase0cw = ComponentsManager.LoadTexture("Components/Transistor/TransistorBase0cw");
            textureBase90cw = ComponentsManager.LoadTexture("Components/Transistor/TransistorBase90cw");
            textureBase180cw = ComponentsManager.LoadTexture("Components/Transistor/TransistorBase180cw");
            textureBase270cw = ComponentsManager.LoadTexture("Components/Transistor/TransistorBase270cw");

            textureEmitter0cw = ComponentsManager.LoadTexture("Components/Transistor/TransistorEmitter0cw");
            textureEmitter90cw = ComponentsManager.LoadTexture("Components/Transistor/TransistorEmitter90cw");
            textureEmitter180cw = ComponentsManager.LoadTexture("Components/Transistor/TransistorEmitter180cw");
            textureEmitter270cw = ComponentsManager.LoadTexture("Components/Transistor/TransistorEmitter270cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/Transistor";
        }

        public override string GetCSToolTip()
        {
            return "Transistor";
        }

        public override string GetComponentSelectorPath()
        {
            return "Basic";
        }

        public override string GetHandbookFile()
        {
            return "Components/Transistor.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(16, 16);
            else
                return new Vector2(16, 16);
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(32, 40);
            else
                return new Vector2(40, 32);
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            Components.Logics.TransistorLogics l = (Components.Logics.TransistorLogics)parent.Logics;
            Transistor p = parent as Transistor;
            Color c = Color.Red * (float)l.FlowPercentage;

            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(textureBase0cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            c);
                    renderer.Draw(textureEmitter0cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            p.W3.VoltageDropAbs == 0 ? Color.Transparent : c);
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(textureBase90cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            c);
                    renderer.Draw(textureEmitter90cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            p.W3.VoltageDropAbs == 0 ? Color.Transparent : c);
                    break;
                case Component.Rotation.cw180:
                    renderer.Draw(texture180cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(textureBase180cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            c);
                    renderer.Draw(textureEmitter180cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            p.W3.VoltageDropAbs == 0 ? Color.Transparent : c);
                    break;
                case Component.Rotation.cw270:
                    renderer.Draw(texture270cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(textureBase270cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            c);
                    renderer.Draw(textureEmitter270cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            p.W3.VoltageDropAbs == 0 ? Color.Transparent : c);
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
