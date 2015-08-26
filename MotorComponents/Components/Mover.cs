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
    class Mover : Component, Properties.IDrawBorder, Properties.IRotator, Properties.IMover
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            -4, 28,
            36, 28
        };
        public static int[] JointLocs90cw = new int[]{
            4, -4,
            4, 36
        };
        public static int[] JointLocs180cw = new int[]{
            36, 4,
            -4, 4
        };
        public static int[] JointLocs270cw = new int[]{
            28, 36,
            28, -4
        };
        #endregion

        public Component connectedComponent;
        public RotatableConnector connector;

        public MicroWorld.Components.Joint[] Joints = new Joint[2];
        public Wire W;
        protected float resistance = 500;//Ohm
        public float Resistance
        {
            get { return resistance; }
            set
            {
                resistance = value;
                if (resistance < 1) resistance = 1;
                if (resistance > Settings.MAX_RESISTANCE) resistance = (float)Settings.MAX_RESISTANCE;
                W.Resistance = resistance;
            }
        }


        #region IMotor
        public void Rotate(float delta)
        {
            (Logics as Logics.MotorLogics).Angle += delta;
            if (connectedComponent != null)
                (connectedComponent as Properties.IRotatable).Rotate(Graphics.Position + Graphics.GetSizeRotated(ComponentRotation) / 2, 
                    delta);
        }
        
        public void Connect(Component c, RotatableConnector rc)
        {
            connector = rc;
            connectedComponent = c;
        }

        public bool CanConnect(Component c)
        {
            return connector == null && c is Properties.IRotatable;
        }

        public void DrawTrajectory(MicroWorld.Graphics.Renderer renderer)
        {
            (Graphics as Graphics.MotorGraphics).DrawTrajectory(renderer);
        }

        public void Disconnect()
        {
            connector = null;
            connectedComponent = null;
        }

        public RotatableConnector GetConnector()
        {
            return connector;
        }
        #endregion




        protected void constructor()
        {
            Logics = new Logics.MoverLogics();
            Graphics = new Graphics.MoverGraphics();
            Graphics.parent = this;
            Logics.parent = this;
        }

        public Mover()
        {
            constructor();
        }

        public Mover(float x, float y)
        {
            constructor();
            Graphics.Position = new Vector2(x, y);
        }

        static short frameType = -1;
        static short[] acceptableTypes;
        public override bool CanPlace(int x, int y, int w, int h)
        {
            if (frameType == -1)
            {
                frameType = Shortcuts.GetComponentTypeID(typeof(Frame));
                acceptableTypes = new short[] { 0, frameType };
            }
            bool b = Components.ComponentsManager.VisibilityMap.IsFree(x - 4, y - 4, w + 9, h + 9) &&
                   MicroWorld.Logics.PlacableAreasManager.IsPlacable(x - 4, y - 4, w + 9, h + 9);//modified base.CanPlace
            if (b)
            {
                switch (MicroWorld.Logics.GameInputHandler.GhostRotation)
                {
                    case Rotation.cw0:
                        return Components.ComponentsManager.VisibilityMap.IsOfTypes(x + 4, y - 12, w - 4, 1, acceptableTypes) &&
                            MicroWorld.Logics.PlacableAreasManager.IsPlacable(x, y, w, h);
                    case Rotation.cw90:
                        return Components.ComponentsManager.VisibilityMap.IsOfTypes(x + w + 12, y + 4, 1, h - 4, acceptableTypes) &&
                            MicroWorld.Logics.PlacableAreasManager.IsPlacable(x, y, w, h);
                    case Rotation.cw180:
                        return Components.ComponentsManager.VisibilityMap.IsOfTypes(x + 4, y + w + 12, w - 4, 1, acceptableTypes) &&
                            MicroWorld.Logics.PlacableAreasManager.IsPlacable(x, y, w, h);
                    case Rotation.cw270:
                        return Components.ComponentsManager.VisibilityMap.IsOfTypes(x - 12, y + 4, 1, h - 4, acceptableTypes) &&
                            MicroWorld.Logics.PlacableAreasManager.IsPlacable(x, y, w, h);
                    default:
                        break;
                }
            }
            return false;
        }

        public override void InitAddChildComponents()
        {
            for (int i = 0; i < Joints.Length; i++)
            {
                switch (rotation)
                {
                    case Rotation.cw0:
                        Joints[i] = Joint.GetJoint(new Vector2(JointLocs0cw[i * 2], JointLocs0cw[i * 2 + 1]) + Graphics.Position);
                        break;
                    case Rotation.cw90:
                        Joints[i] = Joint.GetJoint(new Vector2(JointLocs90cw[i * 2], JointLocs90cw[i * 2 + 1]) + Graphics.Position);
                        break;
                    case Rotation.cw180:
                        Joints[i] = Joint.GetJoint(new Vector2(JointLocs180cw[i * 2], JointLocs180cw[i * 2 + 1]) + Graphics.Position);
                        break;
                    case Rotation.cw270:
                        Joints[i] = Joint.GetJoint(new Vector2(JointLocs270cw[i * 2], JointLocs270cw[i * 2 + 1]) + Graphics.Position);
                        break;
                    default:
                        break;
                }
                Joints[i].Initialize();
                Joints[i].CanRemove = false;
                Joints[i].ContainingComponents.Add(this);
            }

            W = new Wire(Joints[0], Joints[1]);
            W.Resistance = Resistance;
            W.AddComponentToManager();
            W.Graphics.Visible = false;
            W.Initialize();
        }

        public override void AddComponentToManager()
        {
            //ComponentsManager.Remove(this);
            ID = ComponentsManager.GetFreeID();
            //ComponentsManager.Add(this);
            base.AddComponentToManager();
        }

        public static void LoadContentStatic()
        {
            Components.Graphics.MoverGraphics.LoadContentStatic();
        }

        public override void Remove()
        {
            if (!IsRemovable) return;
            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].IsRemovable = true;
                Joints[i].ContainingComponents.Remove(this);
            }
            W.IsRemovable = true;

            W.Remove();
            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].CanRemove = true;
                if (Joints[i].ConnectedWires.Count == 0)
                {
                    Joints[i].Remove();
                }
            }

            var a = connector;
            connectedComponent = null;
            if (a != null) a.Disconnect();//leave this as is. prevents stack overflow
            base.Remove();
        }

        public override bool CanDragDrop()
        {
            return true;
        }

        public override void OnMove(int dx, int dy)
        {
            base.OnMove(dx, dy);
            for (int i = 0; i < Joints.Length; i++)
                Joints[i].OnMove(dx, dy);
            SetComponentOnVisibilityMap();
        }

        public override string GetName()
        {
            return "Mover";
        }

        public override int[] getJoints()
        {
            int[] r = new int[Joints.Length];
            for (int i = 0; i < r.Length; i++)
            {
                r[i] = Joints[i].ID;
            }
            return r;
        }

        public override int[] GetJointCoords(Component.Rotation r)
        {
            switch (r)
            {
                case Rotation.cw0:
                    return new int[] { JointLocs0cw[0], JointLocs0cw[1], JointLocs0cw[2], JointLocs0cw[3] };
                case Rotation.cw90:
                    return new int[] { JointLocs90cw[0], JointLocs90cw[1], JointLocs90cw[2], JointLocs90cw[3] };
                case Rotation.cw180:
                    return new int[] { JointLocs180cw[0], JointLocs180cw[1], JointLocs180cw[2], JointLocs180cw[3] };
                case Rotation.cw270:
                    return new int[] { JointLocs270cw[0], JointLocs270cw[1], JointLocs270cw[2], JointLocs270cw[3] };
                default:
                    return new int[0];
            }
        }

        public override Joint[] FindAccessibleJoints(Joint from)
        {
            return new Joint[] { from == Joints[0] ? Joints[1] : Joints[0] };
        }

        //============================================================LOGICS========================================================


        //==============================================================IO==========================================================


        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);

            Compound.Add("J0", Joints[0].ID);
            Compound.Add("J1", Joints[1].ID);
            Compound.Add("W", W.ID);

            Compound.Add("Resistance", Resistance);


            if (connectedComponent == null)
            {
                Compound.Add("ConComp", -1);
                Compound.Add("Connector", -1);
            }
            else
            {
                Compound.Add("ConComp", connectedComponent.ID);
                Compound.Add("Connector", connector.ID);
            }
        }

        private int j0, j1, w;
        private float res;
        int con, com;

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            j0 = Compound.GetInt("J0");
            j1 = Compound.GetInt("J1");
            w = Compound.GetInt("W");

            res = (float)Compound.GetDouble("Resistance");

            con = Compound.GetInt("Connector");
            com = Compound.GetInt("ConComp");
        }

        public override void PostLoad()
        {
            base.PostLoad();

            Joints[0] = (Joint)Components.ComponentsManager.GetComponent(j0);
            Joints[1] = (Joint)Components.ComponentsManager.GetComponent(j1);
            W = (Wire)Components.ComponentsManager.GetComponent(w);
            Resistance = res;
            if (com != -1)
            {
                connectedComponent = ComponentsManager.GetComponent(com);
                connector = ComponentsManager.GetComponent(con) as RotatableConnector;
            }

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].ContainingComponents.Add(this);
            }
        }
    }
}
