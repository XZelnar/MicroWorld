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

namespace MicroWorld.Components
{
    public class Wire : Component, Properties.IAffectedByEMP
    {
        public enum WireDirection
        {
            J1ToJ2 = 1,
            J2ToJ1 = 2,
            Both = 3
        }

        internal MicroWorld.Logics.CircuitPart circuitPart = null;
        public MicroWorld.Logics.CircuitPart CircuitPart
        {
            get { return circuitPart; }
        }

        public Joint J1, J2;
        private WireDirection direction = WireDirection.Both;
        public WireDirection Direction
        {
            get { return direction; }
            set
            {
                if (direction != value)
                {
                    var old = direction;
                    direction = value;
                    if (onWireDirectionChanged != null)
                        onWireDirectionChanged.Invoke(this, old, direction);
                }
            }
        }

        private double resistance = 2;
        public double Resistance
        {
            get { return resistance; }
            set
            {
                var old = resistance;
                resistance = value;
                if (resistance <= 1) 
                    resistance = 1;
                //if (Decimal.IsNaN(resistance))
                //    resistance = 1;
                if (resistance > 1000000)//1mom
                    resistance = 1000000;

                if (old != resistance && onWireResistanceChanged != null)
                {
                    onWireResistanceChanged.Invoke(this, resistance, old);
                    updateVoltageCurrent();
                }
            }
        }

        #region DirectionStuff
        private bool isConnected = true;
        private bool isDirectionConnected = true;
        private bool pushedIsDirectionConnected = false;
        public bool IsConnected
        {
            get { return isConnected && isDirectionConnected; }
            set
            {
                bool old = IsConnected;
                isConnected = value;
                if (IsBurnt) isConnected = false;

                if (old != IsConnected && onWireIsConnectedChanged != null)
                    onWireIsConnectedChanged.Invoke(this, IsConnected, old);
            }
        }
        internal bool IsDirectionConnected
        {
            get { return isDirectionConnected; }
            set
            {
                bool old = IsConnected;
                isDirectionConnected = value;

                if (old != IsConnected && onWireIsConnectedChanged != null)
                    onWireIsConnectedChanged.Invoke(this, IsConnected, old);
            }
        }

        internal void PushDirectionConnected()
        {
            pushedIsDirectionConnected = isDirectionConnected;
        }

        internal void PopDirectionConnected()
        {
            IsDirectionConnected = pushedIsDirectionConnected;
        }
        #endregion

        public bool IsUpdated = false;

        internal int localJointInd1, localJointInd2, localIndSelf;

        internal double _current;
        public double Current
        {
            get
            {
                return (IsIsolated || VoltageDropAbs < 0.000001f) ? 0 : _current;
            }
        }
        internal void SetCurrent(double c)
        {
            _current = c;
        }
        public double VoltageDropAbs
        {
            get { return IsIsolated ? 0 : Math.Abs(J1.Voltage - J2.Voltage); }
        }
        //For CircuitPart. Wether to assign potential or not
        internal bool IsIsolated = false;

        public bool IsBurnt = false;
        public double MaxWithstandingCurrent = -1;

        private double sendingVoltage = 0;
        private double sendingCurrent = 0;
        internal double resultSendingCurrent = 0;
        public bool CanSendVoltageOrCurrent = false;
        public double SendingVoltage
        {
            get { return sendingVoltage; }
            set
            {
                if (value < -Settings.MAX_VOLTAGE)
                    value = -Settings.MAX_VOLTAGE;
                if (value > Settings.MAX_VOLTAGE)
                    value = Settings.MAX_VOLTAGE;
                if (value != sendingVoltage)
                {
                    double old = sendingVoltage;
                    sendingVoltage = value;
                    updateVoltageCurrent();
                }
            }
        }
        public double SendingCurrent
        {
            get { return sendingCurrent; }
            set
            {
                if (value < -Settings.MAX_VOLTAGE)
                    value = -Settings.MAX_VOLTAGE;
                if (value > Settings.MAX_VOLTAGE)
                    value = Settings.MAX_VOLTAGE;
                if (value != sendingCurrent)
                {
                    double old = sendingCurrent;
                    sendingCurrent = value;
                    updateVoltageCurrent();
                }
            }
        }

