using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class VoltageGraphLogics : LogicalComponent
    {
        internal double[] values = new double[156];
        internal int min = -5, max = 5, frequency = 1;
        int curTick = 0;

        public override void Reset()
        {
            for (int i = 0; i < values.Length; i++)
                values[i] = 0;
            curTick = 0;
        }

        public override void Update()
        {
            curTick++;
            if (curTick >= frequency)
            {
                curTick = 0;
                for (int i = 1; i < values.Length; i++)
                    values[i - 1] = values[i];
                values[values.Length - 1] = (parent as VoltageGraph).Joints[0].Voltage - (parent as VoltageGraph).Joints[1].Voltage;
            }
        }
    }
}
