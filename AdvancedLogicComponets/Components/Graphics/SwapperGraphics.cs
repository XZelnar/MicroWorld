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
    class SwapperGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, texture90cw;
        public static Texture2D textureConnection1_0cw, textureConnection1_90cw;
        public static Texture2D textureConnection2_0cw, textureConnection2_90cw;

        public SwapperGraphics()
        {
            Size = new Vector2(56, 40);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/Swapper/Swapper0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/Swapper/Swapper90cw");

            textureConnection1_0cw = ComponentsManager.LoadTexture("Components/Swapper/SwapperConnection1_0cw");
            textureConnection1_90cw = ComponentsManager.LoadTexture("Components/Swapper/SwapperConnection1_90cw");

            textureConnection2_0cw = ComponentsManager.LoadTexture("Components/Swapper/SwapperConnection2_0cw");
            textureConnection2_90cw = ComponentsManager.LoadTexture("Components/Swapper/SwapperConnection2_90cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/Swapper";
        }

        public override string GetCSToolTip()
        {
            return "Swapper";
        }

        public override string GetComponentSelectorPath()
        {
            return "Logics";
        }

        public override string GetHandbookFile()
        {
            return "Components/Swapper.edf";
        }

        public override Component.Rotation[] GetPossibleRotations()
        {
            return new Component.Rotation[] { Component.Rotation.cw0, Component.Rotation.cw90 };
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
                    return new Vector2();
            }
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(56, 40);
            else
                return new Vector2(40, 56);
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            Components.Swapper p = (Components.Swapper)parent;
            Components.Logics.SwapperLogics l = (Components.Logics.SwapperLogics)parent.Logics;

            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            Color.White);
                    if (p.Swapped)
                        renderer.Draw(textureConnection2_0cw,
                            new Rectangle((int)Position.X, (int)Position.Y,
                                (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                                Color.Red);
                    else
                        renderer.Draw(textureConnection1_0cw,
                            new Rectangle((int)Position.X, (int)Position.Y,
                                (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                                Color.Red);
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw, 
                        new Rectangle((int)Position.X, (int)Position.Y, 
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null, 
                            Color.White);
                    if (p.Swapped)
                        renderer.Draw(textureConnection2_90cw,
                            new Rectangle((int)Position.X, (int)Position.Y,
                                (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                                Color.Red);
                    else
                        renderer.Draw(textureConnection1_90cw,
                            new Rectangle((int)Position.X, (int)Position.Y,
                                (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                                Color.Red);
                    break;
                case Component.Rotation.cw180:
                    break;
                case Component.Rotation.cw270:
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
                    break;
                case Component.Rotation.cw270:
                    break;
                default:
                    break;
            }
        }
    }
}
