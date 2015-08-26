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
    class TeslaCoilGraphics : GraphicalComponent
    {
        public static Texture2D texture0cw;

        public TeslaCoilGraphics()
        {
            Size = new Vector2(40, 40);
            Layer = 0;
        }

        public static void LoadContentStatic()
        {
            texture0cw = ComponentsManager.LoadTexture("Components/TeslaCoil/TeslaCoil0cw");
        }

        public override string GetIconName()
        {
            return "Components/Icons/TeslaCoil";
        }

        public override string GetCSToolTip()
        {
            return "Tesla coil";
        }

        public override string GetComponentSelectorPath()
        {
            return "Advanced";
        }

        public override string GetHandbookFile()
        {
            return "Components/Tesla Coil.edf";
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

        public override void MoveVisually(Vector2 d)
        {
            base.MoveVisually(d);
            (parent.Logics as Logics.TeslaCoilLogics).MoveParticles(d);
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

        public override void DrawBorder(MicroWorld.Graphics.Renderer renderer)
        {
            base.DrawBorder(renderer);

            float a = (float)((Main.Ticks % 1200) * Math.PI / 600f);
            float r = (parent.Logics as Logics.TeslaCoilLogics).Range;
            MicroWorld.Graphics.RenderHelper.DrawDottedCircle(r, Center, (int)(r / 2), a, renderer, Color.White);
        }

        public override void DrawGhost(int x, int y, MicroWorld.Graphics.Renderer renderer, Component.Rotation rotation)
        {
            if (texture0cw == null) return;

            renderer.Draw(texture0cw, new Rectangle(x, y, (int)GetSizeRotated(rotation).X, (int)GetSizeRotated(rotation).Y),
                new Color(1f, 1f, 1f, 0.5f));
        }
    }
}
