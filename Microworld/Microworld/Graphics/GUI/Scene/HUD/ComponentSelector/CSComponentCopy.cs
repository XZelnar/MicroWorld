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
    public class CSComponentCopy : CSTile
    {
        internal static Texture2D fav;

        internal CSComponent component;

        #region Links
        internal Components.Graphics.GraphicalComponent componentGraphics
        {
            get { return component.componentGraphics; }
        }
        public Type objType
        {
            get { return component.objType; }
        }
        public bool IsDragDropPlacement
        {
            get { return component.IsDragDropPlacement; }
        }
        public String Name
        {
            get { return component.Name; }
        }
        public int Avalable
        {
            get { return component.Avalable; }
        }
        public bool Enabled
        {
            get { return component.Enabled; }
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
        public override Color Color
        {
            get
            {
                return component.Color;
            }
            set
            {
                base.Color = value;
            }
        }
        public bool IsComponentOfType<T>()
        {
            return component.instance is T;
        }
        #endregion

        public CSComponentCopy() { }

        public CSComponentCopy(CSComponent c)
        {
            component = c;
            Texture = c.Texture;
        }

        public CSComponentCopy(CSComponentCopy c)
        {
            component = c.component;
            position = c.position;
            this.Color = c.Color;
            this.isVisible = c.isVisible;
            this.localIndex = c.localIndex;
            this.parent = c.parent;
            this.size = c.size;
            this.Texture = c.Texture;
        }

        public override void OnClicked()
        {
            //component.position = position;
            GUIEngine.s_componentSelector.SelectedComponent = this;
            if (parent == null)
            {
                foreach (var item in GUIEngine.s_componentSelector.rootTiles)
                {
                    if (item is CSFolder && (item as CSFolder).IsOpened)
                        (item as CSFolder).IsOpened = false;
                }
            }
            base.OnClicked();
        }

        public override void Draw(Renderer renderer)
        {
            base.Draw(renderer);
            String s = "";
            if (Avalable == -1)
                s = "oo";
            else
                s = Avalable.ToString();
            var a = ComponentSelector.ComponentsLeftFont.MeasureString(s);
            if (component.drawCount)
                Main.renderer.DrawString(ComponentSelector.ComponentsLeftFont, s,
                    new Rectangle((int)Position.X, (int)(Position.Y + SIZE_X - a.Y), (int)SIZE_X - 2, (int)a.Y),
                    Color.White, Renderer.TextAlignment.Right);

            renderer.Draw(fav, new Rectangle((int)position.X + SIZE_X - 13, (int)position.Y + 1, 12, 12),
                component.IsFavourite ? Color.White : Color.Gray);
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
            if (Avalable == -1)
                s = "oo";
            else
                s = Avalable.ToString();
            var a = ComponentSelector.ComponentsLeftFont.MeasureString(s);
            if (component.drawCount)
                Main.renderer.DrawString(ComponentSelector.ComponentsLeftFont, s,
                    new Rectangle((int)Position.X, (int)(Position.Y + SIZE_X - a.Y), (int)SIZE_X - 1, (int)a.Y),
                    Color.White, Renderer.TextAlignment.Right);

            position = t;
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            if (e.Handled || !isVisible) return;
            if (e.button == 2 && IsIn(e.curState.X, e.curState.Y))//wheel
            {
                String link = componentGraphics.GetHandbookFile();

                if (!Main.curState.StartsWith("GAME") || link == null) return;
                if (link != null && link != "")
                {
                    GUIEngine.AddHUDScene(GUIEngine.s_mainMenu);
                    GUIEngine.s_mainMenu.Show();
                    GUIEngine.s_mainMenu.InitForHandbook(true);
                    GUIEngine.s_handbook.InitForFolder("Content/Encyclopedia/" + link.Substring(0, link.IndexOf("/")));
                    GUIEngine.s_handbook.OpenPage("Content/Encyclopedia/" + link);
                    e.Handled = true;
                }
            }
            else
            {
                base.onButtonClick(e);
                if (e.button == 0 &&
                    e.curState.X >= position.X + SIZE_X - 12 && e.curState.X <= position.X + SIZE_X &&
                    e.curState.Y >= position.Y && e.curState.Y <= position.Y + 12)
                {
                    component.IsFavourite = !component.IsFavourite;
                    e.Handled = true;
                }
            }
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y))
            {
                String s = "";
                if (Avalable == -1)
                    s = "oo";
                else
                    s = Avalable.ToString();
                Shortcuts.SetInGameStatus(Text + " (" + s + " avalable)", 
                    "<Click> to select, <Middle click> for handbook");
            }
            base.onMouseMove(e);
        }
    }
}
