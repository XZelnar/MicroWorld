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
    class TeslaCoil : Component, Properties.IDrawBorder
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            20, -4,
            44, 20,
            20, 44,
            -4, 20,
            16, 8,
            24, 16,
            16, 24,
            8, 16
        };
        #endregion


        public MicroWorld.Components.Joint[] Joints = new Joint[8];
        public Wire W1, W2, W3, W4;



        protected void constructor()
        {
            Logics = new Logics.TeslaCoilLogics();
            Graphics = new Graphics.TeslaCoilGraphics();
            Graphics.parent = this;
            Logics.parent = this;

            ToolTip = new GUI.TeslaCoilProperties();
        }

        public TeslaCoil()
        {
            constructor();
        }

        public TeslaCoil(float x, float y)
        {
            constructor();
            Graphics.Position = new Vector2(x, y);
        }

        public override void InitAddChildComponents()
        {
            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i] = Joint.GetJoint(new Vector2(JointLocs0cw[i * 2], JointLocs0cw[i * 2 + 1]) + Graphics.Position);
                Joints[i].CanRemove = false;
                Joints[i].ContainingComponents.Add(this);
            }

            for (int i = 4; i < Joints.Length; i++)
            {
                Joints[i].CanBeGround = true;
                Joints[i].IsGround = true;
                Joints[i].Graphics.Visible = false;
            }

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].Initialize();
            }

            W1 = new Wire(Joints[0], Joints[4]);
            W1.Resistance = 400;
            W1.AddComponentToManager();
            W1.Graphics.Visible = false;
            W1.IsRemovable = false;
            W1.Initialize();

            W2 = new Wire(Joints[1], Joints[5]);
            W2.Resistance = 400;
            W2.AddComponentToManager();
            W2.Graphics.Visible = false;
            W2.IsRemovable = false;
            W2.Initialize();

            W3 = new Wire(Joints[2], Joints[6]);
            W3.Resistance = 400;
            W3.AddComponentToManager();
            W3.Graphics.Visible = false;
            W3.IsRemovable = false;
            W3.Initialize();

            W4 = new Wire(Joints[3], Joints[7]);
            W4.Resistance = 400;
            W4.AddComponentToManager();
            W4.Graphics.Visible = false;
            W4.IsRemovable = false;
            W4.Initialize();
        }

        public override void AddComponentToManager()
        {
            ID = ComponentsManager.GetFreeID();
            base.AddComponentToManager();
        }

        public static void LoadContentStatic()
        {
            Components.Graphics.TeslaCoilGraphics.LoadContentStatic();
        }

        public override void Remove()
        {
            if (!IsRemovable) return;
            if (W1 != null)
            {
                W1.IsRemovable = true;
                W2.IsRemovable = true;
                W3.IsRemovable = true;
                W4.IsRemovable = true;
            }
            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].IsRemovable = true;
                Joints[i].ContainingComponents.Remove(this);
            }

            if (W1 != null)
            {
                W1.Remove();
                W2.Remove();
                W3.Remove();
                W4.Remove();
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
            {
                Joints[i].OnMove(dx, dy);
            }
        }

        public override string GetName()
        {
            return "Tesla coil";
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

        //============================================================LOGICS========================================================


        //==============================================================IO==========================================================


        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);

            for (int i = 0; i < Joints.Length; i++)
            {
                Compound.Add("J" + i.ToString(), Joints[i].ID);
            }
            Compound.Add("W1", W1.ID);
            Compound.Add("W2", W2.ID);
            Compound.Add("W3", W3.ID);
            Compound.Add("W4", W4.ID);

            var l = Logics as Logics.TeslaCoilLogics;
            Compound.Add("DischargeVoltage", l.DischargeVoltage);
            Compound.Add("Capacitance", l.Capacitance);
            Compound.Add("Range", l.Range);
            Compound.Add("Charge", l.CurCharge);
        }

        private int[] j = new int[8];
        private int w1, w2, w3, w4;

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            for (int i = 0; i < Joints.Length; i++)
            {
                j[i] = Compound.GetInt("J" + i.ToString());
            }
            w1 = Compound.GetInt("W1");
            w2 = Compound.GetInt("W2");
            w3 = Compound.GetInt("W3");
            w4 = Compound.GetInt("W4");

            var l = Logics as Logics.TeslaCoilLogics;
            l.Range = (float)Compound.GetDouble("Range");
            l.Capacitance = (float)Compound.GetDouble("Capacitance");
            l.DischargeVoltage = (float)Compound.GetDouble("DischargeVoltage");
            l.CurCharge = (float)Compound.GetDouble("Charge");

            if (l.CurCharge < 0)
                l.CurCharge = 0;
        }

        public override void PostLoad()
        {
            base.PostLoad();

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i] = (Joint)Components.ComponentsManager.GetComponent(j[i]);
            }
            W1 = (Wire)Components.ComponentsManager.GetComponent(w1);
            W2 = (Wire)Components.ComponentsManager.GetComponent(w2);
            W3 = (Wire)Components.ComponentsManager.GetComponent(w3);
            W4 = (Wire)Components.ComponentsManager.GetComponent(w4);

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].ContainingComponents.Add(this);
            }
        }
    }
}
