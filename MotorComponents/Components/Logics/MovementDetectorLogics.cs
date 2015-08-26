using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class MovementDetectorLogics : LogicalComponent
    {
        double InputVoltage = 0;

        public override void Update()
        {
            var p = parent as MovementDetector;

            InputVoltage = 0;
            for (int i = 0; i < p.Wires.Length; i++)
            {
                if (p.Joints[i + 4].IsGround)
                    InputVoltage = Math.Max(InputVoltage, p.Wires[i].VoltageDropAbs);
            }
            p.HasEnoughPowerSupply = InputVoltage > 2.5;

            if (p.ShouldEmmitOutput())
            {
                for (int i = 0; i < p.Wires.Length; i++)
                {
                    if (p.Joints[i + 4].IsProvidingPower)
                        p.Joints[i + 4].SendingVoltage = 5;
                }
            }
            else
            {
                for (int i = 0; i < p.Wires.Length; i++)
                {
                    if (p.Joints[i + 4].IsProvidingPower)
                        p.Joints[i + 4].SendingVoltage = 0;
                }
            }

            base.Update();
        }
    }
}
