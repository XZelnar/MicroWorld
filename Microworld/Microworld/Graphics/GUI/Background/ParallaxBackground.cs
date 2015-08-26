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

namespace MicroWorld.Graphics.GUI.Background
{
    class ParallaxBackground : Background
    {
        class PRectangle
        {
            public float x, y, w, h;
            public float opacity;
            public float speedx, speedy;
            public bool isDead;

            public static PRectangle GetNew()
            {
                PRectangle p = new PRectangle();

                p.h = p.w = rand.Next(100, 400);
                p.y = rand.Next(-(int)p.h, Main.WindowHeight);
                p.opacity = rand.Next(0, 3);
                if (p.opacity == 0)
                    p.opacity = 0.03f;
                else if (p.opacity == 1)
                    p.opacity = 0.06f;
                else if (p.opacity == 2)
                    p.opacity = 0.09f;
                p.speedx = rand.Next(0, 3);
                if (p.speedx == 0)
                    p.speedx = 0.5f;
                else if (p.speedx == 2)
                    p.speedx = 1.5f;
                p.speedx *= rand.Next(0, 2) == 1 ? 1 : -1;
                p.speedx /= 2;
                if (p.speedx < 0)
                    p.x = Main.WindowWidth;
                else
                    p.x = -p.w;

                p.isDead = false;
                p.speedy = 0;

                return p;
            }

            public void Update()
            {
                x += speedx;
                y += speedy;
                if (speedx < 0 && x + w <= 0)
                    isDead = true;
                else if (speedx > 0 && x >= Main.WindowWidth)
                    isDead = true;
            }

            public void Draw(Renderer r)
            {
                Color c = Color.White * opacity;
                r.Draw(GraphicsEngine.pixel, new Rectangle((int)x, (int)y, (int)w, 1), c);
                r.Draw(GraphicsEngine.pixel, new Rectangle((int)x, (int)y, 1, (int)h), c);
                r.Draw(GraphicsEngine.pixel, new Rectangle((int)(x + w), (int)y, 1, (int)h), c);
                r.Draw(GraphicsEngine.pixel, new Rectangle((int)x, (int)(y + h), (int)w, 1), c);
            }
        }

        static Random rand = new Random();
        List<PRectangle> rects = new List<PRectangle>();
        int ticksSinceLastRect = 0;

        public override void Initialize()
        {
            for (int i = 0; i < 10000; i++)
            {
                Update();
            }
        }

        public override void Update()
        {
            for (int i = 0; i < rects.Count; i++)
            {
                rects[i].Update();
                if (rects[i].isDead)
                {
                    rects.RemoveAt(i);
                    i--;
                }
            }

            ticksSinceLastRect++;
            if (ticksSinceLastRect > 90)
            {
                ticksSinceLastRect = 0;
                rects.Add(PRectangle.GetNew());
            }
        }

        public override void Draw(Renderer renderer)
        {
            for (int i = 0; i < rects.Count; i++)
            {
                rects[i].Draw(renderer);
            }
        }
    }
}
