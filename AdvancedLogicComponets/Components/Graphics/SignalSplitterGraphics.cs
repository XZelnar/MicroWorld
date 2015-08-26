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
    class SignalSplitterGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw;
        public static Texture2D textureTop0cw;
        public static Texture2D textureBottom0cw;

        public SignalSplitterGraphics()
        {
            Size = new Vector2(24, 24);
            Layer = 50;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/SignalSplitter/SignalSplitter0cw");

            textureTop0cw = ComponentsManager.LoadTexture("Components/SignalSplitter/SignalSplitterTop0cw");

            textureBottom0cw = ComponentsManager.LoadTexture("Components/SignalSplitter/SignalSplitterBottom0cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/SignalSplitter";
        }

        public override string GetCSToolTip()
        {
            return "Signal Splitter";
        }

        public override string GetComponentSelectorPath()
        {
            return "Logics";
        }

        public override string GetHandbookFile()
        {
            return "Components/Signal Splitter.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            return new Vector2(8, 8);
        }

        public override Vector2 GetSize()
        {
            return new Vector2(24, 24);
        }

        public override Component.Rotation[] GetPossibleRotations()
        {
            return new Component.Rotation[] { Component.Rotation.cw0 };
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            return new Vector2(24, 24);
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;
            Components.Logics.SignalSplitterLogics l = (Components.Logics.SignalSplitterLogics)parent.Logics;

            renderer.Draw(texture0cw,
                new Rectangle((int)Position.X, (int)Position.Y,
                    (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                    Color.White);
            //topbg
            renderer.Draw(textureTop0cw,
                new Rectangle((int)Position.X, (int)Position.Y,
                    (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                    Color.DarkRed);
            //bottombg
            renderer.Draw(textureBottom0cw,
                new Rectangle((int)Position.X, (int)Position.Y,
                    (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                    Color.DarkRed);
            if (Settings.GameState != Settings.GameStates.Stopped)
            {
                //top
                if (l.IsTopActive)
                {
                    renderer.Draw(textureTop0cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.Red * (float)(l.v1 / 5));
                }
                //bottom
                if (l.IsBottomActive)
                {
                    renderer.Draw(textureBottom0cw,
                        new Rectangle((int)Position.X, (int)Position.Y,
                            (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                            Color.Red * (float)(l.v1 / 5));
                }
            }
        }

        public override void DrawGhost(int x, int y, MicroWorld.Graphics.Renderer renderer, Component.Rotation rotation)
        {
            if (texture0cw == null) return;

            renderer.Draw(texture0cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                new Color(1f, 1f, 1f, 0.5f));
            renderer.Draw(textureTop0cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                new Color(0.5f, 0f, 0f, 0.5f));
            renderer.Draw(textureBottom0cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                new Color(0.5f, 0f, 0f, 0.5f));
        }
    }
}
