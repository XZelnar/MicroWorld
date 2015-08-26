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
    class LevelDesignerSaveMenu : SaveMenu
    {
        public override void Initialize()
        {
            folder = "Levels";
            mask = "*.lvl";
            base.Initialize();
        }

        protected override void Save()
        {
            IO.SaveEngine.SaveAll("Saves/" + folder + "/" + tb.Text + ".lvl", IO.SaveEngine.SaveType.LevelDesigner);
        }

    }
}
