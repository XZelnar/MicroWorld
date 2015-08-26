using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MicroWorld.Graphics.Overlays
{
    class Arrow : Overlay
    {
        static Texture2D cw0, cw45, cw90, cw135, cw180, cw225, cw270, cw315;

        private Direction direction = Direction.None;
        internal Texture2D cur;
        public Color color = Color.White;
        public float opacity = 0f;
        public bool Disappear = false;

        public Direction Direction
        {
            get { return direction; }
            set
            {
                direction = value;

                switch (direction)
                {
                    case Direction.None:
                        cur = null;
                        break;
                    case Direction.Left:
                        cur = cw180;
                        break;
                    case Direction.Up:
                        cur = cw270;
                        break;
                    case Direction.Right:
                        cur = cw0;
                        break;
                    case Direction.Down:
                        cur = cw90;
                        break;
                    case Direction.LeftUp:
                        cur = cw225;
                        break;
                    case Direction.UpRight:
                        cur = cw315;
                        break;
                    case Direction.RightDown:
                        cur = cw45;
                        break;
                    case Direction.DownLeft:
                        cur = cw135;
                        break;
                    default:
                        cur = null;
                        break;
                }
            }
        }

        public static void LoadArrows()
        {
            cw0 = ResourceManager.Load<Texture2D>("GUI/Arrows/ArrowRight");
            cw45 = ResourceManager.Load<Texture2D>("GUI/Arrows/ArrowRightDown");
            cw90 = ResourceManager.Load<Texture2D>("GUI/Arrows/ArrowDown");
            cw135 = ResourceManager.Load<Texture2D>("GUI/Arrows/ArrowDownLeft");
            cw180 = ResourceManager.Load<Texture2D>("GUI/Arrows/ArrowLeft");
            cw225 = ResourceManager.Load<Texture2D>("GUI/Arrows/ArrowLeftUp");
            cw270 = ResourceManager.Load<Texture2D>("GUI/Arrows/ArrowUp");
            cw315 = ResourceManager.Load<Texture2D>("GUI/Arrows/ArrowUpRight");
        }

        public override void Initialize()
        {
            if (Size.X == 0 && Size.Y == 0)
                Size = new Vector2(80, 80);

            base.Initialize();
        }

        public override void Update()
        {
            if (Disappear)
            {
                if (opacity > 0)
                    opacity -= 0.05f;
                if (opacity <= 0)
                    IsDead = true;
            }
            else
            {
                if (opacity < 1)
                    opacity += 0.02f;
                if (opacity > 1)
                    opacity = 1;
            }
        }

        public override void Draw(Renderer r)
        {
            if (cur != null)
                r.Draw(cur, Position, Size, color * opacity * 0.75f);
        }
    }
}
