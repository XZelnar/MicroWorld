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
    public static class JointState
    {
        public static void Draw(float voltage, int x, int y)
        {
            Main.renderer.Draw(global::MicroWorld.Graphics.GraphicsEngine.pixel,
                new Rectangle(x, y, 3, 3), Color.CornflowerBlue);
            Main.renderer.Draw(global::MicroWorld.Graphics.GraphicsEngine.pixel,
                new Rectangle(x, y, 3, 3), Color.Red * (float)(voltage / 5));
        }
    }
}
