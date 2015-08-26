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
    class Lamp : Component, Properties.IDrawBorder, Properties.ILightEmitting
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            -4, 20,
            52, 20
        };
        public static int[] JointLocs90cw = new int[]{
            20, -4,
            20, 52
        };
        public static int[] JointLocs180cw = new int[]{
            52, 20,
            -4, 20
        };
        public static int[] JointLocs270cw = new int[]{
            20, 52,
            20, -4
        };
        #endregion

        public MicroWorld.Components.Joint[] Joints = new Joint[2];
        public Wire W;
        public float Luminosity = 200;

        private void constructor()
        {
            Logics = new Logics.LampLogics();
            Graphics = new Graphics.LampGraphics();
            Graphics.parent = this;
            Logics.parent = this;

            Graphics.Size = new Vector2(48, 32);
            ToolTip = new GUI.LampProperties();
        }

        public Lamp()
        {
            constructor();
        }

        public Lamp(float x, float y)
        {
            constructor();
            Graphics.Position = new Vector2(x, y);
        }

        #region ILightEmitting
        public float GetBrightness(float x, float y)
        {
            double v;
            if ((v = (Logics as Logics.LEDLogics).Brightness) > 0)
            {
                float dx = x - Graphics.Position.X - Graphics.Size.X / 2, 
                    dy = y - Graphics.Position.Y - Graphics.Size.Y / 2;
                float t = (float)(1 - Math.Sqrt(dx * dx + dy * dy) / Luminosity);
                t = t < 0 ? 0 : t;
                return t;
            }
            return 0f;
        }

        public bool IsInRange(Component c)
        {
            return Math.Sqrt(Math.Pow(c.Graphics.Position.X + c.Graphics.GetSize().X / 2 - Graphics.Position.X - Graphics.GetSize().X / 2, 2) +
                             Math.Pow(c.Graphics.Position.Y + c.Graphics.GetSize().Y / 2 - Graphics.Position.Y - Graphics.GetSize().Y / 2, 2)) < Luminosity;
        }
        #endregion

        public void DrawAOE(MicroWorld.Graphics.Renderer renderer, float opacityMultiplier)
        {
            (Graphics as Graphics.LampGraphics).DrawAOE(renderer, opacityMultiplier);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void NonGameUpdate()
        {
            var g = Graphics as Graphics.LampGraphics;
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
                Joints[i].Initialize();
                Joints[i].CanRemove = false;
                Joints[i].ContainingComponents.Add(this);
            }

            W = new Wire(Joints[0], Joints[1]);
            W.Resistance = 250;
            W.AddComponentToManager();
            W.Graphics.Visible = false;
            W.Initialize();
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
            Components.Graphics.LampGraphics.LoadContentStatic();
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
            return "Lamp";
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
                    return new int[] { JointLocs180cw[0], JointLocs180cw[1], JointLocs180cw[2], JointLocs180cw[3] };
                case Rotation.cw270:
                    return new int[] { JointLocs270cw[0], JointLocs270cw[1], JointLocs270cw[2], JointLocs270cw[3] };
                default:
                    return new int[0];
            }
        }

        public override Joint[] FindAccessibleJoints(Joint from)
        {
            if (from == Joints[0])
                return new Joint[] { Joints[1] };
            return new Joint[0];
        }

        //============================================================LOGICS========================================================

        public override void Reset()
        {
            base.Reset();

            var l = Logics as Components.Logics.LampLogics;
            l.Brightness = 0;
        }
        
        //==============================================================IO==========================================================


        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);

            Compound.Add("J0", Joints[0].ID);
            Compound.Add("J1", Joints[1].ID);
            Compound.Add("W", W.ID);
            Compound.Add("Luminosity", Luminosity);
        }

        private int j0, j1,w;

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            j0 = Compound.GetInt("J0");
            j1 = Compound.GetInt("J1");
            w = Compound.GetInt("W");
            var a = Compound.GetDouble("Luminosity");
            if (!Double.IsNaN(a))
                Luminosity = (float)a;
        }

        public override void PostLoad()
        {
            base.PostLoad();

            Joints[0] = (Joint)Components.ComponentsManager.GetComponent(j0);
            Joints[1] = (Joint)Components.ComponentsManager.GetComponent(j1);
            W = (Wire)Components.ComponentsManager.GetComponent(w);
            W.IsRemovable = IsRemovable;

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].ContainingComponents.Add(this);
            }
        }
    }
}
