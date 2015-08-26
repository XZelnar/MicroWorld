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
    class ReedSwitchGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, texture90cw;
        public static Texture2D textureConnected0cw, textureConnected90cw;

        public ReedSwitchGraphics()
        {
            Size = new Vector2(40, 24);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/ReedSwitch/ReedSwitch0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/ReedSwitch/ReedSwitch90cw");
            textureConnected0cw = ComponentsManager.LoadTexture("Components/ReedSwitch/ReedSwitchConnected0cw");
            textureConnected90cw = ComponentsManager.LoadTexture("Components/ReedSwitch/ReedSwitchConnected90cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/ReedSwitch";
        }

        public override string GetCSToolTip()
        {
            return "Reed Switch";
        }

        public override string GetComponentSelectorPath()
        {
            return "Magnets";
        }

        public override string GetHandbookFile()
        {
            return "Components/ReedSwitch.edf";
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
                case Component.Rotation.cw180:
                    return new Vector2(16, 8);
                case Component.Rotation.cw90:
                case Component.Rotation.cw270:
                    return new Vector2(8, 16);
                default:
                    return new Vector2();
            }
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(40, 24);
            else
                return new Vector2(24, 40);
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            ReedSwitch d = parent as ReedSwitch;

            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(d.W.IsConnected ? textureConnected0cw : texture0cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.White);
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(d.W.IsConnected ? textureConnected90cw : texture90cw,
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
