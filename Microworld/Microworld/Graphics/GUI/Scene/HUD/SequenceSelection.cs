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

namespace MicroWorld.Graphics.GUI.Scene
{
    class SequenceSelection : HUDScene
    {
        #region Events
        public delegate void SequenceSelectedEventHandler(object sender, IO.InputSequence sequence);
        public event SequenceSelectedEventHandler onSequenceSelected;
        #endregion

        IO.InputSequence seq = new IO.InputSequence();
        MenuButton bok, breset, bcancel;

        public override void Initialize()
        {
            Layer = 100100;

            bok = new MenuButton(Main.windowWidth / 2 - 200, Main.windowHeight * 2 / 3, 120, 30, "OK");
            bok.onClicked += new Button.ClickedEventHandler(bok_onClicked);
            controls.Add(bok);

            breset = new MenuButton(Main.windowWidth / 2 - 60, Main.windowHeight * 2 / 3, 120, 30, "Reset");
            breset.onClicked += new Button.ClickedEventHandler(breset_onClicked);
            controls.Add(breset);

            bcancel = new MenuButton(Main.windowWidth / 2 + 80, Main.windowHeight * 2 / 3, 120, 30, "Cancel");
            bcancel.onClicked += new Button.ClickedEventHandler(bcancel_onClicked);
            controls.Add(bcancel);

            base.Initialize();
        }

        void bcancel_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Close();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);

            bok.Position = new Vector2(Main.windowWidth / 2 - 130, Main.windowHeight * 2 / 3);
            breset.Position = new Vector2(Main.windowWidth / 2 + 10, Main.windowHeight * 2 / 3);
        }

        void breset_onClicked(object sender, InputEngine.MouseArgs e)
        {
            seq.Clear();
        }

        void bok_onClicked(object sender, InputEngine.MouseArgs e)
        {
            if (onSequenceSelected != null)
                onSequenceSelected.Invoke(this, seq);
            Close();
        }

        public static SequenceSelection ShowNew()
        {
            SequenceSelection k = new SequenceSelection();
            k.isVisible = true;
            k.Initialize();
            MicroWorld.Graphics.GUI.GUIEngine.AddHUDScene(k);
            return k;
        }

        public override void onShow()
        {
            base.onShow();
            seq = new IO.InputSequence();
        }

        public override void onClose()
        {
            onSequenceSelected = null;
            base.onClose();
        }

        public override void Draw(Renderer renderer)
        {
            renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight), Shortcuts.BG_COLOR * 0.5f);
            renderer.DrawString(GUIEngine.font, "Press the sequence (mouse and/or keyboard) you wish to select...",
                new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight), Color.White, Renderer.TextAlignment.Center);
            renderer.DrawString(GUIEngine.font, seq.ToString(GUIEngine.font, Main.windowWidth, true),
                new Rectangle(0, 50, Main.WindowWidth, Main.WindowHeight), Color.White, Renderer.TextAlignment.Center);
            base.Draw(renderer);
        }

        #region IO
        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            base.onButtonClick(e);
            e.Handled = true;
        }

        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            base.onButtonDown(e);
            e.Handled = true;
            if (!bok.IsIn(e.curState.X, e.curState.Y) && !breset.IsIn(e.curState.X, e.curState.Y) && 
                !bcancel.IsIn(e.curState.X, e.curState.Y))
                seq.AddRecord();
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            base.onButtonUp(e);
            e.Handled = true;
        }

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
            base.onKeyDown(e);
            e.Handled = true;
            seq.AddRecord();
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            base.onKeyPressed(e);
            e.Handled = true;
        }

        public override void onKeyUp(InputEngine.KeyboardArgs e)
        {
            base.onKeyUp(e);
            e.Handled = true;
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            base.onMouseWheelMove(e);
            e.Handled = true;
            seq.AddRecord();
        }
        #endregion
    }
}
