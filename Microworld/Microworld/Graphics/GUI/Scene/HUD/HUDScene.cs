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
    public abstract class HUDScene : Scene
    {
        //private int layer = 0;
        public new int Layer
        {
            get { return base.Layer; }
            set
            {
                base.Layer = value;
                GUIEngine.SortScenes();
            }
        }

        public override void onShow()
        {
            Graphics.GUI.GUIEngine.InvokeSceneOpened(ToString());
        }

        public override void onClose()
        {
            Graphics.GUI.GUIEngine.InvokeSceneClosed(ToString());
        }

        public virtual Vector2 GetPosition()
        {
            return new Vector2();
        }

        public virtual Vector2 GetSize()
        {
            return new Vector2();
        }

        public virtual void Close()
        {
            GUIEngine.RemoveHUDScene(this);
        }

        public virtual void Show()
        {
            GUIEngine.AddHUDScene(this);
        }

        public virtual bool IsIn(int x, int y)
        {
            return false;
        }
    }
}
