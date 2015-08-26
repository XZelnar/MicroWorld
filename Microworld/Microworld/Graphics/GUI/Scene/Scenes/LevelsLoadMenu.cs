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
    class LevelsLoadMenu : LoadMenu
    {
        public override void Initialize()
        {
            folder = "Levels";
            base.Initialize();
        }

        public override void backClick(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.ChangeScene(GUIEngine.s_levelsMenu, "GUILevels");
            //Main.curState = "GUILevels";
            //GUIEngine.curScene = GUIEngine.s_levelsMenu;
        }

        protected override void _load()
        {
            //Logics.LevelEngine.Stop();
            //Logics.GameInputHandler.PlacableAreas.Clear();
            //Settings.ResetInGameSettings();
            Logics.GameLogicsHelper.InitForGame();

            IO.SaveEngine.LoadAll("Saves/" + folder + "/" + saves.GetSelected() + ".sav", IO.SaveEngine.SaveType.LevelsSave);
            Logics.GameLogicsHelper.InitScenesForGame();
            //Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_componentSelector);
            //Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_runControl);
            GUIEngine.curScene = Graphics.GUI.GUIEngine.s_game;
            Main.curState = "GAMELevels";
        }
    }
}
