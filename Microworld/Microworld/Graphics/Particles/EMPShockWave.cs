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
    public class EMPShockWave : Particle
    {
        public float radius = 0;
        public float thickness = 60;
        float speed = 0;
        public Color color = Color.White;
        Lightning[] particles = new Lightning[50];
        int startInd = 0;

        public EMPShockWave(Vector2 centerGamePos, float expansionSpeed, float thickness = 60)
        {
            Position = centerGamePos;
            speed = expansionSpeed;
            if (thickness < 3)
                thickness = 3;
            this.thickness = thickness;
        }

        public override void Dispose()
        {
            base.Dispose();

            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i] != null)
                {
                    ParticleManager.Remove(particles[i]);
                    particles[i].Dispose();
                    particles[i] = null;
                }
            }
        }

        public override void InGameUpdate()
        {
            base.Update();

            radius += speed;
            
            //generate lightnings
            Random r = new Random();
            float a = 0;
            Vector2 pos, del;
            Lightning l;
            for (int i = 0; i < 5; i++)
            {
                a = (float)(r.Next(360) * Math.PI / 180);
                pos = new Vector2((float)Math.Cos(a), (float)Math.Sin(a)) * (radius + thickness) + Position;
                del = new Vector2(r.Next(40, 50) * r.Next(2) == 0 ? 1 : -1, r.Next(40, 50) * r.Next(2) == 0 ? 1 : -1) * 8;
                l = new Lightning(pos, pos + del, 2, 10, 4);
                ParticleManager.Add(l);
                particles[i + startInd] = l;
            }
            startInd += 5;
            if (startInd >= particles.Length)
                startInd = 0;
        }

        public override void Draw(Renderer r)
        {
            r.End();
            _setUpShader();
            r.BeginUnscaled(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                    Graphics.Effects.Effects.shockWave);
            r.Draw(Shortcuts.pixel, new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight), color * 0.75f);
            r.End();
            r.Begin();
        }

        private void _setUpShader()
        {
            Matrix projection = Matrix.CreateOrthographicOffCenter(0,
                Main.WindowWidth, Main.WindowHeight, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            Graphics.Effects.Effects.shockWave.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);
            Graphics.Effects.Effects.shockWave.Parameters["innerRadius"].SetValue(radius * Settings.GameScale);
            Graphics.Effects.Effects.shockWave.Parameters["thickness"].SetValue(thickness);
            Graphics.Effects.Effects.shockWave.Parameters["center"].SetValue(Utilities.Tools.GameToScreenCoords(Position));
        }
    }
}
