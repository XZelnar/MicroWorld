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
    class PulseCoreGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw, texture90cw, texture180cw, texture270cw;
        public static Texture2D textureCenter0cw, textureCenter90cw, textureCenter180cw, textureCenter270cw;

        public PulseCoreGraphics()
        {
            Size = new Vector2(48, 24);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/PulseCore/PulseCore0cw");
            texture90cw = ComponentsManager.LoadTexture("Components/PulseCore/PulseCore90cw");
            texture180cw = ComponentsManager.LoadTexture("Components/PulseCore/PulseCore180cw");
            texture270cw = ComponentsManager.LoadTexture("Components/PulseCore/PulseCore270cw");

            textureCenter0cw = ComponentsManager.LoadTexture("Components/PulseCore/PulseCoreCenter0cw");
            textureCenter90cw = ComponentsManager.LoadTexture("Components/PulseCore/PulseCoreCenter90cw");
            textureCenter180cw = ComponentsManager.LoadTexture("Components/PulseCore/PulseCoreCenter180cw");
            textureCenter270cw = ComponentsManager.LoadTexture("Components/PulseCore/PulseCoreCenter270cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/PulseCore";
        }

        public override string GetCSToolTip()
        {
            return "Pulse Core";
        }

        public override string GetComponentSelectorPath()
        {
            return "Cores";
        }

        public override string GetHandbookFile()
        {
            return "Components/PulseCore.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            if (rotation == Component.Rotation.cw0 || rotation == Component.Rotation.cw180)
                return new Vector2(24, 8);
            else
                return new Vector2(8, 24);
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
            Components.Logics.PulseCoreLogics l = (Components.Logics.PulseCoreLogics)parent.Logics;
            Color c = l.IsComplete ? Color.Lime : l.LastActiveFor > 0 ? Color.Red : Color.DarkRed;

            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    renderer.Draw(texture0cw, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), null, Color.White);
                    renderer.Draw(textureCenter0cw, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), null, c);
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), null, Color.White);
                    renderer.Draw(textureCenter90cw, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), null, c);
                    break;
                case Component.Rotation.cw180:
                    renderer.Draw(texture180cw, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), null, Color.White);
                    renderer.Draw(textureCenter180cw, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), null, c);
                    break;
                case Component.Rotation.cw270:
                    renderer.Draw(texture270cw, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), null, Color.White);
                    renderer.Draw(textureCenter270cw, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y), null, c);
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
                    renderer.Draw(texture0cw, new Rectangle(x, y, 48, 24), new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(textureCenter0cw, new Rectangle(x, y, 48, 24), new Color(0.8f, 0f, 0f, 0.5f));
                    break;
                case Component.Rotation.cw90:
                    renderer.Draw(texture90cw, new Rectangle(x, y, 24, 48), new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(textureCenter90cw, new Rectangle(x, y, 24, 48), new Color(0.8f, 0f, 0f, 0.5f));
                    break;
                case Component.Rotation.cw180:
                    renderer.Draw(texture180cw, new Rectangle(x, y, 48, 24), new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(textureCenter180cw, new Rectangle(x, y, 48, 24), new Color(0.8f, 0f, 0f, 0.5f));
                    break;
                case Component.Rotation.cw270:
                    renderer.Draw(texture270cw, new Rectangle(x, y, 24, 48), new Color(1f, 1f, 1f, 0.5f));
                    renderer.Draw(textureCenter270cw, new Rectangle(x, y, 24, 48), new Color(0.8f, 0f, 0f, 0.5f));
                    break;
                default:
                    break;
            }
        }

    }
}
