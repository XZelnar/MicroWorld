using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MicroWorld.Logics.LUA
{
    //TODO private
    public class LUALevelCommands
    {
        #region Components

        #region ILightEmitting
        [AttrLuaFunc("getComponentBrightness", "Returns max brightness of a component", "Component ID")]
        public double getComponentBrightness(int id)
        {
            var a = Components.ComponentsManager.GetComponent((int)id);
            if (a == null || !(a is Components.Properties.ILightEmitting)) return 0;
            if (Settings.GameState == Settings.GameStates.Stopped) return 0;
            return (a as Components.Properties.ILightEmitting).GetBrightness(a.Graphics.Center.X, a.Graphics.Center.Y);
        }
        #endregion

        [AttrLuaFunc("getComponentName", "Returns the name of specific element", "Component ID")]
        public String getComponentName(double id)
        {
            var a = Components.ComponentsManager.GetComponent((int)id);
            if (a == null) return "";
            return a.GetName();
        }

        [AttrLuaFunc("getComponentAvilability", "Returns ComponentSelector's element's avilability", "Name")]
        public int getComponentAvilability(String name)
        {
            return Graphics.GUI.GUIEngine.s_componentSelector.GetComponentAvilability(name);
        }

        [AttrLuaFunc("setComponentAvilability", "Sets ComponentSelector's element's avilability", "Name", "Count")]
        public void setComponentAvilability(String name, int count)
        {
            Graphics.GUI.GUIEngine.s_componentSelector.SetComponentAvilability(name, count);
        }

        [AttrLuaFunc("incComponentAvilability", "Increases or decreases ComponentSelector's element's avilability", "Name", "Count")]
        public void incComponentAvilability(String name, int count)
        {
            Graphics.GUI.GUIEngine.s_componentSelector.IncComponentAvilability(name, count);
        }

        [AttrLuaFunc("setAllComponentsUnavalable", "Sets all ComponentSelector's element's avilability to 0")]
        public void setAllComponentsUnavalable()
        {
            Graphics.GUI.GUIEngine.s_componentSelector.SetAllUnavalable();
        }

        [AttrLuaFunc("getComponentPositionSize", "Returns component position and size", "Component ID")]
        public double[] getComponentPositionSize(int id)
        {
            var a = Components.ComponentsManager.GetComponent(id);
            if (a == null) return new double[] { 0, 0, 0, 0 };
            var s = a.Graphics.GetSizeRotated(a.ComponentRotation);
            return new double[] { a.Graphics.Position.X, a.Graphics.Position.Y, s.X, s.Y };
        }

        [AttrLuaFunc("getComponentRotationCW", "Returns component rotation in degrees clockwise", "Component ID")]
        public int getComponentRotationCW(int id)
        {
            var a = Components.ComponentsManager.GetComponent(id);
            if (a == null) 
                return -1;
            switch (a.ComponentRotation)
            {
                case MicroWorld.Components.Component.Rotation.cw0:
                    return 0;
                case MicroWorld.Components.Component.Rotation.cw90:
                    return 90;
                case MicroWorld.Components.Component.Rotation.cw180:
                    return 180;
                case MicroWorld.Components.Component.Rotation.cw270:
                    return 270;
                default:
                    return -1;
            }
        }

        [AttrLuaFunc("getComponentScreenPositionSize", "Returns component position and size as seen on screen", "Component ID")]
        public double[] getComponentScreenPositionSize(int id)
        {
            var a = Components.ComponentsManager.GetComponent(id);
            if (a == null) return new double[] { 0, 0, 0, 0 };
            var s = a.Graphics.GetSizeRotated(a.ComponentRotation);
            double[] r = new double[] { a.Graphics.Position.X, a.Graphics.Position.Y, s.X, s.Y };
            Utilities.Tools.GameToScreenCoords(r);
            return r;
        }

        [AttrLuaFunc("getComponentType", "Returns Component type", "ID")]
        public String getComponentType(int id)
        {
            return Components.ComponentsManager.GetComponent(id).GetType().FullName;
        }

        [AttrLuaFunc("areJointsConnected", "Checks wether two joints are connected by a single wire", "ID1", "ID2")]
        public bool areJointsConnected(int id1, int id2)
        {
            var aa = Components.ComponentsManager.GetComponent(id1);
            var bb = Components.ComponentsManager.GetComponent(id2);
            if (!(aa is Components.Joint) || !(bb is Components.Joint)) return false;
            var a = aa as Components.Joint;
            var b = bb as Components.Joint;
            for (int i = 0; i < a.ConnectedWires.Count; i++)
            {
                if (a.ConnectedWires[i].J1.ID == a.ID && a.ConnectedWires[i].J2.ID == b.ID) return true;
                if (a.ConnectedWires[i].J2.ID == a.ID && a.ConnectedWires[i].J1.ID == b.ID) return true;
            }
            return false;
        }

        [AttrLuaFunc("removeComponent", "Removes a component", "ID")]
        public void removeComponent(int id)
        {
            Components.ComponentsManager.GetComponent(id).Remove();
        }

        [AttrLuaFunc("getComponentJoints", "Returns IDs of joints that belong to component", "ID")]
        public int[] getComponentJoints(int id)
        {
            return Components.ComponentsManager.GetComponent(id).getJoints();
        }

        [AttrLuaFunc("setComponentRemovability", "Sets wether component is removable or not", "ID", "Value")]
        public void setComponentRemovability(int id, bool value)
        {
            Components.ComponentsManager.GetComponent(id).IsRemovable = value;
        }

        [AttrLuaFunc("setMagnetRange", "Sets max force of a magnet", "ID", "Min", "Max")]
        public void setMagnetRange(int id, double min, double max)
        {
            var a = Components.ComponentsManager.GetComponent(id);
            if (a is Components.Properties.IMagnetic)
            {
                (a as Components.Properties.IMagnetic).SetRange(new Microsoft.Xna.Framework.Vector2((float)min, (float)max));
            }
        }

        [AttrLuaFunc("isCompoentnVisible", "Returns value that remresents component visibility", "ID")]
        public bool isCompoentnVisible(int id)
        {
            return Components.ComponentsManager.GetComponent(id).Graphics.Visible;
        }

        [AttrLuaFunc("areRotatableConnected", "Returns wether two rotatables or motora are connected or now", "ID1", "ID2")]
        public bool areRotatableConnected(int id1, int id2)
        {
            var c1 = Components.ComponentsManager.GetComponent(id1);
            var c2 = Components.ComponentsManager.GetComponent(id2);
            if (c1 == null || c2 == null) return false;
            Components.RotatableConnector a = null;
            if (c1 is Components.Properties.IRotatable)
            {
                a = (c1 as Components.Properties.IRotatable).GetConnector();
            }
            else if (c1 is Components.Properties.IRotator)
            {
                a = (c1 as Components.Properties.IRotator).GetConnector();
            }
            if (a != null && ((a.ConnectedComponent1 == c1 && a.ConnectedComponent2 == c2) ||
                              (a.ConnectedComponent1 == c2 && a.ConnectedComponent2 == c1)))
                return true;

            return false;
        }
        /*
        [AttrLuaFunc("getLiquidContainerVolume", "Returns volume of a liquid container", "ID")]
        public int getLiquidContainerVolume(int id)
        {
            var a = Components.ComponentsManager.GetComponent(id);
            if (a == null || !(a is Components.Properties.ILiquidContainer))
                return 0;
            return (a as Components.Properties.ILiquidContainer).Volume;
        }

        [AttrLuaFunc("getLiquidContainerLiquidVolume", "Returns liquid volume of a liquid container", "ID")]
        public int getLiquidContainerLiquidVolume(int id)
        {
            var a = Components.ComponentsManager.GetComponent(id);
            if (a == null || !(a is Components.Properties.ILiquidContainer))
                return 0;
            return (a as Components.Properties.ILiquidContainer).LiquidVolume;
        }

        [AttrLuaFunc("setLiquidContainerLiquidVolume", "Sets liquid volume of a liquid container", "ID", "Amount")]
        public void setLiquidContainerLiquidVolume(int id, int amount)
        {
            var a = Components.ComponentsManager.GetComponent(id);
            if (a == null || !(a is Components.Properties.ILiquidContainer))
                return;
            int v = (a as Components.Properties.ILiquidContainer).LiquidVolume;
            if (amount > v)
                (a as Components.Properties.ILiquidContainer).PourLiquid(amount - v, new Microsoft.Xna.Framework.Vector2());
            else
                (a as Components.Properties.ILiquidContainer).DecreaseLiquid(v - amount);
        }//*/
        #endregion
        //==========================================================================================================================================================
        #region GUI

        #region PlacableAreas
        [AttrLuaFunc("clearPlacableAreas", "Clears List of placable areas")]
        public void clearPlacableAreas()
        {
            PlacableAreasManager.Clear();
        }

        [AttrLuaFunc("addPlacableArea", "Adds a placable area", "X", "Y", "W", "H")]
        public void addPlacableArea(double x, double y, double w, double h)
        {
            PlacableAreasManager.Add(new Microsoft.Xna.Framework.Rectangle((int)x, (int)y, (int)w, (int)h));
        }

        [AttrLuaFunc("removePlacableArea", "Removes placable area", "ID")]
        public void removePlacableArea(int index)
        {
            PlacableAreasManager.Remove(index);
        }

        [AttrLuaFunc("getPlacableArea", "Returns a placable area", "Index")]
        public double[] getPlacableArea(int index)
        {
            if (index < 0 || index >= PlacableAreasManager.areas.Count) 
                return new double[] { 0, 0, 0, 0 };
            var a = PlacableAreasManager.areas[index];
            return new double[] { a.X, a.Y, a.Width, a.Height };
        }

        [AttrLuaFunc("getSeenPlacableArea", "Returns seen placable area", "Index")]
        public double[] getSeenPlacableArea(int index)
        {
            var r = getPlacableArea(index);
            Utilities.Tools.GameToScreenCoords(r);
            return r;
        }
        #endregion

        #region ClickableAreas
        [AttrLuaFunc("addClickableArea", "Adds clickable area", "X", "Y", "W", "H")]
        public void addClickableArea(int x, int y, int w, int h)
        {
            Graphics.GUI.ClickabilityOverlay.allowedAdd(new Microsoft.Xna.Framework.Rectangle(x, y, w, h), false);
        }

        //[AttrLuaFunc("addClickableScalableArea", "Adds clickable area that will be scaled with camera", "X", "Y", "W", "H")]
        //public void addClickableScalableArea(int x, int y, int w, int h)
        //{
        //    Graphics.GUI.ClickabilityOverlay.allowedAdd(new Microsoft.Xna.Framework.Rectangle(x, y, w, h), true);
        //}

        [AttrLuaFunc("addClickableGameArea", "Adds clickable area from game coords that will be scaled with camera", "X", "Y", "W", "H")]
        public void addClickableGameArea(int x, int y, int w, int h)
        {
            //w = (int)(w * Settings.GameScale);
            //h = (int)(h * Settings.GameScale);
            //Utilities.Tools.GameToScreenCoords(ref x, ref y);
            Graphics.GUI.ClickabilityOverlay.allowedAdd(new Microsoft.Xna.Framework.Rectangle(x, y, w, h), true);
        }

        [AttrLuaFunc("clearClickableAreas", "Clears clickable areas list")]
        public void clearClickableAreas()
        {
            Graphics.GUI.ClickabilityOverlay.allowedClear();
        }

        [AttrLuaFunc("removeClickableAreaAt", "Removes specified clickable area", "index")]
        public void removeClickableAreaAt(int index)
        {
            Graphics.GUI.ClickabilityOverlay.allowedRemoveAt(index);
        }

        [AttrLuaFunc("addClickableComponent", "Adds a clickable component", "ID")]
        public void addClickableComponent(int id)
        {
            Components.ClickableComponents.Instance.AddClickableComponent(id);
        }

        [AttrLuaFunc("removeClickableComponent", "Removes a clickable component", "ID")]
        public void removeClickableComponent(int id)
        {
            Components.ClickableComponents.Instance.RemoveClickableComponent(id);
        }

        [AttrLuaFunc("clearClickableComponents", "Clears clickable components list")]
        public void clearClickableComponents()
        {
            Components.ClickableComponents.Instance.ClearClickableAreas();
        }

        [AttrLuaFunc("addClickablePlacableArea", "Adds a clickable component", "ID")]
        public void addClickablePlacableArea(int id)
        {
            Logics.ClickablePlacableAreas.Instance.AddClickablePlacableArea(id);
        }

        [AttrLuaFunc("removeClickablePlacableArea", "Removes a clickable component", "ID")]
        public void removeClickablePlacableArea(int id)
        {
            Logics.ClickablePlacableAreas.Instance.RemoveClickablePlacableArea(id);
        }

        [AttrLuaFunc("clearClickablePlacableAreas", "Clears clickable components list")]
        public void clearClickablePlacableAreas()
        {
            Logics.ClickablePlacableAreas.Instance.ClearClickableAreas();
        }
        #endregion

        #region CS
        [AttrLuaFunc("getComponentSelectorComponentPosition", "Returns Component position", "Component name")]
        public double[] getComponentSelectorComponentPosition(String name)
        {
            return Graphics.GUI.GUIEngine.s_componentSelector.GetComponentPosition(name);
        }

        [AttrLuaFunc("csTileExists", "Returns whether a specific tile exists in ComponentSelector", "Path to a tile")]
        public bool csTileExists(String path)
        {
            return MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.TileExists(path) != null;
        }

        [AttrLuaFunc("csIsTileVisible", "Returns whether a specific tile is visible in ComponentSelector", "Path to a tile")]
        public bool csIsTileVisible(String path)
        {
            return MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.TileVisible(path);
        }

        [AttrLuaFunc("csGetTilePosition", "Returns position of a specific tile in ComponentSelector", "Path to a tile")]
        public double[] csGetTilePosition(String path)
        {
            var a = MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.TilePosition(path);
            return new double[] { a.X, a.Y };
        }

        [AttrLuaFunc("csClearClickableTiles", "Remover any clickable tiles")]
        public void csClearClickableTiles()
        {
            MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.ClearClickableTiles();
        }

        [AttrLuaFunc("csAddClickableTile", "Adds a clickable tile", "Tile path")]
        public void csAddClickableTile(String tile)
        {
            MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.AddClickableTile(tile);
        }
        #endregion

        #region Arrows
        [AttrLuaFunc("hud_ArrowShow", "Displays an arrow", "X", "Y", "Direction")]
        public int hud_ArrowShow(float x, float y, String dir)
        {
            dir = dir.ToLower();

            var a = new Graphics.Overlays.Arrow();
            a.Position = new Microsoft.Xna.Framework.Vector2(x, y);
            a.Initialize();

            if (dir == "right")
                a.Direction = Direction.Right;
            else if (dir == "rightdown" || dir == "downright")
                a.Direction = Direction.DownRight;
            else if (dir == "down")
                a.Direction = Direction.Down;
            else if (dir == "downleft" || dir == "leftdown")
                a.Direction = Direction.DownLeft;
            else if (dir == "left")
                a.Direction = Direction.Left;
            else if (dir == "leftup" || dir == "upleft")
                a.Direction = Direction.LeftUp;
            else if (dir == "up")
                a.Direction = Direction.Up;
            else if (dir == "upright" || dir == "rightup")
                a.Direction = Direction.RightUp;

            Graphics.OverlayManager.Add(a);
            return a.ID;
        }

        [AttrLuaFunc("hud_ArrowSetPos", "Changes arrow position", "ID", "X", "Y")]
        public void hud_ArrowSetPos(int id, float x, float y)
        {
            var a = Graphics.OverlayManager.GetByID(id);
            if (!(a is Graphics.Overlays.Arrow))
                return;
            a.Position = new Microsoft.Xna.Framework.Vector2(x, y);
        }

        [AttrLuaFunc("hud_ArrowSetSize", "Changes arrow size", "ID", "X", "Y")]
        public void hud_ArrowSetSize(int id, float x, float y)
        {
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            var a = Graphics.OverlayManager.GetByID(id);
            if (!(a is Graphics.Overlays.Arrow))
                return;
            a.Size = new Microsoft.Xna.Framework.Vector2(x, y);
        }

        [AttrLuaFunc("hud_ArrowSetDir", "Changes arrow direction", "ID", "Direction")]
        public void hud_ArrowSetPos(int id, String dir)
        {
            var b = Graphics.OverlayManager.GetByID(id);
            if (!(b is Graphics.Overlays.Arrow))
                return;
            var a = b as Graphics.Overlays.Arrow;

            if (dir == "right")
                a.Direction = Direction.Right;
            else if (dir == "rightdown" || dir == "downright")
                a.Direction = Direction.DownRight;
            else if (dir == "down")
                a.Direction = Direction.Down;
            else if (dir == "downleft" || dir == "leftdown")
                a.Direction = Direction.DownLeft;
            else if (dir == "left")
                a.Direction = Direction.Left;
            else if (dir == "leftup" || dir == "upleft")
                a.Direction = Direction.LeftUp;
            else if (dir == "up")
                a.Direction = Direction.Up;
            else if (dir == "upright" || dir == "rightup")
                a.Direction = Direction.RightUp;
        }

        [AttrLuaFunc("hud_ArrowSetColor", "Changes arrow color", "ID", "R", "G", "B")]
        public void hud_ArrowSetColor(int id, float r, float g, float b)
        {
            var c = Graphics.OverlayManager.GetByID(id);
            if (!(c is Graphics.Overlays.Arrow))
                return;
            var a = c as Graphics.Overlays.Arrow;

            a.color = new Microsoft.Xna.Framework.Color(r, g, b);
        }

        [AttrLuaFunc("hud_ArrowDisappear", "Makes arrow smoothly disappear", "ID")]
        public void hud_ArrowDisappear(int id)
        {
            var b = Graphics.OverlayManager.GetByID(id);
            if (!(b is Graphics.Overlays.Arrow))
                return;
            var a = b as Graphics.Overlays.Arrow;
            a.Disappear = true;
        }

        [AttrLuaFunc("hud_ArrowRemove", "Instantly destroys arrow", "ID")]
        public void hud_ArrowRemove(int id)
        {
            var b = Graphics.OverlayManager.GetByID(id);
            if (!(b is Graphics.Overlays.Arrow))
                return;
            Graphics.OverlayManager.Remove(b);
        }
        #endregion

        #region InfoPanel
        [AttrLuaFunc("hud_infoPanel_Create", "Creates and displays a new info panel", "X", "Y", "Width", "Height", "Text to display")]
        public int getSceneName(double x, double y, double w, double h, String text)
        {
            if (w < 40)
                w = 40;
            if (h < 40)
                h = 40;
            var a = new Graphics.GUI.Scene.ScriptedInfoPanel();
            a.Initialize();
            a.SetPosition(new Microsoft.Xna.Framework.Vector2((float)x, (float)y));
            a.SetSize(new Microsoft.Xna.Framework.Vector2((float)w, (float)h));
            a.Text = text;
            Graphics.GUI.GUIEngine.AddHUDScene(a);
            return a.ID;
        }

        private static Graphics.GUI.Scene.ScriptedInfoPanel getPanel(int id)
        {
            var a = Graphics.GUI.GUIEngine.GetAllHUDSceneType<Graphics.GUI.Scene.ScriptedInfoPanel>();
            for (int i = 0; i < a.Length; i++)
                if (a[i].ID == id)
                    return a[i];
            return null;
        }

        [AttrLuaFunc("hud_infoPanel_GetPosition", "Returns position of specific InfoPanel", "ID of panel")]
        public double[] hud_infoPanelGetPosition(int id)
        {
            var a = getPanel(id);
            if (a == null)
                return new double[] { -1, -1 };
            var b = a.GetPosition();
            return new double[] { b.X, b.Y };
        }

        [AttrLuaFunc("hud_infoPanel_GetSize", "Returns size of specific InfoPanel", "ID of panel")]
        public double[] hud_infoPanelGetSize(int id)
        {
            var a = getPanel(id);
            if (a == null)
                return new double[] { -1, -1 };
            var b = a.GetSize();
            return new double[] { b.X, b.Y };
        }

        [AttrLuaFunc("hud_infoPanel_GetText", "Returns text of specific InfoPanel", "ID of panel")]
        public String hud_infoPanelGetText(int id)
        {
            var a = getPanel(id);
            if (a == null)
                return "";
            return a.Text;
        }

        [AttrLuaFunc("hud_infoPanel_SetPosition", "Sets position of specific InfoPanel", "ID of panel", "X", "Y")]
        public void hud_infoPanelSetPosition(int id, double x, double y)
        {
            var a = getPanel(id);
            if (a == null)
                return;
            a.SetPosition(new Microsoft.Xna.Framework.Vector2((float)x, (float)y));
        }

        [AttrLuaFunc("hud_infoPanel_SetSize", "Sets size of specific InfoPanel", "ID of panel", "Width", "Height")]
        public void hud_infoPanelSetSize(int id, double x, double y)
        {
            var a = getPanel(id);
            if (a == null)
                return;
            a.SetSize(new Microsoft.Xna.Framework.Vector2((float)x, (float)y));
        }

        [AttrLuaFunc("hud_infoPanel_SetText", "Sets text of specific InfoPanel", "ID of panel", "Text")]
        public void hud_infoPanelSetText(int id, String txt)
        {
            var a = getPanel(id);
            if (a == null)
                return;
            a.Text = txt;
        }
        #endregion

        [AttrLuaFunc("getSceneName", "Returns the name of the type of specific GUI HUDScene", "ID of element")]
        public String getSceneName(double index)
        {
            return Graphics.GUI.GUIEngine.GetHUDSceneName((int)index);
        }

        [AttrLuaFunc("showTutorial", "Displays tutorial text", "Text to display", "Link to info page",
            "Time in ms tutorial should stay on screen")]
        public void showTutorial(String text, String link, int timeout)
        {
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_tutorial);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_tutorial);
            Graphics.GUI.GUIEngine.s_tutorial.SetText(text);
            Graphics.GUI.GUIEngine.s_tutorial.SetLink(link);
            Graphics.GUI.GUIEngine.s_tutorial.Timeout = timeout;
            Graphics.GUI.GUIEngine.s_tutorial.isVisible = true;
        }

        [AttrLuaFunc("setTutorialText", "Modifies tutorial text", "Text to display")]
        public void setTutorialText(String text)
        {
            Graphics.GUI.GUIEngine.s_tutorial.SetText(text);
        }

        [AttrLuaFunc("showDialog", "Displays dialog", "Character", "Text to display")]
        public void showDialog(String character, String text)
        {
            text = text.Replace("\n", "\r\n");
            Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_dialog);
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_dialog);
            Graphics.GUI.GUIEngine.s_dialog.SetText(character, text);
            Graphics.GUI.GUIEngine.s_tutorial.isVisible = true;
        }

        [AttrLuaFunc("showPurpose", "Displays Purpose screen")]
        public void showPurpose()
        {
            Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_purpose);
        }

        [AttrLuaFunc("isHUDSceneOpened", "Checks wether specified scene is in list of HUDScenes and is visible", "Name of scene")]
        public bool isHUDSceneOpened(String scene)
        {
            return Graphics.GUI.GUIEngine.ContainsHUDScene(scene);
        }

        [AttrLuaFunc("closeTutorial", "Forces tutorial scene to fade out")]
        public void closeTutorial()
        {
            Graphics.GUI.GUIEngine.s_tutorial.ForceClose();
        }

        [AttrLuaFunc("removeHUDScenes", "Removes HUD scenes of specific type", "Type")]
        public void removeHUDScenes(String type)
        {
            Graphics.GUI.GUIEngine.RemoveHUDScene(type);
        }

        [AttrLuaFunc("setHUDScenesVisibility", "Sets visibility of HUD scenes of specific type", "Type", "Visibility")]
        public void setHUDScenesVisibility(String type, bool visible)
        {
            Type t = null;
            try
            {
                t = Type.GetType(type);
            }
            catch (Exception e)
            {
                Shortcuts.ProcessException(e, "An error has occured while trying to get type from string \"" + type + "\"", "String \"" + type + "\" is not a type");
                return;
            }
            if (t == null)
                return;
            var a = Graphics.GUI.GUIEngine.GetAllHUDSceneType(t);
            for (int i = 0; i < a.Length; i++)
                a[i].isVisible = visible;
        }

        [AttrLuaFunc("yesNoMessageBox", "Shows a YesNoMessageBox, that will write its result to a variable", "MB text", "MB yes text", "MB no text", "Variable")]
        public void yesNoMessageBox(String text, String yes, String no, String var)
        {
            text = text.Replace("\n", "\r\n");
            var a = Graphics.GUI.Scene.YesNoMessageBox.Show(text);
            a.YesText = yes;
            a.NoText = no;
            a.LUAVarToSet = var;
        }

        [AttrLuaFunc("openMainMenu", "Changes scene to MainMenu")]
        public void openMainMenu()
        {
            Logics.GameLogicsHelper.GlobalEvents_onLevelExited();
            Main.curState = "GUIMainMenu";
            Graphics.GUI.GUIEngine.curScene = Graphics.GUI.GUIEngine.s_mainMenu;
        }

        [AttrLuaFunc("openTutorial", "Opens tutorial levels")]
        public void openTutorial()
        {
            Logics.GameLogicsHelper.GlobalEvents_onLevelExited();
            Graphics.GUI.GUIEngine.s_levelSelection.folder = "Levels/Tut/";
            Graphics.GUI.GUIEngine.s_levelSelection.InitForItemsCount(Graphics.GUI.Scene.LevelSelection.TABS_LEVELS_COUNT[0]);
            Graphics.GUI.GUIEngine.s_levelSelection.StartLevel(0);
        }
        #endregion
        //==========================================================================================================================================================
        #region Console
        [AttrLuaFunc("write", "Writes a given command", "String to write")]
        public void write(String strCmd)
        {
            OutputEngine.Write("[LUA]: " + strCmd);
        }

        [AttrLuaFunc("writeln", "Writes a given command and moves cursor to a new line", "String to write")]
        public void writeln(String strCmd)
        {
            OutputEngine.WriteLine("[LUA]: " + strCmd);
        }

        [AttrLuaFunc("executeConsoleCommands", "Executes a console command", "lines to execute")]
        public void executeConsoleCommands(String[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                Graphics.GUI.GUIEngine.s_console.DoCommand(lines[i], false);
            }
            Graphics.GUI.GUIEngine.s_console.DoCommand("..", false);
        }
        #endregion
        //==========================================================================================================================================================
        #region Math
        [AttrLuaFunc("sin", "Returns the sine of the specific angle", "Angle")]
        public double sin(double a)
        {
            return Math.Sin(a);
        }

        [AttrLuaFunc("cos", "Returns the cosine of the specific angle", "Angle")]
        public double cos(double a)
        {
            return Math.Cos(a);
        }

        [AttrLuaFunc("tg", "Returns the tangent of the specific angle", "Angle")]
        public double tg(double a)
        {
            return Math.Tan(a);
        }

        [AttrLuaFunc("ctg", "Returns the cotangent of the specific angle", "Angle")]
        public double ctg(double a)
        {
            return 1d / Math.Tan(a);
        }

        [AttrLuaFunc("bitnot", "~", "Value")]
        public double bitnot(double a)
        {
            try
            {
                uint aa = (uint)a;
                uint b = (~aa);
                return b;
            }
            catch { return -1; }
        }
        #endregion
        //==========================================================================================================================================================
        #region GeneralGame
        [AttrLuaFunc("getGameState", "Returns game's current state (Main.curState)")]
        public String getGameState()
        {
            return Main.curState;
        }

        [AttrLuaFunc("isLuminosityOverlay", "Returns wether luminosity overlay is active or not")]
        public bool isLuminosityOverlay()
        {
            return Graphics.GUI.GUIEngine.s_lightOverlay.IsActive;
        }

        [AttrLuaFunc("setLuminosityOverlay", "Sets wether luminosity overlay is active or not", "Value")]
        public void setLuminosityOverlay(bool value)
        {
            Graphics.GUI.GUIEngine.s_maphud.luminosity.Checked = value;//TODO encapsulate somewhere
        }

        [AttrLuaFunc("isMagneticOverlay", "Returns wether magnetic overlay is active or not")]
        public bool isMagneticOverlay()
        {
            return Graphics.GUI.GUIEngine.s_magneticOverlay.IsActive;
        }

        [AttrLuaFunc("setMagneticOverlay", "Sets wether magnetic overlay is active or not", "Value")]
        public void setMagneticOverlay(bool value)
        {
            Graphics.GUI.GUIEngine.s_maphud.magnetic.Checked = value;//TODO encapsulate somewhere
        }

        [AttrLuaFunc("setInputHandled", "DO NOT USE THIS UNLESS YOU KNOW WHAT YOU'RE DOING!!!\r\n" +
                                        "Ignores the rest of the InputEngine event")]
        public void setInputHandled()
        {
            InputEngine.eventHandled = true;
        }

        [AttrLuaFunc("setAllowedVisibleRectangle", "Sets visible rectangle", "X1", "Y1", "X2", "Y2")]
        public void setAllowedVisibleRectangle(double x1, double y1, double x2, double y2)
        {
            if (x1 > x2)
            {
                double t = x1;
                x1 = x2;
                x2 = t;
            }
            if (y1 > y2)
            {
                double t = y1;
                y1 = y2;
                y2 = t;
            }

            Graphics.GraphicsEngine.camera.AllowedVisibleRectangle = new Microsoft.Xna.Framework.Rectangle((int)x1, (int)y1,
                (int)(x2 - x1), (int)(y2 - y1));
        }

        [AttrLuaFunc("getSceneSize", "Returns scene size", "Scene name")]
        public double[] getSceneSize(String scene)
        {
            var a = Graphics.GUI.GUIEngine.GetHUDSceneSize(scene);
            return new double[] { a.X, a.Y };
        }

        [AttrLuaFunc("getSceneRectangle", "Returns scene rectangle", "Scene name")]
        public double[] getSceneRectangle(String scene)
        {
            return Graphics.GUI.GUIEngine.GetHUDSceneRectangle(scene);
        }

        [AttrLuaFunc("getGameRunningState", "Returns 0 if game is paused, 1 if it's running and 2 if it's stopped")]
        public int getGameRunningState()
        {
            return Settings.GameState.GetHashCode();
        }

        [AttrLuaFunc("gameStart", "Starts the simulation")]
        public void gameStart()
        {
            Logics.GameLogicsHelper.GameStart();
        }

        [AttrLuaFunc("gameStop", "Stops the simulation")]
        public void gameStop()
        {
            Logics.GameLogicsHelper.GameStop();
        }

        [AttrLuaFunc("gamePause", "Pauses/steps the simulation")]
        public void gamePause()
        {
            Logics.GameLogicsHelper.GamePause();
        }

        [AttrLuaFunc("clearSelection", "Deselects selected component")]
        public void clearSelection()
        {
            Graphics.GUI.GUIEngine.RemoveHUDScene("MicroWorld.Graphics.GUI.Scene.SubComponentButtons");
        }

        [AttrLuaFunc("setHistoryEnabled", "Sets wether history is enabled or not", "Value")]
        public void setHistoryEnabled(bool value)
        {
            Logics.ChangeHistory.IsHistoryEnabled = value;
        }

        [AttrLuaFunc("setCameraScale", "Sets camera scale", "Value")]
        public void setCameraScale(float value)
        {
            Settings.GameScale = value;
        }

        [AttrLuaFunc("getCameraScale", "Returns camera scale")]
        public double getCameraScale()
        {
            return Settings.GameScale;
        }

        [AttrLuaFunc("completeLevel", "Immitates level completion")]
        public void completeLevel()
        {
            MicroWorld.Logics.CampaingProgress.SetCurrentCompleted();
            MicroWorld.Graphics.GUI.GUIEngine.AddHUDScene(MicroWorld.Graphics.GUI.GUIEngine.s_victoryMessage);
        }

        [AttrLuaFunc("setCanStart", "Sets wether game is startable or not", "Value")]
        public void setCanStart(bool b)
        {
            MicroWorld.Logics.GameLogicsHelper.CanStart = b;
        }

        [AttrLuaFunc("setCanPause", "Sets wether game is pausable or not", "Value")]
        public void setCanPause(bool b)
        {
            MicroWorld.Logics.GameLogicsHelper.CanPause = b;
        }

        [AttrLuaFunc("setCanStep", "Sets wether simulation is steppable or not", "Value")]
        public void setCanStep(bool b)
        {
            MicroWorld.Logics.GameLogicsHelper.CanStep = b;
        }

        [AttrLuaFunc("setCanStop", "Sets wether game is stoppable or not", "Value")]
        public void setCanStop(bool b)
        {
            MicroWorld.Logics.GameLogicsHelper.CanStop = b;
        }

        [AttrLuaFunc("getCanStart", "Gets wether game is startable or not")]
        public bool getCanStart()
        {
            return MicroWorld.Logics.GameLogicsHelper.CanStart;
        }

        [AttrLuaFunc("getCanPause", "Gets wether game is pausable or not")]
        public bool getCanPause()
        {
            return MicroWorld.Logics.GameLogicsHelper.CanPause;
        }

        [AttrLuaFunc("getCanStep", "Gets wether simulation is steppable or not")]
        public bool getCanStep()
        {
            return MicroWorld.Logics.GameLogicsHelper.CanStep;
        }

        [AttrLuaFunc("getCanStop", "Gets wether game is stoppable or not")]
        public bool getCanStop()
        {
            return MicroWorld.Logics.GameLogicsHelper.CanStop;
        }
        #endregion
    }
}
