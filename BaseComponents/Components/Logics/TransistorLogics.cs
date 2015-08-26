using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class TransistorLogics : LogicalComponent
    {
        public double BaseControlVotage = 5f;
        internal double FlowPercentage = 0f;
        Transistor p;

        public override void Initialize()
        {
            base.Initialize();

            p = parent as Transistor;
        }

        public override void PreUpdate()
        {
            //p.W2.IsConnected = p.W3.IsConnected = false;
        }

        public override void Reset()
        {
            FlowPercentage = 0;

            base.Reset();
        }

        public override void CircuitUpdate()
        {
            double Vbase = p.W1.VoltageDropAbs;
            Vbase = Vbase > BaseControlVotage ? BaseControlVotage : Vbase;

            FlowPercentage = Vbase / BaseControlVotage;
            double n = Math.Max(p.W2.VoltageDropAbs * FlowPercentage, Vbase);
            //double n = p.W2.VoltageDropAbs * Vbase / BaseControlVotage;
            if (n != p.Joints[5].SendingVoltage)
            {
                p.Joints[5].SendingVoltage = n;
                MicroWorld.Logics.CircuitManager.ScheduleReupdate(p.Joints[5]);
                bool b = n > 0.001f;
                if (p.W2.IsConnected != b)
                {
                    p.W2.IsConnected = p.W3.IsConnected = b;
                    MicroWorld.Logics.CircuitManager.ScheduleReupdate(p.W2);
                    MicroWorld.Logics.CircuitManager.ScheduleReupdate(p.W3);
                }
            }
        }

        public override void LastCircuitUpdate()
        {
            //p.W2.IsConnected = p.W3.IsConnected = (FlowPercentage > 0.001f);
        }
    }
}
