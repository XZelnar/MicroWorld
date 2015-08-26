﻿using System;
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
    class HESGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw;

        public HESGraphics()
        {
            Size = new Vector2(24, 24);
            Layer = 0;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/HES/HES0cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/HES";
        }

        public override string GetCSToolTip()
        {
            return "Hall Effect Sensor";
        }

        public override string GetComponentSelectorPath()
        {
            return "Magnets";
        }

        public override string GetHandbookFile()
        {
            return "Components/Hall Effect Sensor.edf";
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            return new Vector2(8, 8);
        }

        public override Vector2 GetSize()
        {
            return new Vector2(24, 24);
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            return new Vector2(24, 24);
        }

        public override Component.Rotation[] GetPossibleRotations()
        {
            return new Component.Rotation[] { Component.Rotation.cw0 };
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (texture0cw == null) return;
            if (!CanDraw()) return;

            renderer.Draw(texture0cw,
                new Rectangle((int)Position.X, (int)Position.Y,
                    (int)GetSizeRotated(parent.ComponentRotation).X, (int)GetSizeRotated(parent.ComponentRotation).Y), null,
                    Color.White);
        }

        public override void DrawGhost(int x, int y, MicroWorld.Graphics.Renderer renderer, Component.Rotation rotation)
        {
            if (texture0cw == null) return;

            renderer.Draw(texture0cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                new Color(1f, 1f, 1f, 0.5f));
        }
    }
}