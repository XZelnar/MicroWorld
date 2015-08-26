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

namespace MicroWorld.Graphics
{
    public class Camera
    {
        public const float ZOOM_MIN = 1f;
        public const float ZOOM_MAX = 4f;

        private Vector2 center;
        private float scale = 2f;
        private Rectangle? allowedVisibleRectangle = null;

        public Vector2 Center
        {
            get { return center; }
            set
            {
                Vector2 old = center;
                center = value;
                CheckPosition();
                if (old != center)
                {
                    GlobalEvents.OnCameraMoved(center - old);
                }
            }
        }
        public Vector2 BottomRight
        {
            get
            {
                var a = VisibleRectangle;
                return -center + new Vector2(a.Width / 2, a.Height / 2);
            }
        }
        public Vector2 TopLeft
        {
            get
            {
                var a = VisibleRectangle;
                return -center - new Vector2(a.Width / 2, a.Height / 2);
            }
        }
        public float Scale
        {
            get { return scale; }
            set
            {
                if (scale == value) return;
                float old = scale;
                Vector2 oldo = BottomRight;
                scale = value;
                CheckScale();
                if (old != scale)
                {
                    CheckPosition();
                    GlobalEvents.OnCameraScaled(old, BottomRight - oldo);
                }
            }
        }
        public Rectangle VisibleRectangle
        {
            get
            {
                int w = (int)(Main.WindowWidth / scale);
                int h = (int)(Main.WindowHeight / scale);
                return new Rectangle((int)(-w / 2 + center.X), (int)(-h / 2 + center.Y), w, h);
            }
        }
        public Rectangle? AllowedVisibleRectangle
        {
            get { return allowedVisibleRectangle; }
            set
            {
                allowedVisibleRectangle = value;
                CheckPosition();
            }
        }

        internal Camera() { }

        private void CheckPosition()
        {
            if (!AllowedVisibleRectangle.HasValue) return;
            var vr = VisibleRectangle;
            if (vr.Width >= allowedVisibleRectangle.Value.Width)
            {
                center.X = allowedVisibleRectangle.Value.X + allowedVisibleRectangle.Value.Width / 2;
            }
            else
            {
                if (center.X - vr.Width / 2 < allowedVisibleRectangle.Value.X) 
                    center.X = allowedVisibleRectangle.Value.X + vr.Width / 2;
                if (center.X + vr.Width / 2 > allowedVisibleRectangle.Value.X + allowedVisibleRectangle.Value.Width)
                    center.X = allowedVisibleRectangle.Value.X + allowedVisibleRectangle.Value.Width - vr.Width / 2;
            }
            if (vr.Height >= allowedVisibleRectangle.Value.Height)
            {
                center.Y = allowedVisibleRectangle.Value.Y + allowedVisibleRectangle.Value.Height / 2;
            }
            else
            {
                if (center.Y - vr.Height / 2 < allowedVisibleRectangle.Value.Y)
                    center.Y = allowedVisibleRectangle.Value.Y + vr.Height / 2;
                if (center.Y + vr.Height / 2 > allowedVisibleRectangle.Value.Y + allowedVisibleRectangle.Value.Height)
                    center.Y = allowedVisibleRectangle.Value.Y + allowedVisibleRectangle.Value.Height - vr.Height / 2;
            }
        }

        private void CheckScale()
        {
            if (scale < ZOOM_MIN) scale = ZOOM_MIN;
            if (scale > ZOOM_MAX) scale = ZOOM_MAX;
        }
    }
}
