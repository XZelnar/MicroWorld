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
    public class SavesLoadsList : Control
    {
        internal List<SaveLoadElement> elements = new List<SaveLoadElement>();

        private Vector2 Offset = new Vector2();
        private ScrollBar scrollbar;

        public override Vector2 Position
        {
            get { return base.Position; }
            set
            {
                base.Position = value;
                for (int i = 0; i < elements.Count; i++)
                {
                    elements[i].Position = new Vector2((int)Position.X + 25, (int)Position.Y + 97 + elements.Count * 106 + 32);
                    elements[i].Size = new Vector2(Size.X - 25 - 23, 81);
                }
                scrollbar.Position = new Vector2((int)(position.X + size.X - 8), (int)(Position.Y + 100));
                scrollbar.Size = new Vector2(16, (int)(size.Y - 94));
            }
        }
        public override Vector2 Size
        {
            get { return base.Size; }
            set
            {
                base.Size = value;
                for (int i = 0; i < elements.Count; i++)
                {
                    elements[i].Position = new Vector2((int)Position.X + 25, (int)Position.Y + 97 + elements.Count * 106 + 32);
                    elements[i].Size = new Vector2(Size.X - 25 - 23, 81);
                }
                scrollbar.Position = new Vector2((int)(position.X + size.X - 8), (int)(Position.Y + 100));
                scrollbar.Size = new Vector2(16, (int)(size.Y - 94));
            }
        }
        public int SelectedIndex
        {
            get
            {
                for (int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].StaySelected)
                        return i;
                }
                return -1;
            }
            set
            {
                if (value < -1 || value >= elements.Count)
                    return;
                for (int i = 0; i < elements.Count; i++)
                {
                    elements[i].StaySelected = false;
                }
                if (value != -1)
                    elements[value].StaySelected = true;
            }
        }

        public delegate void SaveLoadEvent(object sender, int index, bool saveload, bool delete);
        public event SaveLoadEvent onElementSelected;
        public delegate void SelectionEvent(object sender, int selectedIndex);
        public event SelectionEvent onSelectedIndexChanged;

        public SavesLoadsList(int x, int y, int w, int h)
        {
            position = new Vector2(x, y);
            size = new Vector2(w, h);
        }

        public override void Initialize()
        {
            scrollbar = new ScrollBar((int)(position.X + size.X - 8), (int)(Position.Y + 100), 16, (int)(size.Y - 94));
            scrollbar.IsVertical = true;
            scrollbar.MinValue = 0;
            scrollbar.MaxValue = 1;
            scrollbar.Value = 0;
            scrollbar.onValueChanged += new ScrollBar.ValueChangedEventHandler(scrollbar_onValueChanged);
            scrollbar.Initialize();

            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].Initialize();
            }
        }

        void scrollbar_onValueChanged(object sender, ScrollBar.ValueChangedArgs e)
        {
            Offset.Y = -scrollbar.Value;
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].Offset = Offset;
            }
        }

        #region API
        public String GetSelected()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].StaySelected)
                    return elements[i].SaveName;
            }
            return "";
        }

        public void Clear()
        {
            elements.Clear();
            Offset = new Vector2();
            scrollbar.Value = 0;
            scrollbar.MaxValue = 1;
        }

        public void ItemsAdd(String name, DateTime date)
        {
            var a = new SaveLoadElement((int)Position.X + 25, (int)Position.Y + 97 + elements.Count * 106 + 25);
            a.index = elements.Count + 1;
            a.Size = new Vector2(Size.X - 25 - 23, 81);
            a.SaveName = name;
            a.SaveDateTime = date;
            elements.Add(a);
            a.Initialize();
            SortElementsByDate();

            scrollbar.MaxValue = (int)(elements.Count * 106 + 25 - size.Y + 97);
        }

        public void SortElementsByDate()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                for (int j = i + 1; j < elements.Count; j++)
                {
                    if (elements[j].SaveDateTime.Ticks > elements[i].SaveDateTime.Ticks)
                    {
                        SwapElements(i, j);
                    }
                }
            }
        }

        public void SwapElements(int i, int j)
        {
            var a = elements[i];
            elements[i] = elements[j];
            elements[j] = a;

            var b = elements[i].Position;
            elements[i].Position = elements[j].Position;
            elements[j].Position = b;

            elements[i].index = i;
            elements[j].index = j;

        }

        public int ItemsCount()
        {
            return elements.Count;
        }

        public String ItemsGetAt(int i)
        {
            if (i < 0 || i >= elements.Count)
                return "";
            return elements[i].SaveName;
        }
        #endregion

        public override void Update()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].Update();
            }
            scrollbar.Update();
        }

        public override void Draw(Renderer renderer)
        {
            renderer.End();
            renderer.BeginUnscaled(SpriteSortMode.Immediate, null, SamplerState.PointWrap, null, null, null,
                Matrix.CreateTranslation(Scene.MainMenu.FrameOffset, 0, 0));
            scrollbar.Draw(renderer);
            renderer.End();
            renderer.SetScissorRectangle(position.X, position.Y + 96, size.X, size.Y - 96, false);
            renderer.BeginUnscaled(SpriteSortMode.Immediate, null, null, null, Graphics.GraphicsEngine.s_ScissorsOn, null,
                Matrix.CreateTranslation(Scene.MainMenu.FrameOffset, 0, 0));
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].Draw(renderer);
            }
            renderer.End();
            renderer.ResetScissorRectangle();
            renderer.SetScissorRectangle(GUIEngine.s_mainMenu.line3p1.X, GUIEngine.s_mainMenu.line3p1.Y, Main.WindowWidth, Main.WindowHeight, false);
            renderer.BeginUnscaled(SpriteSortMode.Immediate, null, null, null, Graphics.GraphicsEngine.s_ScissorsOn, null,
                Matrix.CreateTranslation(Scene.MainMenu.FrameOffset, 0, 0));
        }

        #region IO
        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            scrollbar.onButtonDown(e);
            if (!IsIn(e.curState.X, e.curState.Y))
                return;
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].onButtonDown(e);
            }
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            scrollbar.onButtonUp(e);
            if (!IsIn(e.curState.X, e.curState.Y))
                return;
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].onButtonUp(e);
            }
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            scrollbar.onButtonClick(e);
            if (!IsIn(e.curState.X, e.curState.Y))
                return;
            bool b;
            int t;
            for (int i = 0; i < elements.Count; i++)
            {
                b = elements[i].StaySelected;
                t = elements[i].lastClick;
                elements[i].onButtonClick(e);
                if (((b && Main.Ticks - t < 30) || e.button == 1) && elements[i].StaySelected)
                {
                    if (onElementSelected != null)
                    {
                        onElementSelected.Invoke(this, i, e.button == 0, e.button == 1);
                    }
                }
                if (!b && elements[i].StaySelected)
                {
                    if (onSelectedIndexChanged != null)
                        onSelectedIndexChanged.Invoke(this, i);
                }
            }
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            scrollbar.onMouseMove(e);
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].onMouseMove(e);
            }
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            Offset.Y += e.delta / 2;
            if (Offset.Y > 0)
            {
                Offset.Y = 0;
            }
            if (Offset.Y < -(elements.Count * 106 + 25 - size.Y + 97))
            {
                Offset.Y = -(elements.Count * 106 + 25 - size.Y + 97);
            }
            scrollbar.Value = (int)-Offset.Y;
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].Offset = Offset;
                elements[i].onMouseWheelMove(e);
            }
        }

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].onKeyDown(e);
            }
        }

        public override void onKeyUp(InputEngine.KeyboardArgs e)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].onKeyUp(e);
            }
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].onKeyPressed(e);
            }
        }
        #endregion
    }
}
