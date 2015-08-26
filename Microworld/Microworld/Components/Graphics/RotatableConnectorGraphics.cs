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
    class RotatableConnectorGraphics : GraphicalComponent
    {
        public static int[] PORTS_STATE_POSITION = new int[] { 
            0, 8,
            48, 8
        };

        public RotatableConnectorGraphics()
        {
            Size = new Vector2(48, 32);
            Layer = 150;
        }

        public static void LoadContentStatic()
        {
        }

        public override string GetIconName()
        {
            return "Components/Icons/Connector";
        }

        public override string GetCSToolTip()
        {
            return "Connector";
        }

        public override string GetComponentSelectorPath()
        {
            return "Motors:Rotatable";
        }

        public override string GetHandbookFile()
        {
            return "Components/Connector.edf";
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

        public override Component.Rotation[] GetPossibleRotations()
        {
            return new Component.Rotation[] { Component.Rotation.cw0 };
        }

        public override bool CanDraw()
        {
            var p = parent as RotatableConnector;
            if (p.ConnectedComponent1 == null)
                return false;
            return p.ConnectedComponent1.Graphics.CanDraw() || p.ConnectedComponent2.Graphics.CanDraw();
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (!CanDraw()) return;

            var p = parent as RotatableConnector;
            DrawConnector(
                (int)(p.ConnectedComponent1.Graphics.Position.X + p.ConnectedComponent1.Graphics.GetSize().X / 2), 
                (int)(p.ConnectedComponent1.Graphics.Position.Y + p.ConnectedComponent1.Graphics.GetSize().Y / 2),
                (int)(p.ConnectedComponent2.Graphics.Position.X + p.ConnectedComponent2.Graphics.GetSize().X / 2),
                (int)(p.ConnectedComponent2.Graphics.Position.Y + p.ConnectedComponent2.Graphics.GetSize().Y / 2),
                renderer, 1f);
        }

        private const int CONNECTION_CIRCLE_SIZE = 4;
        public void DrawConnector(int x1, int y1, int x2, int y2, MicroWorld.Graphics.Renderer renderer, float opacity)
        {
            Color c = new Color(1f, 1f, 1f) * opacity;

            renderer.Draw(MicroWorld.Graphics.GraphicsEngine.circle,
                new Rectangle(x1 - CONNECTION_CIRCLE_SIZE, y1 - CONNECTION_CIRCLE_SIZE,
                    CONNECTION_CIRCLE_SIZE * 2, CONNECTION_CIRCLE_SIZE * 2), c);
            renderer.Draw(MicroWorld.Graphics.GraphicsEngine.circle,
                new Rectangle(x2 - CONNECTION_CIRCLE_SIZE, y2 - CONNECTION_CIRCLE_SIZE,
                    CONNECTION_CIRCLE_SIZE * 2, CONNECTION_CIRCLE_SIZE * 2), c);

            VertexPositionColorTexture[] arr = new VertexPositionColorTexture[12];

            arr[0] = new VertexPositionColorTexture(new Vector3(x1 - 2, y1 - 2, 0), c, new Vector2());
            arr[1] = new VertexPositionColorTexture(new Vector3(x2 - 2, y2 - 2, 0), c, new Vector2());
            arr[2] = new VertexPositionColorTexture(new Vector3(x1 + 2, y1 + 2, 0), c, new Vector2());

            arr[3] = new VertexPositionColorTexture(new Vector3(x1 + 2, y1 + 2, 0), c, new Vector2());
            arr[4] = new VertexPositionColorTexture(new Vector3(x2 - 2, y2 - 2, 0), c, new Vector2());
            arr[5] = new VertexPositionColorTexture(new Vector3(x2 + 2, y2 + 2, 0), c, new Vector2());

            arr[6] = new VertexPositionColorTexture(new Vector3(x1 + 2, y1 - 2, 0), c, new Vector2());
            arr[7] = new VertexPositionColorTexture(new Vector3(x2 + 2, y2 - 2, 0), c, new Vector2());
            arr[8] = new VertexPositionColorTexture(new Vector3(x1 - 2, y1 + 2, 0), c, new Vector2());

            arr[9] = new VertexPositionColorTexture(new Vector3(x1 - 2, y1 + 2, 0), c, new Vector2());
            arr[10] = new VertexPositionColorTexture(new Vector3(x2 + 2, y2 - 2, 0), c, new Vector2());
            arr[11] = new VertexPositionColorTexture(new Vector3(x2 - 2, y2 + 2, 0), c, new Vector2());

            Main.renderer.Draw(MicroWorld.Graphics.GraphicsEngine.pixel, new Rectangle(0, 0, 0, 0), Color.White);
            bool a = renderer.IsDrawing;
            bool b = renderer.IsScaeld;
            if (renderer.IsDrawing) renderer.End();

            renderer.GraphicsDevice.PresentationParameters.MultiSampleCount = 4;
            renderer.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList,
                arr, 0, 4);

            if (a) renderer.Begin(b);
        }

        public override void DrawBorder(MicroWorld.Graphics.Renderer renderer)
        {
            var p = parent as RotatableConnector;
            if (p.ConnectedComponent1 != null && p.ConnectedComponent2 != null)
                DrawBorder(p.ConnectedComponent1.Graphics.Position + p.ConnectedComponent1.Graphics.GetSize() / 2,
                    p.ConnectedComponent2.Graphics.Position + p.ConnectedComponent2.Graphics.GetSize() / 2,
                    renderer);
        }

        public static void DrawBorder(Vector2 p1, Vector2 p2, MicroWorld.Graphics.Renderer renderer)
        {
            MicroWorld.Graphics.RenderHelper.DrawDottedLinesToBorders(new Vector2[] { p1, p2 }, Color.White, renderer, true);
        }

        public override void DrawGhost(int x, int y, MicroWorld.Graphics.Renderer renderer, Component.Rotation rotation)
        {
        }

    }
}
