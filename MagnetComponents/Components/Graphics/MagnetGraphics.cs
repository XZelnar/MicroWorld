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
    class MagnetGraphics : GraphicalComponent
    {
        public static int[] PORTS_STATE_POSITION = new int[] { 
            0, 8,
            48, 8
        };
        //N
        public static Texture2D texture0cw, texture90cw, texture180cw, texture270cw;
        //S
        public static Texture2D textureB0cw, textureB90cw, textureB180cw, textureB270cw;
        public static Texture2D MFAOE;

        public MagnetGraphics()
        {
            Size = new Vector2(48, 32);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/Magnet/MagnetN0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/Magnet/MagnetN90cw");
            texture180cw = ComponentsManager.LoadTexture("Components/Magnet/MagnetN180cw");
            texture270cw = ComponentsManager.LoadTexture("Components/Magnet/MagnetN270cw");

            textureB0cw = ComponentsManager.LoadTexture("Components/Magnet/MagnetS0cw");
            textureB90cw = ComponentsManager.LoadTexture("Components/Magnet/MagnetS90cw");
            textureB180cw = ComponentsManager.LoadTexture("Components/Magnet/MagnetS180cw");
            textureB270cw = ComponentsManager.LoadTexture("Components/Magnet/MagnetS270cw");

            MFAOE = ComponentsManager.LoadTexture("Components/AOE");
        }

        public override string GetIconName()
        {
            return "Components/Icons/Magnet";
        }

        public override string GetCSToolTip()
        {
            return "Magnet";
        }

        public override string GetComponentSelectorPath()
        {
            return "Magnets:Rotatable";
        }

        public override string GetHandbookFile()
        {
            return "Components/Magnet.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            switch (rotation)
            {
                case Component.Rotation.cw0:
                    return new Vector2(16, 16);
                case Component.Rotation.cw90:
                    return new Vector2(16, 16);
                case Component.Rotation.cw180:
                    return new Vector2(16, 16);
                case Component.Rotation.cw270:
                    return new Vector2(16, 16);
                default:
                    return new Vector2(16, 16);
            }
        }

        public override Vector2 GetSize()
        {
            return Size;
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            return new Vector2(32, 32);
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            Magnet p = parent as Magnet;
            if (!wasAOEDrawn && AOEOpacity > 0 && !MicroWorld.Graphics.GraphicsEngine.IsSelectedGlowPass)
            {
                DrawAOE(renderer, 1f);
                wasAOEDrawn = false;
            }
            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(p.pole == MagnetPole.S ? textureB0cw : texture0cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(p.pole == MagnetPole.S ? textureB90cw : texture90cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    break;
                case Component.Rotation.cw180:
                    renderer.Draw(p.pole == MagnetPole.S ? textureB180cw : texture180cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    break;
                case Component.Rotation.cw270:
                    renderer.Draw(p.pole == MagnetPole.S ? textureB270cw : texture270cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    break;
                default:
                    break;
            }
        }

        public override void DrawBorder(MicroWorld.Graphics.Renderer renderer)
        {
            DrawAOE(renderer, 1f);
            DrawTrajectory(renderer);
            base.DrawBorder(renderer);
        }

        internal float AOEOpacity = 0f;
        internal bool wasAOEDrawn = false;
        public void DrawAOE(MicroWorld.Graphics.Renderer renderer, float opacityMultiplier)
        {
            var p = parent as Magnet;
            float a = (float)((Main.Ticks % 1200) * Math.PI / 600f);
            float[] radiuses = new float[4];
            int d = (int)(Main.Ticks % 80) / 2;
            if (p.pole == MagnetPole.S)
            {
                radiuses[0] = p.FieldRadiusAbs - d;
                radiuses[1] = p.FieldRadiusAbs - d - 40;
                radiuses[2] = p.FieldRadiusAbs - d - 80;
                radiuses[3] = p.FieldRadiusAbs - d - 120;
            }
            else
            {
                radiuses[0] = p.FieldRadiusAbs + d - 160;
                radiuses[1] = p.FieldRadiusAbs + d - 120;
                radiuses[2] = p.FieldRadiusAbs + d - 80;
                radiuses[3] = p.FieldRadiusAbs + d - 40;
            }
            if (radiuses[0] < 0) radiuses[0] = 0;
            if (radiuses[1] < 0) radiuses[1] = 0;
            if (radiuses[2] < 0) radiuses[2] = 0;
            if (radiuses[3] < 0) radiuses[3] = 0;

            MicroWorld.Graphics.RenderHelper.DrawDottedCircle(p.FieldRadiusAbs, Position + GetSize() / 2, (int)(p.FieldRadiusAbs / 2),
                a, renderer, Color.White * 0.4f);

            MicroWorld.Graphics.RenderHelper.DrawDottedCircle(radiuses[0], Position + GetSize() / 2, (int)(p.FieldRadiusAbs / 2),
                a, renderer, Color.White * (float)((float)d / 40f));
            MicroWorld.Graphics.RenderHelper.DrawDottedCircle(radiuses[1], Position + GetSize() / 2, (int)(p.FieldRadiusAbs / 2),
                a, renderer, Color.White);
            MicroWorld.Graphics.RenderHelper.DrawDottedCircle(radiuses[2], Position + GetSize() / 2, (int)(p.FieldRadiusAbs / 2),
                a, renderer, Color.White);
            MicroWorld.Graphics.RenderHelper.DrawDottedCircle(radiuses[3], Position + GetSize() / 2, (int)(p.FieldRadiusAbs / 2),
                a, renderer, Color.White * (float)((float)(40 - d) / 40f));
        }

        public void DrawTrajectory(MicroWorld.Graphics.Renderer renderer)
        {
            var p = parent as Magnet;
            if (p.connector != null)
            {
                if (p.connector.ConnectedComponent1 is Properties.IRotator)
                    (p.connector.ConnectedComponent1 as Properties.IRotator).DrawTrajectory(renderer);
                if (p.connector.ConnectedComponent2 is Properties.IRotator)
                    (p.connector.ConnectedComponent2 as Properties.IRotator).DrawTrajectory(renderer);
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
                    renderer.Draw(texture0cw, new Vector2(x, y) + GetCenter(parent.ComponentRotation), null,
                        new Color(1f, 1f, 1f, 0.5f), rotation.GetHashCode() * (float)Math.PI / 2f,
                        GetCenter(parent.ComponentRotation), 1);
                    break;
            }
        }

    }
}
