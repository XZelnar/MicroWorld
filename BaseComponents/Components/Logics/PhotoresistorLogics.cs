using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class PhotoresistorLogics : LogicalComponent
    {
        public double Brightness = 0;
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void CircuitUpdate()
        {
            base.CircuitUpdate();
            Photoresistor p = ((Photoresistor)parent);
            Brightness = ComponentsManager.GetBrightness(parent.Graphics.Position.X, parent.Graphics.Position.Y);
            double res = p.MaxResistance * (1 - Brightness) + 1f;
            if (p.W.Resistance != res)
            {
                p.W.Resistance = res;
                MicroWorld.Logics.CircuitManager.ScheduleReupdate(p.W);
            }
        }

        public override void Update()
        {
            base.Update();
            /*
            Photoresistor p = ((Photoresistor)parent);
            Brightness = ComponentsManager.GetMaxBrightness(parent.Graphics.Position.X, parent.Graphics.Position.Y);
            p.W.Resistance = p.MaxResistance * (1 - Brightness) + 1f;
            //*/
        }

    }
}
