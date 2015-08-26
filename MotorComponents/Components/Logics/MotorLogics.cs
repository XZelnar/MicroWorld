using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class MotorLogics : LogicalComponent
    {
        /// <summary>
        /// IN RADIANS!!!
        /// </summary>
        public float Angle = 0f;
        /// <summary>
        /// IN RADIANS!!!
        /// </summary>
        public float AngleOld = 0f;

        public override void Update()
        {
            var p = parent as Motor;
            p.Rotate((float)(p.W.Current * 200) / 20f);
            base.Update();
        }

        public override void Reset()
        {
            //(parent as Motor).Rotate(-Angle);
            AngleOld = 0;
            Angle = AngleOld;
            base.Reset();
        }

    }
}
