using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld
{
    class ModBaseComponents : Modding.BaseMod
    {
        public override void Initialize()
        {
            Logics.LUA.LUACommands.Initialize();
        }
    }
}
