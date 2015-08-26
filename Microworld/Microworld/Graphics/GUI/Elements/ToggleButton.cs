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
    public class ToggleButton : MenuButton
    {
        int curState = 0;
        public int CurIndex
        {
            get { return curState; }
            set
            {
                if (textures.Count == 0)
                    return;

                if (value > textures.Count - 1)
                    value = textures.Count - 1;
                if (value < 0)
                    value = 0;

                if (curState != value)
                {
                    String oldkey = keys[curState];
                    curState = value;
                    LeftTexture = textures[curState];
                    WasInitiallyDrawn = false;
                    if (onSelectedChanged != null)
                        onSelectedChanged.Invoke(this, oldkey, keys[curState]);
                }
            }
        }
        public String CurSelected
        {
            get
            {
                if (textures.Count == 0)
                    return "";
                return keys[curState];
            }
            set
            {
                if (textures.Count == 0)
                    return;
                int i = keys.IndexOf(value);
                if (i > -1)
                    CurIndex = i;
            }
        }

        public delegate void SelectedChanged(Object sender, String oldKey, String newKey);
        public event SelectedChanged onSelectedChanged;

        List<Texture2D> textures = new List<Texture2D>();
        List<String> keys = new List<string>();

        public ToggleButton()
            : base()
        {
        }

        public ToggleButton(int x, int y, int w, int h)
            : base(x, y, w, h, "")
        {
        }

        /// <summary>
        /// Coppies values from another instance. Does NOT copy event subscribers.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="b"></param>
        public ToggleButton(int x, int y, int w, int h, ToggleButton b)
            : base(x, y, w, h, "")
        {
            textures = b.textures;
            keys = b.keys;
            CurIndex = b.CurIndex;
        }

        public void Add(Texture2D texture, String key)
        {
            textures.Add(texture);
            keys.Add(key);

            if (textures.Count == 1)
            {
                LeftTexture = texture;
                WasInitiallyDrawn = false;
            }
        }

        public void Clear()
        {
            textures.Clear();
            keys.Clear();

            curState = 0;
            LeftTexture = null;
            WasInitiallyDrawn = false;
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            if (IsIn((int)e.curState.X, (int)e.curState.Y))
            {
                InputEngine.eventHandled = true;
                int cur = CurIndex + 1;
                if (cur >= textures.Count)
                    cur = 0;
                CurIndex = cur;
            }
        }
    }
}
