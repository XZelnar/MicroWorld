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
    class VoltageGraph : Component, Properties.IDrawBorder
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            -4, 20,
            68, 20
        };
        public static int[] JointLocs90cw = new int[]{
            20, -4,
            20, 68
        };
        public static int[] JointLocs180cw = new int[]{
            68, 20,
            -4, 20
        };
        public static int[] JointLocs270cw = new int[]{
            20, 68,
            20, -4
        };
        #endregion

        public MicroWorld.Components.Joint[] Joints = new Joint[2];
        public Wire W;

        private void constructor()
        {
            Logics = new Logics.VoltageGraphLogics();
            Graphics = new Graphics.VoltageGraphGraphics();
            Graphics.parent = this;
            Logics.parent = this;
            ToolTip = new GUI.VoltageGraphProperties();
        }

        public VoltageGraph()
        {
            constructor();
        }

        public VoltageGraph(float x, float y)
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
                        Joints[i] = Joint.GetJoint(new Vector2(JointLocs180cw[i * 2], JointLocs180cw[i * 2 + 1]) + Graphics.Position);
                        break;
                    case Rotation.cw270:
                        Joints[i] = Joint.GetJoint(new Vector2(JointLocs270cw[i * 2], JointLocs270cw[i * 2 + 1]) + Graphics.Position);
                        break;
                    default:
                        break;
                }
                Joints[i].CanRemove = false;
                Joints[i].ContainingComponents.Add(this);
            }

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].Initialize();
            }

            W = new Wire(Joints[0], Joints[1]);
            //W.Direction = Wire.WireDirection.J1ToJ2;
            W.Resistance = Settings.MAX_RESISTANCE;
            W.AddComponentToManager();
            W.Graphics.Visible = false;
            W.Initialize();
        }

        public override void AddComponentToManager()
        {
            ID = ComponentsManager.GetFreeID();
            base.AddComponentToManager();
        }

        public static void LoadContentStatic()
        {
            Components.Graphics.VoltageGraphGraphics.LoadContentStatic();
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
            return "Voltage Drop Graph";
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

        
        //==============================================================IO==========================================================


        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);

            Compound.Add("J0", Joints[0].ID);
            Compound.Add("J1", Joints[1].ID);
            Compound.Add("W", W.ID);
            Compound.Add("Min", (Logics as Logics.VoltageGraphLogics).min);
            Compound.Add("Max", (Logics as Logics.VoltageGraphLogics).max);
            Compound.Add("Frequency", (Logics as Logics.VoltageGraphLogics).frequency);
        }

        private int j0, j1, j4, w1;

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            j0 = Compound.GetInt("J0");
            j1 = Compound.GetInt("J1");
            w1 = Compound.GetInt("W");

            (Logics as Logics.VoltageGraphLogics).min = Compound.GetInt("Min");
            (Logics as Logics.VoltageGraphLogics).max = Compound.GetInt("Max");
            (Logics as Logics.VoltageGraphLogics).frequency = Compound.GetInt("Frequency");
        }

        public override void PostLoad()
        {
            base.PostLoad();

            Joints[0] = (Joint)Components.ComponentsManager.GetComponent(j0);
            Joints[1] = (Joint)Components.ComponentsManager.GetComponent(j1);
            W = (Wire)Components.ComponentsManager.GetComponent(w1);

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].ContainingComponents.Add(this);
            }
        }
    }
}
