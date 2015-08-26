using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Modding
{
    static class ModdingLogics
    {
        internal static List<BaseMod> registeredMods = new List<BaseMod>();

        internal static void Initialize()
        {
            OutputEngine.WriteLine("Total mods registered: " + registeredMods.Count);
            IO.Log.Write("  Total mods registered: " + registeredMods.Count);

            for (int i = 0; i < registeredMods.Count; i++)
            {
                Graphics.GUI.Scene.Console.vm.RegisterAssembly(registeredMods[i].GetType().Assembly);
                registeredMods[i].Initialize();
            }
        }

        internal static void LoadContent()
        {
            for (int i = 0; i < registeredMods.Count; i++)
                registeredMods[i].LoadContent();
        }

        internal static void PreUpdate()
        {
            for (int i = 0; i < registeredMods.Count; i++)
                registeredMods[i].PreUpdate();
        }

        internal static void PostUpdate()
        {
            for (int i = 0; i < registeredMods.Count; i++)
                registeredMods[i].PostUpdate();
        }

        internal static void PreDraw()
        {
            for (int i = 0; i < registeredMods.Count; i++)
                registeredMods[i].PreDraw(Graphics.GraphicsEngine.Renderer);
        }

        internal static void PostDraw()
        {
            for (int i = 0; i < registeredMods.Count; i++)
                registeredMods[i].PostDraw(Graphics.GraphicsEngine.Renderer);
        }
    }
}
