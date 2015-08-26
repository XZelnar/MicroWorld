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
    public class CSFolder : CSTile
    {
        internal List<CSTile> Tiles = new List<CSTile>();

        private bool isOpened = false;
        public bool IsOpened
        {
            get { return isOpened; }
            set
            {
                isOpened = value;
                if (isOpened)
                {
                    for (int i = 0; i < Tiles.Count; i++)
                    {
                        Tiles[i].isVisible = true;
                    }
                    if (parent == null)
                    {
                        foreach (var c in GUIEngine.s_componentSelector.rootTiles)
                        {
                            if (c is CSFolder && c != this)
                                (c as CSFolder).IsOpened = false;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < parent.Tiles.Count; i++)
                        {
                            if (parent.Tiles[i] != this && parent.Tiles[i] is CSFolder)
                                (parent.Tiles[i] as CSFolder).isOpened = false;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Tiles.Count; i++)
                    {
                        Tiles[i].isVisible = false;
                        if (Tiles[i] is CSFolder)
                            (Tiles[i] as CSFolder).IsOpened = false;
                        else
                            if (Tiles[i] is CSComponentCopy && 
                                (Tiles[i] as CSComponentCopy) == GUIEngine.s_componentSelector.SelectedComponent)
                                    GUIEngine.s_componentSelector.ResetSelection();
                    }
                }
            }
        }
        public override Vector2 Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                base.Position = value;
                for (int i = 0; i < Tiles.Count; i++)
                {
                    Tiles[i].position = new Vector2(6 + position.X + CSTile.SIZE_X, 4 + i * CSTile.SIZE_Y);
                }
            }
        }

        #region Saves
        public void SaveStructure(System.IO.BinaryWriter bw)
        {
            String t = "";
            for (int i = 0; i < Tiles.Count; i++)
            {
                t = (Tiles[i] is CSComponentCopy ? "0" : "1") + Tiles[i].Text;
                bw.Write(t);
                if (Tiles[i] is CSFolder)
                {
                    (Tiles[i] as CSFolder).SaveStructure(bw);
                }
            }
            bw.Write("2");
        }

        public bool LoadStructure(System.IO.BinaryReader br)
        {
            String s = "";
            while (true)
            {
                s = br.ReadString();
                if (s.StartsWith("0"))//CSC
                {
                    var a = GUIEngine.s_componentSelector._getComponent(s.Substring(1));
                    if (a == null) return false;
                    GUIEngine.s_componentSelector.AddComponent(this, a);
                    continue;
                }
                if (s.StartsWith("1"))//CSF
                {
                    CSFolder f = GetCreateSubfolder(s.Substring(1));

                    if (!f.LoadStructure(br))
                        return false;
                    continue;
                }
                if (s.StartsWith("2"))//folder end
                    break;
            }

            return true;
        }
        #endregion

        public override void OnClicked()
        {
            IsOpened = !IsOpened;
            base.OnClicked();
        }

        public override void Initialize()
        {
        }

        int animationState = 0;
        public override void Update()
        {
            base.Update();
            if (isOpened)
            {
                if (animationState < SIZE_X)
                    animationState += 4;
                for (int i = 0; i < Tiles.Count; i++)
                {
                    Tiles[i].Update();
                }
            }
            else
            {
                if (animationState > 0)
                    animationState -= 4;
            }
        }

        public override void Draw(Renderer renderer)
        {
            base.Draw(renderer);
            int dotOutIndex = -1;
            bool hasAvalableComponents = false;
            for (int i = 0; i < Tiles.Count; i++)
            {
                if (Tiles[i] is CSFolder && (Tiles[i] as CSFolder).IsOpened)
                {
                    dotOutIndex = i;
                    break;
                }
                if (Tiles[i] is CSComponentCopy && (Tiles[i] as CSComponentCopy).Avalable != 0)
                    hasAvalableComponents = true;
            }
            //overlay
            if (!hasAvalableComponents)
            {
                renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)position.X, (int)position.Y, SIZE_X, SIZE_Y), Shortcuts.BG_COLOR * 0.5f);
                renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)position.X, (int)position.Y, SIZE_X, SIZE_Y), Color.Black * 0.4f);
            }
            //opened folder
            if (IsOpened || animationState > 0)
            {
                if (animationState < SIZE_X)
                {
                    renderer.SetScissorRectangle(position.X + SIZE_X, 0, SIZE_X, Main.WindowHeight, false);
                    renderer.End();
                    renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, null,
                        null, Graphics.GraphicsEngine.s_ScissorsOn, null, Matrix.CreateTranslation(animationState - SIZE_X, 0, 0));
                }
                //bg
                RenderHelper.SmartDrawRectangle(ComponentSelector.bg, 8, 42 + (int)position.X, 0, 48, Main.WindowHeight,
                    Color.White, renderer);
                if (dotOutIndex != -1)
                {
                    renderer.End();
                    renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None,
                        RasterizerState.CullNone);
                    renderer.Draw(ComponentSelector.dotOverlay,
                        new Rectangle(44 + (int)position.X, 2, 44, Main.WindowHeight - 4),
                        new Rectangle(44 + (int)position.X, 2, 44, Main.WindowHeight - 4),
                        Color.White);
                }
                //items
                for (int i = 0; i < Tiles.Count; i++)
                {
                    Tiles[i].Draw(renderer);
                    if (dotOutIndex != -1 && dotOutIndex != i)
                    {
                        renderer.Draw(ComponentSelector.dotOverlay,
                            new Rectangle((int)Tiles[i].position.X, (int)Tiles[i].position.Y, CSTile.SIZE_X, CSTile.SIZE_Y),
                            new Rectangle((int)Tiles[i].position.X, (int)Tiles[i].position.Y, CSTile.SIZE_X, CSTile.SIZE_Y),
                            Color.White);
                    }
                }
                if (animationState < SIZE_X)
                {
                    renderer.ResetScissorRectangle();
                }
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (isOpened)
            {
                for (int i = 0; i < Tiles.Count; i++)
                {
                    Tiles[i].PostDraw();
                }
            }
        }

        public CSFolder GetCreateSubfolder(String name)
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                if (Tiles[i] is CSFolder && Tiles[i].Text == name)
                {
                    return Tiles[i] as CSFolder;
                }
            }
            CSFolder f = new CSFolder();
            f.Text = name;
            if (ComponentSelector.folderTextures.ContainsKey(f.Text))
                Texture = ResourceManager.Load<Texture2D>(ComponentSelector.folderTextures[f.Text]);
            else
                Texture = GraphicsEngine.pixel;
            f.position = new Vector2(4 + position.X + 40, position.Y + Tiles.Count * 40);
            f.parent = null;
            f.localIndex = Tiles.Count;
            f.isVisible = true;
            f.Color = Color.White;
            GUIEngine.s_componentSelector.allTiles.Add(f);
            Tiles.Add(f);
            return f;
        }

        public void Sort()
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                for (int j = i + 1; j < Tiles.Count; j++)
                {
                    if (Tiles[i] is CSComponentCopy && Tiles[j] is CSFolder)
                        SwapTiles(i, j);
                    else
                        if (Tiles[i] is CSFolder && Tiles[j] is CSComponentCopy)
                            continue;
                        else
                            if (Tiles[i].Text.CompareTo(Tiles[j].Text) > 0)
                                SwapTiles(i, j);
                }
            }
        }

        internal void SwapTiles(int i, int j)
        {
            var a = Tiles[i];
            Tiles[i] = Tiles[j];
            Tiles[j] = a;

            Tiles[i].localIndex = i;
            Tiles[j].localIndex = j;

            var p = Tiles[i].Position;
            Tiles[i].Position = Tiles[j].Position;
            Tiles[j].Position = p;
        }

        internal void SwapTilesNoPositions(int i, int j)
        {
            var a = Tiles[i];
            Tiles[i] = Tiles[j];
            Tiles[j] = a;

            Tiles[i].localIndex = i;
            Tiles[j].localIndex = j;
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            if (IsIn(e.curState.X, e.curState.Y))
            {
                Shortcuts.SetInGameStatus(Text, "");
            }
            base.onMouseMove(e);
        }
    }
}
