using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MicroWorld.IO
{
    public static unsafe class Log
    {
        public enum State
        {
            INFO = 0,
            WARNING = 1,
            SEVERE = 2,
            ERROR = 3,
            CONSOLE = 4
        }

        static bool isBeingUsed = false;

        internal static void Initialize()
        {
            if (!Settings.Debug) return;
            if (!Directory.Exists("debug"))
                Directory.CreateDirectory("debug");
            if (File.Exists("debug/debug.log"))
                if (new FileInfo("debug/io.log").Length > 0)
                    File.Move("debug/debug.log", "debug/debug" + DateTime.Now.Ticks.ToString() + ".log");
                else
                    File.Delete("debug/debug.log");
            File.CreateText("debug/debug.log").Close();
            if (File.Exists("debug/io.log"))
                if (new FileInfo("debug/io.log").Length > 0)
                    File.Move("debug/io.log", "debug/io" + DateTime.Now.Ticks.ToString() + ".log");
                else
                    File.Delete("debug/io.log");
            File.CreateText("debug/io.log").Close();
        }

        internal static void SanityCheck()
        {
            if (!Directory.Exists("debug"))
                Directory.CreateDirectory("debug");
            if (!File.Exists("debug/debug.log"))
                File.CreateText("debug/debug.log").Close();
            if (!File.Exists("debug/io.log"))
                File.CreateText("debug/io.log").Close();
        }

        private static List<int> privateListToUseInLockToIndicateWhenFileIsBeingWrittenInto = new List<int>();
        public static void Write(State s, String msg)
        {
            if (!Settings.Debug) return;
            while (isBeingUsed) ;

            lock (privateListToUseInLockToIndicateWhenFileIsBeingWrittenInto)
            {
                isBeingUsed = true;
                SanityCheck();
                System.IO.BinaryWriter sw = new BinaryWriter(new FileStream("debug/debug.log", FileMode.Append));
                sw.Write(Encode("[" + DateTime.Now.ToString() + "] " +
                    "[" + Debug.DebugInfo.FramesPerSecond.ToString() + "FPS] [" + Debug.DebugInfo.UpdatesPerSecond.ToString() + "TPS] " +
                    "[" + (System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / 1024 / 1024).ToString() + "mb] " +
                    "[" + s.ToString() + "]" +
                    " : " + msg + "\r\n"));
                sw.Close();
                sw.Dispose();
                isBeingUsed = false;
            }
        }

        public static void UnloadContent()
        {
            if (Settings.LogInput)
            {
                InputEngine.logger.Close();
                InputEngine.logger.Dispose();
            }
        }

        private static char[] Encode(String s)
        {
            char[] a = s.ToCharArray();
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = (char)(a[i] ^ 145);
            }
            return a;
        }

        public static void Write(String msg)
        {
            Write(State.INFO, msg);
        }

        public static void Write(Exception e, int exceptionsInRow, bool draw, bool update)
        {
            String s = "";
            s = "An exception was thrown";
            if (draw) s = "An exception was thrown in Draw";
            if (update) s = "An exception was thrown in Update";
            s += "\r\n";
            s += exceptionsInRow.ToString() + " exceptions in raw";
            s += "\r\n\r\n" + e.Message + "\r\n\r\n" + e.Source + "\r\n\r\n" + e.StackTrace;
            Write(State.ERROR, s);
        }
    }
}
