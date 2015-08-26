using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Logics
{
    public class CampaingProgress
    {
        internal static Dictionary<String, byte[]> levelsCompleted = new Dictionary<string, byte[]>();

        public static void Initialize()
        {
            IO.Log.Write("        Initializing CampaingProgresss");
            if (System.IO.File.Exists("Saves/progress.lpg"))
            {
                IO.Log.Write("            Found save file. Loading...");
                Load();
            }
            else
            {
                IO.Log.Write("            Save file not found. Ignoring...");
            }

            String[] names;
            for (int i = 0; i < (names = Graphics.GUI.Scene.LevelSelection.TABS_NAMES).Length; i++)//dimensions check
            {
                if (!levelsCompleted.ContainsKey(names[i]))
                {
                    levelsCompleted.Add(names[i], new byte[Graphics.GUI.Scene.LevelSelection.TABS_LEVELS_COUNT[i]]);
                }
                else
                {
                    if (levelsCompleted[names[i]].Length != Graphics.GUI.Scene.LevelSelection.TABS_LEVELS_COUNT[i])
                    {
                        levelsCompleted.Remove(names[i]);
                        levelsCompleted.Add(names[i], new byte[Graphics.GUI.Scene.LevelSelection.TABS_LEVELS_COUNT[i]]);
                    }
                }
            }
        }

        public static void Save()
        {
            IO.Log.Write("    Saving CampaingProgresss...");
            byte[][] values = new byte[levelsCompleted.Values.Count][];
            levelsCompleted.Values.CopyTo(values, 0);
            String[] keys = new String[levelsCompleted.Values.Count];
            levelsCompleted.Keys.CopyTo(keys, 0);

            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(new System.IO.FileStream("Saves/progress.lpg", 
                System.IO.FileMode.OpenOrCreate));

            for (int i = 0; i < keys.Length; i++)
            {
                bw.Write(keys[i]);
                bw.Write(values[i].Length);
                for (int j = 0; j < values[i].Length; j++)
                {
                    bw.Write(values[i][j]);
                }
            }

            bw.Close();
            IO.Log.Write("    Saving complete");
        }

        public static void Load()
        {
            IO.Log.Write("    Loading CampaingProgresss...");
            levelsCompleted.Clear();

            System.IO.BinaryReader br = new System.IO.BinaryReader(new System.IO.FileStream("Saves/progress.lpg",
                System.IO.FileMode.Open));

            String s = "";
            int l = 0;
            while (br.PeekChar() > -1)
            {
                s = br.ReadString();
                l = br.ReadInt32();
                byte[] d = new byte[l];
                for (int j = 0; j < l; j++)
                {
                    d[j] = br.ReadByte();
                }
                levelsCompleted.Add(s, d);
            }

            br.Close();
            IO.Log.Write("    Loading complete");
        }

        public static bool IsComplete(String name, int lvl)
        {
            if (!levelsCompleted.ContainsKey(name)) return false;

            var a = levelsCompleted[name];
            if (a.Length <= lvl) return false;

            return a[lvl] > 0;
        }

        public static bool IsOpened(String name, int lvl)
        {
            return lvl == 0 || name == "Levels/Tut/" || IsComplete(name, lvl - 1);
        }

        public static void SetCompleted(String name, int lvl)
        {
            IO.Log.Write("    Setting [" + name + ";" + lvl.ToString() + "] complete...");
            if (!levelsCompleted.ContainsKey(name)) return;

            var a = levelsCompleted[name];
            if (a.Length <= lvl) return;

            a[lvl] = 1;

            Save();
            IO.Log.Write("    Complete set");
        }

        public static void SetCurrentCompleted()
        {
            Graphics.GUI.Scene.LevelSelection a = Graphics.GUI.GUIEngine.s_levelSelection as Graphics.GUI.Scene.LevelSelection;
            SetCompleted(a.folder, a.selectedLevel);
        }


    }
}
