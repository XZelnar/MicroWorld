using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics.Microcontrollers
{
    public class MicrocontrollersLogics : LogicalComponent
    {
        public global::MicroWorld.Logics.LUA.LUAVM Processor = new global::MicroWorld.Logics.LUA.LUAVM();

        public String Code = "";

        /// <summary>
        /// Executes code in LUA VM
        /// </summary>
        /// <param name="s"></param>
        public virtual void DoStringAsync(String s)
        {
            Code = s;
        }

        /// <summary>
        /// Called when Play button is pressed
        /// </summary>
        public override void Start()
        {
            DoStringAsync(Code);
        }
    }
}
