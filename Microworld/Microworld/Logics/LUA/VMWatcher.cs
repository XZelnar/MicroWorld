using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Logics.LUA
{
    abstract class VMWatcher
    {
        public static List<LUAVM> VMs = new List<LUAVM>();

        public static void Unload()
        {
            for (int i = 0; i < VMs.Count; i++)
            {
                VMs[i].Terminate();
            }
        }
    }
}
