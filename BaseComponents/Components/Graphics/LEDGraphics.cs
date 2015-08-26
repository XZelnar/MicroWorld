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
    class LEDGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, texture90cw, texture180cw, texture270cw;
        public static Texture2D textureOn0cw, textureOn90cw, textureOn180cw, textureOn270cw;
        public static Texture2D LECAOE;
        public Color LEDColor = Color.White;

        public LEDGraphics()
        {
            Size = new Vector2(48, 40);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/LED/LED0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/LED/LED90cw");
            texture180cw = ComponentsManager.LoadTexture("Components/LED/LED180cw");
            texture270cw = ComponentsManager.LoadTexture("Components/LED/LED270cw");

            textureOn0cw = ComponentsManager.LoadTexture("Components/LED/LEDOn0cw");
            textureOn90cw = ComponentsManager.LoadTexture("Components/LED/LEDOn90cw");
            textureOn180cw = ComponentsManager.LoadTexture("Components/LED/LEDOn180cw");
            textureOn270cw = ComponentsManager.LoadTexture("Components/LED/LEDOn270cw");

            LECAOE = ComponentsManager.LoadTexture("Components/AOE");
        }

        public override string GetIconName()
        {
            return "Components/Icons/LED";
        }

        public override string GetCSToolTip()
        {
            return "LED";
        }

        public override string GetComponentSelectorPath()
        {
            return "Light-Emitting";
        }

        public override string GetHandbookFile()
        {
            return "Components/LED.edf";
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
            LED p = parent as LED;
            Components.Logics.LEDLogics l = (Components.Logics.LEDLogics)parent.Logics;
            if (!wasAOEDrawn && AOEOpacity > 0 && !MicroWorld.Graphics.GraphicsEngine.IsSelectedGlowPass)
            {
                DrawAOE(renderer, 0.6f);
                wasAOEDrawn = false;
            }
            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(textureOn0cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            LEDColor * (float)(l.Brightness));
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(textureOn90cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            LEDColor * (float)(l.Brightness));
                    break;
                case Component.Rotation.cw180:
                    renderer.Draw(texture180cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(textureOn180cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            LEDColor * (float)(l.Brightness));
                    break;
                case Component.Rotation.cw270:
                    renderer.Draw(texture270cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    renderer.Draw(textureOn270cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            LEDColor * (float)(l.Brightness));
                    break;
                default:
                    break;
            }
        }

        public override void DrawBorder(MicroWorld.Graphics.Renderer renderer)
        {
            DrawAOE(renderer, 0.6f);
            base.DrawBorder(renderer);
        }

        internal float AOEOpacity = 0f;
        internal bool wasAOEDrawn = false;
        public void DrawAOE(MicroWorld.Graphics.Renderer renderer, float opacityMultiplier)
        {
            var p = parent as LED;
            MicroWorld.Graphics.RenderHelper.DrawDottedCircle(p.Luminosity, Position + GetSize() / 2, (int)(p.Luminosity / 2), 
                (float)((Main.Ticks % 40) * 2 * Math.PI / 40f / (int)(p.Luminosity / 4)), renderer, Color.White);
            return;
            if (wasAOEDrawn) return;
            wasAOEDrawn = true;
            var s = GetSizeRotated(parent.ComponentRotation) / 2;
            renderer.Draw(LECAOE, new Rectangle((int)(Position.X + s.X - p.Luminosity), (int)(Position.Y + s.Y - p.Luminosity),
                (int)(p.Luminosity * 2), (int)(p.Luminosity * 2)),
                Color.Yellow * 0.6f * opacityMultiplier * AOEOpacity);
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
                    renderer.Draw(texture0cw, new Vector2(x, y) + GetCenter(parent.ComponentRotation), null,
                        new Color(1f, 1f, 1f, 0.5f), rotation.GetHashCode() * (float)Math.PI / 2f,
                        GetCenter(parent.ComponentRotation), 1);
                    break;
            }
        }

    }
}
