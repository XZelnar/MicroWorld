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

namespace MicroWorld.Components.old
{
    class SegmentDisplay4 : Component, Properties.IDrawBorder, Properties.ILightEmitting
    {
        #region JointLocs
        public static int[] JointLocs = new int[]{
            //Top
            10, -2,
            18, -2,
            26, -2,
            34, -2,
            42, -2,
            50, -2,
            //Down
            10, 30,
            18, 30,
            26, 30,
            34, 30,
            42, 30,
            50, 30
        };
        #endregion

        public MicroWorld.Components.Joint[] Joints = new Joint[12];
        public Wire[] W = new Wire[32];
        public float NeededVoltage = 5f;//TODO
        public int Luminosity = 100;

        private void constructor()
        {
            Logics = new Logics.SegmentDisplay4Logics();
            Graphics = new Graphics.SegmentDisplay4Graphics();
            Graphics.parent = this;
            Logics.parent = this;

            Graphics.Size = new Vector2(64, 32);
        }

        public SegmentDisplay4()
        {
            constructor();
        }

        public SegmentDisplay4(float x, float y)
        {
            constructor();
            Graphics.Position = new Vector2(x, y);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void NonGameUpdate()
        {
            var g = Graphics as Graphics.SegmentDisplay4Graphics;
            if (g.wasAOEDrawn)
            {
                g.AOEOpacity += 0.05f;
            }
            else
            {
                g.AOEOpacity -= 0.05f;
            }
            g.AOEOpacity = g.AOEOpacity < 0f ? 0f : g.AOEOpacity > 1f ? 1f : g.AOEOpacity;
            g.wasAOEDrawn = false;
            base.NonGameUpdate();
        }

        public override void InitAddChildComponents()
        {
            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i] = Joint.GetJoint(new Vector2(JointLocs[i * 2], JointLocs[i * 2 + 1]) + Graphics.Position);
                Joints[i].Initialize();
                Joints[i].CanRemove = false;
                Joints[i].ContainingComponents.Add(this);
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    W[i * 4 + j] = new Wire(Joints[i], Joints[j + 8]);
                    W[i * 4 + j].Resistance = 250;
                    //ComponentsManager.Add(W[i * 4 + j]);
                    W[i * 4 + j].AddComponentToManager();
                    W[i * 4 + j].Graphics.Visible = false;
                    W[i * 4 + j].Direction = Wire.WireDirection.J1ToJ2;
                    W[i * 4 + j].Initialize();
                }
            }
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
            Components.Graphics.SegmentDisplay4Graphics.LoadContentStatic();
        }

        public override void Remove()
        {
            if (!IsRemovable) return;
            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].IsRemovable = true;
                Joints[i].ContainingComponents.Remove(this);
            }
            for (int i = 0; i < W.Length; i++)
            {
                W[i].IsRemovable = true;
            }

            for (int i = 0; i < W.Length; i++)
            {
                W[i].Remove();
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

        public float GetBrightness(float x, float y)
        {
            Components.Logics.SegmentDisplay4Logics l = Logics as Components.Logics.SegmentDisplay4Logics;
            if (l.digits[0] == 0 && l.digits[1] == 0 && l.digits[2] == 0 && l.digits[3] == 0) return 0f;
            double v = 0f;
            for (int i = 0; i < W.Length; i++)
            {
                if (v < W[i].VoltageDropAbs) v = W[i].VoltageDropAbs;
            }
            if (v > NeededVoltage) v = NeededVoltage;
            float dx = x - Graphics.Position.X + Graphics.Size.X / 2,
                dy = y - Graphics.Position.Y + Graphics.Size.Y / 2;
            return (float)(Math.Sqrt(dx * dx + dy + dy) / 100 / (v / NeededVoltage));
        }

        public bool IsInRange(Component c)
        {
            return false;
        }

        public void DrawAOE(MicroWorld.Graphics.Renderer renderer, float opacityMultiplier)
        {
            (Graphics as Graphics.SegmentDisplay4Graphics).DrawAOE(renderer, opacityMultiplier);
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
            return "Segment Display";
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

        //============================================================LOGICS========================================================

        public override void Reset()
        {
            base.Reset();

            Components.Logics.SegmentDisplay4Logics l = Logics as Components.Logics.SegmentDisplay4Logics;
            l.digits[0] = 0;
            l.digits[1] = 0;
            l.digits[2] = 0;
            l.digits[3] = 0;
        }
        
        //==============================================================IO==========================================================


        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);

            for (int i = 0; i < Joints.Length; i++)
            {
                Compound.Add("J" + i.ToString(), Joints[i].ID);
            }

            for (int i = 0; i < W.Length; i++)
            {
                Compound.Add("W" + i.ToString(), W[i].ID);
            }
        }

        private int[] j = new int[12], w = new int[32], d = new int[32];

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            for (int i = 0; i < Joints.Length; i++)
            {
                j[i] = Compound.GetInt("J" + i.ToString());
            }

            for (int i = 0; i < W.Length; i++)
            {
                w[i] = Compound.GetInt("W" + i.ToString());
            }
        }

        public override void PostLoad()
        {
            base.PostLoad();

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i] = (Joint)Components.ComponentsManager.GetComponent(j[i]);
            }

            for (int i = 0; i < W.Length; i++)
            {
                W[i] = (Wire)Components.ComponentsManager.GetComponent(w[i]);
            }

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].ContainingComponents.Add(this);
            }
        }
    }
}
