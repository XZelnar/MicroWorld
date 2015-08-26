using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MicroWorld.Graphics.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MicroWorld.Graphics
{
    public static class ParticleManager
    {
        private static List<Particle> particles = new List<Particle>();

        public static void Add(Particle p)
        {
            particles.Add(p);
        }

        public static void Remove(Particle p)
        {
            particles.Remove(p);
        }

        public static void Clear()
        {
            particles.Clear();
        }






        public static void Initialize()
        {
        }

        public static void LoadContent()
        {
            Smoke.texture = ResourceManager.Load<Texture2D>("Particles/Smoke");
        }

        public static void Update()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Update();
                if (particles[i].IsDead)
                {
                    particles[i].Dispose();
                    particles.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void InGameUpdate()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].InGameUpdate();
                if (particles[i].IsDead)
                {
                    particles.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void Draw()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw(Main.renderer);
            }
        }

        public static void Dispose()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Dispose();
            }
        }
    }
}
