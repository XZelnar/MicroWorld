using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MicroWorld.Components.Logics
{
    class CoilLogics : LogicalComponent, Properties.IRequiresCircuitRecalculation
    {
        public Vector2? oldField = null;

        internal bool IndividualPass = false;

        double MaxResistance = 10000;//10kOhm
        public double chargeState = 0;

        double sendingCurrent = 0;
        internal double pureVoltageDrop = 0, pureCurrent = 0;
        public double inductionCurrent = 0;

        public Field curField = new Field();

        public struct Field//TODO internal
        {
            public double force;//field energy. LII/2
            public int direction;//sign of voltage drop
        }

        #region IRequiresCircuitRecalculation
        public void PreIndividualUpdate()
        {
            IndividualPass = true;
            sendingCurrent = (parent as Coil).W.SendingCurrent;
            (parent as Coil).W.SendingCurrent = 0;
            MicroWorld.Logics.CircuitManager.ScheduleReupdate((parent as Coil).W);
        }

        public void PostIndividualUpdate()
        {
            IndividualPass = false;
            (parent as Coil).W.SendingCurrent = sendingCurrent;
            MicroWorld.Logics.CircuitManager.ScheduleReupdate((parent as Coil).W);
        }

        public int GetPriority()
        {
            return -100;
        }
        #endregion

        public override void Initialize()
        {
            base.Initialize();

            MicroWorld.Logics.CircuitManager.RegisterReUpdatingComponent(this);
        }

        public override void Reset()
        {
            base.Reset();

            chargeState = 0;
            (parent as Coil).W.SendingCurrent = sendingCurrent = 0;
            (parent as Coil).W.SendingVoltage = 0;
            (parent as Coil).W.Resistance = MaxResistance;

            curField.direction = 0;
            curField.force = 0;

            inductionCurrent = 0;
        }

        public override void CircuitUpdate()
        {
            base.CircuitUpdate();

            if (IndividualPass)
            {
                var p = parent as Coil;
                pureVoltageDrop = p.Joints[1].Voltage - p.Joints[0].Voltage;
                pureCurrent = p.W.Current * Math.Sign(pureVoltageDrop);
            }
        }

        public override void LastCircuitUpdate()
        {
            base.LastCircuitUpdate();
        }

        public override void PreUpdate()
        {
            base.PreUpdate();

            //Coil l = (Coil)parent;

            Vector2 v1, v2, v3;
            if (parent.ComponentRotation == Component.Rotation.cw0)
            {
                v1 = ComponentsManager.GetMagneticField(parent.Graphics.Position.X, parent.Graphics.Center.Y, parent as Coil);
                v2 = ComponentsManager.GetMagneticField(parent.Graphics.Position.X + parent.Graphics.GetSize().X, parent.Graphics.Center.Y, parent as Coil);
            }
            else
            {
                v1 = ComponentsManager.GetMagneticField(parent.Graphics.Center.X, parent.Graphics.Position.Y, parent as Coil);
                v2 = ComponentsManager.GetMagneticField(parent.Graphics.Center.X, parent.Graphics.Position.Y + parent.Graphics.GetSize().Y, parent as Coil);
            }
            v3 = ComponentsManager.GetMagneticField(parent.Graphics.Center.X, parent.Graphics.Center.Y, parent as Coil);

            if (oldField == null)
            {
                oldField = (v1 + v2 + v3) / 3;
                return;
            }

            v3 = (v1 + v2 + v3) / 3;

            oldField = v3 - oldField;
            if (parent.ComponentRotation == Component.Rotation.cw0)
                inductionCurrent = oldField.Value.X / 1.5f;
            if (parent.ComponentRotation == Component.Rotation.cw90)
                inductionCurrent = oldField.Value.Y / 1.5f;

            //if (sendingCurrent > inductionCurrent)
            //    inductionCurrent = 0;
            //inductionCurrent = 0;

            oldField = v3;
        }

        public Field desiredField = new CoilLogics.Field();
        public int type = 0;//TODO rm
        public override void Update()
        {
            base.Update();

            Coil l = (Coil)parent;

            desiredField.direction = Math.Sign(pureCurrent);
            desiredField.force = Math.Abs(pureCurrent);

            //TODO charging should account for current and not be + 1

            #region charge
            if ((desiredField.force >= curField.force && (desiredField.direction == curField.direction && desiredField.direction != 0)) ||
                (curField.direction == 0 && desiredField.direction != 0))
            {
                type = 0;
                chargeState = Math.Min(100, chargeState + 1);
                curField.force = desiredField.force;
                curField.direction = desiredField.direction;
                l.W.Resistance = MaxResistance * Math.Pow(Math.E, -chargeState / 10);
                l.W.SendingCurrent = inductionCurrent;
            }
            #endregion
            #region passive discharge
            else if ((pureCurrent == 0 && curField.force != 0) || //freaks out w/ cap
                    (desiredField.force < curField.force && desiredField.direction == curField.direction))
            {
                type = 1;
                double prevstate = chargeState;
                chargeState = Math.Max(0, chargeState - 5f * (pureCurrent == 0 ? 1 : (1 - desiredField.force / curField.force)));

                //l.W.Resistance = MaxResistance * Math.Pow(Math.E, -chargeState / 10);
                //l.W.Resistance = 5;
                //double res = l.W.CircuitPart.GetBranchResistance(l.W);
                l.W.SendingCurrent = MaxAbs(inductionCurrent, (curField.force - desiredField.force) * (curField.direction)) * 10;
                curField.force *= chargeState / prevstate;

                if (chargeState == 0)
                {
                    curField.direction = 0;
                    curField.force = 0;
                }
            }
            #endregion
            #region active discharge
            else if (desiredField.direction != 0 && desiredField.direction != curField.direction && curField.force != 0)
            {
                type = 2;
                double prevstate = chargeState;
                chargeState = Math.Max(0, chargeState - 1f * (pureCurrent == 0 ? 1 : (1 -  desiredField.force / curField.force)));

                l.W.Resistance = MaxResistance * Math.Pow(Math.E, -chargeState / 10);
                //l.W.Resistance = 5;
                l.W.SendingCurrent = MaxAbs(inductionCurrent, (curField.force + desiredField.force) * (curField.direction));
                curField.force *= chargeState / prevstate;

                if (chargeState == 0)
                {
                    curField.direction = 0;
                    curField.force = 0;
                }
            }
            #endregion
            else
            {
                type = 3;
                l.W.SendingCurrent = inductionCurrent;
                if (inductionCurrent == 0)
                    l.W.Resistance = MaxResistance;
                else
                    l.W.Resistance = 1;
            }
        }

        double MaxAbs(double a, double b)
        {
            if (Math.Abs(a) > Math.Abs(b))
                return a;
            return b;
        }
    }
}
