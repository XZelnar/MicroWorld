using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class CapacitorLogics : LogicalComponent, Properties.IRequiresCircuitRecalculation
    {
        private double maxCharge = 100;
        public double Capacitance
        {
            get { return maxCharge; }
            set
            {
                maxCharge = value;
                CurCharge = CurCharge < -maxCharge ? -maxCharge : CurCharge > maxCharge ? maxCharge : CurCharge;
            }
        }
        private double maxOutputVoltage = 50;
        public double MaxOutputVoltage
        {
            get { return maxOutputVoltage; }
            set
            {
                if (value < 5)
                    value = 5;
                if (value > Settings.MAX_VOLTAGE)
                    value = Settings.MAX_VOLTAGE;
                maxOutputVoltage = value;
            }
        }
        private double MaxResistance = 800;//800Ohm
        double _resistance = 0;
        public double Resistance
        {
            get
            {
                _resistance = MaxResistance * (1 - Math.Pow(Math.E / 2, -Math.Abs(CurCharge / maxCharge / 1.5f)));
                return _resistance < 5 ? 5 : _resistance;
            }
        }
        public double OutputPotential
        {
            get 
            {
                if (CurCharge == 0)
                    return 0;
                return Math.Max(1f, Math.Pow(CurCharge / Capacitance, 2f) * MaxOutputVoltage); 
                //return Math.Min(0.1f, MaxOutputVoltage * Math.Pow(Math.E, 1-Math.Abs(CurCharge / maxCharge)));
            }
        }
        public double CurCharge = 0;
        public double CurrentMultiplier = 2;

        Capacitor par;
        internal bool IndividualPass = false;
        private bool probing = false;
        private double maxPotential = 0;



        #region IRequiresCircuitRecalculation
        public void PreIndividualUpdate()
        {
            IndividualPass = true;
            probing = false;
            maxPotential = Math.Max(par.Joints[0].Voltage, par.Joints[1].Voltage);
            if (par.W1.VoltageDropAbs < OutputPotential && CurCharge != 0)
            {
                probing = true;
                MicroWorld.Logics.CircuitManager.Push();

                par.W1.IsConnected = false;
                par.W2.IsConnected = true;
                par.W3.IsConnected = true;

                if (CurCharge < 0)//output left
                {
                    par.Joints[2].IsProvidingPower = true;
                    par.Joints[3].IsGround = true;

                    par.Joints[2].SendingVoltage = OutputPotential - par.W1.VoltageDropAbs;
                    MicroWorld.Logics.CircuitManager.ScheduleReupdate(par.Joints[2]);
                }
                else//output right
                {
                    par.Joints[2].IsGround = true;
                    par.Joints[3].IsProvidingPower = true;

                    par.Joints[3].SendingVoltage = OutputPotential - par.W1.VoltageDropAbs;
                    MicroWorld.Logics.CircuitManager.ScheduleReupdate(par.Joints[3]);
                }
            }
        }

        public void PostIndividualUpdate()
        {
            if (!probing)
            {
                if ((CurCharge == -Capacitance && par.Joints[0].Voltage > par.Joints[1].Voltage) ||
                    (CurCharge == Capacitance && par.Joints[0].Voltage < par.Joints[1].Voltage))//cap is full. can't charge anymore
                    par.W1.IsConnected = false;
                return;
            }
            if (CurCharge == 0)
                return;
            probing = false;
            if ((CurCharge < 0 && !MicroWorld.Logics.CircuitManager.CanReach(par.Joints[0], par.Joints[1])) ||
                (CurCharge > 0 && !MicroWorld.Logics.CircuitManager.CanReach(par.Joints[1], par.Joints[0])))//can't reach. cancel voltage
            {
                ResetStates();
                MicroWorld.Logics.CircuitManager.Pop();
                if (maxPotential <= OutputPotential &&
                    ((CurCharge > 0 && par.Joints[0].Voltage < par.Joints[1].Voltage) ||
                    (CurCharge < 0 && par.Joints[0].Voltage > par.Joints[1].Voltage)))
                    par.W1.IsConnected = false;
                MicroWorld.Logics.CircuitManager.ScheduleReupdate(par.Joints[0]);
            }
            else
            {
                //MicroWorld.Logics.CircuitManager.CanReach(par.Joints[0], par.Joints[1]);
                DisCharge();
            }
            IndividualPass = false;
        }

        public int GetPriority()
        {
            return 0;
        }
        #endregion

        #region Logics

        public void Charge()
        {
            if (par.W1.VoltageDropAbs < OutputPotential)
                return;
            if (par.Joints[0].Voltage > par.Joints[1].Voltage)//left to right
                CurCharge -= par.W1.Current * CurrentMultiplier;
            else
                CurCharge += par.W1.Current * CurrentMultiplier;

            CurCharge = CurCharge < -Capacitance ? -Capacitance : CurCharge > Capacitance ? Capacitance : CurCharge;
            if (Math.Abs(CurCharge) < 0.001)
                CurCharge = 0;
            if (double.IsNaN(CurCharge))
                CurCharge = 0;

            par.W1.Resistance = Resistance;
        }

        public void DisCharge()
        {
            if (CurCharge == 0)
                return;
            if (CurCharge < 0)
                CurCharge += par.W2.Current * CurrentMultiplier;
            else
                CurCharge -= par.W3.Current * CurrentMultiplier;

            if (Math.Abs(CurCharge) < 0.01)
                CurCharge = 0;
            if (double.IsNaN(CurCharge))
                CurCharge = 0;
        }
        #endregion



        public override void Initialize()
        {
            base.Initialize();

            MicroWorld.Logics.CircuitManager.RegisterReUpdatingComponent(this);
            par = parent as Capacitor;
        }

        public override void PreUpdate()
        {
            ResetStates();
        }

        public override void Update()
        {
            if (par.W1.IsConnected && par.W1.Current != 0)//charge
            {
                Charge();
            }

            base.Update();
        }

        private void ResetStates()
        {
            par.W1.IsConnected = true;
            par.W2.IsConnected = false;
            par.W3.IsConnected = false;
            //par.W1.Resistance = par.W2.Resistance = par.W3.Resistance = Resistance;
            par.W1.Resistance = Resistance;
            //par.W2.Resistance = par.W3.Resistance = 1;
            par.Joints[2].SendingVoltage = 0;
            par.Joints[3].SendingVoltage = 0;
            par.Joints[2].IsGround = false;
            par.Joints[3].IsGround = false;
            par.Joints[2].IsProvidingPower = false;
            par.Joints[3].IsProvidingPower = false;
        }

        public override void Reset()
        {
            CurCharge = 0;

            base.Reset();
        }
    }
}
