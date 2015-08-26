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
    class MovementDetector : Component, Properties.IDrawBorder
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            20, -4,
            -4, 20,
            20, 44,
            44, 20,
            16, 8,
            8, 16,
            16, 24,
            24, 16
        };
        #endregion

        private float range = 128;
        public float Range
        {
            get { return range; }
            set
            {
                float old = range;
                range = value;
                if (range < 0)
                    range = 0;
                if (range > 1000)
                    range = 1000;

                if (!IO.SaveEngine.IsLoading && old != range)
                    UpdateLasers();
            }
        }

        private bool prevPowerSupply = false;
        private bool hasEnoughPowerSupply = false;
        internal bool HasEnoughPowerSupply
        {
            get { return hasEnoughPowerSupply; }
            set
            {
                bool b = hasEnoughPowerSupply != value;
                hasEnoughPowerSupply = value;
                if (b)
                    UpdateLasers();
            }
        }

        public MicroWorld.Components.Joint[] Joints = new Joint[8];
        public MicroWorld.Components.Wire[] Wires = new Wire[4];
        private PortState left = PortState.Input, up = PortState.Input, right = PortState.Input, down = PortState.Input;

        internal PortState Up
        {
            get { return up; }
            set
            {
                up = value;
                //Joints[4].CanBeGround = value == PortState.Input;
                Joints[4].IsGround = value == PortState.Input;
                //Joints[4].CanProvidePower = value == PortState.Output;
                Joints[4].IsProvidingPower = value == PortState.Output;
                Wires[0].Resistance = (value == PortState.Output) ? 1 : 400;
            }
        }
        internal PortState Left
        {
            get { return left; }
            set
            {
                left = value;
                //Joints[5].CanBeGround = value == PortState.Input;
                Joints[5].IsGround = value == PortState.Input;
                //Joints[5].CanProvidePower = value == PortState.Output;
                Joints[5].IsProvidingPower = value == PortState.Output;
                Wires[1].Resistance = (value == PortState.Output) ? 1 : 400;
            }
        }
        internal PortState Down
        {
            get { return down; }
            set
            {
                down = value;
                //Joints[6].CanBeGround = value == PortState.Input;
                Joints[6].IsGround = value == PortState.Input;
                //Joints[6].CanProvidePower = value == PortState.Output;
                Joints[6].IsProvidingPower = value == PortState.Output;
                Wires[2].Resistance = (value == PortState.Output) ? 1 : 400;
            }
        }
        internal PortState Right
        {
            get { return right; }
            set
            {
                right = value;
                //Joints[7].CanBeGround = value == PortState.Input;
                Joints[7].IsGround = value == PortState.Input;
                //Joints[7].CanProvidePower = value == PortState.Output;
                Joints[7].IsProvidingPower = value == PortState.Output;
                Wires[3].Resistance = (value == PortState.Output) ? 1 : 400;
            }
        }

        internal MovementDetectorLaser[] lasers = new MovementDetectorLaser[4];//up, left, down, right
        internal MovementDetector[] connectedDetectors = new MovementDetector[4];

        internal bool ShouldEmmitOutput()
        {
            if (!hasEnoughPowerSupply)
                return false;

            for (int i = 0; i < lasers.Length; i++)
            {
                if (connectedDetectors[i] != null && connectedDetectors[i].hasEnoughPowerSupply && lasers[i].Length == lasers[i].PreCollisionLength)
                    return true;
            }

            return false;
        }

        protected void constructor()
        {
            Logics = new Logics.MovementDetectorLogics();
            Graphics = new Graphics.MovementDetectorGraphics();
            Graphics.parent = this;
            Logics.parent = this;

            ToolTip = new GUI.MovementDetectorProperties();
        }

        public MovementDetector()
        {
            constructor();
        }

        public MovementDetector(float x, float y)
        {
            constructor();
            Graphics.Position = new Vector2(x, y);
        }

        public override void Update()
        {
            prevPowerSupply = hasEnoughPowerSupply;

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
                Joints[i].CanProvidePower = true;
                Joints[i].CanBeGround = true;
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

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i] = new MovementDetectorLaser();
                lasers[i].Graphics.Position = Graphics.Center;
                lasers[i].type = (MovementDetectorLaser.Direction)i;
                //lasers[i].AddComponentToManager();
                //lasers[i].Initialize();
            }

            GlobalEvents.onComponentPlacedByPlayer += new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentPlacedByPlayer);
            GlobalEvents.onComponentRemoved += new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentRemovedByPlayer);
            GlobalEvents.onComponentMoved += new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentMoved);
        }

        void GlobalEvents_onComponentMoved(Component sender)
        {
            UpdateLasers();
        }

        void GlobalEvents_onComponentRemovedByPlayer(Component sender)
        {
            if (sender is MovementDetector)
                UpdateLasers();
        }

        void GlobalEvents_onComponentPlacedByPlayer(Component sender)
        {
            if (sender is MovementDetector)
                UpdateLasers();
        }

        public void UpdateLasers()
        {
            for (int i = 0; i < 4; i++)
            {
                connectedDetectors[i] = null;
                lasers[i].Remove();
                lasers[i].Length = 0;
            }

            if (!hasEnoughPowerSupply)
                return;

            int x = (int)Graphics.Center.X, y = (int)Graphics.Center.Y, r = (int)range;
            var a = ComponentsManager.GetComponents<MovementDetector>(x - r, y - r, 2 * r, 2 * r);

            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] != this)
                {
                    if (a[i].Graphics.Position.X == Graphics.Position.X)
                    {
                        if (a[i].Graphics.Position.Y < Graphics.Position.Y)//up
                        {
                            if (connectedDetectors[0] == null || connectedDetectors[0].Graphics.Position.Y < a[i].Graphics.Position.Y)
                            {
                                if (connectedDetectors[0] != null)
                                    ComponentsManager.Remove(lasers[0]);
                                if (!a[i].HasEnoughPowerSupply)
                                    continue;
                                if (a[i].Range < Graphics.Position.Y - a[i].Graphics.Position.Y)
                                    continue;
                                connectedDetectors[0] = a[i];
                                lasers[0].Length = (int)(Graphics.Position.Y - a[i].Graphics.Position.Y);
                                ComponentsManager.Add(lasers[0]);
                            }
                        }
                        else//down
                        {
                            if (connectedDetectors[2] == null || connectedDetectors[2].Graphics.Position.Y > a[i].Graphics.Position.Y)
                            {
                                if (connectedDetectors[2] != null)
                                    ComponentsManager.Remove(lasers[2]);
                                if (!a[i].HasEnoughPowerSupply)
                                    continue;
                                if (a[i].Range < a[i].Graphics.Position.Y - Graphics.Position.Y)
                                    continue;
                                connectedDetectors[2] = a[i];
                                lasers[2].Length = (int)(a[i].Graphics.Position.Y - Graphics.Position.Y);
                                ComponentsManager.Add(lasers[2]);
                            }
                        }
                    }
                    if (a[i].Graphics.Position.Y == Graphics.Position.Y)
                    {
                        if (a[i].Graphics.Position.X < Graphics.Position.X)//left
                        {
                            if (connectedDetectors[1] == null || connectedDetectors[1].Graphics.Position.X < a[i].Graphics.Position.X)
                            {
                                if (connectedDetectors[1] != null)
                                    ComponentsManager.Remove(lasers[1]);
                                if (!a[i].HasEnoughPowerSupply)
                                    continue;
                                if (a[i].Range < Graphics.Position.X - a[i].Graphics.Position.X)
                                    continue;
                                connectedDetectors[1] = a[i];
                                lasers[1].Length = (int)(Graphics.Position.X - a[i].Graphics.Position.X);
                                ComponentsManager.Add(lasers[1]);
                            }
                        }
                        else//right
                        {
                            if (connectedDetectors[3] == null || connectedDetectors[3].Graphics.Position.Y > a[i].Graphics.Position.Y)
                            {
                                if (connectedDetectors[3] != null)
                                    ComponentsManager.Remove(lasers[3]);
                                if (!a[i].HasEnoughPowerSupply)
                                    continue;
                                if (a[i].Range < a[i].Graphics.Position.X - Graphics.Position.X)
                                    continue;
                                connectedDetectors[3] = a[i];
                                lasers[3].Length = (int)(a[i].Graphics.Position.X - Graphics.Position.X);
                                ComponentsManager.Add(lasers[3]);
                            }
                        }
                    }
                }
            }

            bool b = prevPowerSupply;
            prevPowerSupply = true;
            for (int i = 0; i < 4; i++)
            {
                if (connectedDetectors[i] != null)
                {
                    lasers[i].RegisterColliders();
                    if (!b)
                        connectedDetectors[i].UpdateLasers();
                }
            }
        }

        public override void AddComponentToManager()
        {
            ID = ComponentsManager.GetFreeID();
            base.AddComponentToManager();
        }

        public static void LoadContentStatic()
        {
            Components.Graphics.MovementDetectorGraphics.LoadContentStatic();
        }

        public override void Remove()
        {
            if (!IsRemovable) return;

            GlobalEvents.onComponentPlacedByPlayer -= new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentPlacedByPlayer);
            GlobalEvents.onComponentRemoved -= new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentRemovedByPlayer);
            GlobalEvents.onComponentMoved -= new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentMoved);

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].Remove();
                lasers[i].Length = 0;
            }

            if (Wires[0] != null)
            {
                for (int i = 0; i < Wires.Length; i++)
                {
                    Wires[i].IsRemovable = true;
                }
            }
            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].IsRemovable = true;
                Joints[i].ContainingComponents.Remove(this);
            }

            if (Wires[0] != null)
            {
                for (int i = 0; i < Wires.Length; i++)
                {
                    Wires[i].Remove();
                }
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

            for (int i = 0; i < 4; i++)
            {
                if (connectedDetectors[i] != null)
                {
                    connectedDetectors[i].UpdateLasers();
                }
            }
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

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].Graphics.Position = Graphics.Center;
            }
            UpdateLasers();
        }

        public override string GetName()
        {
            return "Movement detector";
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

        public override void Reset()
        {
            Joints[4].SendingVoltage = 0;
            Joints[5].SendingVoltage = 0;
            Joints[6].SendingVoltage = 0;
            Joints[7].SendingVoltage = 0;

            hasEnoughPowerSupply = false;

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].Remove();
                lasers[i].Length = 0;
                connectedDetectors[i] = null;
            }

            base.Reset();
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

            Compound.Add("Range", Range);
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

            Range = (float)Compound.GetDouble("Range");
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

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i] = new MovementDetectorLaser();
                lasers[i].Graphics.Position = Graphics.Center;
                lasers[i].type = (MovementDetectorLaser.Direction)i;
            }
        }
    }
}
