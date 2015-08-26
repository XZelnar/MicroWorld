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
    public class ContextMenu : Control
    {
        private static ContextMenu curVisMenu = null;

        private List<Button> elements = new List<Button>();
        public int SelectedIndex = -1;

        public bool ShouldBeScaled = true;
        public bool ShouldOffset = true;

        public class ElementSelectedArgs
        {
            public int index;
        }
        public delegate void ElementSelectedHandler(ElementSelectedArgs e);
        public event ElementSelectedHandler onElementSelected;

        public int WantedWidth = -1;

        public new bool isVisible
        {
            get { return base.isVisible; }
            set
            {
                base.isVisible = value;
                if (value)
                {
                    if (curVisMenu != null && curVisMenu != this)
                    {
                        curVisMenu.Close();
                    }
                    curVisMenu = this;
                }
                else
                {
                    if (curVisMenu == this)
                    {
                        curVisMenu = null;
                    }
                }
            }
        }

        public List<Button> Elements
        {
            get { return elements; }
            set
            {
                elements = value;
                updateSize();
                updatePosition();
            }
        }

        public ContextMenu()
        {
            isVisible = false;
        }

        public void Add(String text)
        {
            if (text != null)
            {
                EncyclopediaBrowserButton b = new EncyclopediaBrowserButton(0, 0, 0, 0, text);
                (b as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Elements/ComboBoxBg");
                b.IdleOpacity = 0.8f;
                b.foreground = Color.White;
                b.Initialize();
                b.onClicked += new Button.ClickedEventHandler(ElementClicked);
                elements.Add(b);
            }
            else
            {
                EncyclopediaBrowserButton b = new EncyclopediaBrowserButton(0, 0, 0, 0, "---");
                (b as Elements.EncyclopediaBrowserButton).OverrideTexture("GUI/Elements/ComboBoxBg");
                b.IdleOpacity = 0.8f;
                b.foreground = Color.White;
                b.Initialize();
                b.isEnabled = false;
                b.disabledColor = b.background;
                elements.Add(b);
            }
            updateSize();
            updatePosition();
        }

        public void Remove(String text)
        {
            bool wasremoved = false;
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Text == text)
                {
                    elements.RemoveAt(i);
                    i--;
                    wasremoved = true;
                }
            }
            if (wasremoved)
            {
                updateSize();
                updatePosition();
            }
        }

        public void Clear()
        {
            if (elements.Count != 0)
            {
                elements.Clear();
                updateSize();
                updatePosition();
            }
        }

        public void Show()
        {
            isVisible = true;
        }

        public void Close()
        {
            isVisible = false;
        }

        private void updateSize()
        {
            if (elements == null || elements.Count == 0)
            {
                size = new Vector2(1, 1);
                return;
            }
            Vector2 v = elements[0].Font.MeasureString(elements[0].Text);
            elements[0].size.Y = v.Y;
            float maxX = v.X;
            int maxL = elements[0].Text.Length;
            float t;
            for (int i = 1; i < elements.Count; i++)
            {
                v = elements[i].Font.MeasureString(elements[i].Text);
                elements[i].size.Y = v.Y;
                t = v.X;
                if (t > maxX)
                {
                    maxX = t;
                    maxL = elements[i].Text.Length;
                }
            }
            maxX += 2;
            float y = 0;
            String separator = "";
            for (int i = 0; i < maxL; i++)
            {
                separator += "-";
            }
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].size.X = maxX;
                if (elements[i].Text.StartsWith("--"))
                {
                    elements[i].Text = separator;
                }
                y += elements[i].size.Y;
            }
            size = new Vector2(maxX, y);
        }

        private void updatePosition()
        {
            float y = position.Y;
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].position = new Vector2(position.X, y);
                y += elements[i].size.Y;
            }
        }

        public void SetPosition(Vector2 v)
        {
            position = v;
            updatePosition();
        }

        public void ElementClicked(object sender, InputEngine.MouseArgs e)
        {
            isVisible = false;
            if (onElementSelected != null)
            {
                for(int i = 0; i < elements.Count; i++)
                {
                    if (elements[i].position.Y == ((Button)sender).position.Y)
                    {
                        onElementSelected(new ElementSelectedArgs() { index = i });
                        break;
                    }
                }
            }
        }

        public override void Initialize()
        {
            //for (int i = 0; i < elements.Count; i++)
            //{
            //    elements[i].Initialize();
            //}
        }

        public override void Update()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].Update();
                if (WantedWidth != -1)
                {
                    elements[i].size.X = WantedWidth;
                }
            }
            if (WantedWidth != -1) size.X = WantedWidth;
        }

        public override void Draw(Renderer renderer)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].Draw(renderer);
            }
        }

        public void onButtonDown(Vector2 v)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].onButtonDown(v);
            }
        }

        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            onButtonDown(new Vector2(e.curState.X, e.curState.Y));
        }

        public void onButtonUp(Vector2 v)
        {
            if (!IsIn((int)v.X, (int)v.Y))
            {
                isVisible = false;
                return;
            }
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].onButtonUp(v);
            }
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            onButtonUp(new Vector2(e.curState.X, e.curState.Y));
        }

        public void onButtonClick(Vector2 v)
        {
            if (IsIn((int)v.X, (int)v.Y)) InputEngine.eventHandled = true;
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            onButtonClick(new Vector2(e.curState.X, e.curState.Y));
        }

        public void onMouseMove(Vector2 v)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].onMouseMove(v);
            }
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            onMouseMove(new Vector2(e.curState.X, e.curState.Y));
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
        }

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
        }

        public override void onKeyUp(InputEngine.KeyboardArgs e)
        {
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
        }
    }
}
