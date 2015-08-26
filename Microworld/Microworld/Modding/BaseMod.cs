using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Modding
{
    public class BaseMod
    {
        public virtual void Initialize()
        {
        }

        public virtual void LoadContent()
        {
        }

        public virtual void PreUpdate()
        {
        }

        public virtual void PostUpdate()
        {
        }

        public virtual void PreDraw(Graphics.Renderer renderer)
        {
        }

        public virtual void PostDraw(Graphics.Renderer renderer)
        {
        }
    }
}
