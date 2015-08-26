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
    class Hub : Component, Properties.IDrawBorder
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            -4, 20,
            20, -4,
            44, 20,
            20, 44
        };
        #endregion

        public MicroWorld.Components.Joint[] Joints = new Joint[4];//left, up, right, down
        public Wire[] Wires = new Wire[6];
        //left-up
        //left-right
        //left-down
        //up-right
        //up-down
        //right-down

        private bool connectedLeft = true, connectedRight = true, connectedUp = true, connectedDown = true;

        public bool ConnectedLeft
        {
            get { return connectedLeft; }
            set
            {
                connectedLeft = value;
                UpdateWiresConnectivity();
            }
        }
        public bool ConnectedUp
        {
            get { return connectedUp; }
            set
            {
                connectedUp = value;
                UpdateWiresConnectivity();
            }
        }
        public bool ConnectedRight
        {
            get { return connectedRight; }
            set
            {
                connectedRight = value;
                UpdateWiresConnectivity();
            }
        }
        public bool ConnectedDown
        {
            get { return connectedDown; }
            set
            {
                connectedDown = value;
                UpdateWiresConnectivity();
            }
        }

        private bool leftUp = true, leftRight = true, leftDown = true, upRight = true, upDown = true, rightDown = true;

        public bool RightDown
        {
            get { return rightDown; }
            set
            {
                rightDown = value;
                UpdateWiresConnectivity();
            }
        }
        public bool UpDown
        {
            get { return upDown; }
            set
            {
                upDown = value;
                UpdateWiresConnectivity();
            }
        }
        public bool UpRight
        {
            get { return upRight; }
            set
            {
                upRight = value;
                UpdateWiresConnectivity();
            }
        }
        public bool LeftDown
        {
            get { return leftDown; }
            set
            {
                leftDown = value;
                UpdateWiresConnectivity();
            }
        }
        public bool LeftRight
        {
            get { return leftRight; }
            set
            {
                leftRight = value;
                UpdateWiresConnectivity();
            }
        }
        public bool LeftUp
        {
            get { return leftUp; }
            set
            {
                leftUp = value;
                UpdateWiresConnectivity();
            }
        }

        private PortState up = PortState.Both, down = PortState.Both, left = PortState.Both, right = PortState.Both;

        public PortState Up
        {
            get { return up; }
            set
            {
                up = value;
                UpdateWiresConnectivity();
            }
        }
        public PortState Down
        {
            get { return down; }
            set
            {
                down = value;
                UpdateWiresConnectivity();
            }
        }
        public PortState Left
        {
            get { return left; }
            set
            {
                left = value;
                UpdateWiresConnectivity();
            }
        }
        public PortState Right
        {
            get { return right; }
            set
            {
                right = value;
                UpdateWiresConnectivity();
            }
        }
            
        private void constructor()
        {
            Logics = new Logics.HubLogics();
            Graphics = new Graphics.HubGraphics();
            Graphics.parent = this;
            Logics.parent = this;

            ToolTip = new GUI.HubProperties();
        }

        public Hub()
        {
            constructor();
        }

        public Hub(float x, float y)
        {
            constructor();
            Graphics.Position = new Vector2(x, y);
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
                Joints[i].Initialize();
                Joints[i].CanRemove = false;
                Joints[i].ContainingComponents.Add(this);
            }

            Wires[0] = new Wire(Joints[0], Joints[1]);
            Wires[0].AddComponentToManager();
            Wires[1] = new Wire(Joints[0], Joints[2]);
            Wires[1].AddComponentToManager();
            Wires[2] = new Wire(Joints[0], Joints[3]);
            Wires[2].AddComponentToManager();
            Wires[3] = new Wire(Joints[1], Joints[2]);
            Wires[3].AddComponentToManager();
            Wires[4] = new Wire(Joints[1], Joints[3]);
            Wires[4].AddComponentToManager();
            Wires[5] = new Wire(Joints[2], Joints[3]);
            Wires[5].AddComponentToManager();

            for (int i = 0; i < Wires.Length; i++)
            {
                Wires[i].Resistance = 10;
                Wires[i].Graphics.Visible = false;
                Wires[i].IsRemovable = false;
                Wires[i].Initialize();
            }
        }

        private void UpdateWiresConnectivity()
        {
            Wires[0].IsConnected = connectedLeft && connectedUp && leftUp;
            Wires[1].IsConnected = connectedLeft && connectedRight && leftRight;
            Wires[2].IsConnected = connectedLeft && connectedDown && leftDown;
            Wires[3].IsConnected = connectedUp && connectedRight && upRight;
            Wires[4].IsConnected = connectedUp && connectedDown && upDown;
            Wires[5].IsConnected = connectedRight && connectedDown && rightDown;

            Wire.WireDirection d = Wire.WireDirection.Both;

            //Wires[0]
            d = Wire.WireDirection.Both;
            if ((left & PortState.Input) == 0 || (up & PortState.Output) == 0)
                d ^= Wire.WireDirection.J1ToJ2;
            if ((left & PortState.Output) == 0 || (up & PortState.Input) == 0)
                d ^= Wire.WireDirection.J2ToJ1;
            Wires[0].Direction = d;

            //Wires[1]
            d = Wire.WireDirection.Both;
            if ((left & PortState.Input) == 0 || (right & PortState.Output) == 0)
                d ^= Wire.WireDirection.J1ToJ2;
            if ((left & PortState.Output) == 0 || (right & PortState.Input) == 0)
                d ^= Wire.WireDirection.J2ToJ1;
            Wires[1].Direction = d;

            //Wires[2]
            d = Wire.WireDirection.Both;
            if ((left & PortState.Input) == 0 || (down & PortState.Output) == 0)
                d ^= Wire.WireDirection.J1ToJ2;
            if ((left & PortState.Output) == 0 || (down & PortState.Input) == 0)
                d ^= Wire.WireDirection.J2ToJ1;
            Wires[2].Direction = d;

            //Wires[3]
            d = Wire.WireDirection.Both;
            if ((up & PortState.Input) == 0 || (right & PortState.Output) == 0)
                d ^= Wire.WireDirection.J1ToJ2;
            if ((up & PortState.Output) == 0 || (right & PortState.Input) == 0)
                d ^= Wire.WireDirection.J2ToJ1;
            Wires[3].Direction = d;

            //Wires[4]
            d = Wire.WireDirection.Both;
            if ((up & PortState.Input) == 0 || (down & PortState.Output) == 0)
                d ^= Wire.WireDirection.J1ToJ2;
            if ((up & PortState.Output) == 0 || (down & PortState.Input) == 0)
                d ^= Wire.WireDirection.J2ToJ1;
            Wires[4].Direction = d;

            //Wires[5]
            d = Wire.WireDirection.Both;
            if ((right & PortState.Input) == 0 || (down & PortState.Output) == 0)
                d ^= Wire.WireDirection.J1ToJ2;
            if ((right & PortState.Output) == 0 || (down & PortState.Input) == 0)
                d ^= Wire.WireDirection.J2ToJ1;
            Wires[5].Direction = d;
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
            Components.Graphics.HubGraphics.LoadContentStatic();
        }

        public override void Remove()
        {
            if (!IsRemovable) return;
            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].IsRemovable = true;
                Joints[i].ContainingComponents.Remove(this);
            }

            for (int i = 0; i < Wires.Length; i++)
                Wires[i].IsRemovable = true;
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
            for (int i = 0; i < Joints.Length; i++)
                Joints[i].OnMove(dx, dy);
            SetComponentOnVisibilityMap();
        }

        public override string GetName()
        {
            return "Hub";
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

        public override Joint[] FindAccessibleJoints(Joint from)
        {
            List<Joint> t = new List<Joint>();

            if (from == Joints[0])
            {
                if (left == PortState.Input)
                    return new Joint[0];

                if (leftUp && up != PortState.Input)
                    t.Add(Joints[1]);
                if (leftRight && right != PortState.Input)
                    t.Add(Joints[2]);
                if (leftDown && down != PortState.Input)
                    t.Add(Joints[3]);
            }
            else if (from == Joints[1])
            {
                if (up == PortState.Input)
                    return new Joint[0];

                if (leftUp && left != PortState.Input)
                    t.Add(Joints[0]);
                if (upRight && right != PortState.Input)
                    t.Add(Joints[2]);
                if (upDown && down != PortState.Input)
                    t.Add(Joints[3]);
            }
            else if (from == Joints[2])
            {
                if (right == PortState.Input)
                    return new Joint[0];

                if (leftRight && left != PortState.Input)
                    t.Add(Joints[0]);
                if (upRight && up != PortState.Input)
                    t.Add(Joints[1]);
                if (rightDown && down != PortState.Input)
                    t.Add(Joints[3]);
            }
            else if (from == Joints[3])
            {
                if (down == PortState.Input)
                    return new Joint[0];

                if (leftDown && left != PortState.Input)
                    t.Add(Joints[0]);
                if (upDown && up != PortState.Input)
                    t.Add(Joints[1]);
                if (rightDown && right != PortState.Input)
                    t.Add(Joints[2]);
            }

            return t.ToArray();
        }

        //============================================================LOGICS========================================================


        public override void Start()
        {
            UpdateWiresConnectivity();

            base.Start();
        }


        //============================================================INPUT=========================================================

        public override void OnMouseClick(InputEngine.MouseArgs e)
        {
            float x = e.curState.X - Graphics.Center.X, y = e.curState.Y - Graphics.Center.Y;
            double a = Math.Atan2(y, x);

            if (a < -3 * Math.PI / 4 || a > 3 * Math.PI / 4)
                ConnectedLeft = !ConnectedLeft;
            else if (a < -Math.PI / 4)
                ConnectedUp = !ConnectedUp;
            else if (a < Math.PI / 4)
                ConnectedRight = !ConnectedRight;
            else
                ConnectedDown = !ConnectedDown;
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

            Compound.Add("Left", connectedLeft);
            Compound.Add("Up", connectedUp);
            Compound.Add("Right", connectedRight);
            Compound.Add("Down", connectedDown);

            Compound.Add("LeftUp", leftUp);
            Compound.Add("LeftRight", leftRight);
            Compound.Add("LeftDown", leftDown);
            Compound.Add("UpRight", upRight);
            Compound.Add("UpDown", upDown);
            Compound.Add("RightDown", RightDown);

            Compound.Add("PortLeft", left.GetHashCode());
            Compound.Add("PortUp", up.GetHashCode());
            Compound.Add("PortRight", right.GetHashCode());
            Compound.Add("PortDown", down.GetHashCode());
        }

        private int[] j = new int[4], w = new int[6];

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

            connectedLeft = Compound.GetBool("Left");
            connectedUp = Compound.GetBool("Up");
            connectedRight = Compound.GetBool("Right");
            connectedDown = Compound.GetBool("Down");

            leftUp = Compound.GetBool("LeftUp");
            leftRight = Compound.GetBool("LeftRight");
            leftDown = Compound.GetBool("LeftDown");
            upRight = Compound.GetBool("UpRight");
            upDown = Compound.GetBool("UpDown");
            rightDown = Compound.GetBool("RightDown");

            left = (PortState)Compound.GetInt("PortLeft");
            up = (PortState)Compound.GetInt("PortUp");
            right = (PortState)Compound.GetInt("PortRight");
            down = (PortState)Compound.GetInt("PortDown");
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

            UpdateWiresConnectivity();
        }
    }
}
