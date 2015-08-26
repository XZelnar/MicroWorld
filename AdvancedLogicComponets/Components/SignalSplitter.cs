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
    class SignalSplitter : Component, Properties.IDrawBorder
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            -4, 12,
            20, -4,
            20, 28,
            8, 16,
            24, 8,
            24, 24
        };
        #endregion

        public MicroWorld.Components.Joint[] Joints = new Joint[6];
        public Wire W1, W2, W3;

        private void constructor()
        {
            Logics = new Logics.SignalSplitterLogics();
            Graphics = new Graphics.SignalSplitterGraphics();
            Graphics.parent = this;
            Logics.parent = this;

            ToolTip = new GUI.SignalSplitterProperties();
        }

        public SignalSplitter()
        {
            constructor();
        }

        public SignalSplitter(float x, float y)
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
                Joints[i] = Joint.GetJoint(new Vector2(JointLocs0cw[i * 2], JointLocs0cw[i * 2 + 1]) + Graphics.Position);
                Joints[i].CanRemove = false;
                Joints[i].ContainingComponents.Add(this);
            }
            Joints[3].Graphics.Visible = false;
            Joints[3].CanBeGround = true;
            Joints[3].IsGround = true;
            Joints[3].CanProvidePower = false;
            Joints[3].IsProvidingPower = false;

            Joints[4].Graphics.Visible = false;
            Joints[4].CanProvidePower = true;
            Joints[4].IsProvidingPower = true;
            Joints[4].CanBeGround = false;
            Joints[4].IsGround = false;

            Joints[5].Graphics.Visible = false;
            Joints[5].CanProvidePower = true;
            Joints[5].IsProvidingPower = false;
            Joints[5].CanBeGround = false;
            Joints[5].IsGround = false;

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].Initialize();
            }

            W1 = new Wire(Joints[0], Joints[3]);
            W1.Direction = Wire.WireDirection.J1ToJ2;
            W1.Resistance = 300;
            W1.AddComponentToManager();
            W1.Graphics.Visible = false;
            W1.Initialize();

            W2 = new Wire(Joints[5], Joints[1]);
            W2.Direction = Wire.WireDirection.J1ToJ2;
            W2.Resistance = 5;
            W2.AddComponentToManager();
            W2.Graphics.Visible = false;
            W2.Initialize();

            W3 = new Wire(Joints[4], Joints[2]);
            W3.Direction = Wire.WireDirection.J1ToJ2;
            W3.Resistance = 5;
            W3.AddComponentToManager();
            W3.Graphics.Visible = false;
            W3.Initialize();
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
            Components.Graphics.SignalSplitterGraphics.LoadContentStatic();
        }

        public override void Remove()
        {
            if (!IsRemovable) return;
            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].IsRemovable = true;
                Joints[i].ContainingComponents.Remove(this);
            }
            W1.IsRemovable = true;
            W2.IsRemovable = true;
            W3.IsRemovable = true;

            W1.Remove();
            W2.Remove();
            W3.Remove();
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
            return "Signal Splitter";
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
                    return new int[] { JointLocs0cw[0], JointLocs0cw[1], JointLocs0cw[2], JointLocs0cw[3], JointLocs0cw[4], JointLocs0cw[5] };
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

            Compound.Add("J0", Joints[0].ID);
            Compound.Add("J1", Joints[1].ID);
            Compound.Add("J2", Joints[2].ID);
            Compound.Add("J3", Joints[3].ID);
            Compound.Add("J4", Joints[4].ID);
            Compound.Add("J5", Joints[5].ID);
            Compound.Add("W1", W1.ID);
            Compound.Add("W2", W2.ID);
            Compound.Add("W3", W3.ID);
            Compound.Add("Period", (Logics as Logics.SignalSplitterLogics).Period);
        }

        private int j0, j1, j2, j3, j4, j5, w1, w2, w3;

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            j0 = Compound.GetInt("J0");
            j1 = Compound.GetInt("J1");
            j2 = Compound.GetInt("J2");
            j3 = Compound.GetInt("J3");
            j4 = Compound.GetInt("J4");
            j5 = Compound.GetInt("J5");
            w1 = Compound.GetInt("W1");
            w2 = Compound.GetInt("W2");
            w3 = Compound.GetInt("W3");

            (Logics as Logics.SignalSplitterLogics).Period = Compound.GetInt("Period");
        }

        public override void PostLoad()
        {
            base.PostLoad();

            Joints[0] = (Joint)Components.ComponentsManager.GetComponent(j0);
            Joints[1] = (Joint)Components.ComponentsManager.GetComponent(j1);
            Joints[2] = (Joint)Components.ComponentsManager.GetComponent(j2);
            Joints[3] = (Joint)Components.ComponentsManager.GetComponent(j3);
            Joints[4] = (Joint)Components.ComponentsManager.GetComponent(j4);
            Joints[5] = (Joint)Components.ComponentsManager.GetComponent(j5);
            W1 = (Wire)Components.ComponentsManager.GetComponent(w1);
            W2 = (Wire)Components.ComponentsManager.GetComponent(w2);
            W3 = (Wire)Components.ComponentsManager.GetComponent(w3);

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].ContainingComponents.Add(this);
            }
        }
    }
}
