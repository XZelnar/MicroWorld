using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Graphics.GUI.Scene
{
    class LoadLevelDesigner : LoadFrameScene
    {
        public override void Initialize()
        {
            folder = "Levels";
            mask = "*.lvl";
            base.Initialize();
            lTitle.text = "Load Level";
        }

        protected override void _load()
        {
            Logics.GameLogicsHelper.InitForGame();

            IO.SaveEngine.LoadAll("Saves/" + folder + "/" + saves.GetSelected() + ".lvl", IO.SaveEngine.SaveType.LevelDesigner);

            Main.curState = "GAMElvlDesign";
            Logics.GameLogicsHelper.InitScenesForGame();
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_placableAreasCreator);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_scriptEditor);
            Graphics.GUI.GUIEngine.s_componentSelector.ClearCount();
            GUIEngine.curScene = Graphics.GUI.GUIEngine.s_game;
        }
    }
}
