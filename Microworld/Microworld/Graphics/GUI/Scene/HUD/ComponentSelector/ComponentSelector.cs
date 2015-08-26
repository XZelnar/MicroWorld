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
using MicroWorld.Graphics.GUI.Scene.ComponentSelector;
using System.IO;

namespace MicroWorld.Graphics.GUI.Scene.ComponentSelector
{
    //V2.0
    public class ComponentSelector : HUDScene, IProvidesClickabilityAreas
    {
        public static SpriteFont ComponentsLeftFont;
        public static Texture2D ComponentBackground, Selected, bg, dotOverlay;

        public static Dictionary<String, String> folderTextures = new Dictionary<string, string>();

        internal List<CSTile> allTiles = new List<CSTile>();
        internal List<CSTile> rootTiles = new List<CSTile>();
        internal List<CSComponent> components = new List<CSComponent>();
        private CSComponentCopy selectedComponent;

        internal List<CSComponentCopy> clickable = new List<CSComponentCopy>();

        internal CSComponent cursor;

        public CSComponentCopy SelectedComponent
        {
            get { return selectedComponent; }
            set
            {
                if (value != selectedComponent)
                {
                    var old = selectedComponent;
                    selectedComponent = value;
                    Logics.GameInputHandler.ResetComponentRotation();
                    if (onSelectionChanged != null)
                        onSelectionChanged.Invoke(selectedComponent, old);
                }
            }
        }
        public Components.Graphics.GraphicalComponent SelectedComponentGraphics
        {
            get { return selectedComponent.componentGraphics; }
        }


        #region Events
        public delegate void SelectionHandler(CSComponentCopy current, CSComponentCopy last);
        public static event SelectionHandler onSelectionChanged;
        #endregion

        internal ComponentSelector()
        {
        }

        public static void StaticInit()
        {
            IO.Log.Write("        CS static init");
            folderTextures.Add("Magnets", "Components/Icons/Folders/Magnets");
            folderTextures.Add("Light-Emitting", "Components/Icons/Folders/Light-Emitting");
            folderTextures.Add("Basic", "Components/Icons/Folders/Basic");
            folderTextures.Add("Motors", "Components/Icons/Folders/Motors");
            folderTextures.Add("Logics", "Components/Icons/Folders/Logics");
            folderTextures.Add("Cores", "Components/Icons/Folders/Cores");
            folderTextures.Add("Rotatable", "Components/Icons/Folders/Rotatable");
            folderTextures.Add("Favourite", "Components/Icons/Folders/Favourite");
            folderTextures.Add("Advanced", "Components/Icons/Folders/Advanced");
            folderTextures.Add("Interactive", "Components/Icons/Folders/Interactive");
            folderTextures.Add("Liquids", "Components/Icons/Folders/Liquids");
            folderTextures.Add("Information", "Components/Icons/Folders/Information");
        }

        #region Interfaces
        public Rectangle[] GetClickabilityRectangles()
        {
            List<Rectangle> t = new List<Rectangle>();
            CSFolder cur = null;
            for (int i = 0; i < clickable.Count; i++)
            {
                if (clickable[i].isVisible)
                    t.Add(new Rectangle((int)clickable[i].position.X, (int)clickable[i].position.Y, CSTile.SIZE_X + 2, CSTile.SIZE_Y + 2));
                cur = clickable[i].parent;
                while (cur != null)
                {
                    if (cur.isVisible)
                        t.Add(new Rectangle((int)cur.position.X, (int)cur.position.Y, CSTile.SIZE_X + 2, CSTile.SIZE_Y + 2));
                    cur = cur.parent;
                }
            }
            return t.ToArray();
        }

        public bool HasClickableRectangles()
        {
            return clickable.Count != 0;
        }

        public void ClearClickableAreas()
        {
            clickable.Clear();
        }
        #endregion

        #region API
        public void SetAllUnavalable()
        {
            for (int i = 1; i < components.Count; i++)
            {
                components[i].Avalable = 0;
            }
        }

