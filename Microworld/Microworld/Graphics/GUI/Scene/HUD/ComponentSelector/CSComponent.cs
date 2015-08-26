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
    public class CSComponent : CSTile
    {
        internal int avalable;
        internal bool enabled;
        internal Components.Graphics.GraphicalComponent componentGraphics;
        internal Components.Component instance;
        internal bool isFavourite = false;
        internal bool drawCount = true;
        internal int[] jointCoords0cw = new int[0];
        internal int[] jointCoords90cw = new int[0];
        internal int[] jointCoords180cw = new int[0];
        internal int[] jointCoords270cw = new int[0];

        public Type objType;
        public bool IsDragDropPlacement = false;
        public String Name;

        public bool IsFavourite
        {
            get { return isFavourite; }
            set
            {
                if (isFavourite == value)
                    return;
                isFavourite = value;
                if (isFavourite)
                    AddFav();
                else
                    RemoveFav();
            }
        }
        public int Avalable
        {
            get { return avalable; }
            set
            {
                avalable = value;
                if (avalable < -1)
                    avalable = -1;
                if (avalable == -1)
                {
                    enabled = true;
                    Color = Color.White;
                }
                if (avalable > 0)
                {
                    enabled = true;
                    Color = Color.White;
                }
                if (avalable == 0)
                {
                    enabled = false;
                    Color = Color.Gray;
                }
            }
        }
        public bool Enabled
        {
            get { return enabled; }
        }
        public Components.Graphics.GraphicalComponent ComponentGraphics
        {
            get { return componentGraphics; }
        }
        public override string Text
        {
            get
            {
                return Name;
            }
        }

        public CSComponent()
        {
        }

        public CSComponent(CSComponent c)
        {
            avalable = c.avalable;
            enabled = c.enabled;
            componentGraphics = c.componentGraphics;
            isFavourite = c.isFavourite;
            drawCount = c.drawCount;
            objType = c.objType;
            IsDragDropPlacement = c.IsDragDropPlacement;
            Name = c.Name;
        }

        internal int[] GetJointCoords(Components.Component.Rotation r)
        {
            switch (r)
            {
                case MicroWorld.Components.Component.Rotation.cw0:
                    return jointCoords0cw;
                case MicroWorld.Components.Component.Rotation.cw90:
                    return jointCoords90cw;
                case MicroWorld.Components.Component.Rotation.cw180:
                    return jointCoords180cw;
                case MicroWorld.Components.Component.Rotation.cw270:
                    return jointCoords270cw;
                default:
                    return new int[0];
            }
        }

        #region fav
        public void AddFav()
        {
            CSFolder f = GUIEngine.s_componentSelector.GetFavFolder();
            if (f == null)
                f = GUIEngine.s_componentSelector.AddRootFolder("Favourite");
            GUIEngine.s_componentSelector.AddComponent(f, this);
        }

        public void RemoveFav()
        {
            CSFolder f = GUIEngine.s_componentSelector.GetFavFolder();
            if (f == null)
                return;
            GUIEngine.s_componentSelector.RemoveComponent(f, this);
        }
        #endregion

        public override void Initialize()//TODO load stats
        {
        }

        public override void OnClicked()
        {
            //GUIEngine.s_componentSelector.SelectedComponent = this;
        }

        public override void Draw(Renderer renderer)
        {
            base.Draw(renderer);
            String s = "";
            if (avalable == -1)
                s = "oo";
            else
                s = avalable.ToString();
            var a = ComponentSelector.ComponentsLeftFont.MeasureString(s);
            if (drawCount)
                Main.renderer.DrawString(ComponentSelector.ComponentsLeftFont, s,
                    new Rectangle((int)Position.X, (int)(Position.Y + SIZE_X - a.Y), (int)SIZE_X, (int)a.Y),
                    Color.White, Renderer.TextAlignment.Right);
        }

        public override void DrawAt(int x, int y, Renderer renderer)
        {
            Vector2 t = position;
            position = new Vector2(x, y);

            if (Texture == null)
                return;
            if (!isVisible) return;
            renderer.Draw(ComponentSelector.ComponentBackground, new Rectangle((int)position.X, (int)position.Y, SIZE_X, SIZE_Y),
                Color.White);
            renderer.Draw(Texture, new Rectangle((int)position.X + 4, (int)position.Y + 4, SIZE_X - 8, SIZE_Y - 8), Color);
            String s = "";
            if (avalable == -1)
                s = "oo";
            else
                s = avalable.ToString();
            var a = ComponentSelector.ComponentsLeftFont.MeasureString(s);
            if (drawCount)
                Main.renderer.DrawString(ComponentSelector.ComponentsLeftFont, s,
                    new Rectangle((int)Position.X, (int)(Position.Y + SIZE_X - a.Y), (int)SIZE_X, (int)a.Y),
                    Color.White, Renderer.TextAlignment.Right);

            position = t;
        }

        #region ComponentStuff
        public object GetNewInstance()
        {
            if (avalable == 0) return null;
            if (objType == null) return null;
            try
            {
                var a = Activator.CreateInstance(objType) as Components.Component;
                return a;
            }
            catch
            {
                return null;
            }
        }

        public void DecreaseAvilability()
        {
            if (Avalable > 0) Avalable--;
        }
        #endregion

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            base.onButtonClick(e);
            if (e.button == 2 && IsIn(e.curState.X, e.curState.Y))//wheel
            {
                String link = componentGraphics.GetHandbookFile();

                if (!Main.curState.StartsWith("GAME") || link == null) return;
                if (link != null && link != "")
                {
                    e.Handled = true;
                    GUIEngine.s_encyclopediaPage.LastState = Main.curState;
                    GUIEngine.s_encyclopediaPage.LastScene = GUIEngine.curScene;
                    Main.curState = "GUIEncyclopedia";
                    GUIEngine.curScene = GUIEngine.s_encyclopediaPage;
                    GUIEngine.s_encyclopediaPage.OpenPage("Content/Encyclopedia/" + link);
                }
            }
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y))
            {
                String s = "";
                if (avalable == -1)
                    s = "oo";
                else
                    s = avalable.ToString();
                Shortcuts.SetInGameStatus(Text + " (" + s + " avalable)", 
                    "<Click> to select, <Middle click> for handbook");
            }
            base.onMouseMove(e);
        }
    }
}
