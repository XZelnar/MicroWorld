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
    class LevelDesignerLoadMenu : LoadMenu
    {
        public override void Initialize()
        {
            folder = "Levels";
            mask = "*.lvl";
            base.Initialize();
        }

        public override void backClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.ChangeScene(GUIEngine.s_lvlDesignerMenu, "GAMElvlDesignerMenu");
            //Main.curState = "GUILevels";
            //GUIEngine.curScene = GUIEngine.s_levelsMenu;
        }

        protected override void _load()
        {
            //Logics.CircuitManager.Clear();
            //Components.ComponentsManager.Clear();
            //Logics.LevelEngine.Stop();
            //Logics.GameInputHandler.PlacableAreas.Clear();
            //Settings.ResetInGameSettings();
            Logics.GameLogicsHelper.InitForGame();

            IO.SaveEngine.LoadAll("Saves/" + folder + "/" + saves.GetSelected() + ".lvl", IO.SaveEngine.SaveType.LevelDesigner);

            Main.curState = "GAMElvlDesign";
            Logics.GameLogicsHelper.InitScenesForGame();
            //Graphics.GUI.GUIEngine.s_componentSelector.SelectedIndex = 0;
            //Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_componentSelector);
            //Graphics.GUI.GUIEngine.s_componentSelector.AddCores();
            //Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_runControl);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_placableAreasCreator);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_scriptEditor);
            GUIEngine.curScene = Graphics.GUI.GUIEngine.s_game;
        }
    }
}
