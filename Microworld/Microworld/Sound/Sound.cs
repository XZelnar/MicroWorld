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
    public unsafe class Sound
    {
        internal SoundEffect soundEffect;
        private String name;
        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        public bool IsLoaded = false;

        public void Load(String _name)
        {
            Name = _name;

            System.Threading.Thread load = new System.Threading.Thread(new System.Threading.ThreadStart(_load));
            load.IsBackground = true;
            load.Start();
        }

        private void _load()
        {
            soundEffect = ResourceManager.Load<SoundEffect>(Name);
            IsLoaded = true;
        }

        public EffectInstance Play(float volume, float pitch, float pan, bool isLooped)
        {
            while (!IsLoaded)
                System.Threading.Thread.Sleep(1);
            EffectInstance e = (EffectInstance)soundEffect.CreateInstance();
            e.parent = this;
            e.IsLooped = isLooped;
            e.Volume = volume;
            e.Pitch = pitch;
            e.Pan = pan;
            e.Play();
            return e;
        }

        public EffectInstance Play(float volume)
        {
            return Play(volume, 0f, 0f, false);
        }

        public EffectInstance Play(float volume, bool isLooped)
        {
            return Play(volume, 0f, 0f, isLooped);
        }

        public EffectInstance Play(float volume, float pitch, float pan)
        {
            return Play(volume, pitch, pan, false);
        }
    }
}
