using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class SignalSplitterLogics : LogicalComponent
    {
        private int tick = 0;
        private bool top = false;

        internal double v1 = 0;

        public int Period = 30;
        public bool IsTopActive
        {
            get { return top; }
            set { top = value; }
        }
        public bool IsBottomActive
        {
            get { return !top; }
            set { top = !value; }
        }

        public override void Reset()
        {
            tick = 0;
            v1 = 0;

            base.Reset();
        }

        public override void Update()
        {
            tick++;
            if (tick >= Period * 2)
            {
                tick = 0;
            }

            base.Update();
        }

        public override void CircuitUpdate()
        {
            base.CircuitUpdate();

            SignalSplitter l = ((SignalSplitter)parent);
            v1 = l.Joints[0].Voltage;

            if (tick >= Period)
            {
                top = false;
                if (l.Joints[4].SendingVoltage != v1)
                {
                    l.Joints[4].SendingVoltage = v1;
                    l.Joints[4].IsProvidingPower = true;
                    MicroWorld.Logics.CircuitManager.ScheduleReupdate(l.Joints[4]);
                }
                if (l.Joints[5].SendingVoltage != 0)
                {
                    l.Joints[5].SendingVoltage = 0;
                    l.Joints[5].IsProvidingPower = false;
                    MicroWorld.Logics.CircuitManager.ScheduleReupdate(l.Joints[5]);
                }
            }
            else
            {
                top = true;
                if (l.Joints[4].SendingVoltage != 0)
                {
                    l.Joints[4].SendingVoltage = 0;
                    l.Joints[4].IsProvidingPower = false;
                    MicroWorld.Logics.CircuitManager.ScheduleReupdate(l.Joints[4]);
                }
                if (l.Joints[5].SendingVoltage != v1)
                {
                    l.Joints[5].SendingVoltage = v1;
                    l.Joints[5].IsProvidingPower = true;
                    MicroWorld.Logics.CircuitManager.ScheduleReupdate(l.Joints[5]);
                }
            }
        }

    }
}
