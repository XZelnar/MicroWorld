using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MicroWorld.Components.Logics
{
    class JointLogics : LogicalComponent
    {
        static Random seedRandom = new Random();

        Random rand;
        MicroWorld.Graphics.Particles.Lightning[] particles = new MicroWorld.Graphics.Particles.Lightning[5];

        public JointLogics()
        {
            rand = new Random(seedRandom.Next());
        }

        public override void CircuitUpdate()
        {
            base.CircuitUpdate();

            if ((parent as Joint).WasEMPd)
                (parent as Joint).Voltage = rand.NextDouble() * 5;
        }

        public override void Update()
        {
            base.Update();

            if (parent.Graphics.Visible && (parent as Joint).WasEMPd && Main.Ticks % 5 == 0 && rand.Next(4) == 0)
            {
                var l = new MicroWorld.Graphics.Particles.Lightning(parent.Graphics.Position + new Vector2(4, 4), 
                    parent.Graphics.Position + new Vector2(4 + rand.Next(-8, 9), 4 + rand.Next(-8, 9)), 2f, 20, 4);
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
