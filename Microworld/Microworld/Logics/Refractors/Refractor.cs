using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Logics.Refractors
{
    public class Refractor
    {
        public virtual void RefractorString(ref String s)
        {
            s = s.Replace("~(", "bitnot(");
            s = s.Replace(" && ", " and ");
            s = s.Replace(" || ", " or ");
            s = s.Replace("!", "not ");
            s = s.Replace("nop", "nop()");
        }
    }
}
