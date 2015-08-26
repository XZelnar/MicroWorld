using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MicroWorld.Logics;
using System.Collections;

namespace MicroWorld.Logics
{
    public unsafe class CircuitPart2
    {/*
        private static unsafe class ConnectedChecker
        {
            static List<int> checkedIDs = new List<int>();
            static int startJoint = 0;
            static bool srcFound = false, grndFound = false;

            public static bool ShouldStayConnected(Components.Wire w, Components.Joint src)
            {
                startJoint = src.ID;
                checkedIDs.Clear();
                srcFound = false;
                grndFound = false;
                srcFound |= src.IsProvidingPower;
                grndFound |= src.IsGround;
                check(w, src);
                return srcFound && grndFound;
            }

            private static void check(Components.Wire w, Components.Joint src)
            {
                if (grndFound && srcFound)
                    return;

                checkedIDs.Add(w.ID);
                if (w.J1 == src)
                {
                    if (w.J2.ID == startJoint)
                        return;
                    srcFound |= w.J2.IsProvidingPower;
                    grndFound |= w.J2.IsGround;
                    for (int i = 0; i < w.J2.ConnectedWires.Count && !(grndFound && srcFound); i++)
                    {
                        if (!checkedIDs.Contains(w.J2.ConnectedWires[i].ID))
                        {
                            check(w.J2.ConnectedWires[i], w.J2);
                        }
                    }
                }
                else
                {
                    if (w.J1.ID == startJoint)
                        return;
                    srcFound |= w.J1.IsProvidingPower;
                    grndFound |= w.J1.IsGround;
                    for (int i = 0; i < w.J1.ConnectedWires.Count && !(grndFound && srcFound); i++)
                    {
                        if (!checkedIDs.Contains(w.J1.ConnectedWires[i].ID))
                        {
                            check(w.J1.ConnectedWires[i], w.J1);
                        }
                    }
                }
            }

            public static void SetIsolated(Components.Wire w, Components.Joint src)
            {
                startJoint = src.ID;
                checkedIDs.Clear();
                isolate(w, src);
            }

            private static void isolate(Components.Wire w, Components.Joint src)
            {
                w.IsIsolated = true;
                checkedIDs.Add(w.ID);
                if (w.J1 == src)
                {
                    if (w.J2.ID == startJoint)
                        return;
                    for (int i = 0; i < w.J2.ConnectedWires.Count; i++)
                    {
                        if (!checkedIDs.Contains(w.J2.ConnectedWires[i].ID))
                        {
                            isolate(w.J2.ConnectedWires[i], w.J2);
                        }
                    }
                }
                else
                {
                    if (w.J1.ID == startJoint)
                        return;
                    for (int i = 0; i < w.J1.ConnectedWires.Count; i++)
                    {
                        if (!checkedIDs.Contains(w.J1.ConnectedWires[i].ID))
                        {
                            isolate(w.J1.ConnectedWires[i], w.J1);
                        }
                    }
                }
            }
        }

        public const double ZERO_RESISTANCE = 100000000;//10^8

        public Matrix A;
        public Matrix Y;
        public Matrix J;
        public Matrix E;

        private Matrix At;
        private Matrix mA;
        private Matrix AY;
        private Matrix AYAt;
        private Matrix YE;
        private Matrix JYE;
        private Matrix mAJYE;

        private List<int> potentialPowerSources = new List<int>();
        private List<int> potentialGrounds = new List<int>();
        internal List<Components.Joint> joints = new List<Components.Joint>();
        internal List<Components.Wire> wires = new List<Components.Wire>();
        private Hashtable jointIDIndex = new Hashtable();

        public double[] potentials;
        public double[] potentials2;



        public void Dispose()
        {
            jointIDIndex.Clear();
            wires.Clear();
            joints.Clear();
            potentialGrounds.Clear();
            potentialPowerSources.Clear();
        }

        #region Init

        public void Init(Components.Joint j)
        {
            potentialGrounds.Clear();
            potentialPowerSources.Clear();
            _processInitJoint(j);
            InitMatrixes();
        }

        private void _processInitJoint(Components.Joint j)
        {
            if (j == null || j.IsUpdated)
            {
                return;
            }
            if (joints == null || jointIDIndex == null || potentialGrounds == null || potentialPowerSources == null || wires == null)
                return;
            //Console.WriteLine(j.ID);
            j.IsUpdated = true;
            joints.Add(j);
            jointIDIndex.Add(j.ID, joints.Count - 1);
            if (j.CanBeGround) potentialGrounds.Add((int)jointIDIndex[j.ID]);
            if (j.CanProvidePower) potentialPowerSources.Add((int)jointIDIndex[j.ID]);

            for (int i = 0; i < j.ConnectedWires.Count; i++)
            {
                if (j.ConnectedWires[i].IsUpdated) continue;
                j.ConnectedWires[i].IsUpdated = true;
                if (j.ConnectedWires[i].J1.ID == j.ID)
                {
                    _processInitJoint(j.ConnectedWires[i].J2);
                    j.ConnectedWires[i].localJointInd1 = (int)jointIDIndex[j.ID];
                    j.ConnectedWires[i].localJointInd2 = (int)jointIDIndex[j.ConnectedWires[i].J2.ID];
                }
                else
                {
                    _processInitJoint(j.ConnectedWires[i].J1);
                    j.ConnectedWires[i].localJointInd1 = (int)jointIDIndex[j.ID];
                    j.ConnectedWires[i].localJointInd2 = (int)jointIDIndex[j.ConnectedWires[i].J1.ID];
                }
                wires.Add(j.ConnectedWires[i]);
            }
        }

        public void InitMatrixes()
        {
            InitA();
            InitY();
            InitJ();
            InitE();
        }

        public void InitA()
        {
            A = new Matrix(wires.Count + potentialPowerSources.Count * potentialGrounds.Count, joints.Count - 1);
            for (int i = 0; i < wires.Count; i++)
            {
                if(wires[i].IsConnected)
                    SetAWire(i, wires[i].localJointInd1, wires[i].localJointInd2, 1, -1);
                //A[i, wires[i].localInd1] = 1;
                //A[i, wires[i].localInd2] = -1;
            }
            onAChanged();
        }

        public void InitY()
        {
            Y = new Matrix(wires.Count + potentialPowerSources.Count * potentialGrounds.Count,
                wires.Count + potentialPowerSources.Count * potentialGrounds.Count);

            for (int i = wires.Count; i < Y.W; i++)
            {
                Y[i, i] = ZERO_RESISTANCE;
            }

            for (int i = 0; i < wires.Count; i++)
            {
                if (wires[i].Resistance == 0)
                {
                    Y[i, i] = ZERO_RESISTANCE;
                }
                else
                {
                    Y[i, i] = 1 / wires[i].Resistance;
                }
            }
        }

        public void InitJ()
        {
            J = new Matrix(1, wires.Count + potentialPowerSources.Count * potentialGrounds.Count);
        }

        public void InitE()
        {
            E = new Matrix(1, wires.Count + potentialPowerSources.Count * potentialGrounds.Count);
        }

        #endregion



        public void SetAWire(int column, int w1row, int w2row, int w1, int w2)
        {
            if (w1row < A.H)
            {
                A[column, w1row] = w1;
            }
            if (w2row < A.H)
            {
                A[column, w2row] = w2;
            }
        }









        public void onAChanged()
        {
            At = new Matrix(A); At.Transpose();
            mA = Matrix.Mul(A, -1);
        }

        public unsafe void Update()
        {
            if (wires.Count < 1 || joints.Count < 2 || potentialGrounds.Count < 1 || potentialPowerSources.Count < 1) return;
            int groundsCount = GetActualGroundCount();
            int powersCount = GetPowerProvidersCount();
            if (groundsCount < 1 || powersCount < 1)
            {
                potentials = new double[joints.Count - 1];
                ProcessUpdatedPotentials();
                SetCurrents();
                return;
            }

            //Wire check
            bool aChanged = false;
            for (int i = 0; i < wires.Count; i++)
            {
                if (wires[i].IsConnected != wires[i].wasConnected)
                {
                    aChanged = true;
                    wires[i].wasConnected = wires[i].IsConnected;
                    if (wires[i].IsConnected)
                    {
                        SetAWire(i, wires[i].localJointInd1, wires[i].localJointInd2, 1, -1);
                        aChanged = true;
                    }
                    else
                    {
                        SetAWire(i, wires[i].localJointInd1, wires[i].localJointInd2, 0, 0);
                        aChanged = true;
                    }
                }
                if (wires[i].Resistance != wires[i].oldResistance)
                {
                    wires[i].oldResistance = wires[i].Resistance;
                    if (wires[i].Resistance == 0)
                    {
                        Y[i, i] = ZERO_RESISTANCE;
                    }
                    else
                    {
                        Y[i, i] = 1 / wires[i].Resistance;
                    }
                }
                if (wires[i].SendingVoltage != wires[i].oldSendingVoltage)
                {
                    wires[i].oldSendingVoltage = wires[i].SendingVoltage;
                    E[0, i] = wires[i].SendingVoltage;
                }
            }

            //Power and ground check

            double currentPower = 1d / (float)groundsCount / (float)powersCount;

            for (int i = 0; i < potentialGrounds.Count; i++)
            {
                if (!joints[potentialGrounds[i]].IsGround)
                {
                    for (int j = 0; j < potentialPowerSources.Count; j++)
                    {
                        if (potentialGrounds[i] == potentialPowerSources[j]) continue;
                        if (joints[potentialGrounds[i]].IsGround != joints[potentialGrounds[i]].WasGround ||
                            joints[potentialPowerSources[j]].IsProvidingPower != joints[potentialPowerSources[j]].WasProvider ||
                            joints[potentialGrounds[i]].SendingVoltage != joints[potentialGrounds[i]].OldSendingVoltage ||
                            joints[potentialPowerSources[j]].SendingVoltage != joints[potentialPowerSources[j]].OldSendingVoltage)
                        {
                            SetAWire(wires.Count + i * potentialPowerSources.Count + j,
                                potentialGrounds[i], potentialPowerSources[j], 0, 0);
                            aChanged = true;
                            //J[0, wires.Count + i * potentialPowerSources.Count + j] = 0;
                            E[0, wires.Count + i * potentialPowerSources.Count + j] = 0;
                            Y[wires.Count + i * potentialPowerSources.Count + j, wires.Count + i * potentialPowerSources.Count + j] =
                                ZERO_RESISTANCE;//!!!
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < potentialPowerSources.Count; j++)
                    {
                        if (potentialGrounds[i] == potentialPowerSources[j]) continue;
                        if (joints[potentialGrounds[i]].IsGround != joints[potentialGrounds[i]].WasGround ||
                            joints[potentialPowerSources[j]].IsProvidingPower != joints[potentialPowerSources[j]].WasProvider ||
                            joints[potentialGrounds[i]].SendingVoltage != joints[potentialGrounds[i]].OldSendingVoltage ||
                            joints[potentialPowerSources[j]].SendingVoltage != joints[potentialPowerSources[j]].OldSendingVoltage)
                        {
                            if (joints[potentialPowerSources[j]].IsProvidingPower)
                            {
                                SetAWire(wires.Count + i * potentialPowerSources.Count + j,
                                    potentialGrounds[i], potentialPowerSources[j], 1, -1);
                                aChanged = true;
                                //J[0, wires.Count + i * potentialPowerSources.Count + j] = 0;
                                E[0, wires.Count + i * potentialPowerSources.Count + j] = joints[potentialPowerSources[j]].SendingVoltage;
                                Y[wires.Count + i * potentialPowerSources.Count + j, wires.Count + i * potentialPowerSources.Count + j] =
                                    ZERO_RESISTANCE;//!!!
                            }
                            else
                            {
                                SetAWire(wires.Count + i * potentialPowerSources.Count + j,
                                    potentialGrounds[i], potentialPowerSources[j], 0, 0);
                                aChanged = true;
                                //J[0, wires.Count + i * potentialPowerSources.Count + j] = 0;
                                E[0, wires.Count + i * potentialPowerSources.Count + j] = 0;
                                Y[wires.Count + i * potentialPowerSources.Count + j, wires.Count + i * potentialPowerSources.Count + j] =
                                    ZERO_RESISTANCE;//!!!
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < potentialGrounds.Count; i++)
            {
                joints[potentialGrounds[i]].WasGround = joints[potentialGrounds[i]].IsGround;
                joints[potentialGrounds[i]].OldSendingVoltage = joints[potentialGrounds[i]].SendingVoltage;

                foreach (var w in joints[potentialGrounds[i]].ConnectedWires)
                {
                    if (!ConnectedChecker.ShouldStayConnected(w, joints[potentialGrounds[i]]))
                    {
                        ConnectedChecker.SetIsolated(w, joints[potentialGrounds[i]]);
                    }
                }
            }
            for (int j = 0; j < potentialPowerSources.Count; j++)
            {
                joints[potentialPowerSources[j]].WasProvider = joints[potentialPowerSources[j]].IsProvidingPower;
                joints[potentialPowerSources[j]].OldSendingVoltage = joints[potentialPowerSources[j]].SendingVoltage;

                foreach (var w in joints[potentialPowerSources[j]].ConnectedWires)
                {
                    if (!ConnectedChecker.ShouldStayConnected(w, joints[potentialPowerSources[j]]))
                    {
                        ConnectedChecker.SetIsolated(w, joints[potentialPowerSources[j]]);
                    }
                }
            }

            //actual count

            if (aChanged)
            {
                onAChanged();
            }

            _countPotentials();
            ProcessUpdatedPotentials();
            SetCurrents();

            debug();//TODO rm
        }

        public void Reset()
        {
            if (wires.Count < 1 || joints.Count < 2 || potentialGrounds.Count < 1 || potentialPowerSources.Count < 1) return;
            int groundsCount = GetActualGroundCount();
            int powersCount = GetPowerProvidersCount();
            if (groundsCount < 1 || powersCount < 1) return;

            //Power and ground check

            double currentPower = 1d / (float)groundsCount / (float)powersCount;

            for (int i = 0; i < potentialGrounds.Count; i++)
            {
                for (int j = 0; j < potentialPowerSources.Count; j++)
                {
                    if (potentialGrounds[i] == potentialPowerSources[j]) continue;
                    SetAWire(wires.Count + i * potentialPowerSources.Count + j,
                        potentialGrounds[i], potentialPowerSources[j], 0, 0);
                    //J[0, wires.Count + i * potentialPowerSources.Count + j] = 0;
                    E[0, wires.Count + i * potentialPowerSources.Count + j] = 0;
                }
            }

            //actual count

            onAChanged();

            _countPotentials();
            ProcessUpdatedPotentials();
        }

        public void ProcessUpdatedPotentials()
        {
            //normalizes potentials to [0..5]
            double min = potentials[0];
            double max = potentials[0];

            for (int i = 1; i < potentials.Length; i++)
            {
                if (Double.IsNaN(potentials[i]))
                    continue;
                if (min > potentials[i]) min = potentials[i];
                if (max < potentials[i]) max = potentials[i];
            }

            if (min > 0) min = 0;
            if (max < 0) max = 0;

            double normalizer = max - min;

            for (int i = 0; i < potentials.Length; i++)
            {
                if (Double.IsNaN(potentials[i])) 
                    potentials[i] = 0;
                else
                    potentials[i] -= min;
            }

            //updates joints
            for (int i = 0; i < potentials.Length; i++)
            {
                if (AreConnectedWiresIsolated(joints[i]))
                    joints[i].Voltage = 0;
                else
                    joints[i].Voltage = potentials[i];
            }
            //joints[potentials.Length].Voltage = 0;
            if (AreConnectedWiresIsolated(joints[potentials.Length]))
                joints[potentials.Length].Voltage = 0;
            else
                joints[potentials.Length].Voltage = -min;
        }

        private bool AreConnectedWiresIsolated(Components.Joint j)
        {
            foreach (var w in j.ConnectedWires)
            {
                if (!w.IsIsolated)
                    return false;
            }
            return true;
        }

        #region Current
        public void SetCurrents()
        {
            for (int i = 0; i < wires.Count; i++)
            {
                wires[i].IsUpdated = false;
            }
            for (int i = 0; i < wires.Count; i++)
            {
                if (wires[i].IsUpdated) continue;
                SetPartCurrent(wires[i]);
            }
            for (int i = 0; i < wires.Count; i++)
            {
                wires[i].IsIsolated = false;
            }
        }

        private void SetPartCurrent(Components.Wire w)
        {
            if (w.VoltageDropAbs < 0.0001)
            {
                w.SetCurrent(0);
                SetPartTailCurrent(w.J1, w, 0);
                w.IsUpdated = false;
                SetPartTailCurrent(w.J2, w, 0);
                return;
            }
            Components.Joint j1 = GetPartTail(w.J1, w);
            Components.Joint j2 = GetPartTail(w.J2, w);
            double u = Math.Abs(j1.Voltage - j2.Voltage);
            double r = GetPartResistance(w);
            double i = u / r;
            SetPartTailCurrent(w.J1, w, i);
            SetPartTailCurrent(w.J2, w, i);
        }

        private Components.Joint GetPartTail(Components.Joint cur, Components.Wire from)
        {
            if (cur.ConnectedWires.Count != 2) return cur;
            if (cur.ConnectedWires[1] != from)
            {
                if (cur.ConnectedWires[1].J2 != cur)
                    return GetPartTail(cur.ConnectedWires[1].J2, cur.ConnectedWires[1]);
                if (cur.ConnectedWires[1].J1 != cur)
                    return GetPartTail(cur.ConnectedWires[1].J1, cur.ConnectedWires[1]);
            }
            if (cur.ConnectedWires[0] != from)
            {
                if (cur.ConnectedWires[0].J2 != cur)
                    return GetPartTail(cur.ConnectedWires[0].J2, cur.ConnectedWires[0]);
                if (cur.ConnectedWires[0].J1 != cur)
                    return GetPartTail(cur.ConnectedWires[0].J1, cur.ConnectedWires[0]);
            }
            return cur;
        }

        private double GetPartResistance(Components.Wire w)
        {
            double res = 0;
            GetPartTailResistance(w.J1, w, ref res);
            GetPartTailResistance(w.J2, w, ref res);
            res -= w.Resistance;
            return res;
        }

        private void GetPartTailResistance(Components.Joint from, Components.Wire cur, ref double res)
        {
            if (cur.J1 == cur.J2) return;
            res += cur.Resistance;
            if (cur.J2 != from)
            {
                if (cur.J2.ConnectedWires.Count != 2) return;
                if (cur.J2.ConnectedWires[1] != cur)
                    GetPartTailResistance(cur.J2, cur.J2.ConnectedWires[1], ref res);
                if (cur.J2.ConnectedWires[0] != cur)
                    GetPartTailResistance(cur.J2, cur.J2.ConnectedWires[0], ref res);
            }
            if (cur.J1 != from)
            {
                if (cur.J1.ConnectedWires.Count != 2) return;
                if (cur.J1.ConnectedWires[1] != cur)
                    GetPartTailResistance(cur.J1, cur.J1.ConnectedWires[1], ref res);
                if (cur.J1.ConnectedWires[0] != cur)
                    GetPartTailResistance(cur.J1, cur.J1.ConnectedWires[0], ref res);
            }
        }

        private void SetPartTailCurrent(Components.Joint from, Components.Wire cur, double current)
        {
            if (cur.IsUpdated) return;
            cur.SetCurrent(current);
            cur.IsUpdated = true;
            if (cur.J2 != from)
            {
                if (cur.J2.ConnectedWires.Count != 2) return;
                if (cur.J2.ConnectedWires[1] != cur)
                    SetPartTailCurrent(cur.J2, cur.J2.ConnectedWires[1], current);
                if (cur.J2.ConnectedWires[0] != cur)
                    SetPartTailCurrent(cur.J2, cur.J2.ConnectedWires[0], current);
            }
            if (cur.J1 != from)
            {
                if (cur.J1.ConnectedWires.Count != 2) return;
                if (cur.J1.ConnectedWires[1] != cur)
                    SetPartTailCurrent(cur.J1, cur.J1.ConnectedWires[1], current);
                if (cur.J1.ConnectedWires[0] != cur)
                    SetPartTailCurrent(cur.J1, cur.J1.ConnectedWires[0], current);
            }
        }
        #endregion

        public int GetPowerProvidersCount()
        {
            int r = 0;
            for (int i = 0; i < potentialPowerSources.Count; i++)
            {
                if (joints[potentialPowerSources[i]].IsProvidingPower) r++;
            }
            return r;
        }

        public int GetActualGroundCount()
        {
            int r = 0;
            for (int i = 0; i < potentialGrounds.Count; i++)
            {
                if (joints[potentialGrounds[i]].IsGround) r++;
            }
            return r;
        }

        private void _countPotentials()
        {
            AY = Matrix.Mul(A, Y);
            AYAt = Matrix.Mul(AY, At);

            YE = Matrix.Mul(Y, E);
            //JYE = Matrix.Plus(J, YE);
            mAJYE = Matrix.Mul(mA, YE);

            potentials = MathHelper.Solve(AYAt, mAJYE);
        }

        //==========================================================DEBUG==============================================================

        public void debug()
        {
            if (!Main.mv.Visible) return;

            if (joints.Count >= 2)
            {
                try
                {
                    Main.mv.tA.Text = A.ToString();
                    Main.mv.tY.Text = Y.ToString();
                    //Main.mv.tJ.Text = J.ToString();
                    Main.mv.tE.Text = E.ToString();
                    Main.mv.tAY.Text = AY.ToString();
                    Main.mv.tAYAt.Text = AYAt.ToString();
                    Main.mv.tYE.Text = YE.ToString();
                    //Main.mv.tJYE.Text = JYE.ToString();
                    Main.mv.tAJYE.Text = mAJYE.ToString();

                    Main.mv.tr.Text = "";
                    for (int i = 0; i < potentials.Length; i++)
                    {
                        Main.mv.tr.Text += potentials[i].ToString() + "\r\n";
                    }
                }
                catch { }
            }
        }//*/
    }
}
