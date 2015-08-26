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

namespace MicroWorld.Logics
{
    public static unsafe class GameInputHandler//TODO private
    {
        private static Components.Component.Rotation ghostRotation = Components.Component.Rotation.cw0;
        internal static int[] ghostJointsPos = new int[0];
        public static Components.Component.Rotation GhostRotation
        {
            get { return GameInputHandler.ghostRotation; }
            internal set
            {
                if (ghostRotation != value)
                {
                    GameInputHandler.ghostRotation = value;
                    ghostJointsPos = Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponent.component.GetJointCoords(GhostRotation);
                }
            }
        }

        internal static bool canMove = false;
        internal static bool isDragDrop = false;
        internal static bool isPlacableAreaPending = false;
        internal static Components.Component DragDropComponent = null;
        internal static Vector2 DragDropStart = new Vector2(), DnDStartOffset = new Vector2();
        internal static Vector2 DragDropDelta = new Vector2();
        internal static bool isLine = false, isComponentDnD = false;
        internal static Vector2 pendingWireP1, pendingWireP2;
        internal static List<Point> pendingWirePath = new List<Point>();
        static Components.Joint j1;
        internal static Components.Properties.IDragDropPlacable DnDComponent;
        internal static Components.Component MouseOverComponent = null;
        internal static Components.Component[] MouseOverComponents = new Components.Component[0];
        internal static Vector2? LastLeftClick = null;
        internal static bool IgnoreNextClick = false;

        internal static bool isResizing = false;
        internal static Direction resizeType = Direction.None;
        internal static Vector2 resizeLastPoint = new Vector2();
        internal static Components.Properties.IResizable resizeComponent = null;

