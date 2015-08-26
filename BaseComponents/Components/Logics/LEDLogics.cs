using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class LEDLogics : LogicalComponent
    {
        public double Brightness = 0;
        public double VoltageThreshold = 2;
        public double Current = 0.02;//TODO param

        public override void CircuitUpdate()
        {
            base.CircuitUpdate();

            LED l = (LED)parent;
            if (l.W.IsConnected && l.W.VoltageDropAbs >= l.NeededVoltage)
                Brightness = Math.Min(1, l.W.Current / Current) * Math.Min(1, l.W.VoltageDropAbs - l.NeededVoltage);
            else
                Brightness = 0;

            //if (Brightness > l.NeededVoltage) 
            //    Brightness = l.NeededVoltage;
        }

        public override void Update()
        {
            base.Update();
        }

    }
}
