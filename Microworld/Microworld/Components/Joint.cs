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
    public class Joint : Component, Properties.IDrawBorder, Properties.IAffectedByEMP
    {
        internal MicroWorld.Logics.CircuitPart circuitPart = null;
        public MicroWorld.Logics.CircuitPart CircuitPart
        {
            get { return circuitPart; }
        }

        public bool CanProvidePower = false;
        public bool CanBeGround = false;
        private bool isProvidingPower = false;
        private bool isGround = false;
        private double sendingVoltage = 0;

        public double SendingVoltage
        {
            get { return sendingVoltage; }
            set
            {
                double old = sendingVoltage;

                sendingVoltage = value;
                if (sendingVoltage < -Settings.MAX_VOLTAGE)
                    sendingVoltage = -Settings.MAX_VOLTAGE;
                if (sendingVoltage > Settings.MAX_VOLTAGE)
                    sendingVoltage = Settings.MAX_VOLTAGE;

                if (old != sendingVoltage && onJointSendingVoltageChanged != null)
                    onJointSendingVoltageChanged.Invoke(this, sendingVoltage, old);
            }
        }
        public bool IsProvidingPower
        {
            get { return isProvidingPower; }
            set
            {
                bool b = isProvidingPower != value;
                isProvidingPower = value;

                if (b && onJointIsProviderChanged != null)
                    onJointIsProviderChanged.Invoke(this, isProvidingPower, !isProvidingPower);
            }
        }
        public bool IsGround
        {
            get { return isGround; }
            set
            {
                bool b = isGround != value;
                isGround = value;

                if (b && onJointIsGroundChanged != null)
                    onJointIsGroundChanged.Invoke(this, isGround, !isGround);
            }
        }

        public bool CanRemove = true;
        public List<Component> ContainingComponents = new List<Component>();

        internal static short _TypeID = 0;
        public static short TypeID
        {
            get { return _TypeID; }
        }

        internal int localProviderInd, localGroundInd;
        public int LocalProviderInd
        {
            get { return localProviderInd; }
        }
        public int LocalGroundInd
        {
            get { return localGroundInd; }
        }

        #region Events
        public delegate void JointIsGroundChanged(Joint j, bool v_new, bool v_old);
        public event JointIsGroundChanged onJointIsGroundChanged;
        public delegate void JointIsProviderChanged(Joint j, bool v_new, bool v_old);
        public event JointIsProviderChanged onJointIsProviderChanged;
        public delegate void JointSendingVoltageChanged(Joint j, double v_new, double v_old);
        public event JointSendingVoltageChanged onJointSendingVoltageChanged;
        #endregion



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
            Logics = new Logics.JointLogics();
            Graphics = new Graphics.JointGraphics();
            Graphics.parent = this;
            Logics.parent = this;
        }

        public Joint()
        {
            constructor();
        }

        private Joint(Vector2 position)
        {
            constructor();
            MicroWorld.Logics.GridHelper.GridCoords(ref position);
            Graphics.Position = position;
        }

        private Joint(float x, float y)
        {
            constructor();
            MicroWorld.Logics.GridHelper.GridCoords(ref x, ref y);
            Graphics.Position = new Vector2(x, y);
        }

        public static Joint GetJoint(float x, float y, bool init = true)
        {
            return GetJoint(new Vector2(x, y), init);
        }

        public static Joint GetJoint(Vector2 position, bool init = true)
        {
            var a = MicroWorld.Logics.GameInputHandler.GetJointForCoord((int)position.X, (int)position.Y, init);
            if (a != null)
            {
                return a;
            }
            return new Joint(position);
        }

        internal static Joint GetJointNoCheck(Vector2 position)
        {
            return GetJointNoCheck(position.X, position.Y);
        }

        internal static Joint GetJointNoCheck(float x, float y)
        {
            return new Joint(x, y);
        }

        public override void Initialize()
        {
            MicroWorld.Logics.CircuitManager.Joints.Add(this);
            base.Initialize();
        }

        public override bool CanPlace(int x, int y, int w, int h)
        {
            return Components.ComponentsManager.VisibilityMap.IsFree(x + 4, y + 4, 1, 1) &&
                   MicroWorld.Logics.PlacableAreasManager.IsPlacable(x, y, w, h);
        }

        public override void SetComponentOnVisibilityMap()
        {

            if (!Graphics.Visible) return;
            ComponentsManager.VisibilityMap.Generate(Graphics.Position.X, Graphics.Position.Y);
            //ComponentsManager.MapVisibility.SetRectangle(Graphics.Position.X, Graphics.Position.Y, Graphics.Size.X + 4, Graphics.Size.Y + 4, 2);
            ComponentsManager.VisibilityMap.SetPoint(this, Graphics.Position.X + 4, Graphics.Position.Y + 4);
        }

        public override void RemoveComponentFromVisibilityMap()
        {
            if (!Graphics.Visible) return;
            ComponentsManager.VisibilityMap.Generate(Graphics.Position.X, Graphics.Position.Y);
            //ComponentsManager.MapVisibility.SetRectangle(Graphics.Position.X, Graphics.Position.Y, Graphics.Size.X + 4, Graphics.Size.Y + 4, 0);
            ComponentsManager.VisibilityMap.SetPoint(null, Graphics.Position.X + 4, Graphics.Position.Y + 4);
        }

        public override void NonGameUpdate()
        {
            base.NonGameUpdate();

            if (!Graphics.Visible) return;

            if (GetComponentToolTip().isVisible && 
                MicroWorld.Graphics.GUI.GUIEngine.GetHUDSceneTypeCount<GUI.GeneralProperties>() > 1 &&
                MicroWorld.Graphics.GUI.GUIEngine.GetHUDSceneTypeCount<GUI.WireProperties>() == 0)
            {
                var a = MicroWorld.Graphics.GUI.GUIEngine.GetAllHUDSceneType<GUI.GeneralProperties>();
                for (int i = 0; i < a.Length; i++)
                    if (a[i] != GetComponentToolTip() && a[i].fadeState != GUI.GeneralProperties.FadeState.FadeOut)
                    {
                        MicroWorld.Graphics.GUI.GUIEngine.RemoveHUDScene(GetComponentToolTip());
                        break;
                    }
            }

            if (MicroWorld.Graphics.GUI.GUIEngine.ContainsHUDScene(MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons) &&
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent == this)
            {
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.bRemove.isEnabled = CanRemove;
            }
        }

        public override bool  CanDragDrop()
        {
 	        return IsRemovable && ContainingComponents.Count == 0;
        }

        public override void OnMove(int dx, int dy)
        {
            //check for merge
            var a = new Vector2(Graphics.Position.X + dx, Graphics.Position.Y + dy);
            Joint j;
            if ((j = MicroWorld.Logics.GameInputHandler.HasJointAtCoord((int)a.X, (int)a.Y)) != null && ContainingComponents.Count == 0 && j.ContainingComponents.Count == 0)
            {
                for (int i = 0; i < ConnectedWires.Count; i++)
                {
                    if (ConnectedWires[i].J1.ID == ID)
                    {
                        ConnectedWires[i].J1 = j;
                    }
                    else
                    {
                        ConnectedWires[i].J2 = j;
                    }
                    //if (ConnectedWires[i].Graphics.Visible)
                    //{
                    ConnectedWires[i].RemoveComponentFromVisibilityMap();
                    if (ConnectedWires[i].J1.ID == ConnectedWires[i].J2.ID)
                    {
                        ConnectedWires[i].Remove();
                        ConnectedWires.RemoveAt(i);
                        i--;
                        continue;
                    }
                    ConnectedWires[i].Initialize();
                    if ((ConnectedWires[i].Graphics as Graphics.WireGraphics).DrawPath.Count == 0)
                    {
                        ConnectedWires[i].Remove();
                        ConnectedWires.RemoveAt(i);
                        i--;
                        continue;
                    }
                    ConnectedWires[i].SetComponentOnVisibilityMap();
                    //}
                    j.ConnectedWires.Add(ConnectedWires[i]);
                    ConnectedWires.RemoveAt(0);
                    i--;
                }
                Remove();
                return;
            }

            base.OnMove(dx, dy);
            for (int i = 0; i < ConnectedWires.Count; i++)
            {
                if (ConnectedWires[i].Graphics.Visible)
                {
                    ConnectedWires[i].RemoveComponentFromVisibilityMap();
                    if (ConnectedWires[i].J1.Graphics.Position == ConnectedWires[i].J2.Graphics.Position)
                    {
                        ConnectedWires[i].Remove();
                        ConnectedWires.RemoveAt(i);
                        i--;
                        continue;
                    }
                    ConnectedWires[i].Graphics.Initialize();
                    if ((ConnectedWires[i].Graphics as Graphics.WireGraphics).DrawPath.Count == 0)
                    {
                        int t = ConnectedWires.Count;
                        ConnectedWires[i].Remove();
                        if (t == ConnectedWires.Count)
                            ConnectedWires.RemoveAt(i);
                        i--;
                        continue;
                    }
                    ConnectedWires[i].SetComponentOnVisibilityMap();
                }
            }
        }

        public override void OnPlaced()
        {
            var a = MicroWorld.Logics.GameInputHandler.GetWiresForCoord((int)Graphics.Position.X, (int)Graphics.Position.Y);
            for (int i = 0; i < a.Count; i++)
            {
                MicroWorld.Logics.GameInputHandler.SplitWire(a[i], this);
            }
            base.OnPlaced();
        }

        public override void Remove()
        {
            if (!CanRemove || !IsRemovable) return;
            if (ContainingComponents.Count != 0) return;
            for (int i = 0; i < ConnectedWires.Count; i++)
            {
                if (!ConnectedWires[i].IsRemovable) return;
            }
            while (ConnectedWires.Count > 0)
                ConnectedWires[0].Remove();

            MicroWorld.Logics.CircuitManager.Joints.Remove(this);

            base.Remove();
        }

        public override void Reset()
        {
            base.Reset();
            OldSendingVoltage = -0.1;
            Voltage = 0;
            WasEMPd = false;
        }

        public override string GetName()
        {
            return "Joint";
        }

        public override int[] getJoints()
        {
            return new int[] { ID };
        }

        public static void LoadContentStatic()
        {
            Components.Graphics.JointGraphics.LoadContentStatic();
        }

        //===============================================LOGICS===========================================

        public List<Wire> ConnectedWires = new List<Wire>();
        public double Voltage = 0;
        private double SendingCurrent = 0;
        internal bool IsUpdated = false;
        internal bool IsInSubCircuit = false;

        internal double OldSendingVoltage = 0;


        //=============================================================ID==============================================================


        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);

            String s = "";
            for (int i = 0; i < ConnectedWires.Count; i++)
            {
                s += ConnectedWires[i].ID + ";";
            }
            if (s.Length > 0)
                s = s.Substring(0, s.Length - 1);
            Compound.Add("ConnectedWires", s);

            Compound.Add("CanGround", CanBeGround);
            Compound.Add("CanPower", CanProvidePower);
            Compound.Add("IsGround", isGround);
            Compound.Add("IsPower", isProvidingPower);

            Compound.Add("SendingVoltage", SendingVoltage);
            Compound.Add("SendingCurrent", SendingCurrent);

            /*
            s = "";
            for (int i = 0; i < ContainingComponents.Count; i++)
            {
                s += ContainingComponents[i].ID.ToString() + ";";
            }
            if (s.Length > 0)
                s = s.Substring(0, s.Length - 1);
            Compound.Add("ContainingComponents", s);//*/
        }

        private String connectedWiresData;//, containingComponentsData;
        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            connectedWiresData = Compound.GetString("ConnectedWires");
            //containingComponentsData = Compound.GetString("ContainingComponents");

            CanBeGround = Compound.GetBool("CanGround");
            CanProvidePower = Compound.GetBool("CanPower");
            isGround = Compound.GetBool("IsGround");
            isProvidingPower = Compound.GetBool("IsPower");

            SendingVoltage = Compound.GetDouble("SendingVoltage");
            SendingCurrent = Compound.GetDouble("SendingCurrent");
        }

        public override void PostLoad()
        {
            base.PostLoad();

            ConnectedWires.Clear();
            if (connectedWiresData != null && connectedWiresData != "")
            {
                var a = connectedWiresData.Split(';');
                for (int i = 0; i < a.Length; i++)
                {
                    ConnectedWires.Add(ComponentsManager.GetComponent(Convert.ToInt32(a[i])) as Wire);
                }
            }
            /*
            ContainingComponents.Clear();
            if (containingComponentsData != null && containingComponentsData != "")
            {
                var a = containingComponentsData.Split(';');
                for (int i = 0; i < a.Length; i++)
                {
                    ContainingComponents.Add(ComponentsManager.GetComponent(Convert.ToInt32(a[i])) as Wire);
                }
            }//*/
        }

    }
}