        private void updateVoltageCurrent()
        {
            if (resistance == 0)
                resistance = 1;
            double r = sendingCurrent + sendingVoltage / resistance;
            if (Double.IsNaN(r))
                r = 0;
            if (r != resultSendingCurrent && onWireSendingCurrentChanged != null)
            {
                onWireSendingCurrentChanged.Invoke(this, r, resultSendingCurrent);
                resultSendingCurrent = r;
            }
        }


        #region Events
        public delegate void WireIsConnectedChanged(Wire w, bool v_new, bool v_old);
        public event WireIsConnectedChanged onWireIsConnectedChanged;
        public delegate void WireDoubleValueChanged(Wire w, double v_new, double v_old);
        public event WireDoubleValueChanged onWireResistanceChanged;
        public event WireDoubleValueChanged onWireSendingCurrentChanged;
        public delegate void WireDirectionChanged(Wire w, WireDirection v_old, WireDirection v_new);
        public event WireDirectionChanged onWireDirectionChanged;
        #endregion

        internal static short TypeID = 0;



        #region IAffectedByEMP
        internal bool WasEMPd = false;

        public void TouchedByEMP(Vector2 EMPCenter)
        {
            WasEMPd = true;
        }
        #endregion



        private void constructor()
        {
            ID = ComponentsManager.GetFreeID();
            Logics = new Logics.WireLogics();
            Graphics = new Graphics.WireGraphics();
            Graphics.parent = this;
            Logics.parent = this;

            if (J1 != null)
            {
                J1.ConnectedWires.Remove(this);
                J1.ConnectedWires.Add(this);
                J2.ConnectedWires.Remove(this);
                J2.ConnectedWires.Add(this);

                Statistics.WireLengthPlaced +=
                    Math.Sqrt(Math.Pow(J1.Graphics.Position.X - J2.Graphics.Position.X, 2) +
                              Math.Pow(J1.Graphics.Position.Y - J2.Graphics.Position.Y, 2));

                MicroWorld.Logics.CircuitManager.ScheduleReCreate();
            }

            ToolTip = new GUI.WireProperties();
        }

        public Wire()
        {
            constructor();
        }

        public Wire(Joint j1, Joint j2)
        {
            J1 = j1;
            J2 = j2;
            constructor();
        }

        public Wire(int id1, int id2)
        {
            Component t = ComponentsManager.GetComponent(id1);
            if (t == null || !(t is Joint))
                J1 = new Joint();
            else
                J1 = (Joint)t;
            
            t = ComponentsManager.GetComponent(id2);
            if (t == null || !(t is Joint))
                J2 = new Joint();
            else
                J2 = (Joint)t;
            constructor();
        }

        public override void Initialize()
        {
            MicroWorld.Logics.CircuitManager.Wires.Add(this);
            base.Initialize();
        }

        public static void LoadContentStatic()
        {
            Components.Graphics.WireGraphics.LoadContentStatic();
        }

        public override void SetComponentOnVisibilityMap()
        {
            if (!Graphics.Visible) return;
            var g = Graphics as Graphics.WireGraphics;
            for (int i = 0; i < g.DrawPath.Count - 1; i++)
            {
                ComponentsManager.VisibilityMap.SetLine(this, g.DrawPath[i].X, g.DrawPath[i].Y, g.DrawPath[i + 1].X, g.DrawPath[i + 1].Y);
            }
            J1.SetComponentOnVisibilityMap();
            J2.SetComponentOnVisibilityMap();
        }

        public override void RemoveComponentFromVisibilityMap()
        {
            if (!Graphics.Visible) return;
            var g = Graphics as Graphics.WireGraphics;
            for (int i = 0; i < g.DrawPath.Count - 1; i++)
            {
                ComponentsManager.VisibilityMap.SetLine(null, g.DrawPath[i].X, g.DrawPath[i].Y, g.DrawPath[i + 1].X, g.DrawPath[i + 1].Y);
            }
        }

        public override void Update()
        {
            base.Update();

            if (MaxWithstandingCurrent > 0 && !IsBurnt && Current > MaxWithstandingCurrent)
            {
                IsBurnt = true;
                isConnected = false;
                Burn();
            }
        }

