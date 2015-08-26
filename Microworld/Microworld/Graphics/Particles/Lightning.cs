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
    public class Lightning : Particle
    {
        int GEN_DEPTH = 6;
        const int MUTATE_POWER = 2;

        Vector2[] vertices = null;
        int[] indices = null;

        public int RemainingTime
        {
            get { return MaxLife - curLife; }
        }

        private int curLife = 0, MaxLife = 60;
        private float w = 0;
        RenderTarget2D fbo;
        Matrix transform = new Matrix();
        float rot = 0;
        Sound.EffectInstance _sound;

        public Lightning(Vector2 p1, Vector2 p2, float width = 2f, int life = 60, int detailLevel = 6, bool sound = false, 
            float volume = 0.1f)
        {
            sound = false;//TODO rm

            MaxLife = life;
            w = width;
            GEN_DEPTH = detailLevel;

            Position = p1;

            float l = (p2 - p1).Length() * 4;
            rot = (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            transform = Matrix.CreateRotationZ(rot);
            fbo = new RenderTarget2D(GraphicsEngine.Renderer.GraphicsDevice, (int)l + 64, (int)l + 64);

            GenLightning(new Vector2(32, 32 + l / 2), new Vector2(32 + l, 32 + l / 2));
            var r = GraphicsEngine.Renderer;
            r.EnableFBO(fbo);
            r.GraphicsDevice.Clear(Color.Transparent);
            r.BeginUnscaled();
            DrawFBO(r);
            r.End();
            r.DisableFBO();

            if (sound)
                _sound = Sound.Sounds.tesla.Play(volume, (float)(new Random().Next(-250, 250)) / 1000f, 0f);
        }

        public void Move(Vector2 p1, Vector2 p2)
        {
            Position = p1;

            float oldw = fbo.Width - 64;
            float l = (p2 - p1).Length() * 4;
            float k = l / oldw;
            rot = (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
            transform = Matrix.CreateRotationZ(rot);
            fbo = new RenderTarget2D(GraphicsEngine.Renderer.GraphicsDevice, (int)l + 64, (int)l + 64);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] *= k;
            }

            //GenLightning(new Vector2(32, 32 + l / 2), new Vector2(32 + l, 32 + l / 2));
            var r = GraphicsEngine.Renderer;
            r.EnableFBO(fbo);
            r.GraphicsDevice.Clear(Color.Transparent);
            r.BeginUnscaled();
            DrawFBO(r);
            r.End();
            r.DisableFBO();
        }

        public override void Dispose()
        {
            fbo.Dispose();
            if (_sound != null)
            {
                _sound.Stop();
                _sound.Dispose();
                _sound = null;
            }
            base.Dispose();
        }

        public override void InGameUpdate()
        {
            base.Update();

            MutateLightning();
            var r = GraphicsEngine.Renderer;
            r.EnableFBO(fbo);
            r.GraphicsDevice.Clear(Color.Transparent);
            r.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
            DrawFBO(r);
            r.End();
            r.DisableFBO();
            //var a = new System.IO.FileStream("C:/tp.png", System.IO.FileMode.Create);
            //fbo.SaveAsPng(a, fbo.Width, fbo.Height);
            //a.Close();

            curLife++;
            if (curLife >= MaxLife)
            {
                IsDead = true;
                if (_sound != null)
                {
                    _sound.Stop();
                    _sound.Dispose();
                    Sound.SoundManager.instances.Remove(_sound);
                    _sound = null;
                }
            }
        }

        public override void Draw(Renderer r)
        {
            Matrix projection = Matrix.CreateOrthographicOffCenter(0,
                Main.renderer.GraphicsDevice.Viewport.Width, Main.renderer.GraphicsDevice.Viewport.Height, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
            Effects.Effects.lightning.Parameters["MatrixTransform"].SetValue(r.GetTransformMatrix() * halfPixelOffset * projection);
            float[] p = new float[]{
                    1f/fbo.Width, 
                    1f/fbo.Height};
            Effects.Effects.lightning.Parameters["pixel"].SetValue(p);

            r.End();
            r.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, 
                Effects.Effects.lightning);
            Vector2 o = new Vector2(32, fbo.Height / 2);
            r.Draw(fbo, new Rectangle((int)Position.X, (int)Position.Y, fbo.Width / 4, fbo.Height / 4), null,
                Color.White, rot, o);
            r.End();
            r.Begin();
        }

        public void DrawFBO(Renderer r)
        {
            VertexPositionColorTexture[] v = new VertexPositionColorTexture[indices.Length * 3];
            VertexPositionColorTexture v1, v2, v3, v4;

            for (int i = 0; i < indices.Length; i += 2)
            {
                v1 = new VertexPositionColorTexture(new Vector3(vertices[indices[i]].X, vertices[indices[i]].Y - w, 0), Color.White, new Vector2());
                v2 = new VertexPositionColorTexture(new Vector3(vertices[indices[i]].X, vertices[indices[i]].Y + w, 0), Color.White, new Vector2());
                v3 = new VertexPositionColorTexture(new Vector3(vertices[indices[i + 1]].X, vertices[indices[i + 1]].Y - w, 0), Color.White, new Vector2());
                v4 = new VertexPositionColorTexture(new Vector3(vertices[indices[i + 1]].X, vertices[indices[i + 1]].Y + w, 0), Color.White, new Vector2());

                v[i * 3 + 0] = v1;
                v[i * 3 + 1] = v2;
                v[i * 3 + 2] = v3;
                v[i * 3 + 3] = v2;
                v[i * 3 + 4] = v3;
                v[i * 3 + 5] = v4;
            }

            r.GraphicsDevice.Textures[0] = Graphics.GraphicsEngine.pixel;
            r.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, v, 0, v.Length / 3);
        }

        public void GenLightning(Vector2 p1, Vector2 p2)
        {
            List<Vector2> v = new List<Vector2>();
            List<int> s = new List<int>();
            List<int> s2 = new List<int>();

            v.Add(p1);
            v.Add(p2);
            s.Add(0);
            s.Add(1);
            int depth = 1;
            Random r = new Random();
            float maxrand = (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y + p2.Y, 2)) / 8;
            Vector2 j;
            int ti = 0;
            while (depth < GEN_DEPTH)
            {
                for (int i = 0; i < s.Count - 1; i += 2)
                {
                    j = new Vector2((v[s[i]].X + v[s[i + 1]].X) / 2,
                                    (v[s[i]].Y + v[s[i + 1]].Y) / 2 + ((float)r.NextDouble() - 0.5f) * maxrand / depth * 2);
                    ti = v.Count;
                    v.Add(j);
                    s2.Add(s[i]);
                    s2.Add(ti);
                    s2.Add(ti);
                    s2.Add(s[i + 1]);
                    if (v[s[i + 1]].X < p2.X - 20 &&  r.Next(3) == 0)
                    {
                        j = new Vector2(v[s[i + 1]].X,
                                        v[s[i + 1]].Y + ((float)r.NextDouble() - 0.5f) * maxrand * 2f / depth);
                        s2.Add(ti);
                        ti = v.Count;
                        v.Add(j);
                        s2.Add(ti);
                    }
                }
                s.Clear();
                s.AddRange(s2);
                s2.Clear();
                depth++;
            }

            vertices = v.ToArray();
            indices = s.ToArray();
        }

        public void MutateLightning()
        {
            Random r = new Random();
            int t = 0;
            for (int i = 2; i < vertices.Length; i++)
            {
                t = r.Next(-MUTATE_POWER, MUTATE_POWER + 1);
                vertices[i].Y += t;
            }
        }
    }
}
