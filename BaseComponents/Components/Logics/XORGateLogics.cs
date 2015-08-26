using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class XORGateLogics : LogicalComponent
    {
        internal double v1, v2, resv;
        XORGate par;

        public override void Initialize()
        {
            base.Initialize();

            par = parent as XORGate;
        }

        public override void PreUpdate()
        {
            par.W1.IsConnected = par.W2.IsConnected = true;
        }

        public override void CircuitUpdate()
        {
            base.CircuitUpdate();

            v1 = par.Joints[0].Voltage;
            v2 = par.Joints[1].Voltage;

            //correct for isolated
            if (par.W1.VoltageDropAbs == 0)
                v1 = 0;
            if (par.W2.VoltageDropAbs == 0)
                v2 = 0;

            if ((v1 >= 2.5) != (v2 > 2.5))
            {
                resv = Math.Max(5, Math.Max(par.W1.VoltageDropAbs, par.W2.VoltageDropAbs));
                par.Joints[4].IsProvidingPower = true;
            }
            else
            {
                resv = 0;
                par.Joints[4].IsProvidingPower = false;
            }

            if (par.Joints[4].SendingVoltage != resv)
            {
                par.Joints[4].SendingVoltage = resv;
                MicroWorld.Logics.CircuitManager.ScheduleReupdate(par.Joints[4]);
            }
        }

        public override void LastCircuitUpdate()
        {
            par.W1.IsConnected = par.W2.IsConnected = (resv > 2.5);
        }

        public override void Reset()
        {
            v1 = 0;
            v2 = 0;
            resv = 0;
            par.Joints[4].SendingVoltage = 0;

            base.Reset();
        }

    }
}
