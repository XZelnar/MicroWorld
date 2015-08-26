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
    class LevelsSaveMenu : SaveMenu
    {
        public override void Initialize()
        {
            folder = "Levels";
            base.Initialize();
        }

        protected override void Save()
        {
            IO.SaveEngine.SaveAll("Saves/" + folder + "/" + tb.Text + ".sav", IO.SaveEngine.SaveType.Levels);
        }

    }
}
