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

namespace MicroWorld.Utilities
{
    public static class Tools//TODO overloads
    {
        public static void GameToScreenCoords(ref float x, ref float y, float scale)
        {
            x = (float)((x + Settings.GameOffset.X) * scale);
            y = (float)((y + Settings.GameOffset.Y) * scale);
        }

        public static void GameToScreenCoords(ref float x, ref float y)
        {
            x = (float)((x + Settings.GameOffset.X) * Settings.GameScale);
            y = (float)((y + Settings.GameOffset.Y) * Settings.GameScale);
        }

        public static void GameToScreenCoords(ref int x, ref int y)
        {
            x = (int)((x + Settings.GameOffset.X) * Settings.GameScale);
            y = (int)((y + Settings.GameOffset.Y) * Settings.GameScale);
        }

        public static Vector2 GameToScreenCoords(Vector2 v)
        {
            v.X = (float)((v.X + Settings.GameOffset.X) * Settings.GameScale);
            v.Y = (float)((v.Y + Settings.GameOffset.Y) * Settings.GameScale);
            return v;
        }

        public static void GameToScreenCoords(ref double x, ref double y, ref double w, ref double h)
        {
            x = (x + Settings.GameOffset.X) * Settings.GameScale;
            y = (y + Settings.GameOffset.Y) * Settings.GameScale;
            w = w * Settings.GameScale;
            h = h * Settings.GameScale;
        }

        public static void GameToScreenCoords(ref RectangleF r)
        {
            r.X = (r.X + Settings.GameOffset.X) * Settings.GameScale;
            r.Y = (r.Y + Settings.GameOffset.Y) * Settings.GameScale;
            r.Width = r.Width * Settings.GameScale;
            r.Height = r.Height * Settings.GameScale;
        }

        public static void GameToScreenCoords(double[] d)
        {
            d[0] = (d[0] + Settings.GameOffset.X) * Settings.GameScale;
            d[1] = (d[1] + Settings.GameOffset.Y) * Settings.GameScale;
            d[2] = d[2] * Settings.GameScale;
            d[3] = d[3] * Settings.GameScale;
        }

        public static void ScreenToGameCoords(ref int x, ref int y)
        {
            x = (int)((float)x / Settings.GameScale - Settings.GameOffset.X);
            y = (int)((float)y / Settings.GameScale - Settings.GameOffset.Y);
        }

        public static void ScreenToGameCoords(ref float x, ref float y)
        {
            x = x / Settings.GameScale - Settings.GameOffset.X;
            y = y / Settings.GameScale - Settings.GameOffset.Y;
        }

        public static void ScreenToGameCoords(ref float x, ref float y, float scale, Vector2 offset)
        {
            x = (float)((float)x / scale - offset.X);
            y = (float)((float)y / scale - offset.Y);
        }

        public static float DistancePointToRectangle(Vector2 point, Rectangle rect)
        {
            if (point.X < rect.X)//left
            {
                if (point.Y < rect.Y)//top
                {
                    return new Vector2(point.X - rect.X, point.Y - rect.Y).Length();
                }
                if (point.Y > rect.Y + rect.Height)//bottom
                {
                    return new Vector2(point.X - rect.X, point.Y - (rect.Y + rect.Height)).Length();
                }
                //center
                return rect.X - point.X;
            }
            if (point.X > rect.X + rect.Width)//right
            {
                if (point.Y < rect.Y)//top
                {
                    return new Vector2(point.X - (rect.X + rect.Width), point.Y - rect.Y).Length();
                }
                if (point.Y > rect.Y + rect.Height)//bottom
                {
                    return new Vector2(point.X - (rect.X + rect.Width), point.Y - (rect.Y + rect.Height)).Length();
                }
                //center
                return point.X - (rect.X + rect.Width);
            }
            //middle
            if (point.Y < rect.Y)//top
            {
                return rect.Y - point.Y;
            }
            if (point.Y > rect.Y + rect.Height)//bottom
            {
                return point.Y - (rect.Y + rect.Height);
            }
            //inside
            return 0f;
        }

        public static bool IsRunningOnMono()
        {
            return Program.ExecutingFramework == Program.Framework.MONO;
        }
    }
}
