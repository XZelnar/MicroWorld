using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MicroWorld.Logics
{
    public static class GridHelper
    {
        public static void GridCoords(ref int x, ref int y)
        {
            if (x % 8 != 0)
            {
                if (x < 0)
                    x -= 8;
                x = (int)(Graphics.GUI.GridDraw.Step * (int)(x / Graphics.GUI.GridDraw.Step));
            }
            if (y % 8 != 0)
            {
                if (y < 0)
                    y -= 8;
                y = (int)(Graphics.GUI.GridDraw.Step * (int)(y / Graphics.GUI.GridDraw.Step));
            }
            //x = (int)(Graphics.GUI.GridDraw.Step * Math.Round((double)x / Graphics.GUI.GridDraw.Step));
            //y = (int)(Graphics.GUI.GridDraw.Step * Math.Round((double)y / Graphics.GUI.GridDraw.Step));
        }

        public static void GridCoords(ref float x, ref float y)
        {
            if (x % 8 != 0)
            {
                if (x < 0)
                    x -= 8;
                x = (int)(Graphics.GUI.GridDraw.Step * (int)(x / Graphics.GUI.GridDraw.Step));
            }
            if (y % 8 != 0)
            {
                if (y < 0)
                    y -= 8;
                y = (int)(Graphics.GUI.GridDraw.Step * (int)(y / Graphics.GUI.GridDraw.Step));
            }
            //x = (int)(Graphics.GUI.GridDraw.Step * Math.Round((double)x / Graphics.GUI.GridDraw.Step));
            //y = (int)(Graphics.GUI.GridDraw.Step * Math.Round((double)y / Graphics.GUI.GridDraw.Step));
        }

        public static void GridCoords(ref Microsoft.Xna.Framework.Vector2 v)
        {
            if (v.X % 8 != 0)
            {
                if (v.X < 0)
                    v.X -= 8;
                v.X = (int)(Graphics.GUI.GridDraw.Step * (int)(v.X / Graphics.GUI.GridDraw.Step));
            }
            if (v.Y % 8 != 0)
            {
                if (v.Y < 0)
                    v.Y -= 8;
                v.Y = (int)(Graphics.GUI.GridDraw.Step * (int)(v.Y / Graphics.GUI.GridDraw.Step));
            }
            //v.X = (int)(Graphics.GUI.GridDraw.Step * Math.Round((double)v.X / Graphics.GUI.GridDraw.Step));
            //v.Y = (int)(Graphics.GUI.GridDraw.Step * Math.Round((double)v.Y / Graphics.GUI.GridDraw.Step));
        }

        public static void GridCoordsOffset(ref int x, ref int y)
        {
            x -= (int)Settings.GameOffset.X;
            y -= (int)Settings.GameOffset.Y;
            x = (int)(Graphics.GUI.GridDraw.Step * Math.Round((double)x / Graphics.GUI.GridDraw.Step));
            y = (int)(Graphics.GUI.GridDraw.Step * Math.Round((double)y / Graphics.GUI.GridDraw.Step));
        }

        public static void GridCoordsOffset(ref float x, ref float y, int cellsMultiplier = 1)
        {
            x -= Settings.GameOffset.X;
            y -= Settings.GameOffset.Y;
            x = (float)(Graphics.GUI.GridDraw.Step * Math.Round(x / Graphics.GUI.GridDraw.Step / cellsMultiplier));
            y = (float)(Graphics.GUI.GridDraw.Step * Math.Round(y / Graphics.GUI.GridDraw.Step / cellsMultiplier));
        }

        public static bool ArePointsInSameCell(Vector2 p1, Vector2 p2)
        {
            GridCoords(ref p1);
            GridCoords(ref p2);
            return (p1.X == p2.X) && (p1.Y == p2.Y);
        }

        public static bool IsPointInLineSegment(Vector2 lineStart, Vector2 lineEnd, Vector2 p)
        {
            GridCoords(ref lineStart);
            GridCoords(ref lineEnd);
            GridCoords(ref p);
            return (lineStart.X == lineEnd.X && ((lineStart.X <= p.X && lineEnd.X >= p.X) || (lineStart.X >= p.X && lineEnd.X <= p.X))) ||
                   (lineStart.Y == lineEnd.Y && ((lineStart.Y <= p.Y && lineEnd.Y >= p.Y) || (lineStart.Y >= p.Y && lineEnd.Y <= p.Y)));
        }
    }
}
