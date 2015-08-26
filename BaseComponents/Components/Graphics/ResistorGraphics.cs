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
    class ResistorGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, texture90cw;
        public static SpriteFont font;
        public Color LEDColor = Color.Lime;

        public ResistorGraphics()
        {
            Size = new Vector2(48, 24);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/Resistor/Resistor0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/Resistor/Resistor90cw");
            font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_7");
        }

        public override string GetIconName()
        {
            return "Components/Icons/Resistor";
        }

        public override string GetCSToolTip()
        {
            return "Resistor";
        }

        public override string GetComponentSelectorPath()
        {
            return "Basic";
        }

        public override string GetHandbookFile()
        {
            return "Components/Resistor.edf";
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
                    return new Vector2(24, 8);
                case Component.Rotation.cw90:
                    return new Vector2(8, 24);
                case Component.Rotation.cw180:
                    return new Vector2(24, 8);
                case Component.Rotation.cw270:
                    return new Vector2(8, 24);
                default:
                    return new Vector2();
            }
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(48, 24);
            else
                return new Vector2(24, 48);
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            Resistor p = parent as Resistor;
            Components.Logics.ResistorLogics l = (Components.Logics.ResistorLogics)parent.Logics;

            String pr = "Ω";
            double tr = p.Resistance;
            if (tr > 1000000)//m
            {
                pr = "m" + pr;
                tr /= 1000000;
            }
            else if (tr > 1000)
            {
                pr = "k" + pr;
                tr /= 1000;
            }
            tr = Math.Round(tr, 1);
            if (parent.ComponentRotation == Component.Rotation.cw0)
                pr = tr.ToString() + " " + pr;
            else
            {
                String t = tr.ToString();
                for (int i = 2; i < t.Length; i += 4)
                    t = t.Insert(i, "\r\n");
                pr = t + "\r\n" + pr;
            }
            var a = font.MeasureString(pr);

            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    //text
                    renderer.DrawString(font, pr, Position + (GetSizeRotated(parent.ComponentRotation) - a) / 2, Color.White, MicroWorld.Graphics.Renderer.TextAlignment.Center);
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    //text
                    renderer.DrawString(font, pr, Position + (GetSizeRotated(parent.ComponentRotation) - a) / 2, Color.White, MicroWorld.Graphics.Renderer.TextAlignment.Center);
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
