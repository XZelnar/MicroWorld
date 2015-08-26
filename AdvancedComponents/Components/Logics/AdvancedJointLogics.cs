using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class AdvancedJointLogics : LogicalComponent
    {
        internal class VoltageSource
        {
            public int TimeRemaining = 0;
            public float Voltage = 0;

            public VoltageSource(int time, float vol)
            {
                Voltage = vol;
                TimeRemaining = time;
            }
        }

        private List<VoltageSource> sources = new List<VoltageSource>();

        double OutputVoltage = 0;
        float maxIn = 0;



        public void AddSource(int time, float voltage)
        {
            sources.Add(new VoltageSource(time, voltage));
        }

        public void AddSource(VoltageSource s)
        {
            sources.Add(s);
        }

        public override void Reset()
        {
            maxIn = 0;
            OutputVoltage = 0;
            sources.Clear();

            base.Reset();
        }

        public override void Update()
        {
            OutputVoltage = 0;
            var p = parent as AdvancedJoint;
            for (int i = 0; i < p.Wires.Length; i++)
            {
                if (p.Joints[i + 4].IsGround)
                    OutputVoltage = Math.Max(OutputVoltage, p.Wires[i].VoltageDropAbs);
            }
            sources.Add(new VoltageSource(1, (float)OutputVoltage));

            var a = ComponentsManager.GetComponents<AdvancedJoint>((int)parent.Graphics.Center.X, (int)parent.Graphics.Center.Y, parent.Graphics.Size.Y);
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] != parent)
                    (a[i].Logics as AdvancedJointLogics).AddSource(1, (float)OutputVoltage);
            }

            maxIn = 0;
            for (int i = 0; i < sources.Count; i++)
            {
                maxIn = (float)Math.Max(maxIn, sources[i].Voltage);
                sources[i].TimeRemaining--;
                if (sources[i].TimeRemaining <= 0)
                {
                    sources.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 4; i < p.Joints.Length; i++)
            {
                if (p.Joints[i].IsProvidingPower)
                {
                    if (maxIn != 0)
                    {
                    }
                    p.Joints[i].SendingVoltage = maxIn;
                }
            }

            base.Update();
        }
    }
}
