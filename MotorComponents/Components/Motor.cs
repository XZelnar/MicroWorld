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
    class Motor : Component, Properties.IDrawBorder, Properties.IRotator
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            -4, 20,
            52, 20
        };
        public static int[] JointLocs90cw = new int[]{
            20, -4,
            20, 52
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
            Logics = new Logics.MotorLogics();
            Graphics = new Graphics.MotorGraphics();
            Graphics.parent = this;
            Logics.parent = this;

            ToolTip = new GUI.MotorProperties();
        }

        public Motor()
        {
            constructor();
        }

        public Motor(float x, float y)
        {
            constructor();
            Graphics.Position = new Vector2(x, y);
        }

        public override void Update()
        {
            base.Update();
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
                        break;
                    case Rotation.cw270:
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
            Components.Graphics.MotorGraphics.LoadContentStatic();
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
            return "Motor";
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
                case Rotation.cw270:
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
