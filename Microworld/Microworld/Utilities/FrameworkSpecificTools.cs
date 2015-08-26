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
    public static class FrameworkSpecificTools
    {
        public static void ChangeWindowPosition(int dx, int dy)
        {
            var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Main.window.Handle);
            form.Location = new System.Drawing.Point(form.Location.X + dx, form.Location.Y + dy);
        }

        public static void SetWindowPosition(int x, int y)
        {
            var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Main.window.Handle);
            form.Location = new System.Drawing.Point(x, y);
        }

        public static void SetWindowBordered(bool borders)
        {
            var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(Main.window.Handle);
            form.FormBorderStyle = borders ? System.Windows.Forms.FormBorderStyle.Fixed3D : 
                System.Windows.Forms.FormBorderStyle.None;
        }
    }
}