        public static void onButtonDown(InputEngine.MouseArgs e)
        {
            //create placable area
            if (Graphics.GUI.Scene.PlacableAreasCreator.create)
            {
                int x1 = e.curState.X;
                int y1 = e.curState.Y;
                GridHelper.GridCoords(ref x1, ref y1);
                pendingWireP1 = new Vector2(x1, y1);
                pendingWireP2 = new Vector2(pendingWireP1.X, pendingWireP1.Y);
                isPlacableAreaPending = true;
                return;
            }
            //delete placable area
            if (Graphics.GUI.Scene.PlacableAreasCreator.delete)
            {
                if (PlacableAreasManager.Remove(e.curState.X, e.curState.Y) && !InputEngine.Shift)
                    Graphics.GUI.Scene.PlacableAreasCreator.delete = false;
            }
            //Resize
            if (e.button == 0 && MouseOverComponent != null && MouseOverComponent is Components.Properties.IResizable)
            {
                int x1 = e.curState.X;
                int y1 = e.curState.Y;
                GridHelper.GridCoords(ref x1, ref y1);
                var a = (MouseOverComponent as Components.Properties.IResizable).CanResize(new Vector2(x1, y1));
                if (a != Direction.None)
                {
                    resizeLastPoint = new Vector2(InputEngine.curMouse.X, InputEngine.curMouse.Y);
                    resizeType = a;
                    resizeComponent = MouseOverComponent as Components.Properties.IResizable;
                    (MouseOverComponent as Components.Properties.IResizable).OnResizeStart(a, new Vector2(x1, y1));
                    return;
                }
            }
            //Cancel resize
            if (e.button == 1 && resizeComponent != null)
            {
                resizeComponent.CancelResize();
                resizeComponent = null;
                resizeType = Direction.None;
                return;
            }
            //DnD component 
            if (e.button == 0 &&
                Graphics.GUI.GUIEngine.ContainsHUDScene(Graphics.GUI.GUIEngine.s_subComponentButtons) &&
                Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent.isIn(e.curState.X, e.curState.Y) &&
                Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent.CanDragDrop() &&
                Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent.IsMovable())
            {
                if (Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent is Components.Joint)
                {
                    if (!InputEngine.Control)
                    {
                        goto DnDPass;
                    }
                }
                //if (Settings.GameState == Settings.GameStates.Stopped)
                {
                    isDragDrop = true;
                    canMove = true;
                    DragDropComponent = Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent;
                    DragDropStart = DragDropComponent.Graphics.Position;
                    DnDStartOffset = new Vector2(e.curState.X, e.curState.Y) - DragDropStart;
                    DragDropDelta = new Vector2();
                }
                //else
                //{
                //    Graphics.OverlayManager.HighlightStop();
                //}
                return;
            DnDPass: ;
            }
            //cancel DnD
            if (isDragDrop && e.button == 1)
            {
                isDragDrop = false;
                DragDropComponent = null;
            }
            if (isComponentDnD && e.button == 1)
            {
                isComponentDnD = false;
                DnDComponent = null;
            }
            //shift-placing wires
            if (e.button == 0 && InputEngine.Shift &&
                Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent != null &&
                Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent is Components.Joint)
            {
                int x1 = e.curState.X;
                int y1 = e.curState.Y;
                GridHelper.GridCoords(ref x1, ref y1);
                var a = HasJointAtCoord(x1, y1);
                if (a != null)
                {
                    bool b = false;
                    foreach (var wr in (Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent as Components.Joint).ConnectedWires)
                    {
                        if ((wr.J1.ID == Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent.ID && wr.J2.ID == a.ID) ||
                            (wr.J2.ID == Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent.ID && wr.J1.ID == a.ID))
                        {
                            b = true;
                            break;
                        }
                    }
                    if (!b)
                    {
                        Components.Wire w = new Components.Wire(Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent as Components.Joint, a);
                        w.Initialize();
                        Graphics.GUI.GUIEngine.s_componentSelector.DecreaseComponentAvilability("Wire");
                        Components.ComponentsManager.Add(w);
                        w.OnPlaced();
                        IgnoreNextClick = true;

                        if (Settings.GameState != Settings.GameStates.Stopped)
                        {
                            CircuitManager.ReCreate();
                        }

                        return;
                    }
                }
            }
            //placing wire
            if (Graphics.GUI.GUIEngine.s_componentSelector.GetComponent("Wire").Enabled && e.button == 0)
            {
                //if (Settings.GameState != Settings.GameStates.Stopped)
                //{
                    //Graphics.OverlayManager.HighlightStop();
                //    return;
                //}
                if (Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponent.Text == "Wire")
                {
                    isLine = true;
                }
                else
                {
                    goto PostWire;
                }
                int x1 = e.curState.X;
                int y1 = e.curState.Y;
                GridHelper.GridCoords(ref x1, ref y1);
                if (isLine)
                {
                    pendingWirePath.Clear();
                    if (PlacableAreasManager.IsPlacable(x1, y1))
                    {
                        if (HasJointAtCoord(x1, y1) == null &&
                            Components.ComponentsManager.VisibilityMap.GetAStarValue(x1, y1) == 0)//smth in the way
                            return;
                        //isLine = true;
                        pendingWirePath.Clear();
                        pendingWireP1 = new Vector2(x1, y1);
                        pendingWireP2 = new Vector2(pendingWireP1.X, pendingWireP1.Y);
                        Components.Joint jj;
                        Components.ComponentsManager.IgnoreAS1 = null;
                        Components.ComponentsManager.IgnoreAS2 = null;
                        if ((jj = HasJointAtCoord(x1, y1)) != null)
                        {
                            Components.ComponentsManager.IgnoreAS1 = jj;
                        }
                    }
                    else
                    {
                        isLine = false;
                    }
                }
                return;
            PostWire: ;
                //j1 = GetJointForCoord(e);
            }
            //canceling wire
            if (isLine && e.button == 1)
            {
                isLine = false;
            }
            //DnD-placing component
            if (Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponent.IsDragDropPlacement && 
                !isLine && !isComponentDnD)
            {
                //if (Settings.GameState != Settings.GameStates.Stopped)
                //{
                //    Graphics.OverlayManager.HighlightStop();
                //    return;
                //}
                DnDComponent = Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponent.component.GetNewInstance()
                    as Components.Properties.IDragDropPlacable;
                if (DnDComponent == null) return;
                int x1 = e.curState.X;
                int y1 = e.curState.Y;
                GridHelper.GridCoords(ref x1, ref y1);
                if (DnDComponent.CanStart(x1, y1))
                {
                    pendingWireP1 = new Vector2(x1, y1);
                    pendingWireP2 = new Vector2(pendingWireP1.X, pendingWireP1.Y);
                    isComponentDnD = true;
                }
                else
                {
                    DnDComponent = null;
                    isComponentDnD = false;
                }
                return;
            }
            //dragging wire from joint
            Components.Joint j;
            if (!InputEngine.Control && (j = HasJointAtCoord(e.curState.X, e.curState.Y)) != null &&
                Graphics.GUI.GUIEngine.s_componentSelector.GetComponent("Wire").Enabled)
            {
                //if (Settings.GameState != Settings.GameStates.Stopped)
                //{
                //    Graphics.OverlayManager.HighlightStop();
                //    return;
                //}
                int x1 = e.curState.X;
                int y1 = e.curState.Y;
                GridHelper.GridCoords(ref x1, ref y1);
                if (PlacableAreasManager.IsPlacable(x1, y1))
                {
                    isLine = true;
                    pendingWirePath.Clear();
                    pendingWireP1 = new Vector2(x1, y1);
                    pendingWireP2 = new Vector2(pendingWireP1.X, pendingWireP1.Y);
                    Components.ComponentsManager.IgnoreAS1 = null;
                    Components.ComponentsManager.IgnoreAS2 = null;
                }
                return;
            }
            //DnD w/o selection
            if (e.button == 0)
            {
                LastLeftClick = new Vector2(e.curState.X, e.curState.Y);
            }
            else
            {
                LastLeftClick = null;
            }
        }