        public override void NonGameUpdate()
        {
            base.NonGameUpdate();
            
            if (GetComponentToolTip() != null &&
                GetComponentToolTip().isVisible &&
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent != this &&
                MicroWorld.Graphics.GUI.GUIEngine.GetHUDSceneTypeCount<GUI.GeneralProperties>() > 2 &&
                (!MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.isVisible ||
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent != this))
            {
                GetComponentToolTip().Close();
                //croWorld.Graphics.GUI.GUIEngine.RemoveHUDScene(GetComponentToolTip());
            }
            
            var a1 = J1.Graphics.Position;
            var a2 = J2.Graphics.Position;
            Graphics.Position = new Microsoft.Xna.Framework.Vector2(
                Math.Min(a1.X, a2.X),
                Math.Min(a1.Y, a2.Y));
            Graphics.Size = new Microsoft.Xna.Framework.Vector2(
                Math.Max(a1.X, a2.X),
                Math.Max(a1.Y, a2.Y));
            Graphics.Size -= Graphics.Position;



            if (MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent == this)
            {
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.AutoUpdatePosition = false;
                /*
                var a = (Graphics as Components.Graphics.WireGraphics).GetPosSubButtons();
                float x = a.X, y = a.Y;
                Utilities.Tools.GameToScreenCoords(ref x, ref y);
                a = new Vector2(x, y);
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.Position =
                    a + new Vector2(-MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.Position.X / 2, 10);//*/
                //MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.Position =
                //    new Microsoft.Xna.Framework.Vector2(clickedx - 25 / Settings.GameScale, clickedy + 10) * Settings.GameScale;
            }
        }

        public override bool isIn(int x, int y)
        {
            if (!Graphics.Visible) 
                return false;

            if (InputEngine.IsKeyPressed(Keys.Q))
            {
            }

            var g = Graphics as Graphics.WireGraphics;
            if (g.DrawPath.Count < 2)
                return false;

            Vector2 a1, a2;
            float minx, miny, maxx, maxy;
            a2 = g.DrawPath[0];
            for (int i = 1; i < g.DrawPath.Count; i++)
            {
                a1 = a2;
                a2 = g.DrawPath[i];
                minx = Math.Min(a1.X, a2.X);
                miny = Math.Min(a1.Y, a2.Y);
                maxx = Math.Max(a1.X, a2.X);
                maxy = Math.Max(a1.Y, a2.Y);
                if (x >= minx - 4 && x <= maxx + 4 &&
                    y >= miny - 4 && y <= maxy + 4)
                    return true;
            }

            return false;
        }

        public override bool isIn(int x, int y, int w, int h)
        {
            return J1.isIn(x, y, w, h) || J2.isIn(x, y, w, h);
        }

        public override bool Intersects(int x, int y, int w, int h)
        {
            return J1.Intersects(x, y, w, h) || J2.Intersects(x, y, w, h);
        }

        int clickedx, clickedy;

        public override void OnMouseClick(InputEngine.MouseArgs e)
        {
            base.OnMouseClick(e);

            clickedx = e.curState.X;
            clickedy = e.curState.Y;
        }

        public override void Remove()
        {
            if (!IsRemovable) return;
            MicroWorld.Logics.CircuitManager.Wires.Remove(this);
            J1.ConnectedWires.Remove(this);
            J2.ConnectedWires.Remove(this);

            if (J1.ConnectedWires.Count == 0 && J1.ContainingComponents.Count == 0)
                J1.Remove();
            else
                J1.SetComponentOnVisibilityMap();
            if (J2.ConnectedWires.Count == 0 && J2.ContainingComponents.Count == 0)
                J2.Remove();
            else
                J2.SetComponentOnVisibilityMap();

            base.Remove();
        }

        public void Burn()
        {
            Random r = new Random();
            var g = Graphics as Graphics.WireGraphics;
            for (int j = 0; j < g.DrawPath.Count - 1; j++)
            {
                var a = g.DrawPath[j];
                var b = g.DrawPath[j + 1] - a;
                var d = b / 20;
                var c = a;
                for (int i = 0; i < 20; i++)
                {
                    MicroWorld.Graphics.ParticleManager.Add(new MicroWorld.Graphics.Particles.Smoke(
                        new Microsoft.Xna.Framework.Vector2(c.X, c.Y), new Microsoft.Xna.Framework.Vector2(3, 3),
                        new Microsoft.Xna.Framework.Vector2((float)(r.NextDouble() - 0.5) / 5f, (float)(r.NextDouble() - 0.5) / 5f),
                        0.01f));

                    c += d;
                }
            }
            for (int i = 0; i < 100; i++)
            {
            }
        }

        public override void Reset()
        {
            IsBurnt = false;
            IsConnected = true;
            WasEMPd = false;
            //wasConnected = false;
            base.Reset();
        }

