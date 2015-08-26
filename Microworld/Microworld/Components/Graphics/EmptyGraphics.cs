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

namespace MicroWorld.Components.Graphics
{
    class EmptyGraphics : GraphicalComponent
    {
        public EmptyGraphics()
        {
            Layer = -1;
            Visible = false;
        }

        public override string GetIconName()
        {
            return null;
        }

        public override string GetCSToolTip()
        {
            return null;
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
        }
    }
}
