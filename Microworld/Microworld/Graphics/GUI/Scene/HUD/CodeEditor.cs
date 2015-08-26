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
    public class CodeEditor : HUDScene
    {
        public int ComponentIndex = 0;
        private Vector2 size;

        Elements.TextBox tbCode;
        public new void Initialize()
        {
            //ShouldBeScaled = true;
            CanOffset = true;
            Layer = 50;

            tbCode = new Elements.TextBox(0, 0, 500, 250);
            //tbCode.Font = ResourceManager.Load<SpriteFont>("Fonts/CodeEditor");
            size = tbCode.size;
            tbCode.ForegroundColor = new Color(0.1f, 0.6f, 0.1f);
            tbCode.CursorColor = new Color(0.1f, 0.6f, 0.1f);
            tbCode.BackgroundColor = new Color(0f, 0f, 0f) * 0.85f;
            tbCode.SelectionColor = new Color(0.8f,0.8f,0.8f);
            //tbCode.ShouldBeScaled = true;
            tbCode.ShouldOffset = true;
            controls.Add(tbCode);
        }

        public override void Update()
        {
            base.Update();
        }

        public void InitForElement(int index)
        {
            ComponentIndex = index;
            tbCode.Text = ((Components.Logics.Microcontrollers.MicrocontrollersLogics)
                Components.ComponentsManager.Components[ComponentIndex].Logics).Code;
            tbCode.position = Components.ComponentsManager.Components[ComponentIndex].Graphics.Position + 
                Components.ComponentsManager.Components[ComponentIndex].Graphics.Size + 
                new Vector2(5, -Components.ComponentsManager.Components[ComponentIndex].Graphics.Size.Y);
            tbCode.position *= Settings.GameScale;
        }

        public void Move(int dx, int dy)
        {
            tbCode.position += new Vector2(dx, dy);
        }

        System.Threading.Thread fadethread;
        public override void onShow()
        {
            if (fadethread != null && fadethread.ThreadState != System.Threading.ThreadState.Stopped)
            {
                try
                {
                    fadethread.Abort();
                }
                catch { }
            }
            fadethread = new System.Threading.Thread(new System.Threading.ThreadStart(_fadein));
            fadethread.Start();

            base.onShow();
        }

        private void _fadein()
        {
            var a = size;
            tbCode.size = new Vector2();
            var d = a / 100f;
            for (int i = 0; i < 100; i++)
            {
                tbCode.size += d;
                System.Threading.Thread.Sleep(2);
            }
            tbCode.size = a;
        }

        public override void onClose()
        {
            ((Components.Logics.Microcontrollers.MicrocontrollersLogics)
                Components.ComponentsManager.Components[ComponentIndex].Logics).Code = tbCode.Text;
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            if (!IsIn(e.curState.X, e.curState.Y) && !GUIEngine.s_subComponentButtons.IsIn(e.curState.X, e.curState.Y))
            {
                GUIEngine.RemoveHUDScene(this);
                GUIEngine.RemoveHUDScene(GUIEngine.s_subComponentButtons);
                GUIEngine.s_subComponentButtons.isVisible = false;
                e.Handled = true;
            }
            base.onButtonClick(e);
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            if (IsIn(e.curState.X,e.curState.Y))
            {
                e.Handled = true;
            }
            base.onMouseWheelMove(e);
        }

        public override bool IsIn(int x, int y)
        {
            return GUIEngine.ContainsHUDScene(this) && isVisible && 
                tbCode.IsIn(x - (int)Settings.GameOffset.X, y - (int)Settings.GameOffset.Y);
        }
    }
}