        public static void onButtonUp(InputEngine.MouseArgs e)
        {
            if (Graphics.GUI.Scene.PlacableAreasCreator.create)
            {
                Graphics.GUI.Scene.PlacableAreasCreator.create = false;
                int x2 = e.curState.X;
                int y2 = e.curState.Y;
                GridHelper.GridCoords(ref x2, ref y2);
                isPlacableAreaPending = false;
                PlacableAreasManager.Add(new Rectangle((int)pendingWireP1.X, (int)pendingWireP1.Y,
                    (int)(x2 - pendingWireP1.X), (int)(y2 - pendingWireP1.Y)));
                return;
            }
            if (resizeComponent != null)
            {
                resizeComponent.OnResizeFinished(resizeType);
                resizeComponent = null;
                resizeType = Direction.None;
                return;
            }
            if (isDragDrop)
            {
                isDragDrop = false;
                DragDropComponent = null;
                return;
            }
            if (isLine)
            {
                int x2 = e.curState.X;
                int y2 = e.curState.Y;
                PlacableAreasManager.MakePlacable(ref x2, ref y2);
                GridHelper.GridCoords(ref x2, ref y2);

                if (pendingWireP1.X == x2 && pendingWireP1.Y == y2)
                {
                    isLine = false;
                    pendingWireP1 = new Vector2();
                    return;
                }

                Components.Joint tj1 = HasJointAtCoord((int)pendingWireP1.X, (int)pendingWireP1.Y),
                                 tj2 = HasJointAtCoordForWire(x2, y2);

                var a1 = Components.ComponentsManager.VisibilityMap.GetAStarValue((int)pendingWireP1.X + 2, (int)pendingWireP1.Y + 2);
                var a2 = tj2 == null ? Components.ComponentsManager.VisibilityMap.GetAStarValue((int)x2 + 2, (int)y2 + 2) : 0;

                if (tj2 == null)
                {
                    tj2 = GetJointForCoordIfCan(x2, y2);
                    if (tj2 == null)
                    {
                        isLine = false;
                        pendingWireP1 = new Vector2();
                        pendingWireP2 = new Vector2(pendingWireP1.X, pendingWireP1.Y);
                        return;
                    }
                }
                if ((pendingWireP1.X != tj2.Graphics.Position.X || pendingWireP1.Y != tj2.Graphics.Position.Y) && 
                    !Components.Graphics.WireGraphics.CanFindPath(tj1, tj2,
                        (int)pendingWireP1.X + 4, (int)pendingWireP1.Y + 4, (int)pendingWireP2.X + 4, (int)pendingWireP2.Y + 4))
                {
                    isLine = false;
                    pendingWireP1 = new Vector2();
                    pendingWireP2 = new Vector2(pendingWireP1.X, pendingWireP1.Y);
                    return;
                }

                if (pendingWireP1.X != tj2.Graphics.Position.X || pendingWireP1.Y != tj2.Graphics.Position.Y)
                {
                    MicroWorld.Logics.ChangeHistory.Push();

                    Statistics.ElementsPlaced++;
                    j1 = GetJointForCoord(pendingWireP1);
                    Components.Joint j2 = GetJointForCoord((int)tj2.Graphics.Position.X, (int)tj2.Graphics.Position.Y);
                    //w
                    Components.Wire w = new Components.Wire(j1, j2);
                    w.Initialize();
                    Graphics.GUI.GUIEngine.s_componentSelector.DecreaseComponentAvilability("Wire");
                    Components.ComponentsManager.Add(w);
                    w.OnPlaced();

                    if (a1 == Components.VisibilityMap.WIRE)
                    {
                        var a = GetWiresForCoord((int)pendingWireP1.X, (int)pendingWireP1.Y);
                        for (int i = 0; i < a.Count; i++)
                        {
                            if (a[i] != w && a[i].J1 != j1 && a[i].J2 != j1)
                                SplitWire(a[i], j1);
                        }
                    }

                    if (a2 == Components.VisibilityMap.WIRE)
                    {
                        var a = GetWiresForCoord((int)tj2.Graphics.Position.X, (int)tj2.Graphics.Position.Y);
                        for (int i = 0; i < a.Count; i++)
                        {
                            if (a[i] != w && a[i].J1 != j2 && a[i].J2 != j2)
                                SplitWire(a[i], j2);
                        }
                    }

                    CircuitManager.ReCreate();
                }
                isLine = false;
                return;
            }
            if (isComponentDnD)
            {
                int x2 = e.curState.X;
                int y2 = e.curState.Y;
                PlacableAreasManager.MakePlacable(ref x2, ref y2);
                GridHelper.GridCoords(ref x2, ref y2);
                if (pendingWireP1.X == x2 && pendingWireP1.Y == y2)
                {
                    isComponentDnD = false;
                    pendingWireP1 = new Vector2();
                    pendingWireP2 = new Vector2(pendingWireP1.X, pendingWireP1.Y);
                    DnDComponent = null;
                    return;
                }

                if (!DnDComponent.CanEnd((int)pendingWireP1.X, (int)pendingWireP1.Y, x2, y2))
                {
                    isComponentDnD = false;
                    pendingWireP1 = new Vector2();
                    pendingWireP2 = new Vector2(pendingWireP1.X, pendingWireP1.Y);
                    DnDComponent = null;
                    return;
                }

                MicroWorld.Logics.ChangeHistory.Push();

                DnDComponent.Place((int)pendingWireP1.X, (int)pendingWireP1.Y, x2, y2);
                MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.DecreaseComponentAvilability((DnDComponent as Components.Component).Graphics.GetCSToolTip());

                CircuitManager.ReCreate();

                isComponentDnD = false;
                pendingWireP1 = new Vector2();
                pendingWireP2 = new Vector2(pendingWireP1.X, pendingWireP1.Y);
                DnDComponent = null;
                return;
            }
            if (e.button == 0 && InputEngine.LeftMouseButtonDownPos != null &&
                MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponent.IsIn(
                (int)InputEngine.LeftMouseButtonDownPos.Value.X, (int)InputEngine.LeftMouseButtonDownPos.Value.Y))
            {
                TryPlaceComponent(ref e);
            }
        }

