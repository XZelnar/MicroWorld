using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class HESLogics : LogicalComponent
    {
        public override void CircuitUpdate()
        {
            var s = parent.Graphics.GetSizeRotated(parent.ComponentRotation) / 2;
            var p = parent.Graphics.Position;
            var par = parent as HES;
            float t = (float)ComponentsManager.GetMagneticField(p.X + s.X, p.Y + s.Y).Length() / par.MaxMagnetForce;
            t = t > 1 ? 1 : t < 0 ? 0 : t;
            double old = par.Joints[0].SendingVoltage;
            //par.Joints[0].SendingCurrent = par.MaxCurrent * t;
            par.Joints[0].SendingVoltage = par.MaxVoltage * t;
            par.Joints[1].SendingVoltage = par.Joints[0].SendingVoltage;
            par.Joints[2].SendingVoltage = par.Joints[0].SendingVoltage;
            par.Joints[3].SendingVoltage = par.Joints[0].SendingVoltage;

            if (Math.Abs(par.Joints[0].SendingVoltage - old) > 0.05f)
            {
                MicroWorld.Logics.CircuitManager.ScheduleReupdate(par.Joints[0]);
                MicroWorld.Logics.CircuitManager.ScheduleReupdate(par.Joints[1]);
                MicroWorld.Logics.CircuitManager.ScheduleReupdate(par.Joints[2]);
                MicroWorld.Logics.CircuitManager.ScheduleReupdate(par.Joints[3]);
            }

            base.CircuitUpdate();
        }
    }
}
