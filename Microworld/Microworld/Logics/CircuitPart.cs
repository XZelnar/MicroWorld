using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MicroWorld.Components;
using System.Collections;

namespace MicroWorld.Logics
{
    [Serializable]
    public unsafe class CircuitPart
    {
        private const double ZERO_RESISTANCE = 100000000;//10^8

        [NonSerialized]
        internal List<Joint> joints = new List<Joint>();
        public Joint[] Joints
        {
            get { return joints.ToArray(); }
        }
        [NonSerialized]
        internal List<Wire> wires = new List<Wire>();
        public Wire[] Wires
        {
            get { return wires.ToArray(); }
        }

        internal List<int> potentialPowerSources = new List<int>();
        public int[] PotentialPowerSourceJointsIndices
        {
            get { return potentialPowerSources.ToArray(); }
        }
        internal List<int> potentialGrounds = new List<int>();
        public int[] PotentialGroundJointsIndices
        {
            get { return potentialGrounds.ToArray(); }
        }
        internal Hashtable jointIDIndex = new Hashtable();

        internal List<object> jointspos = new List<object>(),
            wirespos = new List<object>();

        private int index = 0;
        private int curSerialization = 0;
        private short wireSendingVoltageCount = 0;
        private short wirePotentialSendingVoltageCount = 0;

        [NonSerialized]
        private Matrix A;
        [NonSerialized]
        private Matrix Y;
        [NonSerialized]
        private Matrix J;
        //[NonSerialized]
        //private Matrix E;

        [NonSerialized]
        private Matrix At;
        [NonSerialized]
        private Matrix mA;
        [NonSerialized]
        private Matrix AY;
        [NonSerialized]
        private Matrix AYAt;
        //[NonSerialized]
        //private Matrix YE;
        //private Matrix JYE;
        [NonSerialized]
        private Matrix mAJYE;

        [NonSerialized]
        private double[] potentials = new double[0];

        [NonSerialized]
        internal bool Recalculate = true;


        public CircuitPart()
        {
            index = CircuitManager.SubCircuits.Count;
        }


        #region Init
        public void Init(Components.Joint j)
        {
            potentialGrounds.Clear();
            potentialPowerSources.Clear();
            _processInitJoint(j);
            InitMatrixes();
            SubscribeToEvents();
            
            Recalculate = true;
        }

        private void Serialize()
        {
            if (!Settings.Debug)
                return;

            jointspos.Clear();
            wirespos.Clear();

            for (int i = 0; i < joints.Count; i++)
            {
                jointspos.Add(joints[i].Graphics.PushPosition());
            }
            for (int i = 0; i < wires.Count; i++)
            {
                wirespos.Add(wires[i].Graphics.PushPosition());
            }

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            System.IO.Stream s = new System.IO.FileStream("debug/CircuitParts/" + index.ToString() + "/" + curSerialization.ToString() + ".bin", System.IO.FileMode.Create);
            bf.Serialize(s, this);
            s.Close();
            curSerialization++;
        }

        private void _processInitJoint(Components.Joint j)
        {
            if (j == null || j.IsInSubCircuit)
                return;

            j.IsInSubCircuit = true;
            joints.Add(j);
            j.circuitPart = this;
            jointIDIndex.Add(j.ID, joints.Count - 1);
            if (j.CanBeGround)
            {
                j.localGroundInd = potentialGrounds.Count;
                potentialGrounds.Add((int)jointIDIndex[j.ID]);
            }
            if (j.CanProvidePower)
            {
                j.localProviderInd = potentialPowerSources.Count;
                potentialPowerSources.Add((int)jointIDIndex[j.ID]);
            }

            Serialize();

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
                j.ConnectedWires[i].localIndSelf = wires.Count;
                if (j.ConnectedWires[i].CanSendVoltageOrCurrent)
                    wirePotentialSendingVoltageCount++;
                wires.Add(j.ConnectedWires[i]);
                j.ConnectedWires[i].circuitPart = this;
                Serialize();
            }
        }

