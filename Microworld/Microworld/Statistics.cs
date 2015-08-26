using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MicroWorld
{
    public static class Statistics
    {
        public static double WireLengthPlaced = 0;
        public static int ElementsPlaced = 0;
        public static int TimesSimulationStarted = 0;
        public static int ComponentsRemoved = 0;
        public static double WiresLengthBurned = 0;
        public static int ButtonsClicked = 0;
        public static int TextCharsEntered = 0;
        public static int GameStarts = 0;

        public static void Reset()
        {
            WireLengthPlaced = 0;
            ElementsPlaced = 0;
            TimesSimulationStarted = 0;
            ComponentsRemoved = 0;
            WiresLengthBurned = 0;
            ButtonsClicked = 0;
            TextCharsEntered = 0;
            GameStarts = 0;
        }

        public static void Save()
        {
            BinaryWriter bw = new BinaryWriter(new FileStream("stat.dat", FileMode.Create), Encoding.Unicode);
            bw.Write(WireLengthPlaced);
            bw.Write(ElementsPlaced);
            bw.Write(TimesSimulationStarted);
            bw.Write(ComponentsRemoved);
            bw.Write(WiresLengthBurned);
            bw.Write(ButtonsClicked);
            bw.Write(TextCharsEntered);
            bw.Write(GameStarts);
            bw.Close();
        }

        public static void Load()
        {
            if (File.Exists("stat.dat"))
            {
                try
                {
                    BinaryReader br = new BinaryReader(new FileStream("stat.dat", FileMode.Open), Encoding.Unicode);
                    WireLengthPlaced = br.ReadDouble();
                    ElementsPlaced = br.ReadInt32();
                    TimesSimulationStarted = br.ReadInt32();
                    ComponentsRemoved = br.ReadInt32();
                    WiresLengthBurned = br.ReadDouble();
                    ButtonsClicked = br.ReadInt32();
                    TextCharsEntered = br.ReadInt32();
                    GameStarts = br.ReadInt32();
                    br.Close();
                }
                catch { }
            }
        }
    }
}
