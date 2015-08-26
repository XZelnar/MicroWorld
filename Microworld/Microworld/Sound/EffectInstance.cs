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
    public class EffectInstance
    {
        public enum Effects
        {
            None = 0,
            FadeIn = 1,
            FadeOut = 2
        }

        private bool isDisposing = false;
        internal SoundEffectInstance instance;

        public Effects effect = Effects.None;
        public Sound parent;
        public float OriginalVolume = 1f;

        public bool IsLooped
        {
            get { return instance.IsLooped; }
            set { instance.IsLooped = value; }
        }
        public float Pitch
        {
            get { return instance.Pitch; }
            set { instance.Pitch = value; }
        }
        public float Pan
        {
            get { return instance.Pan; }
            set { instance.Pan = value; }
        }
        public SoundState State
        {
            get { return instance.IsDisposed ? SoundState.Stopped : instance.State; }
        }
        public float Volume
        {
            get { return instance.Volume; }
            set
            {
                if (instance.IsDisposed) 
                    return;
                value = value > 1 ? 1 : value < 0 ? 0 : value;
                OriginalVolume = value;
                instance.Volume = value * SoundManager.MasterVolume;
            }
        }

        public static implicit operator EffectInstance(SoundEffectInstance e)
        {
            return new EffectInstance() { instance = e };
        }

        public EffectInstance()
        {
            SoundManager.instances.Add(this);
        }

        public void Dispose()
        {
            isDisposing = true;
            while (isDisposing && effect != Effects.None)
                System.Threading.Thread.Sleep(1);
            instance.Dispose();
        }

        public void Play()
        {
            if (!instance.IsDisposed)
                instance.Play();
        }

        public void Pause()
        {
            if (!instance.IsDisposed)
                instance.Pause();
        }

        public void Resume()
        {
            if (!instance.IsDisposed)
                instance.Resume();
        }

        public void Stop()
        {
            if (!instance.IsDisposed)
                instance.Stop();
        }

        #region Effects
        System.Threading.Thread t;
        public void FadeOut()
        {
            if (instance.IsDisposed)
                return;
            if (t != null) t.Abort();
            t = new System.Threading.Thread(new System.Threading.ThreadStart(_fadeOut));
            t.Start();
        }

        private void _fadeOut()
        {
            effect = Effects.FadeOut;
            var a = this;
            for (float i = OriginalVolume; i > 0 && !isDisposing; i -= 0.002f)
            {
                if (i < 0) i = 0;
                try
                {
                    a.instance.Volume = i * SoundManager.MasterVolume;
                }
                catch { }
                System.Threading.Thread.Sleep(10);
            }
            a.Stop();
            effect = Effects.None;
            isDisposing = false;
        }

        public void FadeIn()
        {
            if (instance.IsDisposed)
                return;
            if (t != null) t.Abort();
            t = new System.Threading.Thread(new System.Threading.ThreadStart(_fadeIn));
            t.Start();
        }

        private void _fadeIn()
        {
            effect = Effects.FadeIn;
            var a = this;
            for (float i = 0; i < 1 && !isDisposing; i += 0.002f)
            {
                if (i > 1) i = 1;
                try
                {
                    a.instance.Volume = i * OriginalVolume * SoundManager.MasterVolume;
                }
                catch { }
                System.Threading.Thread.Sleep(10);
            }
            effect = Effects.None;
            isDisposing = false;
        }
        #endregion

    }
}