        private void SubscribeToEvents()
        {
            for (int i = 0; i < joints.Count; i++)
            {
                if (joints[i].CanProvidePower)
                {
                    joints[i].onJointSendingVoltageChanged += new Joint.JointSendingVoltageChanged(CircuitPart_onJointSendingVoltageChanged);
                    joints[i].onJointIsProviderChanged += new Joint.JointIsProviderChanged(CircuitPart_onJointIsProviderChanged);
                }
                if (joints[i].CanBeGround)
                    joints[i].onJointIsGroundChanged += new Joint.JointIsGroundChanged(CircuitPart_onJointIsGroundChanged);
            }

            for (int i = 0; i < wires.Count; i++)
            {
                wires[i].onWireIsConnectedChanged += new Wire.WireIsConnectedChanged(CircuitPart_onWireIsConnectedChanged);
                wires[i].onWireResistanceChanged += new Wire.WireDoubleValueChanged(CircuitPart_onWireResistanceChanged);
                wires[i].onWireDirectionChanged += new Wire.WireDirectionChanged(CircuitPart_onWireDirectionChanged);
                wires[i].onWireSendingCurrentChanged += new Wire.WireDoubleValueChanged(CircuitPart_onWireSendingCurrentChanged);
            }
        }

        #region InitMatrixes
        public void InitMatrixes()
        {
            InitA();
            InitY();
            InitJ();
            //InitE();
        }

        public void InitA()
        {
            A = new Matrix(wires.Count + potentialPowerSources.Count * potentialGrounds.Count, joints.Count - 1);
            for (int i = 0; i < wires.Count; i++)
            {
                if (wires[i].IsConnected)
                    SetAWire(i, wires[i].localJointInd1, wires[i].localJointInd2, 1, -1);
            }
            onAChanged();
        }

        public void InitY()
        {
            Y = new Matrix(wires.Count + potentialPowerSources.Count * potentialGrounds.Count,
                wires.Count + potentialPowerSources.Count * potentialGrounds.Count);

            for (int i = 0; i < wires.Count; i++)
            {
                if (wires[i].Resistance < 1)
                {
                    Y[i, i] = 1;
                }
                else
                {
                    Y[i, i] = 1 / wires[i].Resistance;
                }
            }

            for (int i = wires.Count; i < Y.W; i++)
            {
                Y[i, i] = 1;
            }
        }

