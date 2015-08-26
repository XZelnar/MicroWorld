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
    class Loading : Scene
    {
        public String LoadingMainText = "";
        public String LoadingDescriptiveText = "";

        public new void Initialize()
        {
            ShouldBeScaled = false;

            base.Initialize();

            background = GUIEngine.s_mainMenu.background;
        }

        public override void onShow()
        {
            base.onShow();
            LoadingDescriptiveText = "";
            LoadingMainText = "";
        }

        public override void Draw(Renderer renderer)
        {
            base.Draw(renderer);
            Main.renderer.DrawStringCentered(Main.LoadingFont, LoadingMainText,
                new Rectangle(0, Main.WindowHeight - 50, Main.WindowWidth, 30), Color.White);
            Main.renderer.DrawStringCentered(Main.LoadingSmallFont, LoadingDescriptiveText,
                new Rectangle(0, Main.WindowHeight - 20, Main.WindowWidth, 20), Color.White);
        }
    }
}
