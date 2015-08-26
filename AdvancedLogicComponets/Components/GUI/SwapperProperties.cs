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
using MicroWorld.Graphics.GUI.Elements;
using MicroWorld.Graphics.GUI.Scene;
using MicroWorld.Graphics.GUI;
using MicroWorld.Graphics;

namespace MicroWorld.Components.GUI
{
    public class SwapperProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public Button invert;

        public override void Initialize()
        {
            WasInitialized = true;

            size = new Vector2(150, 105);

            title = new Label(0, 5, AssociatedComponent.Graphics.GetCSToolTip());
            title.font = TitleFont;
            title.UpdateSizeToTextSize();
            title.TextAlignment = Renderer.TextAlignment.Center;
            title.foreground = Color.White;
            controls.Add(title);

            Size = new Vector2(Math.Max(title.font.MeasureString(title.text).X + 80, Size.X), Size.Y);

            title.Size = new Vector2(Size.X - 20, title.Size.Y);

            removable = new CheckBox(5, 55, (int)size.X - 10, 20, "Removable: ", false);
            removable.foreground = Color.White;
            removable.onCheckedChanged += new CheckBox.CheckBoxCheckedHandler(removable_onCheckedChanged);
            controls.Add(removable);

            invert = new Button(5, 80, (int)size.X - 10, 20, "Invert");
            invert.onClicked += new Button.ClickedEventHandler(invert_onClicked);
            controls.Add(invert);

            base.Initialize();
        }

        void invert_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Swapper s = AssociatedComponent as Swapper;
            s.Swapped = !s.Swapped;
            if (Settings.GameState == Settings.GameStates.Stopped)
                s.OrigSwapped = !s.OrigSwapped;
        }
    }
}
