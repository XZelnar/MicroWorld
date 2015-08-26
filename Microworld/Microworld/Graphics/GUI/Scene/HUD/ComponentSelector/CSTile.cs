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

namespace MicroWorld.Graphics.GUI.Scene.ComponentSelector
{
    public class CSTile : Control//TODO tooltip
    {
        public const int SIZE_X = 40;
        public const int SIZE_Y = 40;

        internal int localIndex = 0;
        internal CSFolder parent;

        private Texture2D texture;
        private String text = "";
        private Color color = Color.White;
        private float highlightIntensity = 0;
        private bool isMouseOver = false;

        public virtual Color Color
        {
            get { return color; }
            set { color = value; }
        }
        public CSTile Parent
        {
            get { return parent; }
        }//null for root
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        public virtual String Text
        {
            get { return text; }
            set { text = value; }
        }

        #region events
        public delegate void TileClicked(object sender);
        public event TileClicked onTileClicked;
        #endregion

        public CSTile()
        {
        }

        public CSTile(CSTile t)
        {
            localIndex = t.localIndex;
            parent = t.parent;
            texture = t.texture;
            text = t.text;
            color = t.color;
            onTileClicked = t.onTileClicked;
        }

        public override void Initialize()
        {
        }

        public override void Update()
        {
            base.Update();
            if (isdnd)
            {
                if (InputEngine.curMouse.LeftButton == ButtonState.Released)
                    isdnd = false;
            }
            if (!isdnd)
            {
                float delta = 0;
                float movey = 0;

                if ((delta = position.Y - localIndex * SIZE_Y - 4) != 0)
                    movey -= Math.Sign(position.Y - localIndex * SIZE_Y - 4);

                if (movey != 0)
                {
                    delta = Math.Abs(delta);
                    if (delta > 10) movey *= 2;
                    if (delta > 20) movey *= 2;
                    if (delta > 50) movey *= 2;
                    if (delta > 100) movey *= 2;
                    position.Y += movey;
                }
            }

            if (isMouseOver || (this is CSComponentCopy && (this as CSComponentCopy) == GUIEngine.s_componentSelector.SelectedComponent))
            {
                if (highlightIntensity < 1)
                    highlightIntensity += 0.05f;
            }
            else
            {
                if (highlightIntensity > 0)
                {
                    highlightIntensity -= 0.05f;
                }
            }
        }

        public override void Draw(Renderer renderer)
        {
            if (texture == null)
            {
                return;
            }
            if (!isVisible) return;
            renderer.Draw(ComponentSelector.ComponentBackground, new Rectangle((int)position.X, (int)position.Y, SIZE_X, SIZE_Y), 
                Color);
            renderer.Draw(texture, new Rectangle((int)position.X + 4, (int)position.Y + 4, SIZE_X - 9, SIZE_Y - 8), Color);

            if (this is CSComponentCopy && (this as CSComponentCopy) == GUIEngine.s_componentSelector.SelectedComponent)
            {
                renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)position.X+1, (int)position.Y+1, SIZE_X-2, SIZE_Y-2),
                    Color.White * 0.3f);
            }
            else
            {
                renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)position.X+1, (int)position.Y+1, SIZE_X-2, SIZE_Y-2),
                    Color.White * 0.3f * highlightIntensity);
            }
        }

        public override void PostDraw()//redraw dnd component ontop
        {
            if (isdnd) Draw(Main.renderer);
            base.PostDraw();
        }

        public virtual void DrawAt(int x, int y, Renderer renderer)
        {
            if (texture == null)
                return;
            if (!isVisible) return;
            renderer.Draw(ComponentSelector.ComponentBackground, new Rectangle(x, y, SIZE_X, SIZE_Y),
                Color.White);
            renderer.Draw(texture, new Rectangle(x + 4, y + 4, SIZE_X - 8, SIZE_Y - 8), color);
        }

        public virtual void OnClicked()
        {
            Sound.SoundPlayer.PlayButtonClick();
        }

        public override bool IsIn(int x, int y)
        {
            return isVisible &&
                x >= position.X && x < position.X + SIZE_X &&
                y >= position.Y && y < position.Y + SIZE_Y;
        }

        public String GetFullPath()
        {
            String r = Text;
            CSFolder cur = parent;
            while (cur != null)
            {
                r = cur.Text + "/" + r;
                cur = cur.parent;
            }
            return r;
        }

        #region IO
        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y))
            {
                e.Handled = true;
                //OnClicked();//TODO reenable???
                //if (onTileClicked != null)
                //    onTileClicked.Invoke(this);
            }
        }

        bool isdnd = false;
        Point DnDCursorOffset = new Point();
        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y))
            {
                e.Handled = true;
                if (e.button == 0)//left button
                {
                    OnClicked();//TODO rm???
                    //isdnd = true;//TODO reenable???
                    DnDCursorOffset = new Point(e.curState.X - (int)position.X, e.curState.Y - (int)position.X);
                    if (onTileClicked != null)
                        onTileClicked.Invoke(this);
                }
            }
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y))
            {
                e.Handled = true;
                if (e.button == 0)
                {
                    isdnd = false;
                }
            }
        }

        bool wasin = false;
        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            isMouseOver = IsIn(e.curState.X, e.curState.Y);
            if (!wasin && isMouseOver)
            {
                GUIEngine.s_toolTip.Position = position + new Vector2(SIZE_X + 1, SIZE_Y / 2);
                GUIEngine.s_toolTip.Text = Text;
                //GUIEngine.s_toolTip.Text = "This is a very long text!!!\r\nThis is a very long text!!!\r\nThis is a very long text!!!\r\nThis is a very long text!!!\r\nThis is a very long text!!!\r\nThis is a very long text!!!\r\n";//TODO rm
                GUIEngine.s_toolTip.Show();
                GUIEngine.s_toolTip.isVisible = true;
                wasin = true;
            }
            if (wasin && !isMouseOver)
            {
                if (GUIEngine.s_toolTip.Text == Text)
                {
                    GUIEngine.s_toolTip.isVisible = false;
                    GUIEngine.s_toolTip.Close();
                }
                wasin = false;
            }
            if (isdnd)
            {
                position.Y += e.dy;
                if (parent == null)
                {
                    while (localIndex > 0 &&
                        position.Y < GUIEngine.s_componentSelector.rootTiles[localIndex - 1].position.Y + SIZE_Y / 2)//move up
                    {
                        GUIEngine.s_componentSelector.SwapRootTilesNoPositions(localIndex, localIndex - 1);
                    }
                    while (localIndex < GUIEngine.s_componentSelector.rootTiles.Count - 1 &&
                        position.Y > GUIEngine.s_componentSelector.rootTiles[localIndex + 1].position.Y + SIZE_Y / 2)//move up
                    {
                        GUIEngine.s_componentSelector.SwapRootTilesNoPositions(localIndex, localIndex + 1);
                    }
                }
                else
                {
                    while (localIndex > 0 &&
                        position.Y < parent.Tiles[localIndex - 1].position.Y + SIZE_Y / 2)//move up
                    {
                        parent.SwapTilesNoPositions(localIndex, localIndex - 1);
                    }
                    while (localIndex < parent.Tiles.Count - 1 &&
                        position.Y > parent.Tiles[localIndex + 1].position.Y + SIZE_Y / 2)//move up
                    {
                        parent.SwapTilesNoPositions(localIndex, localIndex + 1);
                    }
                }
            }
        }
        #endregion
    }
}
