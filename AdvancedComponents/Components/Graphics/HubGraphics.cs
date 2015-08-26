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
    class HubGraphics : GraphicalComponent
    {
        public static Texture2D textureBg, textureLeft, textureUp, textureRight, textureDown;

        public HubGraphics()
        {
            Size = new Vector2(40, 40);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            textureBg = ComponentsManager.LoadTexture("Components/Hub/HubBg0cw");

            textureLeft = ComponentsManager.LoadTexture("Components/Hub/HubLeft0cw");
            textureUp = ComponentsManager.LoadTexture("Components/Hub/HubUp0cw");
            textureRight = ComponentsManager.LoadTexture("Components/Hub/HubRight0cw");
            textureDown = ComponentsManager.LoadTexture("Components/Hub/HubDown0cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/Hub";
        }

        public override string GetCSToolTip()
        {
            return "Hub";
        }

        public override string GetComponentSelectorPath()
        {
            return "Advanced:Interactive";
        }

        public override string GetHandbookFile()
        {
            return "Components/Hub.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            return new Vector2(16, 16);
        }

        public override Vector2 GetSize()
        {
            return new Vector2(40, 40);
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            return new Vector2(40, 40);
        }

        public override Component.Rotation[] GetPossibleRotations()
        {
            return new Component.Rotation[] { Component.Rotation.cw0 };
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (textureBg == null) return;
            if (!CanDraw()) return;
            Hub d = parent as Hub;

            renderer.Draw(textureBg,
                new Rectangle((int)Position.X, (int)Position.Y,
                    (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                    Color.White);

            if (d.ConnectedLeft)
                renderer.Draw(textureLeft,
                    new Rectangle((int)Position.X, (int)Position.Y,
                        (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                        Color.White);

            if (d.ConnectedUp)
                renderer.Draw(textureUp,
                    new Rectangle((int)Position.X, (int)Position.Y,
                        (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                        Color.White);

            if (d.ConnectedRight)
                renderer.Draw(textureRight,
                    new Rectangle((int)Position.X, (int)Position.Y,
                        (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                        Color.White);

            if (d.ConnectedDown)
                renderer.Draw(textureDown,
                    new Rectangle((int)Position.X, (int)Position.Y,
                        (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                        Color.White);
        }

        public override void DrawGhost(int x, int y, MicroWorld.Graphics.Renderer renderer, Component.Rotation rotation)
        {
            if (textureBg == null) return;

            renderer.Draw(textureBg, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                new Color(1f, 1f, 1f, 0.5f));
            renderer.Draw(textureLeft, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                new Color(1f, 1f, 1f, 0.5f));
            renderer.Draw(textureUp, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                new Color(1f, 1f, 1f, 0.5f));
            renderer.Draw(textureRight, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                new Color(1f, 1f, 1f, 0.5f));
            renderer.Draw(textureDown, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                new Color(1f, 1f, 1f, 0.5f));
        }
    }
}
