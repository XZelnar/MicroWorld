using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MicroWorld.Logics.LUA
{
    class LUAVMCommands
    {
        [AttrLuaFunc("write", "Writes a given command", "String to write")]
        public void write(String strCmd)
        {
            OutputEngine.Write(strCmd);
            OutputEngine.WriteLine();
        }

        [AttrLuaFunc("writeln", "Writes a given command and moves cursor to a new line", "String to write")]
        public void writeln(String strCmd)
        {
            OutputEngine.WriteLine(strCmd);
            OutputEngine.WriteLine();
        }

        [AttrLuaFunc("sin", "Returns the sine of the specific angle", "Angle")]
        public double sin(double a)
        {
            return Math.Sin(a);
        }

        [AttrLuaFunc("cos", "Returns the cosine of the specific angle", "Angle")]
        public double cos(double a)
        {
            return Math.Cos(a);
        }

        [AttrLuaFunc("tg", "Returns the tangent of the specific angle", "Angle")]
        public double tg(double a)
        {
            return Math.Tan(a);
        }

        [AttrLuaFunc("ctg", "Returns the cotangent of the specific angle", "Angle")]
        public double ctg(double a)
        {
            return 1d / Math.Tan(a);
        }

        [AttrLuaFunc("bitnot", "~", "Value")]
        public double bitnot(double a)
        {
            try
            {
                uint aa = (uint)a;
                uint b = (~aa);
                return b;
            }
            catch { return -1; }
        }

        [AttrLuaFunc("_delay_ms", "Delays the thread", "Time in milliseconds")]
        public void _delay_ms(double ms)
        {
            System.Threading.Thread.Sleep((int)ms);
        }

        [AttrLuaFunc("nop", "No operation")]
        public void nop()
        {
        }
    }
}
