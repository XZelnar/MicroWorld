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

namespace MicroWorld.Components.old
{
    class ResistorDynamic : Resistor, Properties.IDrawBorder
    {
        public ResistorDynamic()
        {
            constructor();
            Graphics = new Graphics.ResistorDynamicGraphics();
            Graphics.parent = this;
            Graphics.Size = new Vector2(48, 32);
        }

        public ResistorDynamic(float x, float y)
        {
            constructor();
            Graphics = new Graphics.ResistorDynamicGraphics();
            Graphics.parent = this;
            Graphics.Size = new Vector2(48, 32);
            Graphics.Position = new Vector2(x, y);
        }

        public static void LoadContentStatic()
        {
            //Components.Graphics.ResistorGraphics.LoadContentStatic();
        }

        //============================================================LOGICS========================================================

        //=============================================================GUI==========================================================

        float x, y;
        int btn = -1;
        public override void OnMouseDown(InputEngine.MouseArgs e)
        {
            base.OnMouseDown(e);

            x = e.curState.X;
            y = e.curState.Y;
            btn = e.button;
        }

        public override void OnMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.OnMouseMove(e);

            if (btn != -1)
            {
                Resistance += (float)(e.dy)/10f;
            }
        }

        public override void OnMouseUp(InputEngine.MouseArgs e)
        {
            base.OnMouseUp(e);

            btn = -1;
            x = 0;
            y = 0;
        }

        public override void OnMouseWheel(InputEngine.MouseWheelMoveArgs e)
        {
            base.OnMouseWheel(e);

            if (e.delta != 0)
            {
                Resistance += (float)(e.delta) / 120f;
            }
        }

        
    }
}
