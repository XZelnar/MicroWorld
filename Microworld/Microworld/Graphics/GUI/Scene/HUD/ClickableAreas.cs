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

namespace MicroWorld.Graphics.GUI.Scene
{
    class ClickableAreas : HUDScene
    {
        public override void Initialize()
        {
            Layer = 700;
            base.Initialize();
        }

        public override void LoadContent()
        {
            ClickabilityOverlay.LoadContent();
            base.LoadContent();
        }

        public override void Draw(Renderer renderer)
        {
            renderer.End();
            ClickabilityOverlay.DrawOverlay();
            renderer.BeginUnscaled();
        }

        public override void OnGraphicsDeviceReset()
        {
            for (int i = 0; i < 10; i++)
            {
                ClickabilityOverlay.Update();
            }
            base.OnGraphicsDeviceReset();
        }
    }
}