        public override string GetName()
        {
            return "Wire";
        }

        public override int[] getJoints()
        {
            return new int[] { J1.ID, J2.ID };
        }


        //================================================================IO===========================================================


        internal bool dnd = false;
        int dndIndex = -1;
        internal static int DnDState = 0;//0 = none, 1 = vertical, 2 = horizontal
        public override void OnMouseDown(InputEngine.MouseArgs e)
        {
            if (!Graphics.Visible) return;
            if (MicroWorld.Logics.GameInputHandler.isLine)
            {
                dnd = false;
                dndIndex = -1;
                DnDState = 0;
                return;
            }

            if (e.button == 0 && Settings.GameState == Settings.GameStates.Stopped &&
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.isVisible && MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent == this)
            {
                var g = Graphics as Graphics.WireGraphics;
                if (!IsRemovable)
                    goto EndDnD;
                if (e.curState.X == g.DrawPath[0].X && e.curState.Y == g.DrawPath[0].Y) 
                    goto EndDnD;
                if (e.curState.X == g.DrawPath[g.DrawPath.Count - 1].X && e.curState.Y == g.DrawPath[g.DrawPath.Count - 1].Y) 
                    goto EndDnD;
                for (int i = 0; i < g.DrawPath.Count - 1; i++)
                {
                    var a1 = g.DrawPath[i];
                    var a2 = g.DrawPath[i + 1];
                    var d = MicroWorld.Logics.MathHelper.DistancePointToLineSegment(
                        new Microsoft.Xna.Framework.Vector2(e.curState.X, e.curState.Y), a1, a2);
                    if (d < 4)
                    {
                        dnd = true;
                        dndIndex = i;
                        if (a1.X == a2.X) 
                            DnDState = 2;
                        else 
                            DnDState = 1;
                        break;
                    }
                }
                if (dndIndex == 0)
                {
                    dndIndex = 1;
                    g.DrawPath.Insert(0, g.DrawPath[0]);
                }
                if (dndIndex >= g.DrawPath.Count - 2)
                {
                    //dndIndex--;
                    g.DrawPath.Insert(g.DrawPath.Count - 1, g.DrawPath[g.DrawPath.Count - 1]);
                }
            EndDnD: ;
            }

            base.OnMouseDown(e);
        }

        public override bool CanDragDrop()
        {
            return false;
        }

