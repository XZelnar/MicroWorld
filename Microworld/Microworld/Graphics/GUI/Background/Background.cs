using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Graphics.GUI.Background
{
    public abstract class Background
    {
        public int width = 16, height = 16;
        public Random rand = new Random();

        public abstract void Initialize();

        public abstract void Update();

        public abstract void Draw(Renderer renderer);
    }
}