        public static void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            //int x = e.curState.X, y = e.curState.Y;
            //Utilities.Tools.ScreenToGameCoords(ref x, ref y);
            if (LastLeftClick != null && InputEngine.curMouse.LeftButton == ButtonState.Pressed && DnDComponent == null)
            {
                if (MouseOverComponent == null || MouseOverComponent is Components.Joint || MouseOverComponent is Components.Wire ||
                    !MouseOverComponent.CanDragDrop() || !MouseOverComponent.IsMovable() || Graphics.GUI.GUIEngine.IsIn(InputEngine.curMouse.X, InputEngine.curMouse.Y))
                {
                    LastLeftClick = null;
                }
                else
                {
                    if (DragDropComponent == null && LastLeftClick != null &&
                        (new Vector2(e.curState.X, e.curState.Y) - LastLeftClick).Value.Length() > Graphics.GUI.GridDraw.Step)
                    {
                        canMove = true;
                        isDragDrop = true;
                        DragDropComponent = MouseOverComponent;
                        DragDropDelta = new Vector2(e.curState.X, e.curState.Y) - LastLeftClick.Value;
                        DragDropDelta -= new Vector2(e.dx, e.dy);
                        DragDropStart = DragDropComponent.Graphics.Position;
                        DnDStartOffset = new Vector2(e.curState.X, e.curState.Y) - DragDropStart;
                    }
                }
            }
            if (resizeComponent != null)
            {
                resizeComponent.Resize(resizeType, (float)(InputEngine.curMouse.X - resizeLastPoint.X) / Settings.GameScale, (float)(InputEngine.curMouse.Y -                           resizeLastPoint.Y) / Settings.GameScale);
                resizeLastPoint = new Vector2(InputEngine.curMouse.X, InputEngine.curMouse.Y);
            }

            MouseOverComponents = Components.ComponentsManager.GetVisibleComponents(e.curState.X, e.curState.Y);
            MouseOverComponent = Components.ComponentsManager.GetTopVisibleComponent(MouseOverComponents);

            if (isDragDrop)
            {
                //if (Settings.GameState != Settings.GameStates.Stopped)
                //{
                //    isDragDrop = false;
                //    return;
                //}
                int tcx = (int)(e.curState.X - DnDStartOffset.X), tcy = (int)(e.curState.Y - DnDStartOffset.Y);
                GridHelper.GridCoords(ref tcx, ref tcy);
                DragDropDelta = new Vector2(tcx, tcy) - DragDropComponent.Graphics.Position;
                Vector2 t = new Vector2(DragDropDelta.X, DragDropDelta.Y);
                DragDropDelta -= t;
                if (t.X != 0 || t.Y != 0)
                {
                    if (PlacableAreasManager.IsPlacable(DragDropComponent.Graphics.Position.X + t.X,
                        DragDropComponent.Graphics.Position.Y + t.Y,
                        DragDropComponent.Graphics.Size.X,
                        DragDropComponent.Graphics.Size.Y) &&
                        (CanMove(DragDropComponent, DragDropComponent.Graphics.Position + t) ||
                        (DragDropComponent != null && DragDropComponent is Components.Joint && 
                        HasJointAtCoord((int)(DragDropComponent.Graphics.Position.X + t.X), (int)(DragDropComponent.Graphics.Position.Y + t.Y)) != null)))
                    {
                        canMove = true;
                        DragDropComponent.OnMove((int)t.X, (int)t.Y);
                    }
                    else
                    {
                        canMove = false;
                    }
                }
                return;
            }
            if (isLine)
            {
                int x = e.curState.X,
                    y = e.curState.Y;
                GridHelper.GridCoords(ref x, ref y);
                Components.Joint j;
                if ((j = HasJointAtCoordForWire(x, y)) != null)
                {
                    pendingWireP2 = new Vector2(j.Graphics.Position.X, j.Graphics.Position.Y);
                    Components.ComponentsManager.IgnoreAS2 = j;
                }
                else
                {
                    pendingWireP2 = new Vector2(x, y);
                    Components.ComponentsManager.IgnoreAS2 = null;
                }
                if (Components.ComponentsManager.VisibilityMap.GetAStarValue((int)pendingWireP2.X, (int)pendingWireP2.Y) != 0)
                {
                    if (pathThread != null)
                        pathThread.Abort();
                    pathThread =
                        new System.Threading.Thread(new System.Threading.ThreadStart(UpdatePendingWirePath));
                    pathThread.IsBackground = true;
                    pathThread.Start();
                }
            }
        }

        internal static void Initialize()
        {
            Graphics.GUI.Scene.ComponentSelector.ComponentSelector.onSelectionChanged +=
                new Graphics.GUI.Scene.ComponentSelector.ComponentSelector.SelectionHandler(ComponentSelector_onSelectionChanged);
        }

        static void ComponentSelector_onSelectionChanged(Graphics.GUI.Scene.ComponentSelector.CSComponentCopy current,
            Graphics.GUI.Scene.ComponentSelector.CSComponentCopy last)
        {
            ghostJointsPos = current.component.GetJointCoords(GhostRotation);
        }

        internal static System.Threading.Thread pathThread;
        internal static void UpdatePendingWirePath()
        {
            //if (InputEngine.curKeyboard.IsKeyDown(Keys.A))
            //{
            //}
            List<Point> t = new PathFinding.PathFinder().FindPath(new Point((int)pendingWireP1.X + 4, (int)pendingWireP1.Y + 4),
                                                                   new Point((int)pendingWireP2.X + 4, (int)pendingWireP2.Y + 4));
            //List<Point> t = new PathFinding.PathFinderJPS().findPath(new Point((int)pendingWireP1.X, (int)pendingWireP1.Y),
            //                                                       new Point((int)pendingWireP2.X, (int)pendingWireP2.Y))._waypoints;
            lock (pendingWirePath)
            {
                pendingWirePath.Clear();
                if (t != null)
                    pendingWirePath = t;
            }
            pathThread = null;

        }

