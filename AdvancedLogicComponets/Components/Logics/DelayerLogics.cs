using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class DelayerLogics : LogicalComponent
    {
        private bool IsNextUpdate = true;
        internal double[] signals = new double[5];
        private int delay = 5;
        public int Delay
        {
            get { return delay; }
            set
            {
                if (delay != value)
                {
                    delay = value;
                    signals = new double[delay];
                }
            }
        }

        public override void Reset()
        {
            IsNextUpdate = true;
            for (int i = 0; i < signals.Length; i++)
            {
                signals[i] = 0;
            }

            base.Reset();
        }

        public override void Update()
        {
            IsNextUpdate = true;

            base.Update();
        }

        public override void CircuitUpdate()
        {
            var p = (parent as Delayer);

            if (IsNextUpdate)
            {
                if (p.Joints[3].SendingVoltage != signals[signals.Length - 1])
                {
                    p.Joints[3].SendingVoltage = signals[signals.Length - 1];
                    MicroWorld.Logics.CircuitManager.ScheduleReupdate(p.Joints[3]);
                }

                for (int i = signals.Length - 1; i > 0; i--)
                {
                    signals[i] = signals[i - 1];
                }
            }

            signals[0] = p.Joints[0].Voltage;
        }

    }
}