        public int GetElementIndex(String name)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].Name == name) return i;
            }
            return -1;
        }

        public CSComponent GetElement(String name)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].Name == name) 
                    return components[i];
            }
            return null;
        }

        public void SetComponentAvilability(String name, int count)
        {
            int index = GetElementIndex(name);
            if (index == -1) return;
            components[index].Avalable = count;
        }

        public void IncComponentAvilability(String name, int count)
        {
            int ind = GetElementIndex(name);
            if (ind == -1) return;
            components[ind].Avalable += count;
        }

        public double[] GetComponentPosition(String name)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].Name == name)
                    return new double[]{
                        components[i].Position.X, components[i].Position.Y};
            }
            return new double[] { 0, 0 };
        }

        public void ClearCount()
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].Avalable = -1;
            }
        }

        public void DecreaseSelectedComponentAvilability()
        {
            selectedComponent.component.DecreaseAvilability();
        }

        public void DecreaseComponentAvilability(String name)
        {
            var ind = GetElementIndex(name);
            //if (ind == -1) return;
            components[ind].DecreaseAvilability();
        }

        public void ResetSelection()
        {
            for (int i = 0; i < rootTiles.Count; i++)
            {
                if (rootTiles[i] is CSComponentCopy && (rootTiles[i] as CSComponentCopy).Text == "Cursor")
                {
                    selectedComponent = rootTiles[i] as CSComponentCopy;
                    return;
                }
            }
        }

        public CSComponent GetComponent(String name)
        {
            int ind = GetElementIndex(name);
            if (ind == -1) return null;
            return components[ind];
        }

        public int GetComponentAvilability(String name)
        {
            var t = GetComponent(name);
            return t == null ? 0 : t.Avalable;
        }

        public void AddClickableTile(String path)
        {
            var a = TileExists(path);
            if (a != null)
                clickable.Add(a);
        }

        public void ClearClickableTiles()
        {
            clickable.Clear();
        }

        public CSComponentCopy TileExists(String t)
        {
            var a = t.Split('/');
            if (a.Length == 1)
            {
                foreach (var c in rootTiles)
                {
                    if (c is CSComponentCopy && c.Text == t)
                        return c as CSComponentCopy;
                }
                return null;
            }
            CSFolder cur = null;
            foreach (var c in rootTiles)
            {
                if (c is CSFolder && c.Text == a[0])
                {
                    cur = c as CSFolder;
                    break;
                }
            }
            if (cur == null)
                return null;
            bool found = false;
            for (int i = 1; i < a.Length - 1; i++)
            {
                found = false;
                for (int j = 0; j < cur.Tiles.Count; j++)
                {
                    if (cur.Tiles[j] is CSFolder && cur.Tiles[j].Text == a[i])
                    {
                        cur = cur.Tiles[j] as CSFolder;
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return null;
            }
            for (int i = 0; i < cur.Tiles.Count; i++)
            {
                if (cur.Tiles[i] is CSComponentCopy && cur.Tiles[i].Text == a[a.Length - 1])
                {
                    return cur.Tiles[i] as CSComponentCopy;
                }
            }
            return null;
        }

        public bool TileVisible(String t)
        {
            var a = t.Split('/');
            if (a.Length == 1)
            {
                foreach (var c in rootTiles)
                {
                    if (c is CSComponentCopy && c.Text == t)
                        return true;
                }
                return false;
            }
            CSFolder cur = null;
            foreach (var c in rootTiles)
            {
                if (c is CSFolder && c.Text == a[0] && (c as CSFolder).IsOpened)
                {
                    cur = c as CSFolder;
                    break;
                }
            }
            if (cur == null)
                return false;
            bool found = false;
            for (int i = 1; i < a.Length - 1; i++)
            {
                found = false;
                for (int j = 0; j < cur.Tiles.Count; j++)
                {
                    if (cur.Tiles[j] is CSFolder && cur.Tiles[j].Text == a[i] && (cur as CSFolder).IsOpened)
                    {
                        cur = cur.Tiles[j] as CSFolder;
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return false;
            }
            for (int i = 0; i < cur.Tiles.Count; i++)
            {
                if (cur.Tiles[i] is CSComponentCopy && cur.Tiles[i].Text == a[a.Length - 1])
                {
                    return true;
                }
            }
            return false;
        }

        public Vector2 TilePosition(String t)
        {
            var a = t.Split('/');
            if (a.Length == 1)
            {
                foreach (var c in rootTiles)
                {
                    if (c is CSComponentCopy && c.Text == t)
                        return c.Position;
                }
                return new Vector2(-1, -1);
            }
            CSFolder cur = null;
            foreach (var c in rootTiles)
            {
                if (c is CSFolder && c.Text == a[0] && (c as CSFolder).IsOpened)
                {
                    cur = c as CSFolder;
                    break;
                }
            }
            if (cur == null)
                return new Vector2(-1, -1);
            bool found = false;
            for (int i = 1; i < a.Length - 1; i++)
            {
                found = false;
                for (int j = 0; j < cur.Tiles.Count; j++)
                {
                    if (cur.Tiles[j] is CSFolder && cur.Tiles[j].Text == a[i] && (cur as CSFolder).IsOpened)
                    {
                        cur = cur.Tiles[j] as CSFolder;
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return new Vector2(-1, -1);
            }
            for (int i = 0; i < cur.Tiles.Count; i++)
            {
                if (cur.Tiles[i] is CSComponentCopy && cur.Tiles[i].Text == a[a.Length - 1])
                {
                    return cur.Tiles[i].Position;
                }
            }
            return new Vector2(-1, -1);
        }
        #endregion

        #region Utils
        internal List<CSComponent> CloneTiles()
        {
            List<CSComponent> t = new List<CSComponent>();
            for (int i = 0; i < components.Count; i++)
            {
                t.Add(new CSComponent(components[i]));
            }
            return t;
        }

        internal void PopState(List<CSComponent> tiles)
        {
            String n = "";
            for (int i = 0; i < tiles.Count; i++)
            {
                n = tiles[i].Name;
                for (int j = 0; j < components.Count; j++)
                {
                    if (components[j].Name == n)
                        components[j].Avalable = tiles[i].Avalable;
                }
            }
        }

        public object GetComponentForSelected()
        {
            return selectedComponent.component.GetNewInstance();
        }

        public void Collapse()
        {
            for (int i = 0; i < rootTiles.Count; i++)
            {
                if (rootTiles[i] is CSFolder)
                    (rootTiles[i] as CSFolder).IsOpened = false;
            }
        }

        public CSFolder GetFavFolder()
        {
            for (int i = 0; i < rootTiles.Count; i++)
            {
                if (rootTiles[i] is CSFolder && rootTiles[i].Text == "Favourite")
                {
                    return rootTiles[i] as CSFolder;
                }
            }
            return null;
        }

        internal void SelectComponent(Type t)
        {
            Collapse();

            for (int i = 0; i < allTiles.Count; i++)
            {
                if (allTiles[i] is CSComponentCopy && (allTiles[i] as CSComponentCopy).objType == t)
                {
                    var tt = allTiles[i];
                    while (tt != null)
                    {
                        if (tt is CSComponentCopy)
                            tt = (tt as CSComponentCopy).Parent;
                        else
                        {
                            (tt as CSFolder).IsOpened = true;
                            tt = (tt as CSFolder).parent;
                        }
                    }
                    allTiles[i].OnClicked();
                }
            }
        }
        #endregion

        #region Saves
        public void SaveStructure()
        {
            IO.Log.Write("        CS save structure");
            if (!Directory.Exists("Saves"))
                Directory.CreateDirectory("Saves");
            if (File.Exists("Saves/cs.prf"))
                File.Delete("Saves/cs.prf");
            BinaryWriter bw = new BinaryWriter(new FileStream("Saves/cs.prf", FileMode.Create));
            bw.Write(components.Count);//checksum
            IO.Log.Write("        " + components.Count.ToString() + " components. " + allTiles.Count.ToString() + " tiles.");
            String t = "";
            for (int i = 0; i < rootTiles.Count; i++)
            {
                t = (rootTiles[i] is CSComponentCopy ? "0" : "1") + rootTiles[i].Text;
                bw.Write(t);
                if (rootTiles[i] is CSFolder)
                {
                    (rootTiles[i] as CSFolder).SaveStructure(bw);
                }
            }
            bw.Write("3");
            bw.Close();
            IO.Log.Write("        CS save finished");
        }

        public bool LoadStructure()
        {
            IO.Log.Write("        CS load structure");
            rootTiles.Clear();
            allTiles.Clear();

            if (!Directory.Exists("Saves")) return false;
            if (!File.Exists("Saves/cs.prf")) return false;

            IO.Log.Write("        Reading from file...");
            BinaryReader br = new BinaryReader(new FileStream("Saves/cs.prf", FileMode.Open));
            int t = br.ReadInt32();
            IO.Log.Write("        " + t.ToString() + " components in checksum");
            if (t != components.Count)
            {
                br.Close();
                return false;
            }

            String s = "";
            while (true)
            {
                s = br.ReadString();
                if (s.StartsWith("0"))//CSC
                {
                    var a = _getComponent(s.Substring(1));
                    if (a == null)
                    {
                        br.Close();
                        return false;
                    }
                    AddComponent(null, a);
                    continue;
                }
                if (s.StartsWith("1"))//CSF
                {
                    CSFolder f = new CSFolder();
                    f.Text = s.Substring(1);
                    if (ComponentSelector.folderTextures.ContainsKey(f.Text))
                        f.Texture = ResourceManager.Load<Texture2D>(ComponentSelector.folderTextures[f.Text]);
                    else
                        f.Texture = GraphicsEngine.pixel;
                    f.position = new Vector2(4, 4 + rootTiles.Count * 40);
                    f.parent = null;
                    f.localIndex = rootTiles.Count;
                    f.isVisible = true;
                    f.Color = Color.White;
                    rootTiles.Add(f);
                    allTiles.Add(f);

                    if (!f.LoadStructure(br))
                    {
                        br.Close();
                        return false;
                    }
                    continue;
                }
                if (s.StartsWith("3"))//file end
                    break;
            }

            br.Close();
            //scan fav folder
            IO.Log.Write("        Scanning for favourites");
            CSFolder fav = GetFavFolder();
            if (fav != null)
            {
                for (int i = 0; i < fav.Tiles.Count; i++)
                {
                    for (int j = 0; j < components.Count; j++)
                    {
                        if (components[j].Text == fav.Tiles[i].Text)
                        {
                            components[j].isFavourite = true;
                        }
                    }
                }
            }

            return true;
        }

        internal CSComponent _getComponent(String name)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].Text == name)
                    return components[i];
            }
            return null;
        }
        #endregion

        public void RegisterComponent(Components.Component comp)
        {
            CSComponent c = new CSComponent();

            IO.Log.Write("            Adding " + comp.Graphics.GetCSToolTip() + " to ComponentSelector");
            c.Texture = ResourceManager.Load<Texture2D>(comp.Graphics.GetIconName());
            c.objType = comp.GetType();
            c.instance = comp;
            c.componentGraphics = comp.Graphics;
            c.jointCoords0cw = comp.GetJointCoords(Components.Component.Rotation.cw0);
            c.jointCoords90cw = comp.GetJointCoords(Components.Component.Rotation.cw90);
            c.jointCoords180cw = comp.GetJointCoords(Components.Component.Rotation.cw180);
            c.jointCoords270cw = comp.GetJointCoords(Components.Component.Rotation.cw270);
            c.Name = comp.Graphics.GetCSToolTip();
            c.IsDragDropPlacement = (comp is Components.Properties.IDragDropPlacable);

            /*
            if (c.jointCoords0cw.Length % 2 == 1)
                c.jointCoords0cw = new int[0];
            else
                for (int i = 0; i < c.jointCoords0cw.Length; i++)
                    c.jointCoords0cw[i] = c.jointCoords0cw[i] - Math.Abs(c.jointCoords0cw[i] % 8);

            if (c.jointCoords90cw.Length % 2 == 1)
                c.jointCoords90cw = new int[0];
            else
                for (int i = 0; i < c.jointCoords90cw.Length; i++)
                    c.jointCoords90cw[i] = c.jointCoords90cw[i] - Math.Abs(c.jointCoords90cw[i] % 8);

            if (c.jointCoords180cw.Length % 2 == 1)
                c.jointCoords180cw = new int[0];
            else
                for (int i = 0; i < c.jointCoords180cw.Length; i++)
                    c.jointCoords180cw[i] = c.jointCoords180cw[i] - Math.Abs(c.jointCoords180cw[i] % 8);

            if (c.jointCoords270cw.Length % 2 == 1)
                c.jointCoords270cw = new int[0];
            else
                for (int i = 0; i < c.jointCoords270cw.Length; i++)
                    c.jointCoords270cw[i] = c.jointCoords270cw[i] - Math.Abs(c.jointCoords270cw[i] % 8);//*/

            //comp.typeID = (short)components.Count;
            Components.ComponentsManager.TypeIDs.Add(comp.GetType(), (short)Components.ComponentsManager.TypeIDs.Count);
            components.Add(c);
        }

        public void GenerateStructure()
        {
            IO.Log.Write("        CS genereting structure");
            if (LoadStructure())//load saved settings. sorts if can't load
                return;

            allTiles.Clear();
            rootTiles.Clear();

            String path = "";
            for (int i = 0; i < components.Count; i++)
            {
                path = components[i].componentGraphics.GetComponentSelectorPath();
                if (path == null)
                {
                    AddComponent(null, components[i]);
                }
                else
                {
                    var ps = path.Split(':');
                    foreach (var p2 in ps)
                    {
                        String p = p2;
                        if (p.EndsWith("/"))
                            p = p.Substring(0, p.Length - 1);
                        if (p.Length == 0)
                        {
                            AddComponent(null, components[i]);
                        }
                        else
                        {
                            var a = p.Split('/');
                            CSFolder curFolder = null;
                            for (int j = 0; j < rootTiles.Count; j++)
                            {
                                if (rootTiles[j].Text == a[0] && rootTiles[j] is CSFolder)
                                    curFolder = rootTiles[j] as CSFolder;
                            }
                            if (curFolder == null)
                            {
                                curFolder = AddRootFolder(a[0]);
                            }
                            for (int j = 1; j < a.Length; j++)
                            {
                                curFolder = curFolder.GetCreateSubfolder(a[j]);
                            }
                            AddComponent(curFolder, components[i]);
                            //curFolder.Tiles.Add(components[i]);
                        }
                    }
                }
            }
            AddRootFolder("Favourite");

            Sort();
            SaveStructure();
        }

        public CSFolder AddRootFolder(String text)
        {
            var f = new CSFolder();
            f.Text = text;
            if (folderTextures.ContainsKey(f.Text))
                f.Texture = ResourceManager.Load<Texture2D>(folderTextures[f.Text]);
            else
                f.Texture = GraphicsEngine.pixel;
            f.Position = new Vector2(4, 4 + rootTiles.Count * 40);
            f.parent = null;
            f.localIndex = rootTiles.Count;
            f.isVisible = true;
            f.Color = Color.White;
            allTiles.Add(f);
            rootTiles.Add(f);
            return f;
        }

        public void Sort()
        {
            for (int i = 0; i < rootTiles.Count; i++)
            {
                for (int j = i + 1; j < rootTiles.Count; j++)
                {
                    if (rootTiles[i] is CSFolder && rootTiles[j] is CSComponentCopy)
                        SwapRootTiles(i, j);
                    else
                        if (rootTiles[i] is CSComponentCopy && rootTiles[j] is CSFolder)
                            continue;
                        else
                            if (rootTiles[i].Text.CompareTo(rootTiles[j].Text) > 0)
                                SwapRootTiles(i, j);
                }
            }

            for (int i = 0; i < allTiles.Count; i++)
            {
                if (allTiles[i] is CSFolder)
                    (allTiles[i] as CSFolder).Sort();
            }
        }

        internal void SwapRootTiles(int i, int j)
        {
            var a = rootTiles[i];
            rootTiles[i] = rootTiles[j];
            rootTiles[j] = a;

            rootTiles[i].localIndex = i;
            rootTiles[j].localIndex = j;

            var p = rootTiles[i].Position;
            rootTiles[i].Position = rootTiles[j].Position;
            rootTiles[j].Position = p;
        }

        internal void SwapRootTilesNoPositions(int i, int j)
        {
            var a = rootTiles[i];
            rootTiles[i] = rootTiles[j];
            rootTiles[j] = a;

            rootTiles[i].localIndex = i;
            rootTiles[j].localIndex = j;
        }

        public void AddComponent(CSFolder parent, CSComponent component)
        {
            var copy = new CSComponentCopy(component);
            copy.parent = parent;
            if (parent == null)
            {
                copy.Position = new Vector2(4, 4 + rootTiles.Count * 40);
                copy.localIndex = rootTiles.Count;
                copy.isVisible = true;
                rootTiles.Add(copy);
            }
            else
            {
                copy.Position = new Vector2(6 + parent.position.X + 40, 4 + parent.Tiles.Count * 40);
                copy.localIndex = parent.Tiles.Count;
                copy.isVisible = false;
                parent.Tiles.Add(copy);
            }
            copy.Color = Color.White;
            allTiles.Add(copy);
        }

        public void RemoveComponent(CSFolder parent, CSComponent component)
        {
            if (parent == null)
            {
                for (int i = 0; i < rootTiles.Count; i++)
                {
                    if (rootTiles[i] is CSComponentCopy && (rootTiles[i] as CSComponentCopy).component == component)
                    {
                        if (rootTiles[i].position == selectedComponent.position)
                            ResetSelection();
                        allTiles.Remove(rootTiles[i]);
                        rootTiles.RemoveAt(i);
                        break;
                    }
                }
                for (int i = 0; i < rootTiles.Count; i++)
                {
                    rootTiles[i].localIndex = i;
                }
            }
            else
            {
                for (int i = 0; i < parent.Tiles.Count; i++)
                {
                    if (parent.Tiles[i] is CSComponentCopy && (parent.Tiles[i] as CSComponentCopy).component == component)
                    {
                        if (parent.Tiles[i].position == selectedComponent.position)
                            ResetSelection();
                        allTiles.Remove(parent.Tiles[i]);
                        parent.Tiles.RemoveAt(i);
                        break;
                    }
                }
                for (int i = 0; i < parent.Tiles.Count; i++)
                {
                    parent.Tiles[i].localIndex = i;
                }
            }
        }

        public override void Initialize()
        {
            IO.Log.Write("        CS initializing...");
            MicroWorld.Graphics.GUI.ClickabilityOverlay.RegisterExtension(Components.ClickableComponents.Instance);

            ClickabilityOverlay.RegisterExtension(this);

            ShouldBeScaled = false;
            Layer = 500;

            MicroWorld.Components.Cursor cs = new MicroWorld.Components.Cursor();
            RegisterComponent(cs);
            cursor = components[0];
            cursor.drawCount = false;

            MicroWorld.Components.RotatableConnector rc = new MicroWorld.Components.RotatableConnector();
            RegisterComponent(rc);

            MicroWorld.Components.Joint j = new MicroWorld.Components.Joint();
            RegisterComponent(j);
            MicroWorld.Components.Joint._TypeID = j.typeID;

            MicroWorld.Components.Wire w = new MicroWorld.Components.Wire();
            RegisterComponent(w);
            Components.Wire.TypeID = Components.ComponentsManager.TypeIDs[typeof(Components.Wire)];

            MicroWorld.Components.Core cr = new MicroWorld.Components.Core();
            RegisterComponent(cr);

            MicroWorld.Components.PulseCore cr2 = new MicroWorld.Components.PulseCore();
            RegisterComponent(cr2);

            //MicroWorld.Components.PipeJoint pj = new MicroWorld.Components.PipeJoint();
            //RegisterComponent(pj);

            //MicroWorld.Components.Pipe p = new MicroWorld.Components.Pipe();
            //RegisterComponent(p);

            for (int i = 0; i < MicroWorld.Components.ComponentsManager.RegisteredComponents.Count; i++)
            {
                Type t = MicroWorld.Components.ComponentsManager.RegisteredComponents[i];
                MicroWorld.Components.Component comp = (MicroWorld.Components.Component)Activator.CreateInstance(t);
                if (comp.Graphics.ShouldDisplayInCS())
                    RegisterComponent(comp);
                else
                    Components.ComponentsManager.TypeIDs.Add(comp.GetType(), (short)Components.ComponentsManager.TypeIDs.Count);

                try
                {
                    var a = comp.GetType().GetMethod("StaticInit");
                    if (a != null)
                        a.Invoke(null, null);
                }
                catch { };
            }

            GenerateStructure();
            selectedComponent = rootTiles[0] as CSComponentCopy;

            GlobalEvents.onComponentRemovedByPlayer += new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentRemovedByPlayer);
        }

        void GlobalEvents_onComponentRemovedByPlayer(Components.Component sender)
        {
            if (sender is Components.Joint) return;
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].objType == sender.GetType() && components[i].Avalable > -1)
                {
                    components[i].Avalable++;
                }
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();

            ComponentsLeftFont = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_7");
            ComponentBackground = ResourceManager.Load<Texture2D>("GUI/HUD/ComponentSelector/ItemBG");
            Selected = ResourceManager.Load<Texture2D>("GUI/HUD/ComponentSelector/SelectedHighlight");
            bg = ResourceManager.Load<Texture2D>("GUI/HUD/ComponentSelector/bg");
            dotOverlay = ResourceManager.Load<Texture2D>("GUI/HUD/ComponentSelector/dotOverlay");

            CSComponentCopy.fav = ResourceManager.Load<Texture2D>("GUI/HUD/ComponentSelector/Star");
        }

        public override void Update()
        {
            base.Update();
            for (int i = 0; i < rootTiles.Count; i++)
            {
                rootTiles[i].Update();
            }
        }

        float dotOverlayOpacity = 0;
        public override void Draw(Renderer renderer)
        {
            int dotOutIndex = -1;
            for (int i = 0; i < rootTiles.Count; i++)
            {
                if (rootTiles[i] is CSFolder && (rootTiles[i] as CSFolder).IsOpened)
                {
                    dotOutIndex = i;
                    break;
                }
            }
            if (dotOutIndex != -1)
            {
                dotOverlayOpacity += 0.1f;
                if (dotOverlayOpacity > 1)
                    dotOverlayOpacity = 1;
            }
            else
            {
                if (dotOverlayOpacity > 0)
                    dotOverlayOpacity -= 0.1f;
            }
            //bg
            RenderHelper.SmartDrawRectangle(bg, 8, 0, 0, 48, Main.WindowHeight, Color.White, renderer);

            if (dotOutIndex != -1 || dotOverlayOpacity > 0)
            {
                renderer.End();
                renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None,
                    RasterizerState.CullNone);
                renderer.Draw(dotOverlay,
                    new Rectangle(2, 2, 44, Main.WindowHeight - 4),
                    new Rectangle(2, 2, 44, Main.WindowHeight - 4),
                    Color.White * dotOverlayOpacity);
            }
            //components
            for (int i = 0; i < rootTiles.Count; i++)
            {
                rootTiles[i].Draw(renderer);
                if ((dotOutIndex != -1 || dotOverlayOpacity > 0) && dotOutIndex != i)
                {
                    renderer.Draw(dotOverlay,
                        new Rectangle((int)rootTiles[i].position.X, (int)rootTiles[i].position.Y, CSTile.SIZE_X, CSTile.SIZE_Y),
                        new Rectangle((int)rootTiles[i].position.X, (int)rootTiles[i].position.Y, CSTile.SIZE_X, CSTile.SIZE_Y), 
                        Color.White * dotOverlayOpacity);
                }
            }
            //selected overlay
            if (selectedComponent != null)
            {
                renderer.Draw(Selected, new Rectangle((int)selectedComponent.position.X, (int)selectedComponent.position.Y,
                    CSTile.SIZE_X, CSComponent.SIZE_Y), Color.White);
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
            for (int i = 0; i < rootTiles.Count; i++)
            {
                rootTiles[i].PostDraw();
            }
        }

        public override bool IsIn(int x, int y)
        {
            for (int i = 0; i < allTiles.Count; i++)
            {
                if (allTiles[i].IsIn(x, y))
                    return true;
            }
            return false;
        }



        #region IO
        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            if (isVisible)
                for (int i = 0; i < allTiles.Count; i++)
                    allTiles[i].onButtonClick(e);
        }

        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            if (isVisible)
                for (int i = 0; i < allTiles.Count; i++)
                    allTiles[i].onButtonDown(e);
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            if (isVisible)
                for (int i = 0; i < allTiles.Count; i++)
                    allTiles[i].onButtonUp(e);
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            if (isVisible)
                for (int i = 0; i < allTiles.Count; i++)
                    allTiles[i].onMouseMove(e);
        }
        #endregion

    }
}