        public override void OnMouseMove(InputEngine.MouseMoveArgs e)
        {
            if (!Graphics.Visible) return;
            if (MicroWorld.Logics.GameInputHandler.isLine) return;

            if (isIn(e.curState.X,e.curState.Y) && !ToolTip.StayOpened)
            {
                ToolTip.GameOffset = new Vector2(e.curState.X, e.curState.Y) - Graphics.Position;
            }

            if (ToolTip.isVisible && MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent != this)
            {
                var a = MicroWorld.Graphics.GUI.GUIEngine.GetAllHUDSceneType<GUI.GeneralProperties>();
                if (a.Length > 1)
                    for (int i = 0; i < a.Length; i++)
                        if (a[i].AssociatedComponent is Joint && !(a[i].fadeState == GUI.GeneralProperties.FadeState.FadeOut))
                        {
                            ToolTip.Close();
                            return;
                        }
            }

            if (dnd)
            {
                if (Settings.GameState != Settings.GameStates.Stopped)
                {
                    dnd = false;
                    base.OnMouseMove(e);
                    return;
                }
                RemoveComponentFromVisibilityMap();
                var g = Graphics as Graphics.WireGraphics;
                var a1 = g.DrawPath[dndIndex];
                var a2 = g.DrawPath[dndIndex + 1];
                int x = e.curState.X, y = e.curState.Y;
                int sx = Math.Sign(x), sy = Math.Sign(y);
                MicroWorld.Logics.GridHelper.GridCoords(ref x, ref y);
                x += 4;
                y += 4;
                if (a1.Y == a2.Y)
                {
                    bool t = true;
                    if (a1.X < a2.X)
                    {
                        for (float xx = a1.X; xx < a2.X; xx += 8)
                        {
                            if (ComponentsManager.VisibilityMap.GetAStarValue((int)xx, y) == 0)
                            {
                                t = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (float xx = a2.X; xx < a1.X; xx += 8)
                        {
                            if (ComponentsManager.VisibilityMap.GetAStarValue((int)xx, y) == 0)
                            {
                                t = false;
                                break;
                            }
                        }
                    }
                    if (t)
                    {
                        a1.Y = y;
                        a2.Y = y;
                    }
                }
                else
                {
                    bool t = true;
                    if (a1.Y < a2.Y)
                    {
                        for (float yy = a1.Y; yy < a2.Y; yy += 8)
                        {
                            if (ComponentsManager.VisibilityMap.GetAStarValue(x, (int)yy) == 0)
                            {
                                t = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (float yy = a2.Y; yy < a1.Y; yy += 8)
                        {
                            if (ComponentsManager.VisibilityMap.GetAStarValue(x, (int)yy) == 0)
                            {
                                t = false;
                                break;
                            }
                        }
                    }
                    if (t)
                    {
                        a1.X = x;
                        a2.X = x;
                    }
                }
                g.DrawPath[dndIndex] = a1;
                g.DrawPath[dndIndex + 1] = a2;
                g.GenerateElectrons();
                SetComponentOnVisibilityMap();
                //Components.Graphics.WireGraphics.OptimizeDrawPath(ref g.DrawPath);
            }

            base.OnMouseMove(e);
        }

        public override void OnMouseUp(InputEngine.MouseArgs e)
        {
            if (!Graphics.Visible) return;
            if (dnd && e.button == 0)
            {
                dnd = false;
                dndIndex = -1;
                DnDState = 0;
                var g = Graphics as Graphics.WireGraphics;
                Components.Graphics.WireGraphics.OptimizeDrawPath(ref g.DrawPath);

                bool b = false;
                for (int j = 1; j < g.DrawPath.Count; j++)
                    if (g.DrawPath[j - 1] == g.DrawPath[j])
                    {
                        g.DrawPath.RemoveAt(j--);
                        b = true;
                    }
                if (b)
                    g.GenerateElectrons();
            }
            base.OnMouseUp(e);
        }

        public override void OnPlaced()
        {
            GlobalEvents.OnComponentPlacedByPlayer(this);
            Statistics.ElementsPlaced++;
        }


        //================================================================IO===========================================================

        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);

            Compound.Add("J1", J1.ID);
            Compound.Add("J2", J2.ID);

            Compound.Add("Resistance", Resistance);
            Compound.Add("IsConnected", IsConnected);
            Compound.Add("Direction", direction.GetHashCode());

            Compound.Add("SendingVoltage", sendingVoltage);
            Compound.Add("SendingCurrent", sendingCurrent);
            Compound.Add("CanSendVoltage", CanSendVoltageOrCurrent);

            if (Graphics.Visible)
            {
                String d = "";
                var g = Graphics as Graphics.WireGraphics;
                for (int i = 0; i < g.DrawPath.Count; i++)
                {
                    d += g.DrawPath[i].X.ToString() + ";" + g.DrawPath[i].Y.ToString() + ";";
                }
                d = d.Substring(0, d.Length - 1);
                Compound.Add("DrawPath", d);
            }
        }

        private int j1, j2;
        private String dp = "";

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            if (ID == 45)
            {
            }

            j1 = Compound.GetInt("J1");
            j2 = Compound.GetInt("J2");

            Resistance = Compound.GetDouble("Resistance");
            IsConnected = Compound.GetBool("IsConnected");
            direction = (WireDirection)Compound.GetInt("Direction");

            if (Compound.Contains("SendingVoltage"))
            {
                sendingVoltage = Compound.GetDouble("SendingVoltage");
                sendingCurrent = Compound.GetDouble("SendingCurrent");
                CanSendVoltageOrCurrent = Compound.GetBool("CanSendVoltage");
            }

            dp = Compound.GetString("DrawPath");
        }

        public override void PostLoad()
        {
            base.PostLoad();

            J1 = (Joint)ComponentsManager.GetComponent(j1);
            J2 = (Joint)ComponentsManager.GetComponent(j2);

            if (dp != null)
            {
                var a = dp.Split(';');
                var g = Graphics as Graphics.WireGraphics;
                g.DrawPath.Clear();
                g.IgnoreNextPathFinder = true;
                for (int i = 0; i < a.Length; i += 2)
                {
                    g.DrawPath.Add(new Vector2((float)Convert.ToDouble(a[i]), (float)Convert.ToDouble(a[i + 1])));
                }
            }

            (Graphics as Graphics.WireGraphics).GenerateElectrons();
            updateVoltageCurrent();
        }
    }
}
