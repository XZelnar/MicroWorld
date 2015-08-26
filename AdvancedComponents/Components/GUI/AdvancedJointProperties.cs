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
    public class AdvancedJointProperties : GeneralProperties
    {
        public Label title;
        public CheckBox removable;
        public MenuButton left, up, right, down, center;

        static Texture2D arrowLeft, arrowUp, arrowRight, arrowDown;

        public override void Initialize()
        {
            if (arrowDown == null)
            {
                LoadTextures();
            }

            WasInitialized = true;

            size = new Vector2(192, 145);

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

            Label l = new Label(5, 80, "Ports:");
            l.foreground = Color.White;
            controls.Add(l);

            center = new MenuButton((int)(size.X - 20) / 2, 100, 20, 20, "");
            center.LeftTexture = ResourceManager.Load<Texture2D>("Components/Icons/AdvancedJoint");
            center.isEnabled = false;
            center.StretchLeftTextureToSize = true;
            center.disabledColor = Color.White;
            controls.Add(center);

            left = new MenuButton((int)center.Position.X - 20, 100, 20, 20, "");
            left.onClicked += new Button.ClickedEventHandler(left_onClicked);
            controls.Add(left);

            up = new MenuButton((int)center.Position.X, 80, 20, 20, "");
            up.onClicked += new Button.ClickedEventHandler(up_onClicked);
            controls.Add(up);

            right = new MenuButton((int)center.Position.X + 20, 100, 20, 20, "");
            right.onClicked += new Button.ClickedEventHandler(right_onClicked);
            controls.Add(right);

            down = new MenuButton((int)center.Position.X, 120, 20, 20, "");
            down.onClicked += new Button.ClickedEventHandler(down_onClicked);
            controls.Add(down);

            base.Initialize();
        }

        void down_onClicked(object sender, InputEngine.MouseArgs e)
        {
            (AssociatedComponent as AdvancedJoint).Down = Inverse((AssociatedComponent as AdvancedJoint).Down);
            Load();
            (sender as MenuButton).WasInitiallyDrawn = false;
        }

        void right_onClicked(object sender, InputEngine.MouseArgs e)
        {
            (AssociatedComponent as AdvancedJoint).Right = Inverse((AssociatedComponent as AdvancedJoint).Right);
            Load();
            (sender as MenuButton).WasInitiallyDrawn = false;
        }

        void up_onClicked(object sender, InputEngine.MouseArgs e)
        {
            (AssociatedComponent as AdvancedJoint).Up = Inverse((AssociatedComponent as AdvancedJoint).Up);
            Load();
            (sender as MenuButton).WasInitiallyDrawn = false;
        }

        void left_onClicked(object sender, InputEngine.MouseArgs e)
        {
            (AssociatedComponent as AdvancedJoint).Left = Inverse((AssociatedComponent as AdvancedJoint).Left);
            Load();
            (sender as MenuButton).WasInitiallyDrawn = false;
        }

        void LoadTextures()
        {
            arrowLeft = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/ArrowLeft");
            arrowUp = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/ArrowUp");
            arrowRight = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/ArrowRight");
            arrowDown = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/ArrowDown");
        }

        PortState Inverse(PortState p)
        {
            if (p == PortState.Input)
                return PortState.Output;
            return PortState.Input;
        }

        public override void Update()
        {
            if (left.isEnabled != AssociatedComponent.IsRemovable)
            {
                left.isEnabled = AssociatedComponent.IsRemovable;
                up.isEnabled = left.isEnabled;
                right.isEnabled = left.isEnabled;
                down.isEnabled = left.isEnabled;
            }

            base.Update();
        }

        public override void Load()
        {
            var p = AssociatedComponent as AdvancedJoint;

            left.LeftTexture = p.Left == PortState.Input ? arrowRight: arrowLeft;
            up.LeftTexture = p.Up == PortState.Input ? arrowDown: arrowUp;
            right.LeftTexture = p.Right == PortState.Input ? arrowLeft : arrowRight;
            down.LeftTexture = p.Down == PortState.Input ? arrowUp : arrowDown;

            removable.Checked = AssociatedComponent.IsRemovable;
        }

        public override void Save()
        {
        }
    }
}
