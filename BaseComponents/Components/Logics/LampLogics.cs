using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class LampLogics : LogicalComponent
    {
        public double Brightness = 0;
        public double Current = 0.0005;//TODO param

        public override void CircuitUpdate()
        {
            base.CircuitUpdate();
        }

        public override void Update()
        {
            base.Update();

            Lamp l = (Lamp)parent;
            Brightness = Math.Min(1, l.W.Current / Current);
        }

    }
}
