using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Graphics.GUI.Scene.Animation
{
    public abstract class SceneAnimation
    {
        private Scene scene = null;
        private bool isFadeIn = false;
        private bool isFadeOut = false;
        private System.Threading.Thread animatingThread = null;
        private bool forceStop = false;

        protected int ticks = 0;
        protected virtual System.Threading.Thread AnimatingThread
        {
            get { return animatingThread; }
            set { animatingThread = value; }
        }

        public object Tag = null;
        public virtual bool IsFadeIn
        {
            get { return isFadeIn; }
            protected set { isFadeIn = value; }
        }
        public virtual bool IsFadeOut
        {
            get { return isFadeOut; }
            protected set { isFadeOut = value; }
        }
        public virtual bool IsFade
        {
            get { return IsFadeIn || IsFadeOut; }
        }
        public virtual Scene Scene
        {
            get { return scene; }
            set
            {
                Init(value);
            }
        }
        public bool ForceStop
        {
            get { return forceStop; }
            set { forceStop = value; }
        }

        #region Events
        public delegate void FadeEventHandler(object sender);
        public event FadeEventHandler onFadeInFinish;
        public event FadeEventHandler onFadeOutFinish;
        #endregion

        public virtual void Init(Scene s)
        {
            scene = s;
            ForceStop = false;
        }

        public virtual void Dispose()
        {
            StopFade();
            scene = null;
        }

        protected virtual void fadeIn()
        {
            if (onFadeInFinish != null)
                onFadeInFinish.Invoke(this);
            IsFadeIn = false;
        }

        protected virtual void fadeOut()
        {
            if (onFadeOutFinish != null)
                onFadeOutFinish.Invoke(this);
            IsFadeOut = false;
        }

        public virtual void FadeIn()
        {
            if (IsFade)
                return;
            IsFadeIn = true;
            ForceStop = false;
            while (true)
            {
                try
                {
                    AnimatingThread = new System.Threading.Thread(fadeIn);
                    AnimatingThread.IsBackground = true;
                    AnimatingThread.Start();
                    break;
                }
                catch (System.Threading.ThreadStartException) { }
            }
        }

        public virtual void FadeOut()
        {
            if (IsFade)
                return;
            ForceStop = false;
            IsFadeOut = true;
            while (true)
            {
                try
                {
                    AnimatingThread = new System.Threading.Thread(fadeOut);
                    AnimatingThread.IsBackground = true;
                    AnimatingThread.Start();
                    break;
                }
                catch (System.Threading.ThreadStartException) { }
            }
        }

        public virtual void StopFadeIn()
        {
            forceStop = true;
            if (!IsFadeIn)
                return;
            if (AnimatingThread != null)
                AnimatingThread.Abort();
            IsFadeIn = false;
        }

        public virtual void StopFadeOut()
        {
            forceStop = true;
            if (!IsFadeOut)
                return;
            //if (AnimatingThread != null)
            //    AnimatingThread.Abort();
            IsFadeOut = false;
        }

        public virtual void StopFade()
        {
            StopFadeIn();
            StopFadeOut();
        }
    }
}
