using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MicroWorld.Sound
{
    public unsafe static class SoundManager
    {
        internal static Dictionary<String, Sound> sounds = new Dictionary<string, Sound>();
        internal static List<EffectInstance> instances = new List<EffectInstance>();
        private static float masterVolume = 1f;

        internal static float MasterVolume
        {
            get { return SoundManager.masterVolume; }
            set
            {
                value = value > 1 ? 1 : value < 0 ? 0 : value;
                float old = masterVolume;
                SoundManager.masterVolume = value;
                if (value != 0 && old != 0)
                {
                    foreach (var i in instances)
                    {
                        i.instance.Volume = i.instance.Volume / old * masterVolume;
                    }
                }
                else
                {
                    foreach (var i in instances)
                    {
                        i.instance.Volume = i.OriginalVolume * masterVolume;
                    }
                }
            }
        }

        public static void Initialize()
        {
        }

        public static void Update()
        {
            if (MicroWorld.Main.Ticks % 60 == 0)//every second
            {
                for (int i = 0; i < instances.Count; i++)
                {
                    if (instances[i].instance.IsDisposed)
                    {
                        instances.RemoveAt(i);
                        i--;
                    }
                    if (instances[i].instance.State == Microsoft.Xna.Framework.Audio.SoundState.Stopped)
                    {
                        instances[i].Dispose();
                        instances.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public static void UnloadContent()
        {
            while (instances.Count > 0)
            {
                instances[0].Dispose();
                instances.RemoveAt(0);
            }
        }

        public static Sound LoadSound(String path)
        {
            if (sounds.ContainsKey(path))
                return sounds[path];
            var a = new Sound();
            a.Load(path);
            sounds.Add(path, a);
            return a;
        }
    }
}
