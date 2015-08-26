using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class ReedSwitchLogics : LogicalComponent
    {
        public float RequiredField = 100f;

        public override void CircuitUpdate()
        {
            var g = parent.Graphics as Graphics.ReedSwitchGraphics;
            var s = g.GetSizeRotated(parent.ComponentRotation);
            var a = ComponentsManager.GetMagneticField(g.Position.X + s.X / 2, g.Position.Y + s.Y / 2);

            (parent as ReedSwitch).W.IsConnected = a.Length() >= RequiredField;

            base.CircuitUpdate();
        }

        public override void Reset()
        {
            (parent as ReedSwitch).W.IsConnected = false;

            base.Reset();
        }
    }
}
