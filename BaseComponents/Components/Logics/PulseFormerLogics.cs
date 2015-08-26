using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class PulseFormerLogics : LogicalComponent
    {
        public float[] pulsesOld = null;
        public float[] pulses = new float[] { };
        public bool cycle = true;

        public int curTick = 0;

        public override void Reset()
        {
            curTick = 0;
            base.Reset();
        }

        public override void CircuitUpdate()
        {
            base.CircuitUpdate();

            double res = 0;
            var p = (parent as PulseFormer);
            if (curTick < pulses.Length)
            {
                res = (1f - pulses[curTick]) * p.MaxResistance;
                if (res < 1) res = 1;
            }
            else
            {
                res = 1;
            }
            if (p.W.Resistance != res)
            {
                p.W.Resistance = res;
                MicroWorld.Logics.CircuitManager.ScheduleReupdate(p.W);
            }
        }

        public override void Update()
        {
            /*
            var p = (parent as PulseFormer);
            if (curTick < pulses.Length)
            {
                p.W.Resistance = pulses[curTick] ? 1 : p.MaxResistance;
            }
            else
            {
                p.W.Resistance = 1;
            }
            //*/
            curTick++;
            if (cycle && curTick >= pulses.Length) curTick = 0;
            base.Update();
        }
    }
}
