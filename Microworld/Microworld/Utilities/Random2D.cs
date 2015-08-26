using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MicroWorld.Utilities
{
    public unsafe class Random2D
    {
        private short[,] noiseMap;
        private int mapSize = 256;
        private int max = 256;
        private int? seed = 0;
        public int Max
        {
            get { return max; }
        }
        public int MapSize
        {
            get { return mapSize; }
            set
            {
                mapSize = value;
                Init(seed, max);
            }
        }

        public Random2D()
        {
            Init(seed, max);
        }

        public Random2D(int? seed)
        {
            this.seed = seed;
            Init(seed, max);
        }

        public Random2D(int? seed, int max)
        {
            this.seed = seed;
            this.max = max;
            Init(seed, max);
        }

        internal void Init(int? seed, int max)
        {
            this.seed = seed;
            this.max = max;
            noiseMap = new short[mapSize, mapSize];
            Random r = seed.HasValue ? new Random(seed.Value) : new Random();
            for (int x = 0; x < mapSize; x++)
                for (int y = 0; y < mapSize; y++)
                    noiseMap[x, y] = (short)r.Next(max);

        }

        public int Next(int x, int y)
        {
            x /= 8;
            y /= 8;
            x = x % mapSize;
            y = y % mapSize;
            if (x < 0) x += mapSize;
            if (y < 0) y += mapSize;
            return noiseMap[x, y];
        }
    }
}
