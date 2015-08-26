using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components
{
    public unsafe class VisibilityMap
    {
        public const int WIRE = 6;

        public unsafe class Chunk
        {
            public short[,] data = new short[64, 64];

            public Chunk()
            {
            }

            public Chunk(Chunk c)
            {
                data = (short[,])c.data.Clone();
            }
        }

        //0 = p

        /// <summary>
        /// | 
        ///-+-
        /// |v
        /// </summary>
        public List<List<Chunk>> XpYp = new List<List<Chunk>>();

        /// <summary>
        /// |v
        ///-+-
        /// | 
        /// </summary>
        public List<List<Chunk>> XpYn = new List<List<Chunk>>();

        /// <summary>
        ///v| 
        ///-+-
        /// | 
        /// </summary>
        public List<List<Chunk>> XnYn = new List<List<Chunk>>();

        /// <summary>
        /// | 
        ///-+-
        ///v| 
        /// </summary>
        public List<List<Chunk>> XnYp = new List<List<Chunk>>();

        internal int[] IgnoreJointsCoords = new int[0];


        public VisibilityMap()
        {
        }

        public VisibilityMap(VisibilityMap v)
        {
            for (int i = 0; i < v.XpYp.Count; i++)
            {
                XpYp.Add(new List<Chunk>());
                for (int j = 0; j < v.XpYp[i].Count; j++)
                {
                    XpYp[i].Add(new Chunk(v.XpYp[i][j]));
                }
            }
            for (int i = 0; i < v.XpYn.Count; i++)
            {
                XpYn.Add(new List<Chunk>());
                for (int j = 0; j < v.XpYn[i].Count; j++)
                {
                    XpYn[i].Add(new Chunk(v.XpYn[i][j]));
                }
            }
            for (int i = 0; i < v.XnYp.Count; i++)
            {
                XnYp.Add(new List<Chunk>());
                for (int j = 0; j < v.XnYp[i].Count; j++)
                {
                    XnYp[i].Add(new Chunk(v.XnYp[i][j]));
                }
            }
            for (int i = 0; i < v.XnYn.Count; i++)
            {
                XnYn.Add(new List<Chunk>());
                for (int j = 0; j < v.XnYn[i].Count; j++)
                {
                    XnYn[i].Add(new Chunk(v.XnYn[i][j]));
                }
            }
        }



        /// <summary>
        /// Used upon level loading so that wires can draw themselwes
        /// </summary>
        internal bool ignorePlacableRestriction = false;


        /// <summary>
        /// Generates new chunk if such doesn't exist.
        /// </summary>
        /// <param name="x">Game coordinates</param>
        /// <param name="y">Game coordinates</param>
        public void Generate(float x, float y)
        {
            if (Exists(x, y)) return;
            _generateNew(x, y);
        }

        //TODO
        //x,y - game coords
        private void _generateNew(float x, float y)
        {
            if (x < 0) x -= 8;//Needed for proper work
            if (y < 0) y -= 8;//Needed for proper work
            float ox = x, oy = y;
            int divider = 64 * MicroWorld.Graphics.GUI.GridDraw.Step;
            x /= divider;
            y /= divider;
            List<List<Chunk>> cur = null;
            if (x >= 0 && y >= 0)
            {
                cur = XpYp;
            }
            if (x >= 0 && y < 0)
            {
                y *= -1;
                cur = XpYn;
            }
            if (x < 0 && y < 0)
            {
                x *= -1;
                y *= -1;
                cur = XnYn;
            }
            if (x < 0 && y >= 0)
            {
                x *= -1;
                cur = XnYp;
            }

            if (cur.Count < (int)x)
            {
                _generateNew(ox - divider * Math.Sign(ox), Math.Sign(oy));
            }

            if ((int)y != 0 && cur.Count <= (int)x)
            {
                _generateNew(ox, Math.Sign(oy));
            }

            if ((int)y != 0 && cur[(int)x].Count < (int)y)
            {
                _generateNew(ox, oy - divider * Math.Sign(oy));
            }

            _createEmpty((int)x, (int)y, ref cur);
        }

        //vx,vy - coordinates in the array
        private void _createEmpty(int vx, int vy, ref List<List<Chunk>> g)
        {
            while (vx >= g.Count) g.Add(new List<Chunk>());
            g[vx].Add(new Chunk());
        }

        /// <summary>
        /// Sets rectangle to a specific value
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="w">W</param>
        /// <param name="h">H</param>
        /// <param name="v">Value</param>
        public void SetRectangle(Component sender, float x, float y, float w, float h)
        {
            //if (x < 0 && x + w >= 0)
            //    w++;
            //if (y < 0 && y + h >= 0)
            //    h++;
            /*
            int divider = 64 * MicroWorld.Graphics.GUI.GridDraw.Step;
            if ((int)(x / divider) == (int)((x + w) / divider) && (int)(y / divider) == (int)((y + h) / divider))
            {
                var c = GetChunk(x, y);
                int dx = (int)(x % divider) / 8;
                int dy = (int)(y % divider) / 8;
                for (int i = 0; i < w / MicroWorld.Graphics.GUI.GridDraw.Step; i++)
                {
                    for (int j = 0; j < h / MicroWorld.Graphics.GUI.GridDraw.Step; j++)
                    {
                        c.data[dx + i, dy + j] = v;
                    }
                }
            }
            else
            //*/
            //{
            //TODO
            short v = (short)(sender == null ? 0 : sender.typeID);
            for (int i = 0; i < w; i += 8)
            {
                for (int j = 0; j < h; j += 8)
                {
                    SetPoint(x + i, y + j, v);
                }
            }
            //}
        }

        /// <summary>
        /// Sets point to a specificvalue
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="v">Value</param>
        public void SetPoint(Component sender, float x, float y)
        {
            int divider = 64 * MicroWorld.Graphics.GUI.GridDraw.Step;
            GetChunk(x, y).data[(int)Math.Abs(x % divider) / 8, (int)Math.Abs(y % divider) / 8] = (short)(sender == null ? 0 : sender.typeID);
        }

        private void SetPoint(float x, float y, short v)
        {
            int divider = 64 * MicroWorld.Graphics.GUI.GridDraw.Step;
            int yyy = (int)Math.Abs(y % divider) / 8;
            int xxx = (int)Math.Abs(x % divider) / 8;
            var c = GetChunk(x, y);
            GetChunk(x, y).data[(int)Math.Abs(x % divider) / 8, (int)Math.Abs(y % divider) / 8] = v;
        }

        /// <summary>
        /// Sets a line to a specific value
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="v"></param>
        public void SetLine(Component sender, float x1, float y1, float x2, float y2)
        {
            short v = (short)(sender == null ? 0 : sender.typeID);
            if (y1 > y2)
            {
                float t = y1;
                y1 = y2;
                y2 = t;
            }
            if (x1 > x2)
            {
                float t = x1;
                x1 = x2;
                x2 = t;
            }

            if (x1 == x2)
            {
                for (int y = (int)y1; y < (int)y2; y++)
                {
                    SetPoint(x1, y, v);
                }
            }
            if (y1 == y2)
            {
                for (int x = (int)x1; x < (int)x2; x++)
                {
                    SetPoint(x, y1, v);
                }
            }
            else
            {
                if (Math.Abs(x1 - x2) > Math.Abs(y1 - y2))//dx>dy
                {
                    for (float x = x1; x != x2; x += Math.Sign(x2 - x1))
                    {
                        float y = y1 + (y2 - y1) * (x / (x2 - x1));
                        SetPoint(x, y, v);
                    }
                }
                else//dx<dy
                {
                    for (float y = y1; y != y2; y += Math.Sign(y2 - y1))
                    {
                        float x = x1 + (x2 - x1) * (y / (y2 - y1));
                        SetPoint(x, y, v);
                    }
                }
            }
        }

        int divider = 64 * MicroWorld.Graphics.GUI.GridDraw.Step;
        /// <summary>
        /// Returns 1 if nothing is there, 6 if it's a wire, and 0 otherwise
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int GetAStarValue(int x, int y)
        {
            if (Main.curState != "GAMElvlDesign" && !ignorePlacableRestriction)
                if (!MicroWorld.Logics.PlacableAreasManager.IsPlacable(x, y)) 
                    return 0;
            if (ComponentsManager.IgnoreAS1 != null &&
                 x == ComponentsManager.IgnoreAS1.Graphics.Position.X && y == ComponentsManager.IgnoreAS1.Graphics.Position.Y)
                return 1;
            if (ComponentsManager.IgnoreAS2 != null &&
                 x == ComponentsManager.IgnoreAS2.Graphics.Position.X && y == ComponentsManager.IgnoreAS2.Graphics.Position.Y)
                return 1;
            short t = GetValue(x, y);
            //short t = GetChunk(x, y).data[(int)Math.Abs(x % divider) / 8, (int)Math.Abs(y % divider) / 8];
            if (t == Wire.TypeID)
                return WIRE;
            return t == 0 ? 1 : 0;
            //if (t != 0) 
            //    return 0;
            //return 1;
        }

        public bool IsFree(MicroWorld.Components.Colliders.AABB a)
        {
            return IsFree(a.X1, a.Y1, a.Size.X, a.Size.Y);
        }

        public bool IsFree(Microsoft.Xna.Framework.Rectangle a)
        {
            return IsFree(a.X, a.Y, a.Width, a.Height);
        }

        public bool IsFree(float lx, float ty, float w, float h)
        {
            //TODO
            for (int i = (int)lx; i < lx + w; i += 8)
            {
                for (int j = (int)ty; j < ty + h; j += 8)
                {
                    if (!IsFree(i, j))
                        return false;
                }
            }
            return true;
        }

        public bool IsOfTypes(float lx, float ty, float w, float h, short[] types)
        {
            //TODO
            for (int i = (int)lx; i < lx + w; i += 8)
            {
                for (int j = (int)ty; j < ty + h; j += 8)
                {
                    if (!IsOfTypes(i, j, types))
                        return false;
                }
            }
            return true;
        }

        short tempvalue = 0;
        public bool IsFree(int x, int y)
        {
            bool b = (Main.curState != "GAMElvlDesign" && !ignorePlacableRestriction);
            XInDeadZone = x >= -8 && x < 0;
            YInDeadZone = y >= -8 && y < 0;
            if (XInDeadZone)
                x = -8;
            if (YInDeadZone)
                y = -8;
            x = (int)(x / 8) * 8;
            y = (int)(y / 8) * 8;
            tempvalue = _getValue(x, y);
            if (tempvalue != 0)
                if (tempvalue == Joint.TypeID && ShouldIgnore(x, y))
                    return true;
                else
                    return false;
            //if (b && !MicroWorld.Logics.GameInputHandler.IsPlacable(x, y))
            //    return false;
            return true;
        }

        public bool IsOfTypes(int x, int y, short[] types)
        {
            bool b = (Main.curState != "GAMElvlDesign" && !ignorePlacableRestriction);
            XInDeadZone = x >= -8 && x < 0;
            YInDeadZone = y >= -8 && y < 0;
            if (XInDeadZone)
                x = -8;
            if (YInDeadZone)
                y = -8;
            x = (int)(x / 8) * 8;
            y = (int)(y / 8) * 8;
            tempvalue = _getValue(x, y);
            if (tempvalue != 0)
                if (types.Contains(tempvalue))
                    return true;
                else
                    return false;
            //if (b && !MicroWorld.Logics.GameInputHandler.IsPlacable(x, y))
            //    return false;
            return true;
        }

        private bool ShouldIgnore(int x, int y)
        {
            if (IgnoreJointsCoords.Length == 0)
                return false;

            for (int i = 0; i < IgnoreJointsCoords.Length; i += 2)
                if (Math.Abs(IgnoreJointsCoords[i] - x) < 8 && Math.Abs(IgnoreJointsCoords[i + 1] - y) < 8)
                    return true;
            return false;
        }

        bool XInDeadZone, YInDeadZone;
        private short _getValue(int x, int y)
        {
            if (XInDeadZone)
                if (YInDeadZone)
                    return GetChunk(x, y).data[0, 0];
                else
                    return GetChunk(x, y).data[0, (int)Math.Abs(y % divider) / 8];
            else
                if (YInDeadZone)
                    return GetChunk(x, y).data[(int)Math.Abs(x % divider) / 8, 0];
                else
                    return GetChunk(x, y).data[(int)Math.Abs(x % divider) / 8, (int)Math.Abs(y % divider) / 8];
        }

        public short GetValue(int x, int y)
        {
            XInDeadZone = x >= -8 && x < 0;
            YInDeadZone = y >= -8 && y < 0;
            if (XInDeadZone)
                x = -8;
            if (YInDeadZone)
                y = -8;
            x = (int)(x / 8) * 8;
            y = (int)(y / 8) * 8;

            if (XInDeadZone)
                if (YInDeadZone)
                    return GetChunk(x, y).data[0, 0];
                else
                    return GetChunk(x, y).data[0, (int)Math.Abs(y % divider) / 8];
            else
                if (YInDeadZone)
                    return GetChunk(x, y).data[(int)Math.Abs(x % divider) / 8, 0];
                else
                    return GetChunk(x, y).data[(int)Math.Abs(x % divider) / 8, (int)Math.Abs(y % divider) / 8];
        }

        public bool isBlocked(int x, int y)
        {
            return GetAStarValue(x, y) == 0;
        }

        //public bool IsRectangleFreeToPlace(float x, float y, float w, float h)
        //{
        //    for (int i = (int)x; i < (int)(x + w); i++)
        //    {
        //        for (int j = (int)y; j < (int)(y + h); j++)
        //        {
        //            if (GetAStarValue(i, j) != 1) return false;
        //        }
        //    }
        //    return true;
        //}

        /// <summary>
        /// Determines wether specific chunk exists.
        /// </summary>
        /// <param name="x">Game coordinates</param>
        /// <param name="y">Game coordinates</param>
        /// <returns></returns>
        public bool Exists(float x, float y)
        {
            if (x < 0) x -= 8;//Needed for proper work
            if (y < 0) y -= 8;//Needed for proper work
            x /= 64;
            y /= 64;
            x /= MicroWorld.Graphics.GUI.GridDraw.Step;
            y /= MicroWorld.Graphics.GUI.GridDraw.Step;
            if (x >= 0 && y >= 0)
            {
                return (XpYp.Count > (int)x && XpYp[(int)x].Count > (int)y);
            }
            if (x >= 0 && y < 0)
            {
                y *= -1;
                return (XpYn.Count > (int)x && XpYn[(int)x].Count > (int)y);
            }
            if (x < 0 && y < 0)
            {
                x *= -1;
                y *= -1;
                return (XnYn.Count > (int)x && XnYn[(int)x].Count > (int)y);
            }
            if (x < 0 && y >= 0)
            {
                x *= -1;
                return (XnYp.Count > (int)x && XnYp[(int)x].Count > (int)y);
            }
            return false;
        }

        /// <summary>
        /// Returns chunk correspondant to specific coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Chunk GetChunk(float x, float y)
        {
            if (x < 0) x -= 8;//Needed for proper work
            if (y < 0) y -= 8;//Needed for proper work
            if (!Exists(x, y)) _generateNew(x, y);
            return _getChunkNoChecks(x, y);
        }

        private Chunk _getChunkNoChecks(float x, float y)
        {
            if (x < 0) x -= 8;//Needed for proper work
            if (y < 0) y -= 8;//Needed for proper work
            x /= 64;
            y /= 64;
            x /= MicroWorld.Graphics.GUI.GridDraw.Step;
            y /= MicroWorld.Graphics.GUI.GridDraw.Step;
            if (x >= 0 && y >= 0)
            {
                return XpYp[(int)x][(int)y];
            }
            if (x >= 0 && y < 0)
            {
                y *= -1;
                return XpYn[(int)x][(int)y];
            }
            if (x < 0 && y < 0)
            {
                x *= -1;
                y *= -1;
                return XnYn[(int)x][(int)y];
            }
            if (x < 0 && y >= 0)
            {
                x *= -1;
                return XnYp[(int)x][(int)y];
            }
            return null;
        }

        internal void Clear()
        {
            for (int i = 0; i < XpYp.Count; i++)
            {
                XpYp[i].Clear();
            }
            XpYp.Clear();

            for (int i = 0; i < XpYn.Count; i++)
            {
                XpYn[i].Clear();
            }
            XpYn.Clear();

            for (int i = 0; i < XnYn.Count; i++)
            {
                XnYn[i].Clear();
            }
            XnYn.Clear();

            for (int i = 0; i < XnYp.Count; i++)
            {
                XnYp[i].Clear();
            }
            XnYp.Clear();
        }

    }
}
