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
    class AdvancedJoint : Component, Properties.IDrawBorder, Properties.IAttractsLightning
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            -4, 12,
            12, -4,
            28, 12,
            12, 28,
            0, 0,
            16, 0,
            16, 16,
            0, 16
        };
        #endregion

        public MicroWorld.Components.Joint[] Joints = new Joint[8];
        public MicroWorld.Components.Wire[] Wires = new Wire[4];
        private PortState left = PortState.Input, up = PortState.Input, right = PortState.Input, down = PortState.Input;

        internal PortState Left
        {
            get { return left; }
            set
            {
                left = value;
                //Joints[4].CanBeGround = value == PortState.Input;
                Joints[4].IsGround = value == PortState.Input;
                //Joints[4].CanProvidePower = value == PortState.Output;
                Joints[4].IsProvidingPower = value == PortState.Output;
                Wires[0].Resistance = (value == PortState.Output) ? 5 : 400;
            }
        }
        internal PortState Up
        {
            get { return up; }
            set
            {
                up = value;
                //Joints[5].CanBeGround = value == PortState.Input;
                Joints[5].IsGround = value == PortState.Input;
                //Joints[5].CanProvidePower = value == PortState.Output;
                Joints[5].IsProvidingPower = value == PortState.Output;
                Wires[1].Resistance = (value == PortState.Output) ? 5 : 400;
            }
        }
        internal PortState Right
        {
            get { return right; }
            set
            {
                right = value;
                //Joints[6].CanBeGround = value == PortState.Input;
                Joints[6].IsGround = value == PortState.Input;
                //Joints[6].CanProvidePower = value == PortState.Output;
                Joints[6].IsProvidingPower = value == PortState.Output;
                Wires[2].Resistance = (value == PortState.Output) ? 5 : 400;
            }
        }
        internal PortState Down
        {
            get { return down; }
            set
            {
                down = value;
                //Joints[7].CanBeGround = value == PortState.Input;
                Joints[7].IsGround = value == PortState.Input;
                //Joints[7].CanProvidePower = value == PortState.Output;
                Joints[7].IsProvidingPower = value == PortState.Output;
                Wires[3].Resistance = (value == PortState.Output) ? 5 : 400;
            }
        }


        #region IAttractsLightning
        public void GetStruck(Component origin, float voltage, int time)
        {
            (Logics as Logics.AdvancedJointLogics).AddSource(new Logics.AdvancedJointLogics.VoltageSource(time, voltage));
        }
        #endregion

        protected void constructor()
        {
            Logics = new Logics.AdvancedJointLogics();
            Graphics = new Graphics.AdvancedJointGraphics();
            Graphics.parent = this;
            Logics.parent = this;
            ToolTip = new GUI.AdvancedJointProperties();
        }

        public AdvancedJoint()
        {
            constructor();
        }

        public AdvancedJoint(float x, float y)
        {
            constructor();
            Graphics.Position = new Vector2(x, y);
        }

        public void UpdateIO()
        {
            Left = Left;
            Up = Up;
            Right = Right;
            Down = Down;
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
                        break;
                    case Rotation.cw180:
                        break;
                    case Rotation.cw270:
                        break;
                    default:
                        break;
                }
                Joints[i].CanRemove = false;

                Joints[i].CanRemove = false;
                Joints[i].ContainingComponents.Add(this);
            }

            for (int i = 4; i < Joints.Length; i++)
            {
                Joints[i].CanBeGround = true;
                Joints[i].CanProvidePower = true;
                Joints[i].IsGround = true;
                Joints[i].Graphics.Visible = false;
            }

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].Initialize();
            }

            for (int i = 0; i < Wires.Length; i++)
            {
                Wires[i] = new Wire(Joints[i], Joints[i + 4]);
                Wires[i].Resistance = 400;
                Wires[i].AddComponentToManager();
                Wires[i].Graphics.Visible = false;
                Wires[i].IsRemovable = false;
                Wires[i].Initialize();
            }

            UpdateIO();
        }

        public override void AddComponentToManager()
        {
            ID = ComponentsManager.GetFreeID();
            base.AddComponentToManager();
        }

        public static void LoadContentStatic()
        {
            Components.Graphics.AdvancedJointGraphics.LoadContentStatic();
        }

        public override void Remove()
        {
            if (!IsRemovable) return;

            if (Wires[0] != null)
                for (int i = 0; i < Wires.Length; i++)
                    Wires[i].IsRemovable = true;

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].IsRemovable = true;
                Joints[i].ContainingComponents.Remove(this);
            }

            if (Wires[0] != null)
                for (int i = 0; i < Wires.Length; i++)
                    Wires[i].Remove();

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
            Vector2 p;
            for (int i = 0; i < Joints.Length; i++)
                Joints[i].OnMove(dx, dy);
            SetComponentOnVisibilityMap();
        }

        public override string GetName()
        {
            return "Advanced Joint";
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
                    return new int[] { JointLocs0cw[0], JointLocs0cw[1], JointLocs0cw[2], JointLocs0cw[3], JointLocs0cw[4], JointLocs0cw[5], JointLocs0cw[6], 
                        JointLocs0cw[7] };
                case Rotation.cw90:
                case Rotation.cw180:
                case Rotation.cw270:
                default:
                    return new int[0];
            }
        }

        public override void Reset()
        {
            Joints[4].SendingVoltage = 0;
            Joints[5].SendingVoltage = 0;
            Joints[6].SendingVoltage = 0;
            Joints[7].SendingVoltage = 0;
            base.Reset();
        }

        public override Joint[] FindAccessibleJoints(Joint from)
        {
            if (from == Joints[4] || from == Joints[5] || from == Joints[6] || from == Joints[7])
                return new Joint[0];

            if ((from == Joints[0] && left == PortState.Input) ||
                (from == Joints[1] && up == PortState.Input) ||
                (from == Joints[2] && right == PortState.Input) ||
                (from == Joints[3] && down == PortState.Input))
                return new Joint[0];

            List<Joint> t = new List<Joint>();

            if (from != Joints[0] && left != PortState.Input)
                t.Add(Joints[0]);
            if (from != Joints[1] && up != PortState.Input)
                t.Add(Joints[1]);
            if (from != Joints[2] && right != PortState.Input)
                t.Add(Joints[2]);
            if (from != Joints[3] && down != PortState.Input)
                t.Add(Joints[3]);

            return t.ToArray();
        }

        //============================================================LOGICS========================================================


        //==============================================================IO==========================================================


        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);

            for (int i = 0; i < Joints.Length; i++)
            {
                Compound.Add("J" + i.ToString(), Joints[i].ID);
            }

            for (int i = 0; i < Wires.Length; i++)
            {
                Compound.Add("W" + i.ToString(), Wires[i].ID);
            }

            Compound.Add("Left", left.GetHashCode());
            Compound.Add("Up", up.GetHashCode());
            Compound.Add("Right", right.GetHashCode());
            Compound.Add("Down", down.GetHashCode());
        }

        private int[] j = new int[8], w = new int[4];

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            for (int i = 0; i < Joints.Length; i++)
            {
                j[i] = Compound.GetInt("J" + i.ToString());
            }

            for (int i = 0; i < Wires.Length; i++)
            {
                w[i] = Compound.GetInt("W" + i.ToString());
            }

            left = (PortState)Compound.GetInt("Left");
            up = (PortState)Compound.GetInt("Up");
            right = (PortState)Compound.GetInt("Right");
            down = (PortState)Compound.GetInt("Down");
        }

        public override void PostLoad()
        {
            base.PostLoad();

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i] = (Joint)Components.ComponentsManager.GetComponent(j[i]);
                Joints[i].ContainingComponents.Add(this);
            }

            for (int i = 0; i < Wires.Length; i++)
            {
                Wires[i] = (Wire)Components.ComponentsManager.GetComponent(w[i]);
            }
        }
    }
}
