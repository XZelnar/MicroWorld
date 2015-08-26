using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Sound
{
    static class Sounds
    {
        //sounds
        public static Sound menuClickSound;
        public static Sound fadeIn, fadeOut;
        public static Sound menuMouseOver;
        public static Sound componentPlacement;
        public static Sound tesla;
        //music
        public static Sound main1;//, main2;
        public static Sound game1, game2, game3;//game bg

        public static void LoadContent()
        {
            //fx
            Main.LoadingDetails = "Sound/MenuClick";
            IO.Log.Write("        Loading Sounds/MenuClick");
            menuClickSound = SoundManager.LoadSound("Sounds/MenuClick");
            Main.LoadingDetails = "Sound/FadeIn";
            IO.Log.Write("        Loading Sounds/FadeIn");
            fadeIn = SoundManager.LoadSound("Sounds/FadeIn");
            Main.LoadingDetails = "Sound/FadeOut";
            IO.Log.Write("        Loading Sounds/FadeOut");
            fadeOut = SoundManager.LoadSound("Sounds/FadeOut");
            Main.LoadingDetails = "Sound/MouseOver";
            IO.Log.Write("        Loading Sounds/MouseOver");
            menuMouseOver = SoundManager.LoadSound("Sounds/MouseOver");
            Main.LoadingDetails = "Sound/ComponentPlacement";
            IO.Log.Write("        Loading Sounds/ComponentPlacement");
            componentPlacement = SoundManager.LoadSound("Sounds/ComponentPlacement");
            Main.LoadingDetails = "Sound/Tesla";
            IO.Log.Write("        Loading Sounds/Tesla");
            tesla = SoundManager.LoadSound("Sounds/Tesla");
            //main mus
            Main.LoadingDetails = "Sounds/Music/(Main1)";
            IO.Log.Write("        Loading Sounds/Music/(Main1)");
            main1 = SoundManager.LoadSound("Sounds/Music/(Main1)");
            //Main.LoadingDetails = "Sounds/Music/(Main2)";
            //IO.Log.Write("        Loading Sounds/Music/(Main2)");
            //main2 = SoundManager.LoadSound("Sounds/Music/(Main2)");
            //game mus
            Main.LoadingDetails = "Sounds/Music/(Game1)";
            IO.Log.Write("        Loading Sounds/Music/(Game1)");
            game1 = SoundManager.LoadSound("Sounds/Music/(Game1)");
            Main.LoadingDetails = "Sounds/Music/(Game2)";
            IO.Log.Write("        Loading Sounds/Music/(Game2)");
            game2 = SoundManager.LoadSound("Sounds/Music/(Game2)");
            Main.LoadingDetails = "Sounds/Music/(Game3)";
            IO.Log.Write("        Loading Sounds/Music/(Game3)");
            game3 = SoundManager.LoadSound("Sounds/Music/(Game3)");

            //while (!main1.IsLoaded || !main2.IsLoaded) System.Threading.Thread.Sleep(1);
            while (!main1.IsLoaded) System.Threading.Thread.Sleep(1);
        }

    }
}
