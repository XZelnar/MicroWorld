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
    class CapacitorGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, texture90cw;
        public static Texture2D texturePlateLeft0cw, texturePlateLeft90cw;
        public static Texture2D texturePlateRight0cw, texturePlateRight90cw;

        public CapacitorGraphics()
        {
            Size = new Vector2(48, 40);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/Capacitor/Capacitor0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/Capacitor/Capacitor90cw");

            texturePlateLeft0cw = ComponentsManager.LoadTexture("Components/Capacitor/CapacitorPlateLeft0cw");
            texturePlateLeft90cw = ComponentsManager.LoadTexture("Components/Capacitor/CapacitorPlateLeft90cw");

            texturePlateRight0cw = ComponentsManager.LoadTexture("Components/Capacitor/CapacitorPlateRight0cw");
            texturePlateRight90cw = ComponentsManager.LoadTexture("Components/Capacitor/CapacitorPlateRight90cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/Capacitor";
        }

        public override string GetCSToolTip()
        {
            return "Capacitor";
        }

        public override string GetComponentSelectorPath()
        {
            return "Basic";
        }

        public override string GetHandbookFile()
        {
            return "Components/Capacitor.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(24, 16);
            else
                return new Vector2(16, 24);
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(48, 40);
            else
                return new Vector2(40, 48);
        }

        public override Component.Rotation[] GetPossibleRotations()
        {
            return new Component.Rotation[] { Component.Rotation.cw0, Component.Rotation.cw90 };
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            Components.Logics.CapacitorLogics l = (Components.Logics.CapacitorLogics)parent.Logics;

            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            Color.White);
                    renderer.Draw(texturePlateLeft0cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            (l.CurCharge > 0 ? Color.Blue : Color.Red) * (float)Math.Abs(l.CurCharge / l.Capacitance));
                    renderer.Draw(texturePlateRight0cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            (l.CurCharge < 0 ? Color.Blue : Color.Red) * (float)Math.Abs(l.CurCharge / l.Capacitance));
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            Color.White);
                    renderer.Draw(texturePlateLeft90cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            (l.CurCharge > 0 ? Color.Blue : Color.Red) * (float)Math.Abs(l.CurCharge / l.Capacitance));
                    renderer.Draw(texturePlateRight90cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            (l.CurCharge < 0 ? Color.Blue : Color.Red) * (float)Math.Abs(l.CurCharge / l.Capacitance));
                    break;
                case Component.Rotation.cw180:
                case Component.Rotation.cw270:
                default:
                    break;
            }

            //renderer.DrawStringLeft(MicroWorld.Graphics.GUI.GUIEngine.font, l.CurCharge.ToString(), Position, Color.Red);
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
                case Component.Rotation.cw270:
                default:
                    break;
            }
        }
    }
}
