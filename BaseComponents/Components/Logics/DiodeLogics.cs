using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class DiodeLogics : LogicalComponent
    {
        public override void Update()
        {
            base.Update();
            //return;
            /*
            Diode l = parent as Diode;
            double v1 = l.Joints[0].Voltage;
            double v2 = l.Joints[1].Voltage;

            if (v1 > v2 || Math.Abs(v1 - v2) < 0.001)
            {
                l.W.IsConnected = true;
            }
            else
            {
                l.W.IsConnected = false;
            }
            //*/
        }

    }
}
