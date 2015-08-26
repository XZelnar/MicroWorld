using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Graphics.GUI.Scene
{
    class LoadLevel : LoadFrameScene
    {
        public override void Initialize()
        {
            folder = "Levels";
            base.Initialize();
            lTitle.text = "Load Level";
        }

        protected override void _load()
        {
            Logics.GameLogicsHelper.InitForGame();

            IO.SaveEngine.LoadAll("Saves/" + folder + "/" + saves.GetSelected() + ".sav", IO.SaveEngine.SaveType.LevelsSave);
            Logics.GameLogicsHelper.InitScenesForGame();
            GUIEngine.curScene = Graphics.GUI.GUIEngine.s_game;
            Main.curState = "GAMELevels";
        }
    }
}
