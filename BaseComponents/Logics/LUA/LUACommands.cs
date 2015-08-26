using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Logics.LUA
{
    class LUACommands
    {
        public static void Initialize()
        {
            LUALevelEngine.ExternalCommands.Add(new LUACommands());
        }

        [AttrLuaFunc("getSwitchState", "Returns wether switch is conencted or not", "Component ID")]
        public bool getSwitchState(int id)
        {
            var a = Components.ComponentsManager.GetComponent((int)id);
            if (a == null || !(a is Components.Switch)) 
                return false;
            return (a as Components.Switch).Connected;
        }

        [AttrLuaFunc("setSwitchState", "Returns wether switch is conencted or not", "Component ID", "State")]
        public void setSwitchState(int id, bool v)
        {
            var a = Components.ComponentsManager.GetComponent((int)id);
            if (a == null || !(a is Components.Switch))
                return;
            (a as Components.Switch).Connected = v;
        }
    }
}
