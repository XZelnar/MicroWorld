using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class SwapperLogics : LogicalComponent
    {
        double lastV4 = 0, lastV5 = 0;

        public override void Update()
        {
            var p = parent as Swapper;

            if (lastV4 < 2.5 && p.Wires[4].VoltageDropAbs > 2.5)
                p.Swapped = !p.Swapped;
            if (lastV5 < 2.5 && p.Wires[5].VoltageDropAbs > 2.5)
                p.Swapped = !p.Swapped;

            lastV4 = p.Wires[4].VoltageDropAbs;
            lastV5 = p.Wires[5].VoltageDropAbs;

            base.Update();
        }

        public override void Reset()
        {
            lastV4 = 0;
            lastV5 = 0;

            base.Reset();
        }

    }
}
