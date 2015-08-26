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

namespace MicroWorld.Graphics.GUI.Elements
{
    public class ComboBox : Control
    {
        Texture2D downArrow, bg;
        //selectedindex
        ContextMenu contextMenu = new ContextMenu();
        Button curElement, arrow;

        public String text
        {
            get { return curElement.Text; }
            set { curElement.Text = value; }
        }
        private int width = 120;
        public int Width
        {
            get { return width;}
            set
            {
                width = value;
                if (width < 30) width = 30;
                curElement.size.X = width - 20;
                contextMenu.WantedWidth = width - 20;
            }
        }
        private int height = 120;
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
                if (height < 20) height = 20;
                curElement.size.Y = height;
                arrow.size.Y = height;
            }
        }

        public delegate void ElementSelectedHandler(object sender, String text);
        public event ElementSelectedHandler onElementSelected;

        public ComboBox()
        {
        }

        public ComboBox(int x, int y, int w)
        {
            position = new Vector2(x, y);
            width = w;
            height = 20;
        }

        public ComboBox(int x, int y, int w, int h)
        {
            position = new Vector2(x, y);
            width = w;
            height = h;
        }

        public override void Initialize()
        {
            base.Initialize();
            size = new Vector2(120, 20);

            curElement = new Button(0, 0, width - 20, 20, "");
            arrow = new Button(width - 20, 0, 20, 20, "v");
            contextMenu.WantedWidth = 100;

            curElement.onClicked += new Button.ClickedEventHandler(curElement_onClicked);
            arrow.onClicked += new Button.ClickedEventHandler(curElement_onClicked);

            curElement.foreground = Color.White;

            contextMenu.onElementSelected += new ContextMenu.ElementSelectedHandler(contextMenu_onElementSelected);

            curElement.Initialize();
            arrow.Initialize();
            contextMenu.Initialize();

            downArrow = ResourceManager.Load<Texture2D>("GUI/Elements/ComboBoxButton");
            bg = ResourceManager.Load<Texture2D>("GUI/Elements/ComboBoxBg");
        }

        void contextMenu_onElementSelected(ContextMenu.ElementSelectedArgs e)
        {
            contextMenu.isVisible = false;
            curElement.Text = contextMenu.Elements[e.index].Text;

            if (onElementSelected != null)
            {
                onElementSelected.Invoke(this, curElement.Text);
            }
        }

        void curElement_onClicked(object sender, InputEngine.MouseArgs e)
        {
            contextMenu.isVisible = !contextMenu.isVisible;
        }

        public void ItemsAdd(String s)
        {
            contextMenu.Add(s);
        }

        public void ItemsRemove(String s)
        {
            contextMenu.Remove(s);
        }

        public void ItemsClear()
        {
            contextMenu.Clear();
        }

        public override void Update()
        {
            base.Update();

            curElement.position = position;
            arrow.position = position + new Vector2(width - 20, 0);
            contextMenu.SetPosition(position + new Vector2(0, 20));

            curElement.Update();
            arrow.Update();
            contextMenu.Update();
        }

        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            base.onButtonDown(e);
            if (IsIn(e.curState.X, e.curState.Y))
            {
                if (!e.Handled) curElement.onButtonDown(e);
                if (!e.Handled) arrow.onButtonDown(e);
                if (!e.Handled) contextMenu.onButtonDown(e);
            }
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            base.onButtonUp(e);
            if (IsIn(e.curState.X, e.curState.Y))
            {
                if (!e.Handled) curElement.onButtonUp(e);
                if (!e.Handled) arrow.onButtonUp(e);
                if (!e.Handled) contextMenu.onButtonUp(e);
            }
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            base.onButtonClick(e);
            if (IsIn(e.curState.X, e.curState.Y))
            {
                if (!e.Handled) curElement.onButtonClick(e);
                if (!e.Handled) arrow.onButtonClick(e);
                if (!e.Handled) contextMenu.onButtonClick(e);
            }
            else
            {
                if (!e.Handled && contextMenu.isVisible) contextMenu.isVisible = false;
            }
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.onMouseMove(e);
            if (IsIn(e.curState.X, e.curState.Y))
            {
                if (!e.Handled) curElement.onMouseMove(e);
                if (!e.Handled) arrow.onMouseMove(e);
                if (!e.Handled) contextMenu.onMouseMove(e);
            }
        }

        public override void Draw(Renderer renderer)
        {
            base.Draw(renderer);

            //curElement.Draw(renderer);
            //arrow.Draw(renderer);
            RenderHelper.SmartDrawRectangle(bg, 5, (int)curElement.Position.X, (int)curElement.Position.Y,
                (int)curElement.Size.X, (int)curElement.Size.Y, Color.White, renderer);
            Main.renderer.DrawString(curElement.Font, text, new Rectangle((int)curElement.position.X,
                (int)curElement.position.Y, (int)curElement.size.X, (int)curElement.size.Y), curElement.foreground, 
                curElement.textAlignment);

            RenderHelper.SmartDrawRectangle(downArrow, 5, (int)arrow.Position.X, (int)arrow.Position.Y,
                (int)arrow.Size.X, (int)arrow.Size.Y, Color.White, renderer);
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (contextMenu.isVisible) contextMenu.Draw(Main.renderer);
        }

        public override bool IsIn(int x, int y)
        {
            return base.IsIn(x, y) || (contextMenu.isVisible && contextMenu.IsIn(x, y));
        }
    }
}
