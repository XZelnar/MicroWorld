using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class AccumulatorLogics : LogicalComponent
    {
        private double charge = 0;
        private double startCharge = 0;
        private double maxCharge = 1000;

        internal double StartCharge
        {
            get { return startCharge; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > maxCharge)
                    value = maxCharge;
                startCharge = value;
            }
        }

        public double Charge
        {
            get { return charge; }
            set
            {
                if (value > maxCharge)
                    value = maxCharge;
                if (value < 0)
                    value = 0;
                if (charge != value)
                {
                    charge = value;
                    (parent.Graphics as Graphics.AccumulatorGraphics).UpdateFBO();
                }
            }
        }
        public double MaxCharge
        {
            get { return maxCharge; }
            set
            {
                if (value < 1)
                    value = 1;
                if (maxCharge != value)
                {
                    maxCharge = value;
                    if (charge > maxCharge)
                        charge = maxCharge;
                    if (startCharge > maxCharge)
                        startCharge = maxCharge;
                    (parent.Graphics as Graphics.AccumulatorGraphics).UpdateFBO();
                }
            }
        }

        public override void Update()
        {
            var p = parent as Accumulator;

            if (p.Joint1 == PortState.Input)
            {
                Charge += p.W1.VoltageDropAbs;
            }
            else
            {
                if (Charge > 5)
                    p.Joints[2].SendingVoltage = 5;
                else
                    p.Joints[2].SendingVoltage = Charge;

                if (p.W1.VoltageDropAbs > 0.001)
                    Charge -= p.Joints[2].SendingVoltage;
            }

            if (p.Joint2 == PortState.Input)
            {
                Charge += p.W2.VoltageDropAbs;
            }
            else
            {
                if (Charge > 5)
                    p.Joints[3].SendingVoltage = 5;
                else
                    p.Joints[3].SendingVoltage = Charge;

                if (p.W2.VoltageDropAbs > 0.001)
                    Charge -= p.Joints[3].SendingVoltage;
            }

            base.Update();
        }

    }
}
