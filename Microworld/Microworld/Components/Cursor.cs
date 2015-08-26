using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components
{
    public class Cursor : Component
    {
        public Cursor()
        {
            Logics = new Logics.EmptyLogics();
            Graphics = new Graphics.CursorGraphics();
        }

        public override string GetName()
        {
            return "Cursor";
        }

        public override bool CanPlace(int x, int y, int w, int h)
        {
            return false;
        }
    }
}
