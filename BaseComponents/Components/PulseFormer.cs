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
    unsafe class PulseFormer : Component, Properties.IDrawBorder
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            -4, 12,
            60, 12
        };
        #endregion

        public MicroWorld.Components.Joint[] Joints = new Joint[2];
        public Wire W;

        public float MaxResistance = 1000000;

        static GUI.PulseFormerCloseup s_closeup;


        protected void constructor()
        {
            Logics = new Logics.PulseFormerLogics();
            Graphics = new Graphics.PulseFormerGraphics();
            Graphics.parent = this;
            Logics.parent = this;

            ToolTip = new GUI.PulseFormerProperties();
        }

        public PulseFormer()
        {
            constructor();
        }

        public PulseFormer(float x, float y)
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
                Joints[i].Initialize();
                Joints[i].CanRemove = false;
                Joints[i].ContainingComponents.Add(this);
            }
            W = new Wire(Joints[0], Joints[1]);
            W.Direction = Wire.WireDirection.J1ToJ2;
            W.Resistance = 1;
            W.AddComponentToManager();
            W.Graphics.Visible = false;
            W.Initialize();
        }

        public static void StaticInit()
        {
            s_closeup = new GUI.PulseFormerCloseup();
            s_closeup.isVisible = true;
            s_closeup.Initialize();
            s_closeup.LoadContent();
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
            Components.Graphics.PulseFormerGraphics.LoadContentStatic();
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
            return "Pulse Former";
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

        long lastClick = 0;
        public override void OnMouseClick(InputEngine.MouseArgs e)
        {
            if (isIn(e.curState.X, e.curState.Y) && e.button == 0)
            {
                if (Main.Ticks - lastClick < 40)
                {
                    s_closeup.SelectedPF = this;
                    MicroWorld.Graphics.GUI.GUIEngine.AddHUDScene(s_closeup);
                }
                lastClick = Main.Ticks;
            }
            base.OnMouseClick(e);
        }

        //==============================================================IO==========================================================

        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);

            Compound.Add("J0", Joints[0].ID);
            Compound.Add("J1", Joints[1].ID);
            Compound.Add("W", W.ID);
            Compound.Add("Cycle", (Logics as Logics.PulseFormerLogics).cycle);
            String s = "";
            float[] t = (Logics as Logics.PulseFormerLogics).pulses;
            for (int i = 0; i < t.Length; i++)
            {
                s += t[i].ToString() + ";";
            }
            s = s.Substring(0, s.Length - 1);
            Compound.Add("Pulses", s);
            Compound.Add("MaxRes", MaxResistance);
        }

        private int j0, j1, w;

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            j0 = Compound.GetInt("J0");
            j1 = Compound.GetInt("J1");
            w = Compound.GetInt("W");
            (Logics as Logics.PulseFormerLogics).cycle = Compound.GetBool("Cycle");
            double td = Compound.GetDouble("MaxRes");
            if (Double.IsNaN(td)) td = MaxResistance;
            MaxResistance = (float)td;

            String s = Compound.GetString("Pulses");
            var a = s.Split(';');
            float[] t = new float[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                try
                {
                    t[i] = (float)Convert.ToDouble(a[i]);
                }
                catch
                {
                    t[i] = 0;
                }
            }
            (Logics as Logics.PulseFormerLogics).pulses = t;
        }

        public override void PostLoad()
        {
            base.PostLoad();

            Joints[0] = (Joint)Components.ComponentsManager.GetComponent(j0);
            Joints[1] = (Joint)Components.ComponentsManager.GetComponent(j1);
            W = (Wire)Components.ComponentsManager.GetComponent(w);

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].ContainingComponents.Add(this);
            }
        }
    }
}
