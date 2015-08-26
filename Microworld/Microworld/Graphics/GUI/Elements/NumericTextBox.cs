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

namespace MicroWorld.Graphics.GUI.Elements
{
    public class NumericTextBox : TextBox
    {

        public NumericTextBox(int x, int y, int w, int h, String txt)
        {
            position = new Vector2(x, y);
            size = new Vector2(w, h);
            Text = txt;
            //stringSize = font.MeasureString(text);
            Mask = InputMask.Numbers;
        }

    }
}
