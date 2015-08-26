using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class SegmentDisplay4Logics : LogicalComponent
    {
        public byte[] digits = new byte[4];
        public byte[] predigits = new byte[4];
        public byte[][] digitsold = new byte[80][];

        public override void Initialize()
        {
            base.Initialize();
            for (int i = 0; i < digitsold.Length; i++)
            {
                digitsold[i] = new byte[4];
            }
        }

        public override void Update()
        {
            base.Update();

            old.SegmentDisplay4 l = ((old.SegmentDisplay4)parent);

            for (int i = digitsold.Length - 1; i > 0; i--)
            {
                digitsold[i][0] = digitsold[i - 1][0];
                digitsold[i][1] = digitsold[i - 1][1];
                digitsold[i][2] = digitsold[i - 1][2];
                digitsold[i][3] = digitsold[i - 1][3];
            }
            digitsold[0][0] = digits[0];
            digitsold[0][1] = digits[1];
            digitsold[0][2] = digits[2];
            digitsold[0][3] = digits[3];

            //predigits = digits;

            digits[0] = 0;
            digits[1] = 0;
            digits[2] = 0;
            digits[3] = 0;

            Wire w;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    w = l.W[i*4+j];
                    if (w.J1.Voltage > w.J2.Voltage && Math.Abs(w.J1.Voltage - w.J2.Voltage) > 2)//voltage goes thru
                    {
                        if ((predigits[j] & (1 << i)) != 0)
                            digits[j] += (byte)(1 << i);
                    }
                }
            }

            predigits[0] = 0;
            predigits[1] = 0;
            predigits[2] = 0;
            predigits[3] = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    w = l.W[i * 4 + j];
                    if (w.J1.Voltage > w.J2.Voltage && Math.Abs(w.J1.Voltage - w.J2.Voltage) > 2)//voltage goes thru
                    {
                        predigits[j] += (byte)(1 << i);
                    }
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            for (int i = 0; i < digitsold.Length; i++)
            {
                digitsold[i] = new byte[4];
            }
        }

    }
}
