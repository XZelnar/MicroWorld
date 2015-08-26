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

namespace MicroWorld.Graphics.GUI.Cursors
{
    class NormalCursor : Cursor
    {
        public override void LoadContent()
        {
            texture = ResourceManager.Load<Texture2D>("GUI/Cursors/Normal");
        }

        public override void OnShow()
        {
            color = Color.White;
            base.OnShow();
        }

        public override void Update()
        {
            int r = 255;
            int g = 255;
            int b = 255;
            if (InputEngine.curMouse.LeftButton == ButtonState.Pressed)
                r = 200;
            if (InputEngine.curMouse.RightButton == ButtonState.Pressed)
                b = 200;
            if (InputEngine.curMouse.MiddleButton == ButtonState.Pressed)
                g = 200;
            color = new Color(r, g, b);
            base.Update();
        }
    }
}
