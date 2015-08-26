using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class PulseFormerLogics : LogicalComponent
    {
        private double lastV = 0;

        public override void Reset()
        {
            lastV = 0;

            base.Reset();
        }

        public override void Update()
        {
            var p = (parent as PulseFormer);
            lastV = p.Joints[0].Voltage;

            base.Update();
        }

        public override void CircuitUpdate()
        {
            var p = (parent as PulseFormer);

            if (p.Joints[0].Voltage >= 2.5 && lastV <= 2.5)
            {
                if (p.Joints[3].SendingVoltage != 5)
                {
                    p.Joints[3].SendingVoltage = 5;
                    MicroWorld.Logics.CircuitManager.ScheduleReupdate(p.Joints[3]);
                }
            }
            else
            {
                if (p.Joints[3].SendingVoltage != 0)
                {
                    p.Joints[3].SendingVoltage = 0;
                    MicroWorld.Logics.CircuitManager.ScheduleReupdate(p.Joints[3]);
                }
            }
        }

    }
}
