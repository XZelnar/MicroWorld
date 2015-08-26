using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class PowerACLogics : LogicalComponent
    {
        internal double voltage = 5;
        internal float period = 32;
        int ticks = 0;

        public override void Reset()
        {
            ticks = 0;
        }

        public override void Update()
        {
            ticks++;
            (parent as PowerAC).Joints[1].SendingVoltage = voltage * Math.Sin(ticks * Math.PI * 2 / period);
        }
    }
}
