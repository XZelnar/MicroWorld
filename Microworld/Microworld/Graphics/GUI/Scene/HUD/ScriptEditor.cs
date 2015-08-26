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
    public class ScriptEditor : HUDScene
    {
        public String Text
        {
            get { return tbCode.Text; }
            set
            {
                tbCode.Text = value.Replace("\b", "    ");
            }
        }

        Elements.TextBox tbCode;
        Elements.NewStyleButton button;
        public new void Initialize()
        {
            ShouldBeScaled = false;
            CanOffset = false;
            Layer = 700;

            button = new Elements.NewStyleButton(Main.WindowWidth - 28, (Main.WindowHeight - 100) / 2, 156, 34, "S\r\nr\r\ni\r\np\r\nt");
            button.LoadImages("GUI/HUD/ScriptButton");
            button.onClicked+=new Elements.Button.ClickedEventHandler(button_onClicked);
            controls.Add(button);

            tbCode = new Elements.TextBox(50, 50, Main.WindowWidth - 100, Main.WindowHeight - 100);
            tbCode.Font = ResourceManager.Load<SpriteFont>("Fonts/CourierNew_14");
            tbCode.ForegroundColor = new Color(0.1f, 0.6f, 0.1f);
            tbCode.CursorColor = new Color(0.1f, 0.6f, 0.1f);
            tbCode.BackgroundColor = new Color(0f, 0f, 0f);
            tbCode.SelectionColor = new Color(0.8f,0.8f,0.8f);
            tbCode.isVisible = false;
            controls.Add(tbCode);

            Reset();

            base.Initialize();
        }

        public void Reset()
        {
            tbCode.Text = new System.IO.StreamReader("Content/LUALevelTempalate.lua").ReadToEnd();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            button.position = new Vector2(Main.WindowWidth - button.size.X - 4, 42 + 55);
            tbCode.size = new Vector2(Main.WindowWidth - 100, Main.WindowHeight - 100);

            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        void button_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            if (!tbCode.isVisible)
            {
                tbCode.isVisible = true;
                tbCode.isFocused = true;
                InputEngine.blockClick = true;
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw(Renderer renderer)
        {
            button.IsActivated = tbCode.isVisible;

            RenderHelper.SmartDrawRectangle(GraphicsEngine.bg, 5, 
                (int)button.position.X - 4, (int)button.position.Y - 4, (int)button.Size.X + 8, (int)button.size.Y + 8,
                Color.White, renderer);

            base.Draw(renderer);
        }

        public override bool IsIn(int x, int y)
        {
            return GUIEngine.ContainsHUDScene(this) &&
                ((button.isVisible && button.IsIn(x, y)) || (tbCode.isVisible && tbCode.IsIn(x, y)));
        }

        #region IO
        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            base.onButtonDown(e);
            if (tbCode.isVisible)
                e.Handled = true;
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            base.onButtonUp(e);
            if (tbCode.isVisible)
                e.Handled = true;
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            base.onButtonClick(e);
            if (tbCode.isVisible)
            {
                if (!tbCode.IsIn(e.curState.X, e.curState.Y))
                {
                    tbCode.isVisible = false;
                }
                e.Handled = true;
            }
        }

        bool thisToolTip = false;
        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.onMouseMove(e);
            if (button.IsIn(e.curState.X, e.curState.Y))
            {
                thisToolTip = true;
                if (GUIEngine.s_toolTip.Text != "Script editor")
                    Shortcuts.ShowToolTip(new Vector2(button.position.X + button.size.X / 2, button.position.Y + button.size.Y), 
                        "Script editor", ArrowLineDirection.LeftDown);
            }
            else if (thisToolTip)
            {
                thisToolTip = false;
                Shortcuts.CloseToolTip();
            }
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            base.onMouseWheelMove(e);
            if (tbCode.isVisible)
            {
                e.Handled = true;
            }
        }

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
            base.onKeyDown(e);
            if (tbCode.isVisible)
                e.Handled = true;
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            base.onKeyPressed(e);
            if (tbCode.isVisible)
                e.Handled = true;
        }

        public override void onKeyUp(InputEngine.KeyboardArgs e)
        {
            base.onKeyUp(e);
            if (tbCode.isVisible)
                e.Handled = true;
        }
        #endregion

    }
}
