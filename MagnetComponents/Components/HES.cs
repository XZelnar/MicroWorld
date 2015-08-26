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
    class HES : Component, Properties.IDrawBorder, Properties.IUsesMagnetism//TODO MaxMagnetForce
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            -4, 12,
            12, -4,
            28, 12,
            12, 28
        };
        #endregion

        public double MaxVoltage = 5f, MaxCurrent = 1f;
        public float MaxMagnetForce = 100;

        public MicroWorld.Components.Joint[] Joints = new Joint[4];


        #region IUsesMagnetism
        public String GetMapOverlayToolTip(Component c = null)
        {
            if (c == null)
            {
                return ((float)((int)(Joints[0].SendingVoltage * 10)) / 10f).ToString() + " V";
            }
            else
            {
                if (c is Properties.IMagnetic)
                {
                    float l = (c as Properties.IMagnetic).GetFieldForce(Graphics.Center.X, Graphics.Center.Y).Length() / MaxMagnetForce;
                    l = l > 1 ? 1 : l < 0 ? 0 : l;
                    return ((float)((int)(MaxVoltage * l * 10)) / 10f).ToString() + " V";
                }
                else
                {
                    return "0 V";
                }
            }
        }

        public float GetMapLineOpacity(Properties.IMagnetic c = null)
        {
            if (c == null)
            {
                return (float)(MaxVoltage / Joints[0].SendingVoltage);
            }
            else
            {
                return (float)(MaxVoltage / (c as Properties.IMagnetic).GetFieldForce(Graphics.Center.X, Graphics.Center.Y).Length());
            }
        }
        #endregion


        protected void constructor()
        {
            Logics = new Logics.HESLogics();
            Graphics = new Graphics.HESGraphics();
            Graphics.parent = this;
            Logics.parent = this;

            Graphics.Size = new Vector2(32, 32);
            ToolTip = new GUI.HESProperties();
        }

        public HES()
        {
            constructor();
        }

        public HES(float x, float y)
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
                    case Rotation.cw180:
                    case Rotation.cw270:
                    default:
                        break;
                }
                Joints[i].CanRemove = false;

                Joints[i].CanProvidePower = true;
                Joints[i].IsProvidingPower = true;
                Joints[i].SendingVoltage = 0;
                Joints[i].Initialize();
                Joints[i].CanRemove = false;
                Joints[i].ContainingComponents.Add(this);
            }
        }

        public override void AddComponentToManager()
        {
            ID = ComponentsManager.GetFreeID();
            base.AddComponentToManager();
        }

        public static void LoadContentStatic()
        {
            Components.Graphics.HESGraphics.LoadContentStatic();
        }

        public override void Remove()
        {
            if (!IsRemovable) return;
            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].IsRemovable = true;
                Joints[i].ContainingComponents.Remove(this);
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
            return "Hall Effect Sensor";
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
                    return new int[] { JointLocs0cw[0], JointLocs0cw[1], JointLocs0cw[2], JointLocs0cw[3], 
                                       JointLocs0cw[4], JointLocs0cw[5], JointLocs0cw[6], JointLocs0cw[7] };
                case Rotation.cw90:
                case Rotation.cw180:
                case Rotation.cw270:
                default:
                    return new int[0];
            }
        }

        public override void Reset()
        {
            Joints[0].SendingVoltage = 0;
            base.Reset();
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
            Compound.Add("Current", MaxCurrent);
            Compound.Add("Potential", MaxVoltage);
            Compound.Add("Force", MaxMagnetForce);
        }

        private int[] j = new int[4];

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            for (int i = 0; i < Joints.Length; i++)
            {
                j[i] = Compound.GetInt("J" + i.ToString());
            }
            var d = Compound.GetDouble("Current");
            if (!Double.IsNaN(d))
            {
                MaxCurrent = d;
                MaxVoltage = Compound.GetDouble("Potential");
                MaxMagnetForce = (float)Compound.GetDouble("Force");
            }
        }

        public override void PostLoad()
        {
            base.PostLoad();

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i] = (Joint)Components.ComponentsManager.GetComponent(j[i]);
                Joints[i].ContainingComponents.Add(this);
            }
        }
    }
}
