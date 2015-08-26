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
    public class HubProperties : GeneralProperties
    {
        public Label title;
        private CheckBox removable;
        private MenuButton leftUp, leftRight, leftDown, upRight, upDown, rightDown;
        private ToggleButton portLeft, portUp, portRight, portDown;

        static Texture2D t_leftUp, t_leftRight, t_leftDown, t_upRight, t_upDown, t_rightDown,
            arrowLeft, arrowUp, arrowRight, arrowDown, arrowLeftRight, arrowUpDown;

        public override void Initialize()
        {
            if (t_leftUp == null)
            {
                LoadTextures();
            }

            WasInitialized = true;

            size = new Vector2(155, 172);

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

            leftUp = new MenuButton(5, 80, 20, 20, "");
            leftUp.onClicked += new Button.ClickedEventHandler(leftUp_onClicked);
            leftUp.LeftTexture = t_leftUp;
            controls.Add(leftUp);

            leftRight = new MenuButton(30, 80, 20, 20, "");
            leftRight.onClicked += new Button.ClickedEventHandler(leftRight_onClicked);
            leftRight.LeftTexture = t_leftRight;
            controls.Add(leftRight);

            leftDown = new MenuButton(55, 80, 20, 20, "");
            leftDown.onClicked += new Button.ClickedEventHandler(leftDown_onClicked);
            leftDown.LeftTexture = t_leftDown;
            controls.Add(leftDown);

            upRight = new MenuButton(80, 80, 20, 20, "");
            upRight.onClicked += new Button.ClickedEventHandler(upRight_onClicked);
            upRight.LeftTexture = t_upRight;
            controls.Add(upRight);

            upDown = new MenuButton(105, 80, 20, 20, "");
            upDown.onClicked += new Button.ClickedEventHandler(upDown_onClicked);
            upDown.LeftTexture = t_upDown;
            controls.Add(upDown);

            rightDown = new MenuButton(130, 80, 20, 20, "");
            rightDown.onClicked += new Button.ClickedEventHandler(rightDown_onClicked);
            rightDown.LeftTexture = t_rightDown;
            controls.Add(rightDown);


            portLeft = new ToggleButton((int)size.X / 2 - 30, 125, 20, 20);
            portLeft.Add(arrowLeftRight, "IO");
            portLeft.Add(arrowLeft, "O");
            portLeft.Add(arrowRight, "I");
            portLeft.onSelectedChanged += new ToggleButton.SelectedChanged(portLeft_onSelectedChanged);
            controls.Add(portLeft);

            portRight = new ToggleButton((int)size.X / 2 + 10, 125, 20, 20);
            portRight.Add(arrowLeftRight, "IO");
            portRight.Add(arrowRight, "O");
            portRight.Add(arrowLeft, "I");
            portRight.onSelectedChanged += new ToggleButton.SelectedChanged(portRight_onSelectedChanged);
            controls.Add(portRight);

            portUp = new ToggleButton((int)size.X / 2 - 10, 105, 20, 20);
            portUp.Add(arrowUpDown, "IO");
            portUp.Add(arrowUp, "O");
            portUp.Add(arrowDown, "I");
            portUp.onSelectedChanged += new ToggleButton.SelectedChanged(portUp_onSelectedChanged);
            controls.Add(portUp);

            portDown = new ToggleButton((int)size.X / 2 - 10, 145, 20, 20);
            portDown.Add(arrowUpDown, "IO");
            portDown.Add(arrowDown, "O");
            portDown.Add(arrowUp, "I");
            portDown.onSelectedChanged += new ToggleButton.SelectedChanged(portDown_onSelectedChanged);
            controls.Add(portDown);

            base.Initialize();
        }

        void portDown_onSelectedChanged(object sender, string oldKey, string newKey)
        {
            var p = AssociatedComponent as Hub;
            if (newKey == "IO")
                p.Down = PortState.Both;
            else if (newKey == "I")
                p.Down = PortState.Input;
            else
                p.Down = PortState.Output;
            Load();
        }

        void portUp_onSelectedChanged(object sender, string oldKey, string newKey)
        {
            var p = AssociatedComponent as Hub;
            if (newKey == "IO")
                p.Up = PortState.Both;
            else if (newKey == "I")
                p.Up = PortState.Input;
            else
                p.Up = PortState.Output;
            Load();
        }

        void portRight_onSelectedChanged(object sender, string oldKey, string newKey)
        {
            var p = AssociatedComponent as Hub;
            if (newKey == "IO")
                p.Right = PortState.Both;
            else if (newKey == "I")
                p.Right = PortState.Input;
            else
                p.Right = PortState.Output;
            Load();
        }

        void portLeft_onSelectedChanged(object sender, string oldKey, string newKey)
        {
            var p = AssociatedComponent as Hub;
            if (newKey == "IO")
                p.Left = PortState.Both;
            else if (newKey == "I")
                p.Left = PortState.Input;
            else
                p.Left = PortState.Output;
            Load();
        }

        void rightDown_onClicked(object sender, InputEngine.MouseArgs e)
        {
            (AssociatedComponent as Hub).RightDown = !(AssociatedComponent as Hub).RightDown;
            Load();
            (sender as MenuButton).WasInitiallyDrawn = false;
        }

        void upDown_onClicked(object sender, InputEngine.MouseArgs e)
        {
            (AssociatedComponent as Hub).UpDown = !(AssociatedComponent as Hub).UpDown;
            Load();
            (sender as MenuButton).WasInitiallyDrawn = false;
        }

        void upRight_onClicked(object sender, InputEngine.MouseArgs e)
        {
            (AssociatedComponent as Hub).UpRight = !(AssociatedComponent as Hub).UpRight;
            Load();
            (sender as MenuButton).WasInitiallyDrawn = false;
        }

        void leftDown_onClicked(object sender, InputEngine.MouseArgs e)
        {
            (AssociatedComponent as Hub).LeftDown = !(AssociatedComponent as Hub).LeftDown;
            Load();
            (sender as MenuButton).WasInitiallyDrawn = false;
        }

        void leftRight_onClicked(object sender, InputEngine.MouseArgs e)
        {
            (AssociatedComponent as Hub).LeftRight = !(AssociatedComponent as Hub).LeftRight;
            Load();
            (sender as MenuButton).WasInitiallyDrawn = false;
        }

        void leftUp_onClicked(object sender, InputEngine.MouseArgs e)
        {
            (AssociatedComponent as Hub).LeftUp = !(AssociatedComponent as Hub).LeftUp;
            Load();
            (sender as MenuButton).WasInitiallyDrawn = false;
        }

        void LoadTextures()
        {
            t_leftUp = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/Hub/LeftUp");
            t_leftRight = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/Hub/LeftRight");
            t_leftDown = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/Hub/LeftDown");
            t_upRight = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/Hub/UpRight");
            t_upDown = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/Hub/UpDown");
            t_rightDown = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/Hub/RightDown");

            arrowLeft = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/ArrowLeft");
            arrowUp = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/ArrowUp");
            arrowRight = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/ArrowRight");
            arrowDown = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/ArrowDown");
            arrowLeftRight = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/ArrowLeftRight");
            arrowUpDown = ResourceManager.Load<Texture2D>("GUI/HUD/Properties/ArrowUpDown");
        }

        public override void Update()
        {
            if (leftUp.isEnabled != AssociatedComponent.IsRemovable)
            {
                leftUp.isEnabled = AssociatedComponent.IsRemovable;
                leftRight.isEnabled = leftUp.isEnabled;
                leftDown.isEnabled = leftUp.isEnabled;
                upRight.isEnabled = leftUp.isEnabled;
                upDown.isEnabled = leftUp.isEnabled;
                rightDown.isEnabled = leftUp.isEnabled;

                portLeft.isEnabled = leftUp.isEnabled;
                portUp.isEnabled = leftUp.isEnabled;
                portRight.isEnabled = leftUp.isEnabled;
                portDown.isEnabled = leftUp.isEnabled;
            }

            base.Update();
        }

        public override void Load()
        {
            var p = AssociatedComponent as Hub;

            removable.Checked = p.IsRemovable;

            leftUp.LeftTextureColor = p.LeftUp ? Color.White : Color.Gray;
            leftRight.LeftTextureColor = p.LeftRight ? Color.White : Color.Gray;
            leftDown.LeftTextureColor = p.LeftDown ? Color.White : Color.Gray;
            upRight.LeftTextureColor = p.UpRight ? Color.White : Color.Gray;
            upDown.LeftTextureColor = p.UpDown ? Color.White : Color.Gray;
            rightDown.LeftTextureColor = p.RightDown ? Color.White : Color.Gray;

            portLeft.CurIndex = PortStateToIndex(p.Left);
            portUp.CurIndex = PortStateToIndex(p.Up);
            portRight.CurIndex = PortStateToIndex(p.Right);
            portDown.CurIndex = PortStateToIndex(p.Down);
        }

        private int PortStateToIndex(PortState s)
        {
            switch (s)
            {
                case PortState.Input:
                    return 2;
                case PortState.Output:
                    return 1;
                case PortState.Both:
                    return 0;
                default:
                    return 0;
            }
        }

        public override void Save()
        {
        }
    }
}