        public static bool CanMove(Components.Component c, Vector2 pos)
        {
            if (c == null) return false;
            var s = c.Graphics.GetSizeRotated(c.ComponentRotation);
            Rectangle r1 = new Rectangle((int)pos.X, (int)pos.Y, (int)s.X, (int)s.Y);
            r1.Inflate(8, 8);
            Rectangle r2 = new Rectangle();
            var j = c.getJoints();
            if (j == null) return false;
            List<int> jcw = new List<int>();//joints connected wires
            for (int i = 0; i < j.Length; i++)
            {
                var a = Components.ComponentsManager.GetComponent(j[i]) as Components.Joint;
                if (a != null)
                {
                    for (int jj = 0; jj < a.ConnectedWires.Count; jj++)
                    {
                        jcw.Add(a.ConnectedWires[jj].ID);
                    }
                }
            }
            bool b;
            foreach (var c2 in Components.ComponentsManager.Components)
            {
                if (c2 == c || c2 == null || c2 is Components.EmptyComponent) 
                    continue;
                if (!c2.Graphics.Visible) 
                    continue;
                for (int i = 0; i < j.Length; i++)
                    if (c2.ID == j[i]) 
                        goto EndCheck;
                for (int i = 0; i < jcw.Count; i++)
                    if (c2.ID == jcw[i]) 
                        goto EndCheck;

                if (c2 is Components.Wire)
                    if ((c2.Graphics as Components.Graphics.WireGraphics).Intersects(r1))
                        return false;

                s = c2.Graphics.GetSizeRotated(c.ComponentRotation);
                r2 = new Rectangle((int)c2.Graphics.Position.X, (int)c2.Graphics.Position.Y, (int)s.X, (int)s.Y);
                b = r2.Contains(r1);
                if (b && !(c2 is Components.Properties.IContainer)) 
                    return false;
                if (!b && r1.Intersects(r2))
                    return false;
                if (r1.Contains(r2))
                    return false;

                EndCheck: ;
            }
            return true;
        }

        public static void SplitWire(Components.Wire w, Components.Joint j)
        {
            if (!w.IsRemovable) return;
            bool hasPoint = false;
            int point = -1;
            var g = w.Graphics as Components.Graphics.WireGraphics;
            if (GridHelper.ArePointsInSameCell(g.DrawPath[0], j.Graphics.Position))
            {
                hasPoint = true;
                point = 0;
            }
            for (int i = 0; i < g.DrawPath.Count - 1 && !hasPoint; i++)
            {
                if (GridHelper.ArePointsInSameCell(g.DrawPath[i + 1], j.Graphics.Position))
                {
                    g.DrawPath.Insert(i + 1, j.Graphics.Center);
                    hasPoint = true;
                    point = i + 2;//dunno why
                    break;
                }
                if (GridHelper.IsPointInLineSegment(g.DrawPath[i], g.DrawPath[i + 1], j.Graphics.Position))
                {
                    g.DrawPath.Insert(i + 1, j.Graphics.Center);
                    g.DrawPath.Insert(i + 1, j.Graphics.Center);
                    hasPoint = true;
                    point = i + 2;
                    break;
                }
            }


            if (w == null || j == null) return;
            var w2 = new Components.Wire(w.J2, j);
            w.J2.ConnectedWires.Remove(w);
            w.J2 = j;
            j.ConnectedWires.Add(w);
            w2.Initialize();
            if (!hasPoint)
            {
                w.Graphics.Initialize();
            }
            else
            {
                var a = g.DrawPath.GetRange(point, g.DrawPath.Count - point);
                g.DrawPath.RemoveRange(point, a.Count);
                g.DrawPath.Reverse();
                (w2.Graphics as Components.Graphics.WireGraphics).DrawPath = g.DrawPath;
                g.DrawPath = a;
                g.GenerateElectrons();
                (w2.Graphics as Components.Graphics.WireGraphics).GenerateElectrons();
            }
            w2.AddComponentToManager();
        }

