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

namespace MicroWorld.Graphics.GUI.Cursors
{
    public abstract class Cursor
    {
        public Vector2 offset = new Vector2();
        public Texture2D texture;
        public Color color = Color.White;

        public virtual void Initialize()
        {
        }

        public virtual void LoadContent()
        {
        }

        public virtual void OnShow()
        {
        }

        public virtual void OnClose()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Draw(Renderer renderer, Vector2 pos)
        {
            renderer.Draw(texture, pos + offset, color);
        }
    }
}
