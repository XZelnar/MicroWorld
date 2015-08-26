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
    class PhotoresistorGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, texture90cw, texture180cw, texture270cw;

        public PhotoresistorGraphics()
        {
            Size = new Vector2(48, 40);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/Photoresistor/Photoresistor0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/Photoresistor/Photoresistor90cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/Photoresistor";
        }

        public override string GetCSToolTip()
        {
            return "Photoresistor";
        }

        public override string GetComponentSelectorPath()
        {
            return "Light-Emitting";
        }

        public override string GetHandbookFile()
        {
            return "Components/Photoresistor.edf";
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
            Photoresistor p = parent as Photoresistor;
            
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
                case Component.Rotation.cw270:
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
                case Component.Rotation.cw270:
                default:
                    break;
            }
        }

    }
}