        public static void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
        }

        public static void ResetComponentRotation()
        {
            var a = Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponentGraphics.GetPossibleRotations();
            if (a == null || a.Length == 0)
            {
                GhostRotation = Components.Component.Rotation.cw0;
                return;
            }
            GhostRotation = a[0];
        }

        public static Components.Joint HasJointAtCoordForWire(int x, int y)
        {
            var c11 = HasJointAtCoord(x, y);
            if (c11 != null) return c11;
            var c00 = HasJointAtCoord(x - 8, y - 8);
            var c10 = HasJointAtCoord(x, y - 8);
            var c20 = HasJointAtCoord(x + 8, y - 8);
            var c01 = HasJointAtCoord(x - 8, y);
            var c21 = HasJointAtCoord(x + 8, y);
            var c02 = HasJointAtCoord(x - 8, y + 8);
            var c12 = HasJointAtCoord(x, y + 8);
            var c22 = HasJointAtCoord(x + 8, y + 8);
            int c = 0;
            if (c00 != null) c++;
            if (c10 != null) c++;
            if (c20 != null) c++;
            if (c01 != null) c++;
            if (c21 != null) c++;
            if (c02 != null) c++;
            if (c12 != null) c++;
            if (c22 != null) c++;
            if (c == 1)
            {
                return c00 != null ? c00 :
                       c10 != null ? c10 :
                       c20 != null ? c20 :
                       c01 != null ? c01 :
                       c21 != null ? c21 :
                       c02 != null ? c02 :
                       c12 != null ? c12 :
                       c22 != null ? c22 : null;
            }
            return null;
        }

        public static Components.Joint HasJointAtCoord(int x, int y)
        {
            if (x % 8 == 0)
                x += 2;
            if (y % 8 == 0)
                y += 2;
            for (int i = 0; i < Components.ComponentsManager.Components.Count; i++)
            {
                if (Components.ComponentsManager.Components[i] is Components.Joint && 
                    Components.ComponentsManager.Components[i].Graphics.Visible &&
                    Components.ComponentsManager.Components[i].isIn(x, y))
                {
                    return (Components.Joint)Components.ComponentsManager.Components[i];
                }
            }
            return null;
        }

        public static Components.Joint GetJointForCoord(InputEngine.MouseArgs e)
        {
            return GetJointForCoord(e.curState.X, e.curState.Y);
        }

        public static Components.Joint GetJointForCoord(Vector2 e)
        {
            return GetJointForCoord((int)e.X, (int)e.Y);
        }

        public static Components.Joint GetJointForCoord(int x, int y, bool init = true)
        {
            x += 2;
            y += 2;
            for (int i = 0; i < Components.ComponentsManager.Components.Count; i++)
            {
                if (Components.ComponentsManager.Components[i].isIn(x, y))
                {
                    InputEngine.eventHandled = true;
                    if (Components.ComponentsManager.Components[i] is Components.Joint)
                    {
                        return (Components.Joint)Components.ComponentsManager.Components[i];
                    }
                }
            }
            Components.Joint c = Components.Joint.GetJointNoCheck(x - 2, y - 2);
            if (init)
                c.Initialize();
            Components.ComponentsManager.Components.Add(c);
            return c;
        }

        private static Components.Joint GetJointForCoordIfCan(int x, int y)
        {
            GridHelper.GridCoords(ref x, ref y);
            x += 4;
            y += 4;
            for (int i = 0; i < Components.ComponentsManager.Components.Count; i++)
            {
                if (Components.ComponentsManager.Components[i].isIn(x, y))
                {
                    InputEngine.eventHandled = true;
                    if (Components.ComponentsManager.Components[i] is Components.Joint)
                    {
                        return (Components.Joint)Components.ComponentsManager.Components[i];
                    }
                }
            }
            x -= 4;
            y -= 4;
            if (Components.ComponentsManager.VisibilityMap.GetAStarValue(x, y) == 0) return null;
            Components.Joint c = Components.Joint.GetJointNoCheck(x, y);
            c.Initialize();
            Components.ComponentsManager.Components.Add(c);
            return c;
        }

        public static List<Components.Wire> GetWiresForCoord(int x, int y)
        {
            List<Components.Wire> r = new List<Components.Wire>();
            GridHelper.GridCoords(ref x, ref y);
            for (int i = 0; i < Components.ComponentsManager.Components.Count; i++)
            {
                if (Components.ComponentsManager.Components[i].isIn(x, y))
                {
                    if (Components.ComponentsManager.Components[i] is Components.Wire)
                    {
                        r.Add((Components.Wire)Components.ComponentsManager.Components[i]);
                    }
                }
            }
            return r;
        }

        public static void onButtonClick(InputEngine.MouseArgs e)
        {
            if (IgnoreNextClick)
            {
                IgnoreNextClick = false;
                return;
            }
            if (TryPlaceComponent(ref e))
                return;
            //if (isLine || isDragDrop) return;
            ////add component
            //if (!InputEngine.eventHandled && e.button == 0 &&
            //    Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponent.Text != "Cursor" &&
            //    Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponent.Text != "Wire" &&
            //    !Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponent.IsDragDropPlacement)
            //{
            //    //if (Settings.GameState == Settings.GameStates.Stopped)
            //    {
            //        Components.Component c = (Components.Component)Graphics.GUI.GUIEngine.s_componentSelector.GetComponentForSelected();
            //        if (c == null)
            //            goto PostNewComponent;
            //        int x = (int)(e.curState.X);// - Settings.GameOffset.X);
            //        int y = (int)(e.curState.Y);// - Settings.GameOffset.Y);
            //        Vector2 s = c.Graphics.GetSizeRotated(GhostRotation);
            //        GridHelper.GridCoords(ref x, ref y);
            //        PlacableAreasManager.MakePlacable(ref x, ref y, (int)s.X, (int)s.Y);
            //        GridHelper.GridCoords(ref x, ref y);
            //        if (c is Components.Joint)//Joint sanity check
            //        {
            //            if (HasJointAtCoord(x, y) != null)
            //                goto PostNewComponent;
            //        }
            //        Vector2 os = MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponentGraphics.GetCenter(GhostRotation);
            //        int nx = x - (int)os.X, ny = y - (int)os.Y;
            //
            //        if (Graphics.GraphicsEngine.CanDrawGhostComponent(ref nx, ref ny, (int)s.X, (int)s.Y))
            //        {
            //            MicroWorld.Logics.ChangeHistory.Push();
            //
            //            Graphics.GUI.GUIEngine.s_componentSelector.DecreaseSelectedComponentAvilability();
            //
            //            c.Graphics.Position = new Microsoft.Xna.Framework.Vector2(nx, ny);
            //            c.SetRotation(GhostRotation);
            //            c.Initialize();
            //            //Components.ComponentsManager.Components.Add(c);
            //            c.InitAddChildComponents();
            //            c.AddComponentToManager();
            //            c.OnPlaced();
            //
            //            Shortcuts.SetInGameStatus(c.Graphics.GetCSToolTip(), "<Right click> - remove, <Middle click> - properties");
            //            if (Settings.GameState == Settings.GameStates.Stopped)
            //            {
            //                CircuitManager.ReCreate();
            //            }
            //            return;
            //        }
            //    }
            //}
        PostNewComponent: ;
            //code editor && SubButtons
            if (!InputEngine.eventHandled)
            {
                if (MouseOverComponent != null)
                {
                    if (MouseOverComponent is Components.Properties.IContainer &&
                        Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponent.Text != "Cursor")
                    {
                        goto PostMOC;
                    }
                    InputEngine.eventHandled = true;
                    //ToolTip
                    var a = MouseOverComponent.GetComponentToolTip();
                    if (a != null && !a.isVisible)
                    {
                        a.Show();
                    }
                    //SubButtons
                    Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent = MouseOverComponent;
                    Graphics.GUI.GUIEngine.s_subComponentButtons.isVisible = true;
                    Graphics.GUI.GUIEngine.AddHUDScene(Graphics.GUI.GUIEngine.s_subComponentButtons);
                    GlobalEvents.OnComponentSelected(Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent);
                    return;
                PostMOC: ;
                }
                if (!Graphics.GUI.GUIEngine.s_subComponentButtons.IsIn(e.curState.X, e.curState.Y))
                {
                    if (Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent != null)
                    {
                        Graphics.GUI.GUIEngine.RemoveHUDScene(Graphics.GUI.GUIEngine.s_subComponentButtons);
                        Graphics.GUI.GUIEngine.s_subComponentButtons.isVisible = false;
                        Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent = null;
                        e.Handled = true;
                    }
                }
            }
            //clear selection to cursor
            if (!InputEngine.eventHandled && e.button == 1)
            {
                //if (Graphics.GUI.GUIEngine.s_componentSelector.Components[Graphics.GUI.GUIEngine.s_componentSelector.SelectedIndex].
                //    avalable == 0)
                //{
                Graphics.GUI.GUIEngine.s_componentSelector.ResetSelection();
                e.Handled = true;
                //}
            }
        }

        //return for wether to terminate call function
        private static bool TryPlaceComponent(ref InputEngine.MouseArgs e)
        {
            if (isLine || isDragDrop) return true;
            //add component
            if (!InputEngine.eventHandled && e.button == 0 &&
                Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponent.Text != "Cursor" &&
                Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponent.Text != "Wire" &&
                !Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponent.IsDragDropPlacement)
            {
                Components.Component c = (Components.Component)Graphics.GUI.GUIEngine.s_componentSelector.GetComponentForSelected();
                if (c == null)
                    return false;
                int x = (int)(e.curState.X);// - Settings.GameOffset.X);
                int y = (int)(e.curState.Y);// - Settings.GameOffset.Y);
                Vector2 s = c.Graphics.GetSizeRotated(GhostRotation);
                GridHelper.GridCoords(ref x, ref y);
                PlacableAreasManager.MakePlacable(ref x, ref y, (int)s.X, (int)s.Y);
                GridHelper.GridCoords(ref x, ref y);
                if (c is Components.Joint)//Joint sanity check
                {
                    if (HasJointAtCoord(x, y) != null)
                        return false;
                }
                Vector2 os = MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponentGraphics.GetCenter(GhostRotation);
                int nx = x - (int)os.X, ny = y - (int)os.Y;

                if (Graphics.GraphicsEngine.CanDrawGhostComponent(ref nx, ref ny, (int)s.X, (int)s.Y))
                {
                    MicroWorld.Logics.ChangeHistory.Push();

                    Graphics.GUI.GUIEngine.s_componentSelector.DecreaseSelectedComponentAvilability();

                    c.Graphics.Position = new Microsoft.Xna.Framework.Vector2(nx, ny);
                    c.SetRotation(GhostRotation);
                    c.Initialize();
                    //Components.ComponentsManager.Components.Add(c);
                    c.InitAddChildComponents();
                    c.AddComponentToManager();
                    c.OnPlaced();

                    Shortcuts.SetInGameStatus(c.Graphics.GetCSToolTip(), "<Right click> - remove, <Middle click> - properties");
                    if (Settings.GameState == Settings.GameStates.Stopped)
                    {
                        CircuitManager.ReCreate();
                    }
                    e.Handled = true;
                    return true;
                }
            }
            return false;
        }

        public static Vector2 GetRotatedSize(Vector2 v)
        {
            if (GhostRotation == Components.Component.Rotation.cw0) return v;
            if (GhostRotation == Components.Component.Rotation.cw90) return new Vector2(v.Y, v.X);
            if (GhostRotation == Components.Component.Rotation.cw180) return v;
            if (GhostRotation == Components.Component.Rotation.cw270) return new Vector2(v.Y, v.X);
            return v;
        }

        public static void Update()
        {
            if (Main.curState.StartsWith("GAME") && !Graphics.GUI.GUIEngine.ContainsHUDScene(Graphics.GUI.GUIEngine.s_mainMenu))
            {
                #region Simulation
                if (Settings.k_SimulationStart.IsMatched())
                {
                    Graphics.GUI.GUIEngine.s_runControl.strtClick(null, null);
                    goto InputMatched;
                }
                if (Settings.k_SimulationStop.IsMatched())
                {
                    Graphics.GUI.GUIEngine.s_runControl.stpClick(null, null);
                    goto InputMatched;
                }
                if (Settings.k_SimulationPause.IsMatched())
                {
                    Graphics.GUI.GUIEngine.s_runControl.psClick(null, null);
                    goto InputMatched;
                }
                #endregion

                #region History
                if (Settings.k_Undo.IsMatched())
                {
                    Sound.SoundPlayer.PlayButtonClick();
                    Logics.ChangeHistory.Pop();
                    goto InputMatched;
                }
                #endregion

                #region Components
                if (Settings.k_ComponentRemove.IsMatched() || Settings.k_Eraser.IsMatched())
                {
                    bool wasRemoved = false;
                    Components.Component container = null;
                    int x = (int)(InputEngine.curMouse.X / Settings.GameScale - Settings.GameOffset.X);
                    int y = (int)(InputEngine.curMouse.Y / Settings.GameScale - Settings.GameOffset.Y);
                    for (int i = 0; i < Components.ComponentsManager.Components.Count; i++)
                    {
                        if (Components.ComponentsManager.Components[i].isIn(x, y) &&
                            Components.ComponentsManager.Components[i].Graphics.Visible)
                        {
                            if (Components.ComponentsManager.Components[i] is Components.Properties.IContainer)
                            {
                                container = Components.ComponentsManager.Components[i];
                                continue;
                            }
                            //if (Settings.GameState != Settings.GameStates.Stopped)
                            //{
                            //    MicroWorld.Graphics.OverlayManager.HighlightStop();
                            //}
                            //else
                            {
                                wasRemoved = true;
                                if (Components.ComponentsManager.Components[i] == MouseOverComponent)
                                    MouseOverComponent = null;
                                Components.ComponentsManager.Components[i].OnButtonClickedRemove();

                                if (Settings.GameState != Settings.GameStates.Stopped)
                                {
                                    CircuitManager.ReCreate();
                                }
                            }
                            break;
                        }
                    }
                    if (wasRemoved && container != null)
                    {
                        InputEngine.blockClick = true;
                    }
                    if (!wasRemoved && container != null)// && Settings.GameState == Settings.GameStates.Stopped)
                    {
                        if (container == MouseOverComponent)
                            MouseOverComponent = null;
                        container.OnButtonClickedRemove();

                        if (Settings.GameState != Settings.GameStates.Stopped)
                        {
                            CircuitManager.ReCreate();
                        }
                    }
                    goto InputMatched;
                }
                #endregion

                #region Zoom
                if (Settings.k_ZoomIn.IsMatched())
                {
                    if (Settings.k_ZoomIn.WheelDelta != 0)
                    {
                        Settings.GameScale += 
                            (float)(InputEngine.curMouse.ScrollWheelValue - InputEngine.lastMouse.ScrollWheelValue) / 1200;
                    }
                    else
                    {
                        Settings.GameScale += 0.02f;
                    }
                    goto InputMatched;
                }
                if (Settings.k_ZoomOut.IsMatched())
                {
                    if (Settings.k_ZoomOut.WheelDelta != 0)
                    {
                        Settings.GameScale +=
                            (float)(InputEngine.curMouse.ScrollWheelValue - InputEngine.lastMouse.ScrollWheelValue) / 1200;
                    }
                    else
                    {
                        Settings.GameScale -= 0.02f;
                    }
                    goto InputMatched;
                }
                #endregion

                #region Grid
                if (Settings.k_ToggleGrid.IsMatched())
                {
                    Graphics.GUI.GridDraw.ShouldDrawGrid = !Graphics.GUI.GridDraw.ShouldDrawGrid;
                    goto InputMatched;
                }
                #endregion

                #region ComponentRotation
                if (Settings.k_ComponentRotateCW.IsMatched())
                {
                    var old = GhostRotation;
                    var a = Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponentGraphics.GetPossibleRotations();
                    if (a == null || a.Length == 0)
                    {
                        GhostRotation = Components.Component.Rotation.cw0;
                        return;
                    }
                    int t = GhostRotation.GetHashCode() + 1;
                    while (t >= a.Length) t -= a.Length;
                    while (t < 0) t += a.Length;
                    GhostRotation = a[t];

                    goto InputMatched;
                }

                if (Settings.k_ComponentRotateCCW.IsMatched())
                {
                    var old = GhostRotation;
                    var a = Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponentGraphics.GetPossibleRotations();
                    if (a == null || a.Length == 0)
                    {
                        GhostRotation = Components.Component.Rotation.cw0;
                        return;
                    }
                    int t = GhostRotation.GetHashCode() - 1;
                    while (t >= a.Length) t -= a.Length;
                    while (t < 0) t += a.Length;
                    GhostRotation = a[t];

                    goto InputMatched;
                }
                #endregion

                goto PostInput;

            InputMatched:

            PostInput: ;
            }
        }

        public static void OnMouseWheelClick(InputEngine.MouseArgs e)
        {
            if (MouseOverComponent != null)
            {
                Graphics.GUI.GUIEngine.s_componentSelector.SelectComponent(MouseOverComponent.GetType());
            }
            /*
            if (Settings.GameState == Settings.GameStates.Stopped)
            {
                int x = (int)(e.curState.X / Settings.GameScale - Settings.GameOffset.X);
                int y = (int)(e.curState.Y / Settings.GameScale - Settings.GameOffset.Y);
                for (int i = 0; i < Components.ComponentsManager.Components.Count; i++)
                {
                    if (Components.ComponentsManager.Components[i].isIn(x, y))
                    {
                        Components.ComponentsManager.Components[i].OnButtonClickedProperties();
                        return;
                    }
                }
            }//*/
        }


    }
}
