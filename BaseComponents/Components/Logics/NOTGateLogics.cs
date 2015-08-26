using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class NOTGateLogics : LogicalComponent
    {
        internal double v1;
        NOTGate par;

        public override void Initialize()
        {
            base.Initialize();

            par = parent as NOTGate;
        }

        public override void PreUpdate()
        {
            par.W1.IsConnected = true;
        }

        public override void CircuitUpdate()
        {
            base.CircuitUpdate();

            v1 = par.Joints[0].Voltage;
            double oldv = par.Joints[1].SendingVoltage;

            if (par.Joints[0].ConnectedWires.Count == 0)
            {
                par.Joints[1].SendingVoltage = 0;
                par.Joints[1].IsProvidingPower = false;
                return;
            }

            if (v1 >= 2.5)
            {
                par.Joints[1].SendingVoltage = 0;
                par.Joints[1].IsProvidingPower = false;
            }
            else
            {
                par.Joints[1].SendingVoltage = 5;
                double c = 0.5;
                for (int i = 0; i < par.Joints[0].ConnectedWires.Count; i++)
                {
                    if (par.Joints[0].ConnectedWires[i].Current > c)
                        c = par.Joints[0].ConnectedWires[i].Current;
                }
                par.Joints[1].IsProvidingPower = true;
            }

            if (par.Joints[1].SendingVoltage != oldv)
            {
                MicroWorld.Logics.CircuitManager.ScheduleReupdate(par.Joints[1]);
            }
        }

        public override void LastCircuitUpdate()
        {
            par.W1.IsConnected = par.Joints[1].IsProvidingPower;
        }

        public override void Reset()
        {
            v1 = 0;
            par.Joints[1].SendingVoltage = 0;

            base.Reset();
        }

    }
}
