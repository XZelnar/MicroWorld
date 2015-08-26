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
    class JointGraphics : GraphicalComponent
    {
        public static Texture2D jointbg, jointoverlay;

        public JointGraphics()
        {
            Size = new Vector2(8, 8);
            Layer = 100;
        }

        public static void LoadContentStatic()
        {
            jointbg = ResourceManager.Load<Texture2D>("Components/Joint/Joint");
            jointoverlay = ResourceManager.Load<Texture2D>("Components/Joint/JointOverlay");
        }

        public override string GetIconName()
        {
            return "Components/Icons/Joint";
        }

        public override string GetCSToolTip()
        {
            return "Joint";
        }

        public override string GetComponentSelectorPath()
        {
            return null;
        }

        public override string GetHandbookFile()
        {
            return "Components/Joint.edf";
        }

        public override Vector2 GetSize()
        {
            return Size;
        }

        public override Vector2 GetSizeRotated(Component.Rotation rotation)
        {
            return Size;
        }

        public override Vector2 GetCenter(Component.Rotation rotation)
        {
            return new Vector2(0, 0);
        }

        public override Component.Rotation[] GetPossibleRotations()
        {
            return new Component.Rotation[] { Component.Rotation.cw0 };
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (jointbg == null) return;
            if (!CanDraw()) return;
            if (!Visible) return;
            renderer.Draw(jointbg, new Rectangle((int)Position.X, (int)Position.Y, 8, 8), Color.White);
            renderer.Draw(jointoverlay, new Rectangle((int)Position.X, (int)Position.Y, 8, 8), Color.Red * (float)((parent as Joint).Voltage / 5f));

            //renderer.DrawString(MicroWorld.Graphics.GUI.GUIEngine.font, Math.Round((parent as Joint).Voltage, 2).ToString(), Position, Color.Red, MicroWorld.Graphics.Renderer.TextAlignment.Left);
            /*
            String s = Math.Round(((Joint)parent).Voltage, 2).ToString();

            renderer.DrawString(MicroWorld.Graphics.GUI.Elements.TextBox.defaultFont, s,
                Position, Color.White);//*/
        }

        public override void DrawBorder(MicroWorld.Graphics.Renderer renderer)
        {
            MicroWorld.Graphics.RenderHelper.DrawDottedOutline(Position.X, Position.Y, 8, 8, false, renderer);
        }

        public override void DrawGhost(int x, int y, MicroWorld.Graphics.Renderer renderer, Component.Rotation rotation)
        {
            if (jointbg == null) return;
            renderer.Draw(jointbg, new Rectangle(x, y, 8, 8), new Color(1f, 1f, 1f, 0.5f));
        }
    }
}
