using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MicroWorld.Components.Logics
{
    class TeslaCoilLogics : LogicalComponent
    {
        public float CurCharge = 0;
        public float Capacitance = 10000;
        public float DischargeVoltage = 100;
        public float Range = 128;

        internal float ChargingProcess
        {
            get { return CurCharge / Capacitance; }
        }

        TeslaCoil p;
        Random r;

        float a = 0;
        float d = 0;
        float d2 = 0;
        float x1 = 0, y1 = 0;
        float x2 = 0, y2 = 0;

        int ticksSinceFinished = 0;

        List<MicroWorld.Graphics.Particles.Lightning> lightnings = new List<MicroWorld.Graphics.Particles.Lightning>();
        MicroWorld.Graphics.Particles.Lightning lightningBig;
        Component lightningComponent;

        public override void Initialize()
        {
            r = new Random();
            p = parent as TeslaCoil;

            base.Initialize();
        }

        public override void Reset()
        {
            CurCharge = 0;
            ticksSinceFinished = 0;

            for (int i = 0; i < lightnings.Count; i++)
            {
                lightnings[i].IsDead = true;
            }
            if (lightningBig != null)
            {
                lightningBig.IsDead = true;
            }
            lightningBig = null;
            lightningComponent = null;
            lightnings.Clear();

            base.Reset();
        }

        public override void Update()
        {
            if (lightningBig != null && lightningBig.IsDead)
            {
                lightningBig = null;
                lightningComponent = null;
            }

            CurCharge += (float)p.W1.VoltageDropAbs;
            CurCharge += (float)p.W2.VoltageDropAbs;
            CurCharge += (float)p.W3.VoltageDropAbs;
            CurCharge += (float)p.W4.VoltageDropAbs;

            if (CurCharge > Capacitance)
                CurCharge = Capacitance;

            if (CurCharge == Capacitance)
            {
                ticksSinceFinished++;
                if (ticksSinceFinished % 10 != 1)
                    goto PostLightning;

                var a = ComponentsManager.GetComponents<Properties.IAttractsLightning>((int)parent.Graphics.Center.X, (int)parent.Graphics.Center.Y, Range);
                if (a.Count != 0)
                {
                    //find joint to strike
                    #region Search
                    float[] rads = new float[a.Count];
                    int[] prob = new int[a.Count];
                    List<int> minindices = new List<int>();
                    double minDistance = Range * Range * 2;
                    double d;
                    lightningComponent = null;
                    if (a.Count == 1)
                    {
                        lightningComponent = a[0] as Component;
                    }
                    else
                    {
                        for (int i = 0; i < a.Count; i++)
                        {
                            d = (parent.Graphics.Center - (a[i] as Component).Graphics.Center).LengthSquared();
                            rads[i] = (float)d;
                            if (d < minDistance)
                            {
                                minDistance = d;
                                minindices.Clear();
                                minindices.Add(i);
                            }
                            else if (d == minDistance)
                            {
                                minindices.Add(i);
                            }
                        }

                        int maxrand = 0;
                        for (int i = 0; i < a.Count; i++)
                        {
                            prob[i] = (int)(minDistance / rads[i] * 100);
                            if (prob[i] < 95)
                                prob[i] /= 2;
                            maxrand += prob[i];
                        }

                        int t = r.Next(0, maxrand);

                        for (int i = 0; i < a.Count; i++)
                        {
                            t -= prob[i];
                            if (t <= 0)
                            {
                                lightningComponent = a[i] as Component;
                                break;
                            }
                        }
                        if (lightningComponent == null)
                            lightningComponent = a[a.Count - 1] as Component;
                    }
                    #endregion

                    lightningBig = new MicroWorld.Graphics.Particles.Lightning(parent.Graphics.Center, lightningComponent.Graphics.Center, 2, 
                            (int)(Capacitance / DischargeVoltage), 6, true, 0.4f);
                    MicroWorld.Graphics.ParticleManager.Add(lightningBig);
                    (lightningComponent as Properties.IAttractsLightning).GetStruck(parent, DischargeVoltage, lightningBig.RemainingTime);
                    CurCharge = 0;
                    ticksSinceFinished = 0;
                }
            PostLightning: ;
            }

            if (CurCharge > r.Next(0, (int)Capacitance))
            {
                a = (float)r.Next(360) / 180f * (float)Math.PI;
                d = r.Next(10, 19);
                x1 = (float)Math.Cos(a) * d + parent.Graphics.Center.X;
                y1 = (float)Math.Sin(a) * d + parent.Graphics.Center.Y;
                d2 = d + r.Next(8, 12);
                x2 = (float)Math.Cos(a) * d2 + parent.Graphics.Center.X;
                y2 = (float)Math.Sin(a) * d2 + parent.Graphics.Center.Y;

                var ln = new MicroWorld.Graphics.Particles.Lightning(new Vector2(x1, y1), new Vector2(x2, y2), 1, 10);
                lightnings.Add(ln);
                MicroWorld.Graphics.ParticleManager.Add(ln);
            }

            for (int i = 0; i < lightnings.Count; i++)
            {
                if (lightnings[i].IsDead)
                {
                    lightnings.RemoveAt(i);
                    i--;
                }
            }

            base.Update();
        }

        public void MoveParticles(Vector2 d)
        {
            for (int i = 0; i < lightnings.Count; i++)
            {
                lightnings[i].Position += d;
            }
            if (lightningBig != null)
            {
                lightningBig.Move(parent.Graphics.Center, lightningComponent.Graphics.Center);
                //lightningBig.IsDead = true;
                //lightningBig =
                //    new MicroWorld.Graphics.Particles.Lightning(parent.Graphics.Center, lightningComponent.Graphics.Center, 2, lightningBig.RemainingTime);
                //MicroWorld.Graphics.ParticleManager.Add(lightningBig);
            }
        }
    }
}
