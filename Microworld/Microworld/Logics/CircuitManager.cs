using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace MicroWorld.Logics
{
    public unsafe static class CircuitManager
    {
        public static List<Components.Joint> Joints = new List<Components.Joint>();
        public static List<Components.Wire> Wires = new List<Components.Wire>();
        internal static List<CircuitPart> SubCircuits = new List<CircuitPart>();
        public static int t = 0;
        public static bool FoundStrongSource = false;

        internal static List<CircuitPart> reUpdateList = new List<CircuitPart>();

        #region IterationComponents
        private static List<Components.Properties.IRequiresCircuitRecalculation> iterationComponents = 
            new List<Components.Properties.IRequiresCircuitRecalculation>();

        public static void RegisterReUpdatingComponent(Components.Properties.IRequiresCircuitRecalculation c)
        {
            if (!iterationComponents.Contains(c))
                iterationComponents.Add(c);

            for (int i = 0; i < iterationComponents.Count; i++)
                for (int j = i + 1; j < iterationComponents.Count; j++)
                    if (iterationComponents[i].GetPriority() < iterationComponents[j].GetPriority())
                    {
                        var a = iterationComponents[i];
                        iterationComponents[i] = iterationComponents[j];
                        iterationComponents[j] = a;
                    }
        }

        public static void RemoveReUpdatingComponent(Components.Properties.IRequiresCircuitRecalculation c)
        {
            if (iterationComponents.Contains(c))
                iterationComponents.Remove(c);
        }
        #endregion

        public static bool ShouldReUpdate = false;

        public const float MAX_VOLTAGE = 5f;

        public static void InitializeCircuit()
        {
            if (Settings.Debug)
            {
                try
                {
                    System.IO.Directory.Delete("debug/CircuitParts", true);
                }
                catch { }
                System.IO.Directory.CreateDirectory("debug/CircuitParts");
                IO.SaveEngine.SaveAll("debug/CircuitParts/autosave.sav");
            }

            SubCircuits.Clear();

            for (int i = 0; i < Joints.Count; i++)
            {
                Joints[i].IsInSubCircuit = false;
            }
            for (int i = 0; i < Wires.Count; i++)
            {
                Wires[i].IsUpdated = false;
            }


            for (int i = 0; i < Joints.Count; i++)
            {
                if (!Joints[i].IsInSubCircuit)
                {
                    if (Settings.Debug)
                        System.IO.Directory.CreateDirectory("debug/CircuitParts/" + SubCircuits.Count.ToString());
                    CircuitPart c = new CircuitPart();
                    c.Init(Joints[i]);
                    SubCircuits.Add(c);
                }
            }


            for (int i = 0; i < Joints.Count; i++)
            {
                Joints[i].IsInSubCircuit = false;
            }
            for (int i = 0; i < Wires.Count; i++)
            {
                Wires[i].IsUpdated = false;
            }
        }

        public static void Update()
        {
            if (reCreate)
            {
                ReCreate();
                reCreate = false;
            }

            Components.ComponentsManager.PreUpdate();

            Graphics.ParticleManager.InGameUpdate();//TODO move, add event
            Settings.simulationTicks++;
            for (int i = 0; i < Wires.Count; i++)
            {
                Wires[i].IsDirectionConnected = true;
            }

            #region MainUpdate
            reUpdateList.Clear();
            reUpdateList.AddRange(SubCircuits);
            int t = 0;
            while (reUpdateList.Count > 0 && t < 50)
            {
                t++;
                _updateIteration();
                Components.ComponentsManager.CircuitUpdate();
            }
            #endregion

            #region IterationComponents
            for (int i = 0; i < iterationComponents.Count; i++)
            {
                t = 0;
                iterationComponents[i].PreIndividualUpdate();
                while (reUpdateList.Count > 0 && t < 50)
                {
                    t++;
                    _updateIteration();
                    Components.ComponentsManager.CircuitUpdate();
                }

                t = 0;
                iterationComponents[i].PostIndividualUpdate();
                while (reUpdateList.Count > 0 && t < 50)
                {
                    t++;
                    _updateIteration();
                    Components.ComponentsManager.CircuitUpdate();
                }
            }
            #endregion

            #region LastUpdate
            reUpdateList.Clear();
            reUpdateList.AddRange(SubCircuits);
            Components.ComponentsManager.LastCircuitUpdate();
            _update();
            #endregion
        }

        public static void Push()
        {
            for (int i = 0; i < Wires.Count; i++)
                Wires[i].PushDirectionConnected();
        }

        public static void Pop()
        {
            for (int i = 0; i < Wires.Count; i++)
                Wires[i].PopDirectionConnected();
        }

        private static void _updateIteration()
        {
            _update();
            reUpdateList.Clear();
            for (int i = 0; i < Wires.Count; i++)
            {
                if (Wires[i].Direction == Components.Wire.WireDirection.J1ToJ2 &&
                    Wires[i].J1.Voltage < Wires[i].J2.Voltage - 0.001)
                {
                    if (Wires[i].IsDirectionConnected)
                    {
                        Wires[i].IsDirectionConnected = false;
                        ScheduleReupdate(Wires[i]);
                    }
                }
                if (Wires[i].Direction == Components.Wire.WireDirection.J2ToJ1 &&
                    Wires[i].J2.Voltage < Wires[i].J1.Voltage - 0.001)
                {
                    if (Wires[i].IsDirectionConnected)
                    {
                        Wires[i].IsDirectionConnected = false;
                        ScheduleReupdate(Wires[i]);
                    }
                }
            }
        }

        public static int DebugView = -1;
        private static void _update()
        {
            if (reUpdateList.Count == 1)
                reUpdateList[0].Update();
            else if (reUpdateList.Count == 2)
            {
                reUpdateList[0].Update();
                reUpdateList[1].Update();
            }
            else
                Parallel.ForEach(reUpdateList, curPart =>
                    {
                        curPart.Update();
                    });
            //for (int i = 0; i < reUpdateList.Count; i++)
            //{
            //    reUpdateList[i].Update();
            //}
            //if (DebugView >= 0 && DebugView < SubCircuits.Count)
            //    SubCircuits[DebugView].debug();
        }

        #region ReUpdate
        public static void ScheduleReupdate(Components.Wire w)
        {
            for (int i = 0; i < SubCircuits.Count; i++)
            {
                for (int j = 0; j < SubCircuits[i].wires.Count; j++)
                {
                    if (w == SubCircuits[i].wires[j])
                    {
                        for (int k = 0; k < reUpdateList.Count; k++)
                        {
                            if (reUpdateList[k] == SubCircuits[i]) return;
                        }
                        reUpdateList.Add(SubCircuits[i]);
                        SubCircuits[i].Recalculate = true;
                        return;
                    }
                }
            }
        }
        public static void ScheduleReupdate(Components.Joint jc)
        {
            for (int i = 0; i < SubCircuits.Count; i++)
            {
                for (int j = 0; j < SubCircuits[i].joints.Count; j++)
                {
                    if (jc == SubCircuits[i].joints[j])
                    {
                        for (int k = 0; k < reUpdateList.Count; k++)
                        {
                            if (reUpdateList[k] == SubCircuits[i]) return;
                        }
                        reUpdateList.Add(SubCircuits[i]);
                        SubCircuits[i].Recalculate = true;
                        return;
                    }
                }
            }
        }
        #endregion

        public static void Clear()
        {
            for (int i = 0; i < SubCircuits.Count; i++)
            {
                SubCircuits[i].Dispose();
            }
            SubCircuits.Clear();
        }

        public static void Reset()
        {
            for (int i = 0; i < SubCircuits.Count; i++)
            {
                SubCircuits[i].Reset();
                SubCircuits[i].Dispose();
            }
            SubCircuits.Clear();
        }

        public static void ReCreate()
        {
            Reset();
            InitializeCircuit();
        }

        internal static bool reCreate = false;
        public static void ScheduleReCreate()
        {
            reCreate = true;
        }

        public static void UpdateStep()
        {
            MicroWorld.Components.ComponentsManager.Update();
            MicroWorld.Logics.CircuitManager.Update();
        }





        #region Tools
        public static bool CanReach(Components.Joint from, Components.Joint to)
        {
            if (from == null || to == null)
                return false;

            Components.Joint t = null;
            Components.Joint[] ta;
            List<Components.Joint> open = new List<Components.Joint>();
            int curOpenInd = 0;
            open.Add(from);
            int i, j;

            while (open.Count > curOpenInd && (t = open[curOpenInd]) != to)
            {
                for (i = 0; i < t.ContainingComponents.Count; i++)//find connections through components
                {
                    ta = t.ContainingComponents[i].FindAccessibleJoints(t);
                    for (j = 0; j < ta.Length; j++)
                        if (!open.Contains(ta[j]))
                            open.Add(ta[j]);
                }

                for (i = 0; i < t.ConnectedWires.Count; i++)//find connections through wire
                    if (t.ConnectedWires[i].Graphics.Visible && t.ConnectedWires[i].IsConnected)
                        if (t.ConnectedWires[i].J1 == t)
                        {
                            if (t.ConnectedWires[i].Direction != Components.Wire.WireDirection.J2ToJ1 && !open.Contains(t.ConnectedWires[i].J2))
                                open.Add(t.ConnectedWires[i].J2);
                        }
                        else
                            if (t.ConnectedWires[i].Direction != Components.Wire.WireDirection.J1ToJ2 && !open.Contains(t.ConnectedWires[i].J1))
                                open.Add(t.ConnectedWires[i].J1);

                curOpenInd++;
            }

            return t == to;
        }
        #endregion




        //========================================================DEBUG================================================================

        #region DebugTools

        public static void UpdateDebug()
        {
            Main.cd.lv.Items.Clear();
            for (int i = 0; i < Joints.Count; i++)
            {
                var a = new System.Windows.Forms.ListViewItem(Joints[i].ID.ToString());
                a.SubItems.Add(Joints[i].Voltage.ToString());
                a.SubItems.Add(Joints[i].SendingVoltage.ToString());
                Main.cd.lv.Items.Add(a);
            }
            Main.cd.Invalidate();
        }

        public static void SaveCircuitGraph()
        {
            Bitmap b = new Bitmap(800, 600);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage((Image)b);
            foreach (var comp in Components.ComponentsManager.Components)
            {
                if (comp is Components.Joint)
                {
                    try
                    {
                        Components.Joint j = (Components.Joint)comp;
                        g.FillRectangle(new SolidBrush(Color.FromArgb((int)(255 * j.Voltage / 5f), 0, 0)),
                            j.Graphics.Position.X - 1, j.Graphics.Position.Y - 1, 3, 3);
                        //for (int i = 0; i < j.ConnectedJoints.Count; i++)
                        //{
                        //    g.DrawLine(new Pen(Color.FromArgb((int)(255 * j.Voltage / 5f), 0, 0)),
                        //        j.Graphics.Position.X, j.Graphics.Position.Y,
                        //        j.ConnectedJoints[i].Graphics.Position.X, j.ConnectedJoints[i].Graphics.Position.Y);
                        //}
                    }
                    catch { }
                }
            }
            int t = System.IO.Directory.GetFiles("Debug").Length;
            b.Save("Debug/" + t.ToString() + ".bmp");
        }

        #endregion

    }
}
