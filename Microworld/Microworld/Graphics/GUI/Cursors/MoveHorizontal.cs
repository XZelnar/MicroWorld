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
    class MoveCursorHorizontal : Cursor
    {
        public override void Initialize()
        {
            base.Initialize();
            offset = new Vector2(-10, -10);
            color = Color.Cyan;
        }

        public override void LoadContent()
        {
            texture = ResourceManager.Load<Texture2D>("GUI/Cursors/MoveHorizontal");
        }

        public override void OnShow()
        {
            s = 1;
            t = true;
            base.OnShow();
        }

        float s = 1f;
        bool t = true;
        public override void Update()
        {
            if (t)
            {
                s -= 0.01f;
                if (s <= 0.7f)
                    t = false;
            }
            else
            {
                s += 0.01f;
                if (s >= 1f)
                    t = true;
            }
            base.Update();
        }

        public override void Draw(Renderer renderer, Vector2 pos)
        {
            int ts = (int)(20 * s);
            int to = 10 - ts / 2;
            renderer.Draw(texture, new Rectangle((int)(pos.X + offset.X + to), (int)(pos.Y + offset.Y + to), ts, ts), color);
        }
    }
}
