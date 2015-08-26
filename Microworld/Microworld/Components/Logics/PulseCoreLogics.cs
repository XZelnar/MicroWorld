using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class PulseCoreLogics : LogicalComponent
    {
        public int RequiredActivity = 1;
        public int LastActiveFor = 0;
        public bool IsComplete = false;

        public override void Update()
        {
            var p = parent as PulseCore;
            if (p.W.VoltageDropAbs > 2.5f)
            {
                LastActiveFor++;
                if (LastActiveFor >= RequiredActivity && !IsComplete)
                {
                    IsComplete = true;
                    p.InvokeRecievedFinished();
                }
            }
            else
            {
                LastActiveFor = 0;
            }

            base.Update();
        }

        public override void Reset()
        {
            IsComplete = false;
            LastActiveFor = 0;

            base.Reset();
        }

        public bool IsCorrect()
        {
            return IsComplete;
        }
    }
}
