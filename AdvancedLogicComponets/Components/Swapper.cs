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
    class Swapper : Component, Properties.IDrawBorder
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            -4, 12,
            -4, 28,
            60, 12,
            60, 28,
            28, -4,
            28, 44,
            24, 8,
            24, 24
        };
        public static int[] JointLocs90cw = new int[]{
            28, -4,
            12, -4,
            28, 60,
            12, 60,
            44, 28,
            -4, 28,
            24, 24,
            8, 24
        };
        #endregion

        public bool Swapped
        {
            get { return Wires[2].IsConnected; }
            set
            {
                Wires[0].IsConnected = !value;
                Wires[1].IsConnected = !value;
                Wires[2].IsConnected = value;
                Wires[3].IsConnected = value;
            }
        }
        internal bool OrigSwapped = false;

        public MicroWorld.Components.Joint[] Joints = new Joint[8];
        public MicroWorld.Components.Wire[] Wires = new Wire[6];

        private void constructor()
        {
            Logics = new Logics.SwapperLogics();
            Graphics = new Graphics.SwapperGraphics();
            Graphics.parent = this;
            Logics.parent = this;
            ToolTip = new GUI.SwapperProperties();
        }

        public Swapper()
        {
            constructor();
        }

        public Swapper(float x, float y)
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
                Joints[i].CanRemove = false;
                Joints[i].ContainingComponents.Add(this);
            }

            Joints[6].Graphics.Visible = false;
            Joints[7].Graphics.Visible = false;

            Joints[6].CanBeGround = true;
            Joints[7].CanBeGround = true;
            Joints[6].IsGround = true;
            Joints[7].IsGround = true;

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].Initialize();
            }

            Wires[0] = new Wire(Joints[0], Joints[2]);
            Wires[0].AddComponentToManager();
            Wires[1] = new Wire(Joints[1], Joints[3]);
            Wires[1].AddComponentToManager();
            Wires[2] = new Wire(Joints[0], Joints[3]);
            Wires[2].AddComponentToManager();
            Wires[3] = new Wire(Joints[1], Joints[2]);
            Wires[3].AddComponentToManager();
            Wires[4] = new Wire(Joints[4], Joints[6]);
            Wires[4].AddComponentToManager();
            Wires[5] = new Wire(Joints[5], Joints[7]);
            Wires[5].AddComponentToManager();

            Wires[2].IsConnected = false;
            Wires[3].IsConnected = false;

            for (int i = 0; i < Wires.Length; i++)
            {
                Wires[i].Resistance = 10;
                Wires[i].Graphics.Visible = false;
                Wires[i].IsRemovable = false;
                Wires[i].Initialize();
            }

            Wires[4].Resistance = 400;
            Wires[5].Resistance = 400;
        }

        public override void AddComponentToManager()
        {
            ID = ComponentsManager.GetFreeID();
            base.AddComponentToManager();
        }

        public static void LoadContentStatic()
        {
            Components.Graphics.SwapperGraphics.LoadContentStatic();
        }

        public override void Remove()
        {
            if (!IsRemovable) return;
            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].IsRemovable = true;
                Joints[i].ContainingComponents.Remove(this);
            }

            if (Wires[0] != null)
            {
                for (int i = 0; i < Wires.Length; i++)
                    Wires[i].IsRemovable = true;
                for (int i = 0; i < Wires.Length; i++)
                    Wires[i].Remove();
            }

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
            return "Swapper";
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
                    return new int[] { JointLocs0cw[0], JointLocs0cw[1], JointLocs0cw[2], JointLocs0cw[3], JointLocs0cw[4], JointLocs0cw[5], 
                        JointLocs0cw[6], JointLocs0cw[7], JointLocs0cw[8], JointLocs0cw[9], JointLocs0cw[10], JointLocs0cw[11] };
                case Rotation.cw90:
                    return new int[] { JointLocs90cw[0], JointLocs90cw[1], JointLocs90cw[2], JointLocs90cw[3], JointLocs90cw[4], JointLocs90cw[5], 
                        JointLocs90cw[6], JointLocs90cw[7], JointLocs90cw[8], JointLocs90cw[9], JointLocs90cw[10], JointLocs90cw[11] };
                case Rotation.cw180:
                case Rotation.cw270:
                default:
                    return new int[0];
            }
        }

        public override Joint[] FindAccessibleJoints(Joint from)
        {
            for (int i = 0; i < 4; i++)
                if (Wires[i].IsConnected)
                {
                    if (from == Wires[i].J1)
                        return new Joint[] { Wires[i].J2 };
                    if (from == Wires[i].J2)
                        return new Joint[] { Wires[i].J1 };
                }
            return new Joint[0];
        }

        //============================================================LOGICS========================================================


        public override void Start()
        {
            OrigSwapped = Swapped;

            base.Start();
        }

        public override void Reset()
        {
            Swapped = OrigSwapped;

            base.Reset();
        }

        
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

            Compound.Add("Swapped", Swapped);
            Compound.Add("OrigSwapped", OrigSwapped);
        }

        private int[] j = new int[8], w = new int[6];
        bool sw = false;

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

            sw = Compound.GetBool("Swapped");
            OrigSwapped = Compound.GetBool("OrigSwapped");
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

            Swapped = sw;
        }
    }
}
