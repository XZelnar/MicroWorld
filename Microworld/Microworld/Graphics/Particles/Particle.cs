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

namespace MicroWorld.Graphics.Particles
{
    public abstract class Particle
    {
        public bool IsDead = false;
        public Vector2 Position;
        public Vector2 Size;
        public Vector2 Speed;

        public virtual void Initialize()
        {
        }

        public virtual void LoadContent()
        {
        }

        public virtual void Update()
        {
            Position += Speed;
        }

        public virtual void InGameUpdate()
        {
            Position += Speed;
        }

        public virtual void Draw(Renderer r)
        {
        }

        public virtual void Dispose()
        {
        }
    }
}
