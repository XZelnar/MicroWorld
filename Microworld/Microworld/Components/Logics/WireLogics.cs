using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MicroWorld.Components.Logics
{
    class WireLogics : LogicalComponent
    {
        Random rand = new Random();
        MicroWorld.Graphics.Particles.Lightning[] particles = new MicroWorld.Graphics.Particles.Lightning[5];
        Graphics.WireGraphics g;

        public override void Initialize()
        {
            base.Initialize();

            g = parent.Graphics as Graphics.WireGraphics;
        }

        public override void CircuitUpdate()
        {
            base.CircuitUpdate();

            g.step = true;
        }

        public override void Update()
        {
            base.Update();

            if (parent.Graphics.Visible && (parent as Wire).WasEMPd && Main.Ticks % 5 == 0 && rand.Next(4) == 0)
            {
                var p = GetRandomPos();
                var l = new MicroWorld.Graphics.Particles.Lightning(p, p + new Vector2(rand.Next(-8, 8), rand.Next(-8, 8)), 2f, 20, 4);
                MicroWorld.Graphics.ParticleManager.Add(l);
                for (int i = 0; i < particles.Length; i++)
                {
                    if (particles[i] == null || particles[i].IsDead)
                    {
                        particles[i] = l;
                        break;
                    }
                }
            }
        }

        private Vector2 GetRandomPos()
        {
            var g = parent.Graphics as Graphics.WireGraphics;
            int i = rand.Next(g.DrawPath.Count - 1);
            int minx = (int)Math.Min(g.DrawPath[i].X, g.DrawPath[i + 1].X);
            int miny = (int)Math.Min(g.DrawPath[i].Y, g.DrawPath[i + 1].Y);
            int maxx = (int)Math.Max(g.DrawPath[i].X, g.DrawPath[i + 1].X);
            int maxy = (int)Math.Max(g.DrawPath[i].Y, g.DrawPath[i + 1].Y);
            return new Vector2(rand.Next(minx, maxx), rand.Next(miny, maxy));
        }

        public override void Reset()
        {
            base.Reset();

            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i] != null && !particles[i].IsDead)
                {
                    particles[i].Dispose();
                    MicroWorld.Graphics.ParticleManager.Remove(particles[i]);
                }
                particles[i] = null;
            }
        }
    }
}