        public void InitJ()
        {
            J = new Matrix(1, wires.Count + potentialPowerSources.Count * potentialGrounds.Count);
            
                for (int i = 0; i < wires.Count; i++)
                    if (wires[i].resultSendingCurrent != 0)
                    {
                        J[0, wires[i].localIndSelf] = wires[i].resultSendingCurrent;
                        wireSendingVoltageCount++;
                    }
            
                for (int i = 0; i < potentialGrounds.Count; i++)
                {
                    if (joints[potentialGrounds[i]].IsGround)
                    {
                        for (int j = 0; j < potentialPowerSources.Count; j++)
                        {
                            if (joints[potentialPowerSources[j]].IsProvidingPower)
                            {
                                SetAWire(wires.Count + i * potentialPowerSources.Count + j,
                                    potentialGrounds[i], potentialPowerSources[j], 1, -1);
                                J[0, wires.Count + i * potentialPowerSources.Count + j] = joints[potentialPowerSources[j]].SendingVoltage;
                            }
                            else
                            {
                                SetAWire(wires.Count + i * potentialPowerSources.Count + j,
                                    potentialGrounds[i], potentialPowerSources[j], 0, 0);
                                J[0, wires.Count + i * potentialPowerSources.Count + j] = 0;
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < potentialPowerSources.Count; j++)
                        {
                            SetAWire(wires.Count + i * potentialPowerSources.Count + j,
                                potentialGrounds[i], potentialPowerSources[j], 0, 0);
                            J[0, wires.Count + i * potentialPowerSources.Count + j] = 0;
                        }
                    }
                }
                onAChanged();
        }

        //public void InitE()
        //{
        //    E = new Matrix(1, wires.Count + potentialPowerSources.Count * potentialGrounds.Count);
        //
        //    for (int i = 0; i < wires.Count; i++)
        //        if (wires[i].SendingVoltage != 0)
        //        {
        //            E[0, wires[i].localIndSelf] = wires[i].SendingVoltage;
        //            wireSendingVoltageCount++;
        //        }
        //
        //    for (int i = 0; i < potentialGrounds.Count; i++)
        //    {
        //        if (joints[potentialGrounds[i]].IsGround)
        //        {
        //            for (int j = 0; j < potentialPowerSources.Count; j++)
        //            {
        //                if (joints[potentialPowerSources[j]].IsProvidingPower)
        //                {
        //                    SetAWire(wires.Count + i * potentialPowerSources.Count + j,
        //                        potentialGrounds[i], potentialPowerSources[j], 1, -1);
        //                    E[0, wires.Count + i * potentialPowerSources.Count + j] = joints[potentialPowerSources[j]].SendingVoltage;
        //                }
        //                else
        //                {
        //                    SetAWire(wires.Count + i * potentialPowerSources.Count + j,
        //                        potentialGrounds[i], potentialPowerSources[j], 0, 0);
        //                    E[0, wires.Count + i * potentialPowerSources.Count + j] = 0;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            for (int j = 0; j < potentialPowerSources.Count; j++)
        //            {
        //                SetAWire(wires.Count + i * potentialPowerSources.Count + j,
        //                    potentialGrounds[i], potentialPowerSources[j], 0, 0);
        //                E[0, wires.Count + i * potentialPowerSources.Count + j] = 0;
        //            }
        //        }
        //    }
        //    onAChanged();
        //}
        #endregion
        #endregion



        #region Events
        void CircuitPart_onWireSendingCurrentChanged(Wire w, double v_new, double v_old)
        {
            J[0, w.localIndSelf] = v_new;
            if (v_old == 0 && v_new != 0)
                wireSendingVoltageCount++;
            if (v_old != 0 && v_new == 0)
                wireSendingVoltageCount--;
            Recalculate = true;
        }

        void CircuitPart_onWireDirectionChanged(Wire w, Wire.WireDirection v_old, Wire.WireDirection v_new)
        {
            Recalculate = true;
        }

        void CircuitPart_onWireResistanceChanged(Wire w, double v_new, double v_old)
        {
            lock (Y)
            {
                if (v_new == 0)
                {
                    Y[w.localIndSelf, w.localIndSelf] = ZERO_RESISTANCE;
                }
                else
                {
                    Y[w.localIndSelf, w.localIndSelf] = 1 / v_new;
                }

                Recalculate = true;
            }
        }

        void CircuitPart_onWireIsConnectedChanged(Wire w, bool v_new, bool v_old)
        {
            if (v_new)
                SetAWire(w.localIndSelf, w.localJointInd1, w.localJointInd2, 1, -1);
            else
                SetAWire(w.localIndSelf, w.localJointInd1, w.localJointInd2, 0, 0);

            onAChanged();
            Recalculate = true;
        }

        void CircuitPart_onJointIsGroundChanged(Joint j, bool v_new, bool v_old)
        {
            lock (A)
            {
                lock (J)
                {
                    for (int i = 0; i < potentialPowerSources.Count; i++)
                    {
                        if (v_new && joints[potentialPowerSources[i]].IsProvidingPower)
                        {
                            SetAWire(wires.Count + j.LocalGroundInd * potentialPowerSources.Count + i,
                                potentialGrounds[j.LocalGroundInd], potentialPowerSources[i], 1, -1);
                            J[0, wires.Count + j.LocalGroundInd * potentialPowerSources.Count + i] = joints[potentialPowerSources[i]].SendingVoltage;
                            onAChanged();
                        }
                        else
                        {
                            SetAWire(wires.Count + j.LocalGroundInd * potentialPowerSources.Count + i,
                                potentialGrounds[j.LocalGroundInd], potentialPowerSources[i], 0, 0);
                            J[0, wires.Count + j.LocalGroundInd * potentialPowerSources.Count + i] = 0;
                            onAChanged();
                        }
                    }

                    Recalculate = true;
                }
            }
        }

        void CircuitPart_onJointIsProviderChanged(Joint j, bool v_new, bool v_old)
        {
            lock (A)
            {
                lock (J)
                {
                    for (int i = 0; i < potentialGrounds.Count; i++)
                    {
                        if (v_new && joints[potentialGrounds[i]].IsGround)
                        {
                            SetAWire(wires.Count + i * potentialPowerSources.Count + j.LocalProviderInd,
                                potentialGrounds[i], potentialPowerSources[j.LocalProviderInd], 1, -1);
                            J[0, wires.Count + i * potentialPowerSources.Count + j.LocalProviderInd] = j.SendingVoltage;
                            onAChanged();
                        }
                        else
                        {
                            SetAWire(wires.Count + i * potentialPowerSources.Count + j.LocalProviderInd,
                                potentialGrounds[i], potentialPowerSources[j.LocalProviderInd], 0, 0);
                            J[0, wires.Count + i * potentialPowerSources.Count + j.LocalProviderInd] = 0;
                            onAChanged();
                        }
                    }

                    onAChanged();
                    Recalculate = true;
                }
            }
        }

        void CircuitPart_onJointSendingVoltageChanged(Joint j, double v_new, double v_old)
        {
            lock (J)
            {
                for (int i = 0; i < potentialGrounds.Count; i++)
                {
                    if (joints[potentialGrounds[i]].IsGround)
                        J[0, wires.Count + i * potentialPowerSources.Count + j.LocalProviderInd] = v_new;
                }

                Recalculate = true;
            }
        }
        #endregion



        #region Dispose
        public void Dispose()
        {
            UnSubscribeFromEvents();
        }

        private void UnSubscribeFromEvents()
        {
            for (int i = 0; i < joints.Count; i++)
            {
                joints[i].onJointSendingVoltageChanged -= new Joint.JointSendingVoltageChanged(CircuitPart_onJointSendingVoltageChanged);
                joints[i].onJointIsProviderChanged -= new Joint.JointIsProviderChanged(CircuitPart_onJointIsProviderChanged);
                joints[i].onJointIsGroundChanged -= new Joint.JointIsGroundChanged(CircuitPart_onJointIsGroundChanged);
            }

            for (int i = 0; i < wires.Count; i++)
            {
                wires[i].onWireIsConnectedChanged -= new Wire.WireIsConnectedChanged(CircuitPart_onWireIsConnectedChanged);
                wires[i].onWireResistanceChanged -= new Wire.WireDoubleValueChanged(CircuitPart_onWireResistanceChanged);
                wires[i].onWireDirectionChanged -= new Wire.WireDirectionChanged(CircuitPart_onWireDirectionChanged);
                wires[i].onWireSendingCurrentChanged -= new Wire.WireDoubleValueChanged(CircuitPart_onWireSendingCurrentChanged);
            }
        }
        #endregion



        #region Utils
        internal double GetAValue(int i, int j)
        {
            if (i < 0 || i >= A.W || j < 0 || j >= A.H)
                return 0;
            lock (A)
                return A[i, j];
        }

        private void SetAWire(int column, int w1row, int w2row, int w1, int w2)
        {
            lock (A)
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
        }

        private void onAChanged()
        {
            UpdateConnectivity();

            At = new Matrix(A); At.Transpose();
            mA = Matrix.Mul(A, -1);
        }

        public int GetActualPowerProvidersCount()
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

        public bool HasNonIsolatedWires()
        {
            for (int i = 0; i < wires.Count; i++)
                if (!wires[i].IsIsolated)
                    return true;
            return false;
        }

        #region Connectivity
        public void UpdateConnectivity()
        {
            bool p1 = false, g1 = false, p2 = false, g2 = false, w = false;

            //for (int i = 0; i < joints.Count; i++)
            //{
            //    joints[i].IsUpdated = false;
            //}
            for (int i = 0; i < wires.Count; i++)
            {
                //wires[i].IsUpdated = false;
                wires[i].IsIsolated = false;
            }

            for (int i = 0; i < wires.Count; i++)
            {
                if (!wires[i].IsIsolated)
                {
                    for (int j = 0; j < joints.Count; j++)
                        joints[j].IsUpdated = false;
                    for (int j = 0; j < wires.Count; j++)
                        wires[j].IsUpdated = false;

                    p1 = false;
                    p2 = false;
                    g1 = false;
                    g2 = false;
                    w = false;

                    HasGroundProviderAtBranch(wires[i], wires[i].J1, ref g1, ref p1);

                    for (int j = 0; j < joints.Count; j++)
                        joints[j].IsUpdated = false;
                    for (int j = 0; j < wires.Count; j++)
                        wires[j].IsUpdated = false;

                    HasGroundProviderAtBranch(wires[i], wires[i].J2, ref g2, ref p2);

                    if (wirePotentialSendingVoltageCount != 0)
                    {
                        for (int j = 0; j < joints.Count; j++)
                            joints[j].IsUpdated = false;
                        for (int j = 0; j < wires.Count; j++)
                            wires[j].IsUpdated = false;

                        HasWireVoltageSourceAtBranch(wires[i], null, ref w);
                    }

                    if ((!((p1 && g2) || (p2 && g1))) && !w)
                    {
                        wires[i].IsIsolated = true;

                        for (int j = 0; j < joints.Count; j++)
                            joints[j].IsUpdated = false;
                        for (int j = 0; j < wires.Count; j++)
                            wires[j].IsUpdated = false;

                        if (!p1 && !g1)
                            SetFullBranchIsolated(wires[i], wires[i].J1);
                        else
                            SetIsolated(wires[i], wires[i].J1);



                        for (int j = 0; j < joints.Count; j++)
                            joints[j].IsUpdated = false;
                        for (int j = 0; j < wires.Count; j++)
                            wires[j].IsUpdated = false;

                        if (!p2 && !g2)
                            SetFullBranchIsolated(wires[i], wires[i].J2);
                        else
                            SetIsolated(wires[i], wires[i].J2);
                    }
                }
            }

            for (int i = 0; i < joints.Count; i++)
            {
                joints[i].IsUpdated = false;
            }
            for (int i = 0; i < wires.Count; i++)
            {
                wires[i].IsUpdated = false;
            }
        }

        private void HasGroundProviderAtBranch(Wire from, Joint j, ref bool ground, ref bool provider)
        {
            if (ground && provider) return;
            if (j.IsUpdated) return;

            ground |= j.IsGround;
            provider |= j.IsProvidingPower;
            j.IsUpdated = true;

            for (int i = 0; i < j.ConnectedWires.Count; i++)
            {
                if (j.ConnectedWires[i] != from && !j.ConnectedWires[i].IsUpdated && !j.ConnectedWires[i].IsIsolated)
                {
                    j.ConnectedWires[i].IsUpdated = true;
                    if (j.ConnectedWires[i].J1 == j)
                    {
                        if (j.ConnectedWires[i].IsConnected)
                            HasGroundProviderAtBranch(j.ConnectedWires[i], j.ConnectedWires[i].J2, ref ground, ref provider);
                    }
                    else
                        if (j.ConnectedWires[i].IsConnected)
                            HasGroundProviderAtBranch(j.ConnectedWires[i], j.ConnectedWires[i].J1, ref ground, ref provider);
                }
            }
        }

        private void HasWireVoltageSourceAtBranch(Wire w, Joint from, ref bool has)
        {
            if (w.IsUpdated) return;

            has |= w.CanSendVoltageOrCurrent;
            if (has) return;

            w.IsUpdated = true;

            if (from != w.J1)
                for (int i = 0; i < w.J1.ConnectedWires.Count; i++)
                    HasWireVoltageSourceAtBranch(w.J1.ConnectedWires[i], w.J1, ref has);
            if (from != w.J2)
                for (int i = 0; i < w.J2.ConnectedWires.Count; i++)
                    HasWireVoltageSourceAtBranch(w.J2.ConnectedWires[i], w.J2, ref has);
        }

        public void SetFullBranchIsolated(Wire from, Joint j)
        {
            if (j.IsUpdated) return;
            j.IsUpdated = true;

            if (j.ConnectedWires.Count > 2)
                return;
            for (int i = 0; i < j.ConnectedWires.Count; i++)
            {
                if (j.ConnectedWires[i] != from && !j.ConnectedWires[i].IsUpdated && !j.ConnectedWires[i].IsIsolated)
                {
                    j.ConnectedWires[i].IsUpdated = true;
                    j.ConnectedWires[i].IsIsolated = true;
                    if (j.ConnectedWires[i].J1 == j)
                        SetFullBranchIsolated(j.ConnectedWires[i], j.ConnectedWires[i].J2);
                    else
                        SetFullBranchIsolated(j.ConnectedWires[i], j.ConnectedWires[i].J1);
                }
            }
        }

        public void SetIsolated(Wire from, Joint j)
        {
            if (j.IsUpdated || j.ConnectedWires.Count > 2) return;
            j.IsUpdated = true;

            for (int i = 0; i < j.ConnectedWires.Count; i++)
            {
                if (j.ConnectedWires[i] != from && !j.ConnectedWires[i].IsUpdated && !j.ConnectedWires[i].IsIsolated)
                {
                    j.ConnectedWires[i].IsUpdated = true;
                    j.ConnectedWires[i].IsIsolated = true;
                    if (j.ConnectedWires[i].J1 == j)
                        SetIsolated(j.ConnectedWires[i], j.ConnectedWires[i].J2);
                    else
                        SetIsolated(j.ConnectedWires[i], j.ConnectedWires[i].J1);
                }
            }
        }
        #endregion

        #endregion




        internal unsafe void Update()
        {
            if (!Recalculate) return;
            Recalculate = false;

            lock (A)
                lock (Y)
                    //        lock (E)
                    lock (J)
                    {
                        if (wires.Count < 1 || joints.Count < 2 || ((potentialGrounds.Count < 1 || potentialPowerSources.Count < 1) && wirePotentialSendingVoltageCount == 0))
                            return;
                        int groundsCount = GetActualGroundCount();
                        int powersCount = GetActualPowerProvidersCount();

                        //UpdateConnectivity();

                        if (((groundsCount < 1 || powersCount < 1) && wireSendingVoltageCount == 0) || !HasNonIsolatedWires())
                        {
                            for (int i = 0; i < potentials.Length; i++)
                                potentials[i] = 0;
                            ProcessUpdatedPotentials();
                            SetCurrents();
                            return;
                        }

                        _countPotentials();
                        ProcessUpdatedPotentials();
                        SetCurrents();
                    }
        }

        #region test
        public static void ttt()
        {
            Matrix A = new Matrix(new String[]{
                   "1 1 -1 -1",
                   "-1 0 1 0"
            });
            Matrix Y = new Matrix(new String[]{
                 "0.1 0 0 0",
                 "0 0.05 0 0",
                 "0 0 1 0",
                 "0 0 0 1"
            });
            Matrix At = new Matrix(A);
            At.TransposeNormalize();
            Matrix E = new Matrix(new String[]{
                "0",
                "0",
                "5",
                "5"
            });
            Matrix mA = Matrix.Mul(A, -1);
            //mA = new Matrix(new String[]{
            //    "-1 -1 1",
            //    "1 0 -1"
            //});
            //Matrix J = new Matrix(new String[]{
            //    "0",
            //    "0",
            //    "5"
            //});

            Matrix AY = Matrix.Mul(A, Y);
            Matrix AYAt = Matrix.Mul(AY, At);

            Matrix YE = Matrix.Mul(Y, E);
            //Matrix JYE = Matrix.Plus(J, YE);
            Matrix mAJYE = Matrix.Mul(mA, YE);

            var potentials = MathHelper.Solve(AYAt, mAJYE);
            //ttt();
        }
        #endregion

        internal void Reset()
        {
            if (wires.Count < 1 || joints.Count < 2 || potentialGrounds.Count < 1 || potentialPowerSources.Count < 1) 
                return;
            int groundsCount = GetActualGroundCount();
            int powersCount = GetActualPowerProvidersCount();
            if (groundsCount < 1 || powersCount < 1)
                return;

            for (int i = 0; i < potentialGrounds.Count; i++)
                for (int j = 0; j < potentialPowerSources.Count; j++)
                {
                    if (potentialGrounds[i] == potentialPowerSources[j])
                        continue;
                    SetAWire(wires.Count + i * potentialPowerSources.Count + j,
                        potentialGrounds[i], potentialPowerSources[j], 0, 0);
                    J[0, wires.Count + i * potentialPowerSources.Count + j] = 0;
                }
            for (int i = 0; i < wires.Count; i++)
                J[0, i] = 0;


            onAChanged();

            for (int i = 0; i < potentials.Length; i++)
            {
                potentials[i] = 0;
            }
            ProcessUpdatedPotentials();
            SetCurrents();
        }



        #region Math & Stuff
        private void _countPotentials()
        {
            AY = Matrix.MulDiag(A, Y);
            AYAt = Matrix.Mul(AY, At);

            //YE = Matrix.MulByVertical(Y, E);
            //JYE = Matrix.Plus(J, YE);
            //mAJYE = Matrix.Mul(mA, YE);
            mAJYE = Matrix.Mul(mA, J);

            potentials = MathHelper.Solve(AYAt, mAJYE);
        }

        //private double[] _countPotentials(Matrix a, Matrix e)
        //{
        //    Matrix at = new Matrix(a), ma = Matrix.Mul(a, -1);
        //    at.Transpose();
        //    AY = Matrix.Mul(a, Y);
        //    AYAt = Matrix.Mul(AY, at);
        //
        //    YE = Matrix.Mul(Y, e);
        //    mAJYE = Matrix.Mul(ma, YE);
        //
        //    return MathHelper.Solve(AYAt, mAJYE);
        //}

        public void ProcessUpdatedPotentials()
        {
            if (potentials.Length == 0)
                return;

            //normalizes potentials to [0..5]
            double min = Double.MaxValue;
            double max = Double.MinValue;

            for (int i = 1; i < potentials.Length; i++)
            {
                if (Double.IsNaN(potentials[i]) || Double.IsPositiveInfinity(potentials[i]))
                    continue;
                if (min > potentials[i]) 
                    min = potentials[i];
                if (max < potentials[i]) 
                    max = potentials[i];
            }

            if (min > 0) 
                min = 0;
            if (max < 0) 
                max = 0;

            //double normalizer = max - min;

            for (int i = 0; i < potentials.Length; i++)
            {
                if (Double.IsNaN(potentials[i]))
                    potentials[i] = 0;
                else if (Double.IsPositiveInfinity(potentials[i]))
                    potentials[i] = max - min;
                else
                    potentials[i] -= min;
            }

            //updates joints
            for (int i = 0; i < potentials.Length; i++)
            {
                //if (AreConnectedWiresIsolated(joints[i]))
                //    joints[i].Voltage = 0;
                //else
                    joints[i].Voltage = potentials[i];
            }

            //if (AreConnectedWiresIsolated(joints[potentials.Length]))
            //    joints[potentials.Length].Voltage = 0;
            //else
                joints[potentials.Length].Voltage = -min;
        }

        private void ProcessUpdatedPotentials(double[] p)
        {
            if (p.Length == 0)
                return;

            //normalizes potentials to [0..5]
            double min = p[0];
            double max = p[0];

            for (int i = 1; i < p.Length; i++)
            {
                if (Double.IsNaN(p[i]))
                    continue;
                if (min > p[i]) min = p[i];
                if (max < p[i]) max = p[i];
            }

            if (min > 0) min = 0;
            if (max < 0) max = 0;

            //double normalizer = max - min;

            for (int i = 0; i < p.Length; i++)
            {
                if (Double.IsNaN(p[i]))
                    p[i] = 0;
                else
                    p[i] -= min;
            }

            //TODO
            for (int i = 0; i < p.Length; i++)
            {
            }

            ProcessUpdatedPotentials();
            SetCurrents();
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
        private void SetCurrents()
        {
            for (int i = 0; i < wires.Count; i++)
                wires[i].IsUpdated = false;

            for (int i = 0; i < wires.Count; i++)
            {
                if (wires[i].IsUpdated) continue;
                SetPartCurrent(wires[i]);
            }

            for (int i = 0; i < wires.Count; i++)
                wires[i].IsUpdated = false;
        }

        private void SetPartCurrent(Wire w)
        {
            double res = w.Resistance;
            Joint j1 = null, j2 = null;

            if (w.IsIsolated)
            {
                w.IsUpdated = true;
                w.SetCurrent(0);
                return;
            }

            GetBranchInfoForCurrent(w, w.J1, ref res, ref j1);
            GetBranchInfoForCurrent(w, w.J2, ref res, ref j2);

            if (j1 == j2)
            {
                w.IsUpdated = true;
                w.SetCurrent(0);
                return;
            }

            double cur = Math.Abs(j1.Voltage - j2.Voltage) / res;
            w.IsUpdated = true;
            w.SetCurrent(cur);
            SetBranchCurrent(w, w.J1, cur);
            SetBranchCurrent(w, w.J2, cur);
        }

        private void GetBranchInfoForCurrent(Wire from, Joint j, ref double Resistance, ref Joint end, Wire start = null)
        {
            if (j.ConnectedWires.Count > 2 || from == start)
            {
                end = j;
                return;
            }
            if (start == null)
                start = from;

            for (int i = 0; i < j.ConnectedWires.Count; i++)
            {
                if (j.ConnectedWires[i] != from)
                {
                    Resistance += j.ConnectedWires[i].Resistance;
                    if (j.ConnectedWires[i].J2 != j)
                        GetBranchInfoForCurrent(j.ConnectedWires[i], j.ConnectedWires[i].J2, ref Resistance, ref end, start);
                    if (j.ConnectedWires[i].J1 != j)
                        GetBranchInfoForCurrent(j.ConnectedWires[i], j.ConnectedWires[i].J1, ref Resistance, ref end, start);
                }
            }

            if (end == null)
                end = j;
        }

        public double GetBranchResistance(Wire w)
        {
            double r = 0;
            Joint j = null;
            GetBranchInfoForCurrent(w, w.J1, ref r, ref j);
            j = null;
            GetBranchInfoForCurrent(w, w.J2, ref r, ref j);
            r += w.Resistance;
            return r;
        }

        private void SetBranchCurrent(Wire from, Joint j, double current, Wire start = null)
        {
            if (j.ConnectedWires.Count > 2 || from == start)
                return;
            if (start == null)
                start = from;

            for (int i = 0; i < j.ConnectedWires.Count; i++)
            {
                if (j.ConnectedWires[i] != from)
                {
                    j.ConnectedWires[i].SetCurrent(current);
                    j.ConnectedWires[i].IsUpdated = true;
                    if (j.ConnectedWires[i].J2 != j)
                        SetBranchCurrent(j.ConnectedWires[i], j.ConnectedWires[i].J2, current, start);
                    if (j.ConnectedWires[i].J1 != j)
                        SetBranchCurrent(j.ConnectedWires[i], j.ConnectedWires[i].J1, current, start);
                }
            }
        }
        #endregion

        /*
        #region Isolated
        private void SetIsolatedJointsPotentials()
        {
            for (int j = 0; j < joints.Count; j++)
                joints[j].IsUpdated = false;
            for (int j = 0; j < wires.Count; j++)
                wires[j].IsUpdated = false;

            bool b1, b2;
            for (int i = 0; i < joints.Count; i++)
            {
                b1 = false;
                b2 = false;
                for (int j = 0; j < joints[i].ConnectedWires.Count; j++)
                {
                    b1 |= joints[i].ConnectedWires[j].IsIsolated;
                    b2 |= !joints[i].ConnectedWires[j].IsIsolated;
                }
                if (b1 && b2)
                {
                    for (int j = 0; j < joints[i].ConnectedWires.Count; j++)
                    {
                        if (joints[i].ConnectedWires[j].IsIsolated)
                        {
                            for (int k = 0; k < joints.Count; k++)
                                joints[k].IsUpdated = false;
                            for (int k = 0; k < wires.Count; k++)
                                wires[k].IsUpdated = false;
                            SetIsolatedBranchPotential(joints[i].ConnectedWires[j], joints[i], joints[i].Voltage);
                        }
                    }
                }
            }
        }

        private void SetIsolatedBranchPotential(Wire from, Joint j, double v)
        {
            if (j.IsUpdated) return;
            j.IsUpdated = true;
            j.Voltage = v;

            for (int i = 0; i < j.ConnectedWires.Count; i++)
            {
                if (j.ConnectedWires[i] != from && !j.ConnectedWires[i].IsUpdated && j.ConnectedWires[i].IsIsolated)
                {
                    j.ConnectedWires[i].IsUpdated = true;
                    if (j.ConnectedWires[i].J1 == j)
                        SetIsolatedBranchPotential(j.ConnectedWires[i], j.ConnectedWires[i].J2, v);
                    else
                        SetIsolatedBranchPotential(j.ConnectedWires[i], j.ConnectedWires[i].J1, v);
                }
            }
        }
        #endregion
        //*/
        #endregion


    }
}
