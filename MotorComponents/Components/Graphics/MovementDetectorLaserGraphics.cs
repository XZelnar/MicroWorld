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

namespace MicroWorld.Components.Graphics
{
    class MovementDetectorLaserGraphics : GraphicalComponent
    {
        public override Vector2 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;
                (parent as MovementDetectorLaser).UpdateCollider();
            }
        }

        public MovementDetectorLaserGraphics()
        {
            Size = new Vector2(0, 0);
            Layer = 100;
        }

        public override bool ShouldDisplayInCS()
        {
            return false;
        }

        float[] rand = new float[32];
        int tb = 0;
        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            if (!CanDraw()) 
                return;
            MovementDetectorLaser p = parent as MovementDetectorLaser;
            if (p.Length == 0)
                return;

            renderer.End();
            Matrix projection = Matrix.CreateOrthographicOffCenter(0,
                Main.WindowWidth, Main.WindowHeight, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            MicroWorld.Graphics.Effects.Effects.laser.Parameters["MatrixTransform"].SetValue(renderer.GetTransformMatrix() * halfPixelOffset * projection);
            float pw = 1f / Main.WindowWidth;
            MicroWorld.Graphics.Effects.Effects.laser.Parameters["halfpixel"].SetValue(new float[] { pw / 2f, 0.5f / Main.WindowHeight });
            MicroWorld.Graphics.Effects.Effects.laser.Parameters["horizontal"].SetValue(p.type == MovementDetectorLaser.Direction.Left || p.type == MovementDetectorLaser.Direction.Right);
            Random r = new Random();
            if (tb == 4)
            {
                tb = 0;
                for (int i = 0; i < rand.Length; i++)
                    rand[i] = r.Next(256);
            }
            tb++;
            MicroWorld.Graphics.Effects.Effects.laser.Parameters["random"].SetValue(rand);
            renderer.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, 
                MicroWorld.Graphics.Effects.Effects.laser);

            int dis = 1;
            switch (p.type)
            {
                case MovementDetectorLaser.Direction.Up:
                    renderer.Draw(Shortcuts.pixel, new Rectangle((int)Position.X - dis, (int)Position.Y - p.Length, dis * 2, p.Length), Color.White);
                    for (int i = 0; i < 2; i++)
                    {
                        MicroWorld.Graphics.ParticleManager.Add(
                            new MicroWorld.Graphics.Particles.Spark(
                                Position + new Vector2(r.Next(5) - 2, -r.Next(p.Length)),
                                new Vector2(1, 1),
                                new Vector2(((float)r.NextDouble() - 0.5f) / 4, ((float)r.NextDouble() - 0.5f) / 4),
                                (float)(r.NextDouble() + 0.1f) / 10,
                                Color.Red));
                    }
                    break;
                case MovementDetectorLaser.Direction.Left:
                    renderer.Draw(Shortcuts.pixel, new Rectangle((int)Position.X - p.Length, (int)Position.Y - dis, p.Length, dis * 2), Color.White);
                    for (int i = 0; i < 2; i++)
                    {
                        MicroWorld.Graphics.ParticleManager.Add(
                            new MicroWorld.Graphics.Particles.Spark(
                                Position + new Vector2(-r.Next(p.Length), r.Next(5) - 2),
                                new Vector2(1, 1),
                                new Vector2(((float)r.NextDouble() - 0.5f) / 4, ((float)r.NextDouble() - 0.5f) / 4),
                                (float)(r.NextDouble() + 0.1f) / 10,
                                Color.Red));
                    }
                    break;
                case MovementDetectorLaser.Direction.Down:
                    renderer.Draw(Shortcuts.pixel, new Rectangle((int)Position.X - dis, (int)Position.Y, dis * 2, p.Length), Color.White);
                    for (int i = 0; i < 2; i++)
                    {
                        MicroWorld.Graphics.ParticleManager.Add(
                            new MicroWorld.Graphics.Particles.Spark(
                                Position + new Vector2(r.Next(5) - 2, r.Next(p.Length)),
                                new Vector2(1, 1),
                                new Vector2(((float)r.NextDouble() - 0.5f) / 4, ((float)r.NextDouble() - 0.5f) / 4),
                                (float)(r.NextDouble() + 0.1f) / 10,
                                Color.Red));
                    }
                    break;
                case MovementDetectorLaser.Direction.Right:
                    renderer.Draw(Shortcuts.pixel, new Rectangle((int)Position.X, (int)Position.Y - dis, p.Length, dis * 2), Color.White);
                    for (int i = 0; i < 2; i++)
                    {
                        MicroWorld.Graphics.ParticleManager.Add(
                            new MicroWorld.Graphics.Particles.Spark(
                                Position + new Vector2(r.Next(p.Length), r.Next(5) - 2),
                                new Vector2(1, 1),
                                new Vector2(((float)r.NextDouble() - 0.5f) / 4, ((float)r.NextDouble() - 0.5f) / 4),
                                (float)(r.NextDouble() + 0.1f) / 10,
                                Color.Red));
                    }
                    break;
                default:
                    break;
            }
            renderer.End();
            renderer.Begin();
        }

    }
}
