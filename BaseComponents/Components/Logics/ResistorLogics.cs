using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class ResistorLogics : LogicalComponent
    {
        private double voltage = 0;
        public double Voltage
        {
            get { return voltage; }
            set
            {
                voltage = value;
                if (voltage > 5) voltage = 5;
                if (voltage < 0) voltage = 0;
            }
        }

        public int InputPin = -1;
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update()
        {
            base.Update();
            /*
            Resistor r = ((Resistor)parent);
            Voltage = r.GetVoltage();
            r.NullifyPinsOutput();
            if (Voltage == 0)
            {
                return;
            }
            double v1 = r.Joints[0].Voltage;
            if (r.Joints[0].SendingIDs.Count == 1 &&
                (r.Joints[0].SendingIDs[0] == r.Joints[1].ID || r.Joints[0].SendingIDs[0] == r.Joints[0].ID)) v1 = 0;
            double v2 = r.Joints[1].Voltage;
            if (r.Joints[1].SendingIDs.Count == 1 &&
                (r.Joints[1].SendingIDs[0] == r.Joints[1].ID || r.Joints[1].SendingIDs[0] == r.Joints[0].ID)) v2 = 0;
            InputPin = -1;
            if (v1 > v2)
            {
                r.Joints[1].SendingVoltage = MicroWorld.Logics.CircuitManager.GetVoltageAfterResistance(v1, r.Resistance);
                r.Joints[1].SendingStrong = false;
                InputPin = 0;
            }
            else if (v1 < v2)
            {
                r.Joints[0].SendingVoltage = MicroWorld.Logics.CircuitManager.GetVoltageAfterResistance(v2, r.Resistance);
                r.Joints[0].SendingStrong = false;
                InputPin = 1;
            }
            else
            {
                r.Joints[0].SendingVoltage = 0;
                r.Joints[1].SendingVoltage = 0;
            }//*/
        }

    }
}
