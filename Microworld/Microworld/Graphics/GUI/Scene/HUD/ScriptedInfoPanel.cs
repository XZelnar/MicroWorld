using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MicroWorld.Graphics.GUI.Scene
{
    class ScriptedInfoPanel : HUDScene
    {
        static int curID = 0;
        internal static SpriteFont font;

        internal int ID = 0;
        private String text = "";
        public String Text
        {
            get { return text; }
            set
            {
                text = value;
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];
                    if (c == '\r')
                    {
                        text = text.Substring(0, i) + text.Substring(i + 1);
                        i--;
                    }
                    else if (c == '\n')
                    {
                        text = text.Substring(0, i) + "\r\n" + text.Substring(i + 1);
                        i++;
                    }
                    else if (c < 32 || c > 126)
                        text = text.Substring(0, i) + "?" + text.Substring(i + 1);
                }
                l.text = text;
                l.TextAlignment = Renderer.TextAlignment.Center;
                l.size = size;
            }
        }
        Elements.Label l;

        Vector2 pos = new Vector2(), size = new Vector2(80, 40);

        public ScriptedInfoPanel()
        {
            ID = curID++;
        }

        public override void Initialize()
        {
            isVisible = true;
            Layer = 800;

            l = new Elements.Label(0, 0, text);
            l.font = font;
            l.foreground = Color.White;
            l.TextAlignment = Renderer.TextAlignment.Center;
            l.size = size;
            controls.Add(l);

            base.Initialize();
        }

        public Vector2 GetPosition()
        {
            return pos;
        }

        public Vector2 GetSize()
        {
            return size;
        }

        public void SetPosition(Vector2 v)
        {
            pos = v;
            l.position = v;
        }

        public void SetSize(Vector2 v)
        {
            size = v;
            l.size = size;
            l.TextAlignment = Renderer.TextAlignment.Center;
        }

        public override void Draw(Renderer renderer)
        {
            RenderHelper.SmartDrawRectangle(GraphicsEngine.bg, 8, (int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y, Color.White, renderer);
            base.Draw(renderer);
        }
    }
}
