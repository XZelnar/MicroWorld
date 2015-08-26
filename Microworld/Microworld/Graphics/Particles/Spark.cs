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
    public class Spark : Particle
    {
        //public float color = 1f;
        Color color = Color.Yellow;
        float opacity = 1f;
        public float DyingSpeed = 0.01f;

        public Spark(Vector2 pos, Vector2 size, Vector2 motion, float dieSpeed)
        {
            Position = pos;
            Size = size;
            Speed = motion;
            DyingSpeed = dieSpeed;
        }

        public Spark(Vector2 pos, Vector2 size, Vector2 motion, float dieSpeed, Color c)
        {
            Position = pos;
            Size = size;
            Speed = motion;
            DyingSpeed = dieSpeed;
            color = c;
        }

        public override void Update()
        {
            base.Update();

            opacity -= DyingSpeed;
            if (opacity <= 0) IsDead = true;
        }

        public override void Draw(Renderer r)
        {
            base.Draw(r);

            r.Draw(GraphicsEngine.pixel, new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y),
                color * opacity);
        }
    }
}
