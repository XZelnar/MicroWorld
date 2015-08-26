using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld
{
    static class OutputEngine
    {
        public const int LOG_LENGTH = 50;
        public static String[] log = new String[LOG_LENGTH];

        public static void Write(String s)
        {
            log[0] += s;
        }

        public static void WriteLine(String s)
        {
            IO.Log.Write(IO.Log.State.CONSOLE, "[CONSOLE] " + s);
            Write(s);
            WriteLine();
        }

        public static void WriteLine()
        {
            for (int i = LOG_LENGTH - 1; i >= 1; i--)
            {
                log[i] = log[i - 1];
            }
            log[0] = "";
        }
    }
}
