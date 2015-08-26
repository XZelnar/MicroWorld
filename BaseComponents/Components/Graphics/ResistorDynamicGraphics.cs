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
    class ResistorDynamicGraphics : GraphicalComponent
    {
        public static int[] PORTS_STATE_POSITION = new int[] { 
            0, 8,
            48, 8
        };
        public static Texture2D texture0cw, texture90cw;
        public Color LEDColor = Color.Lime;

        public ResistorDynamicGraphics()
        {
            Size = new Vector2(48, 32);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/Resistor/Resistor0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/Resistor/Resistor90cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/Resistor";
        }

        public override string GetCSToolTip()
        {
            return "Dynamic Resistor";
        }

        public override string GetComponentSelectorPath()
        {
            return "Basic";
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
                    return new Vector2(26, 16);
            }
        }

        public override Vector2 GetSize()
        {
            return new Vector2(48, 32);
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(48, 32);
            else
                return new Vector2(32, 48);
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            Resistor p = parent as Resistor;
            Components.Logics.ResistorLogics l = (Components.Logics.ResistorLogics)parent.Logics;

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

            //renderer.DrawStringLeft(MicroWorld.Graphics.GUI.Elements.TextBox.defaultFont, ((Resistor)parent).Resistance.ToString(),
            //    Position, Color.White);
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
