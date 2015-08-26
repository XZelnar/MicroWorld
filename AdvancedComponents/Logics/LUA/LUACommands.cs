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

        [AttrLuaFunc("getAdvancedJointPortState", "Returns wether AJ port is input or output", "AJ ID", "Port")]
        public string getAdvancedJointPortState(int id, String port)
        {
            var a = Components.ComponentsManager.GetComponent((int)id);
            if (a == null || !(a is Components.AdvancedJoint)) 
                return "";
            port = port.ToLower();
            switch (port)
            {
                case "left":
                    return (a as Components.AdvancedJoint).Left.ToString();
                case "up":
                    return (a as Components.AdvancedJoint).Up.ToString();
                case "right":
                    return (a as Components.AdvancedJoint).Right.ToString();
                case "down":
                    return (a as Components.AdvancedJoint).Down.ToString();
                default:
                    return "";
            }
        }

        [AttrLuaFunc("isAdvancedJointPortInput", "Returns wether AJ port is input or not", "AJ ID", "Port")]
        public bool isAdvancedJointPortInput(int id, String port)
        {
            var a = Components.ComponentsManager.GetComponent((int)id);
            if (a == null || !(a is Components.AdvancedJoint))
                return false;
            port = port.ToLower();
            switch (port)
            {
                case "left":
                    return (a as Components.AdvancedJoint).Left == PortState.Input;
                case "up":
                    return (a as Components.AdvancedJoint).Up == PortState.Input;
                case "right":
                    return (a as Components.AdvancedJoint).Right == PortState.Input;
                case "down":
                    return (a as Components.AdvancedJoint).Down == PortState.Input;
                default:
                    return false;
            }
        }

        [AttrLuaFunc("isAdvancedJointPortOutput", "Returns wether AJ port is output or not", "AJ ID", "Port")]
        public bool isAdvancedJointPortOutput(int id, String port)
        {
            var a = Components.ComponentsManager.GetComponent((int)id);
            if (a == null || !(a is Components.AdvancedJoint))
                return false;
            port = port.ToLower();
            switch (port)
            {
                case "left":
                    return (a as Components.AdvancedJoint).Left == PortState.Output;
                case "up":
                    return (a as Components.AdvancedJoint).Up == PortState.Output;
                case "right":
                    return (a as Components.AdvancedJoint).Right == PortState.Output;
                case "down":
                    return (a as Components.AdvancedJoint).Down == PortState.Output;
                default:
                    return false;
            }
        }
    }
}
