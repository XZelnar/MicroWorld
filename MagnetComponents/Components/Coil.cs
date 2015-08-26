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
    class Coil : Component, Properties.IDrawBorder, Properties.IMagnetic//TODO field force
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            -4, 20,
            52, 20
        };
        public static int[] JointLocs90cw = new int[]{
            4, -4,
            4, 52
        };
        #endregion

        public MicroWorld.Components.Joint[] Joints = new Joint[2];
        public Wire W;

        private float fieldRadius = 400f;
        public float FieldRadius
        {
            get { return fieldRadius; }
            set
            {
                fieldRadius = value;
                if (fieldRadius > range.Y) fieldRadius = range.Y;
                if (fieldRadius < range.X) fieldRadius = range.X;
            }
        }
        private Vector2 range = new Vector2(-800, 800);

        private void constructor()
        {
            Logics = new Logics.CoilLogics();
            Graphics = new Graphics.CoilGraphics();
            Graphics.parent = this;
            Logics.parent = this;

            ToolTip = new GUI.CoilProperties();
        }

        public Coil()
        {
            constructor();
        }

        public Coil(float x, float y)
        {
            constructor();
            Graphics.Position = new Vector2(x, y);
        }

        #region IMagnetic
        public Vector2 GetFieldForce(float x, float y)
        {
            var ll = Logics as Logics.CoilLogics;
            if (Math.Abs(ll.inductionCurrent) / 100 > Math.Abs(ll.pureCurrent))
                return Vector2.Zero;
            double v;
            if ((v = ll.chargeState / 100) != 0)
            {
                float dx = x - (Graphics.Position.X + Graphics.Size.X / 2), 
                    dy = y - (Graphics.Position.Y + Graphics.Size.Y / 2);
                double l = Math.Sqrt(dx * dx + dy * dy);
                if (l >= FieldRadius) return new Vector2();

                Vector2 t = new Vector2(dx, dy);
                if (ComponentRotation == Rotation.cw0 || ComponentRotation == Rotation.cw180)
                {
                    float sx = Graphics.GetSize().X / 2;
                    if (dx >= sx)//point to the right of the coil
                    {
                        t.X -= sx;
                        if (v < 0)//Current is right-to-left. Right is S. Invert vector
                            t *= -1;
                    }
                    else if (dx <= -sx)//point to the left of the coil
                    {
                        t.X += sx;
                        if (v > 0)//Current is left-to-right. Left is S. Invert vector
                            t *= -1;
                    }
                    else//point to the side of the coil
                    {
                        if (Math.Abs(dy) < Graphics.GetSize().Y / 2)// point inside Coil
                            t = new Vector2(v > 0 ? 1 : -1, 0);
                        else
                        {
                            if (l > sx)
                                t.Y = (float)Math.Sqrt(sx * sx - t.X * t.X) * Math.Sign(t.Y);

                            //rotate CW
                            if (t.Y < 0)
                                t *= -1;
                            float t2 = t.X;
                            t.X = t.Y;
                            t.Y = -t2;

                            if (v > 0)//Current is right-to-left. Right is S. Invert vector
                                t *= -1;
                        }
                    }
                }
                else
                {
                    float sy = Graphics.GetSize().Y / 2;
                    if (dy >= sy)//point to the bottom of the coil
                    {
                        t.Y -= sy;
                        if (v < 0)//Current is bottom-to-top. Bottom is S. Invert vector
                            t *= -1;
                    }
                    else if (dy <= -sy)//point to the top of the coil
                    {
                        t.Y += sy;
                        if (v > 0)//Current is top-to-bottom. Top is S. Invert vector
                            t *= -1;
                    }
                    else//point to the side of the coil
                    {
                        if (Math.Abs(dx) < Graphics.GetSize().X / 2)// point inside Coil
                            t = new Vector2(0, v > 0 ? 1 : -1);
                        else
                        {
                            if (l > sy)
                                t.X = (float)Math.Sqrt(sy * sy - t.Y * t.Y) * Math.Sign(t.X);

                            //rotate CW
                            if (t.X < 0)
                                t *= -1;
                            float t2 = t.X;
                            t.X = t.Y;
                            t.Y = -t2;

                            if (v < 0)//Current is bottom-to-top. Bottom is S. Invert vector
                                t *= -1;
                        }
                    }
                }

                //Vector2 t = new Vector2(dx, dy);
                t.Normalize();
                //t *= fieldRadius;
                t *= -(Logics as Logics.CoilLogics).curField.direction * (float)((FieldRadius - l) / FieldRadius * Math.Abs(v) * fieldRadius);
                return t;
            }
            return new Vector2();
        }

        public void SetRange(Vector2 v)
        {
            range = v;
            FieldRadius = FieldRadius;
        }

        public Vector2 GetRange()
        {
            return range;
        }

        public bool IsInRange(Component c)
        {
            return Math.Sqrt(Math.Pow(c.Graphics.Position.X + c.Graphics.GetSize().X / 2 - Graphics.Position.X - Graphics.GetSize().X / 2, 2) +
                             Math.Pow(c.Graphics.Position.Y + c.Graphics.GetSize().Y / 2 - Graphics.Position.Y - Graphics.GetSize().Y / 2, 2)) < FieldRadius;
        }

        public float GetRadius()
        {
            return fieldRadius;
        }

        public int GetPolarity()
        {
            return 0;
        }

        public void SetPolarity(MagnetPole p)
        {
        }
        #endregion

        public override void Update()
        {
            base.Update();
        }

        public override void NonGameUpdate()
        {
            var g = Graphics as Graphics.CoilGraphics;
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

            W = new Wire(Joints[0], Joints[1]);
            W.Resistance = 4;
            //W.Direction = Wire.WireDirection.J1ToJ2;
            W.IsRemovable = false;
            W.AddComponentToManager();
            W.Graphics.Visible = false;
            W.CanSendVoltageOrCurrent = true;
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
            Components.Graphics.CoilGraphics.LoadContentStatic();
        }

        public override void Remove()
        {
            if (!IsRemovable) return;
            MicroWorld.Logics.CircuitManager.RemoveReUpdatingComponent(Logics as Logics.CoilLogics);
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
            return "Coil";
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

        public override void Reset()
        {
            base.Reset();
        }
        
        //==============================================================IO==========================================================


        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);

            Compound.Add("J0", Joints[0].ID);
            Compound.Add("J1", Joints[1].ID);
            Compound.Add("W", W.ID);
            Compound.Add("Field", fieldRadius);
        }

        private int j0, j1,w;

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            j0 = Compound.GetInt("J0");
            j1 = Compound.GetInt("J1");
            w = Compound.GetInt("W");
            var a = Compound.GetDouble("Field");
            if (!Double.IsNaN(a))
                fieldRadius = (float)a;
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
