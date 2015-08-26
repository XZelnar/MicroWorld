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
    public class ListBox : Control
    {
        public static SpriteFont font;

        public Color foreground = Color.White, background = Color.Black, selectedColor = Color.Blue;
        private List<String> Items = new List<string>();
        public int ItemHeight = 15;
        public ScrollBar scrollbar;

        private int scrolled = 0;
        public int Scrolled
        {
            get { return scrolled; }
            set
            {
                scrolled = value;
                CheckScrolledBounds();
                IgnoreSBChange = true;
                scrollbar.Value = scrolled;
                IgnoreSBChange = false;
            }
        }
        private int selectedIndex = -1;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                int old = selectedIndex;
                selectedIndex = value;
                if (selectedIndex < -1) selectedIndex = -1;
                if (selectedIndex >= Items.Count) selectedIndex = -1;
                if (selectedIndex != old)
                {
                    if (onSelectedIndexChanged != null)
                    {
                        onSelectedIndexChanged(this, new SelectedIndexArgs() { selectedIndex = selectedIndex });
                    }
                }
            }
        }
        public bool IgnoreSBChange = false;

        #region Events
        public class SelectedIndexArgs
        {
            public int selectedIndex;
        }
        public delegate void SelectedIndexChangedEventHandler(object sender, SelectedIndexArgs e);
        public event SelectedIndexChangedEventHandler onSelectedIndexChanged;
        public delegate void DoubleClickedEventHandler(object sender, SelectedIndexArgs e);
        public event DoubleClickedEventHandler onDoubleClicked;
        #endregion

        #region ItemImplementation
        public void ItemsAdd(String item)
        {
            Items.Add(item);
            UpdateScrollBar();
        }

        public int ItemsCount()
        {
            return Items.Count;
        }

        public void ItemsRemove(String item)
        {
            Items.Remove(item);
            UpdateScrollBar();
        }

        public void ItemsRemoveAt(int ind)
        {
            Items.RemoveAt(ind);
            UpdateScrollBar();
        }

        public void Clear()
        {
            Items.Clear();
            UpdateScrollBar();
        }

        public String ItemsGetAt(int ind)
        {
            return Items[ind];
        }
        #endregion

        public ListBox(int x, int y, int w, int h)
        {
            position = new Vector2(x, y);
            size = new Vector2(w, h);

            scrollbar = new ScrollBar((int)position.X + (int)size.X - 16, (int)position.Y, 16, (int)size.Y);
            scrollbar.MaxValue = 0;
            scrollbar.MinValue = 0;
            scrollbar.Value = 0;
            scrollbar.onValueChanged += new ScrollBar.ValueChangedEventHandler(scrollbar_onValueChanged);
            scrollbar.IsVertical = true;
            scrollbar.isVisible = true;
        }

        public void Reset()
        {
            scrolled = 0;
            SelectedIndex = -1;
            IgnoreSBChange = false;
        }

        public String GetSelected()
        {
            if (SelectedIndex == -1) return "";
            return Items[SelectedIndex];
        }

        void scrollbar_onValueChanged(object sender, ScrollBar.ValueChangedArgs e)
        {
            if (!IgnoreSBChange)
            {
                Scrolled = e.value;
            }
        }

        public void UpdateScrollBar()
        {
            scrollbar.MaxValue = (int)(Items.Count - size.Y / ItemHeight);
            if (scrollbar.MaxValue < 0) scrollbar.MaxValue = 0;
        }

        public override void Initialize()
        {
            scrollbar.Initialize();
        }

        public override void Update()
        {
            scrollbar.Update();
            ticksSinceClicked++;
            if (ticksSinceClicked > 20)
            {
                ticksSinceClicked = 0;
                clickedPos = new Vector2();
                clickedButton = -1;
            }
        }

        public override void Draw(Renderer renderer)
        {
            /*
            #region ScissorInit
            bool scaled = Main.renderer.IsScaeld;
            Main.renderer.End();
            Rectangle curST = Main.graphics.GraphicsDevice.ScissorRectangle;
            Vector2 p = new Vector2((int)position.X, (int)position.Y);
            Vector2 s = new Vector2((int)size.X, (int)size.Y);
            if (s.X + p.X > Main.graphics.PreferredBackBufferWidth)
            {
                s.X = Main.graphics.PreferredBackBufferWidth - p.X;
            }
            if (s.Y + p.Y > Main.graphics.PreferredBackBufferHeight)
            {
                s.Y = Main.graphics.PreferredBackBufferHeight - p.Y;
            }
            Main.graphics.GraphicsDevice.ScissorRectangle = new Rectangle((int)p.X, (int)p.Y, (int)s.X, (int)s.Y);
            Main.renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, null,
                null, Graphics.GraphicsEngine.s_ScissorsOn);
            #endregion
            //*/
            Main.renderer.Draw(Graphics.GraphicsEngine.pixel, position.X, position.Y, size.X, size.Y, background);
            if (SelectedIndex != -1)
            {
                Main.renderer.Draw(Graphics.GraphicsEngine.pixel,
                    position.X, position.Y + (SelectedIndex - Scrolled) * ItemHeight, size.X, ItemHeight, 
                    selectedColor);
            }
            for (int i = Scrolled; i < Items.Count && i < size.Y / ItemHeight + Scrolled; i++)
            {
                Main.renderer.DrawStringLeft(font, Items[i], 
                    new Vector2(position.X, position.Y + (i - Scrolled) * ItemHeight - 3), 
                    foreground);
            }
            scrollbar.Draw(renderer);
            /*
            #region ScissorEnd
            Main.renderer.End();
            Main.graphics.GraphicsDevice.ScissorRectangle = curST;
            Main.renderer.Begin(scaled);
            #endregion
            //*/
        }

        public void SelectFor(int x, int y)
        {
            int i = (int)((y - position.Y) / ItemHeight + Scrolled);
            SelectedIndex = i;
            CheckSelectedBounds();
        }

        public void CheckSelectedBounds()
        {
            if (SelectedIndex < -1) SelectedIndex = -1;
            if (SelectedIndex >= Items.Count) SelectedIndex = Items.Count - 1;
            if (SelectedIndex >= Scrolled + size.X / ItemHeight)
            {
                Scrolled++;
            }
            else
            {
                if (SelectedIndex < Scrolled) Scrolled--;
            }
        }

        public void CheckScrolledBounds()
        {
            if (scrolled > Items.Count - size.Y / ItemHeight) scrolled = (int)(Items.Count - size.Y / ItemHeight);
            if (scrolled < 0) scrolled = 0;
        }

        private int isSelecting = -1;//indicates button
        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y))
            {
                isFocused = true;
                if (scrollbar.IsIn(e.curState.X,e.curState.Y))
                {
                    scrollbar.onButtonDown(e);
                    return;
                }
                e.Handled = true;
                isSelecting = e.button;
                SelectFor(e.curState.X, e.curState.Y);
            }
            else
            {
                isFocused = false;
            }
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            if (e.button == isSelecting)
            {
                isSelecting = -1;
                if (IsIn(e.curState.X, e.curState.Y))
                {
                    e.Handled = true;
                }
            }
            if (IsIn(e.curState.X, e.curState.Y))
            {
                isFocused = true;
                if (scrollbar.IsIn(e.curState.X, e.curState.Y))
                {
                    scrollbar.onButtonUp(e);
                    return;
                }
            }
            else
            {
                isFocused = false;
            }
        }

        int clickedButton = -1;
        int ticksSinceClicked = 0;
        Vector2 clickedPos = new Vector2();
        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            if (scrollbar.IsIn(e.curState.X, e.curState.Y))
            {
                scrollbar.onButtonClick(e);
                return;
            }
            #region DoubleClickCheck
            if (!e.Handled)
            {
                if (clickedButton == -1)
                {
                    clickedButton = e.button;
                    ticksSinceClicked = 0;
                    clickedPos = new Vector2(e.curState.X, e.curState.Y);
                }
                else
                {
                    if (clickedButton == e.button)
                    {
                        if (Math.Abs(clickedPos.X - e.curState.X) < 5 && Math.Abs(clickedPos.Y - e.curState.Y) < 5)
                        {
                            if (onDoubleClicked != null)
                            {
                                onDoubleClicked(this, new SelectedIndexArgs() { selectedIndex = selectedIndex });
                            }
                            clickedButton = -1;
                            ticksSinceClicked = 0;
                            clickedPos = new Vector2();
                        }
                        else
                        {
                            clickedButton = e.button;
                            ticksSinceClicked = 0;
                            clickedPos = new Vector2(e.curState.X, e.curState.Y);
                        }
                    }
                    else
                    {
                        clickedButton = e.button;
                        ticksSinceClicked = 0;
                        clickedPos = new Vector2(e.curState.X, e.curState.Y);
                    }
                }
            }
            #endregion
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            if (isSelecting != -1)
            {
                SelectFor(e.curState.X, e.curState.Y);
            }
            if (scrollbar.IsIn(e.curState.X, e.curState.Y))
            {
                scrollbar.onMouseMove(e);
                return;
            }
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y))
            {
                e.Handled = true;
                Scrolled -= e.delta / 120;
                //CheckScrolledBounds();
                //Scrolled = scrolled;
            }
        }

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
        }

        public override void onKeyUp(InputEngine.KeyboardArgs e)
        {
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            if (isFocused)
            {
                if (e.key == Keys.Down.GetHashCode())
                {
                    SelectedIndex++;
                    CheckSelectedBounds();
                    e.Handled = true;
                }
                if (e.key == Keys.Up.GetHashCode())
                {
                    SelectedIndex--;
                    CheckSelectedBounds();
                    e.Handled = true;
                }
            }
        }

    }
}
