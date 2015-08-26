using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class CoreLogics : LogicalComponent
    {
        public const int START_SLEEP = 0;

        internal bool[] target = new bool[1];
        internal bool[] lasttarget = null;
        internal bool[] result = new bool[1];
        internal bool[] lastresult = null;
        internal int cur = 0;

        internal bool isFilled = false;
        internal bool WasMatched = false;
        int sleeped = 0;

        public bool record = false;
        public float RequiredAccuracy = 1f;

        public float CorrectPercent
        {
            get
            {
                float c = 0;
                for (int i = 0; i < result.Length && i < cur; i++)
                {
                    if (result[i] == target[i]) c++;
                }
                return c / result.Length;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Reset()
        {
            sleeped = 0;
            isFilled = false;
            WasMatched = false;
            cur = 0;
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = false;
            }
            base.Reset();
        }

        public override void Update()
        {
            if (sleeped < START_SLEEP)
            {
                sleeped++;
                base.Update();
                return;
            }

            if (WasMatched && !record)
            {
                base.Update();
                return;
            }

            var a = parent as Core;
            if (cur <= target.Length - 1)
            {
                result[cur] = a.Joints[0].Voltage > 2.5;
                cur++;
            }
            else
            {
                isFilled = true;
                for (int i = 0; i < result.Length - 1; i++)
                {
                    result[i] = result[i + 1];
                }
                result[result.Length - 1] = a.Joints[0].Voltage > 2.5;
            }
            if (cur > target.Length - 1)
            {
                if (record)
                    target = (bool[])result.Clone();
                else if (IsCorrect())
                {
                    WasMatched = true;
                    (parent as Core).InvokeRecievedFinished();
                }
            }
            base.Update();
        }

        public bool IsCorrect()
        {
            if (WasMatched)
                return true;
            float c = 0;
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] == target[i]) c++;
            }
            return c / result.Length >= RequiredAccuracy;
        }

        public void Load(String s)
        {
            if (s == null) return;
            target = new bool[s.Length];
            result = new bool[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                target[i] = s[i] == '1';
            }
        }

    }
}
