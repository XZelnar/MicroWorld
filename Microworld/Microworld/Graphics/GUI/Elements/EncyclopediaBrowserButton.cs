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
    public class EncyclopediaBrowserButton : Button
    {
        internal Texture2D selectedbg;

        public override bool isMouseOver
        {
            get
            {
                return base.isMouseOver;
            }
            set
            {
                if (isMouseOver == value) return;
                base.isMouseOver = value;

                if (isMouseOver)
                {
                    OnMouseOver();
                }
                else
                {
                    if (isPressed) return;
                    tppos = Vector2.Zero;
                    tpsize = Vector2.Zero;
                    if (fadingthread != null && fadingthread.ThreadState != System.Threading.ThreadState.Stopped)
                    {
                        try
                        {
                            fadingthread.Abort();
                            fadingthread = null;
                        }
                        catch { }
                    }
                }
            }
        }
        public override bool isPressed
        {
            get
            {
                return base.isPressed;
            }
            set
            {
                if (isPressed == value) return;
                base.isPressed = value;

                if (isPressed)
                {
                    if (fadingthread == null && tpsize.Y == 0)
                        OnMouseOver();
                }
                else
                {
                    tppos = Vector2.Zero;
                    tpsize = Vector2.Zero;
                    if (fadingthread != null && fadingthread.ThreadState != System.Threading.ThreadState.Stopped)
                    {
                        try
                        {
                            fadingthread.Abort();
                            fadingthread = null;
                        }
                        catch { }
                    }
                }
            }
        }

        public float IdleOpacity = 0.4f;

        System.Threading.Thread fadingthread;
        public EncyclopediaBrowserButton(int x, int y, int w, int h, String txt)
            : base(x, y, w, h, txt)
        {
        }

        public override void Initialize()
        {
            if (selectedbg == null)
                selectedbg = ResourceManager.Load<Texture2D>("GUI/Menus/ButtonBackground");
            base.Initialize();
        }

        public void OverrideTexture(String t)
        {
            selectedbg = ResourceManager.Load<Texture2D>(t);
        }

        public void OnMouseOver()
        {
            fadingthread = new System.Threading.Thread(new System.Threading.ThreadStart(_mouseOverFadeIn));
            fadingthread.Start();
        }

        internal Vector2 tppos, tpsize;
        public void _mouseOverFadeIn()
        {
            tppos = position;
            tpsize = size;
            tppos.Y = position.Y + size.Y / 2;
            tpsize.Y = 0;
            for (int i = 0; i < size.Y / 2; i++)
            {
                tppos.Y--;
                tpsize.Y += 2;
                System.Threading.Thread.Sleep(3);
            }
            tpsize = size;
            tppos = position;
            fadingthread = null;
        }

        public override void Draw(Renderer renderer)
        {
            if (!isVisible) return;
            if (texture == null) texture = textureGlobal;
            if (Font == null) Font = GUIEngine.font;
            //Main.renderer.Draw(texture,
            //    new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
            //    new Rectangle(4, 4, (int)1, (int)1),
            //    isEnabled ? (isPressed ? pressedColor : (isMouseOver ? mouseOverColor : background)) : disabledColor);

            //drawBorder(ref texture,
            //    isEnabled ? (isPressed ? pressedColor : (isMouseOver ? mouseOverColor : background)) : disabledColor,
            //    position, size, 4, false);

            if (isEnabled)
            {
                renderer.Draw(selectedbg, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), 
                    Color.White * IdleOpacity);
                if (tpsize.Y != 0)
                {
                    renderer.Draw(selectedbg, new Rectangle((int)tppos.X, (int)tppos.Y, (int)tpsize.X, (int)tpsize.Y), Color.White);
                }
            }
            else
            {
                renderer.Draw(selectedbg, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y),
                    new Color(80, 80, 80) * IdleOpacity);
            }

            if (textOld != Text)
            {
                textOld = Text;
                stringSize = GUIEngine.font.MeasureString(Text);
            }

            Main.renderer.DrawString(Font, Text, new Rectangle((int)position.X,
                (int)(position.Y + (size.Y - stringSize.Y) / 2), (int)size.X, (int)stringSize.Y), foreground * (isEnabled ? 1f : 0.4f), 
                textAlignment);
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            bool wasover = isMouseOver;
            base.onMouseMove(e);
            if (isMouseOver && !wasover)
            {
                Sound.SoundPlayer.MenuMouseOver();
            }
        }
    }
}
