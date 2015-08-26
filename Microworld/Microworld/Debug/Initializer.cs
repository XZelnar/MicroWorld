using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Debug
{
    static class Initializer
    {
        private static List<System.Windows.Forms.Form> todo = new List<System.Windows.Forms.Form>();

        public static void ShowForm(System.Windows.Forms.Form f)
        {
            todo.Add(f);
        }

        internal static void Update()
        {
            for (int i = 0; i < todo.Count; i++)
                todo[i].Show();
            todo.Clear();
        }
    }
}
