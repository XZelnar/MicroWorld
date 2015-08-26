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

namespace MicroWorld.Graphics.Overlays
{
    abstract class Overlay
    {
        public bool IsDead = false;
        public Vector2 Position;
        public Vector2 Size;
        public int ID = 0;

        public virtual void Initialize()
        {
            ID = OverlayManager.GetID();
        }

        public virtual void LoadContent()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Draw(Renderer r)
        {
        }

        public virtual void Dispose()
        {
        }
    }
}
