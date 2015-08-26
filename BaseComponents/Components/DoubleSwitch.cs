﻿using System;
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
    class DoubleSwitch : Component, Properties.IDrawBorder
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            -4, 12,
            36, 4,
            36, 20
        };
        public static int[] JointLocs90cw = new int[]{
            12, -4,
            20, 36,
            4, 36
        };
        public static int[] JointLocs180cw = new int[]{
            36, 12,
            -4, 20,
            -4, 4
        };
        public static int[] JointLocs270cw = new int[]{
            12, 36,
            4, -4,
            20, -4
        };
        #endregion

        public enum Connection
        {
            Connection1 = 1,
            Connection2 = 2
        }

        public MicroWorld.Components.Joint[] Joints = new Joint[3];
        public Wire W1, W2;
        public Connection connection = Connection.Connection1;
            
        private void constructor()
        {
            Logics = new Logics.DoubleSwitchLogics();
            Graphics = new Graphics.DoubleSwitchGraphics();
            Graphics.parent = this;
            Logics.parent = this;
        }

        public DoubleSwitch()
        {
            constructor();
        }

        public DoubleSwitch(float x, float y)
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

            W1 = new Wire(Joints[0], Joints[1]);
            W1.Resistance = 0;
            W1.IsConnected = true;
            W1.AddComponentToManager();
            W1.Graphics.Visible = false;
            W1.Initialize();

            W2 = new Wire(Joints[0], Joints[2]);
            W2.Resistance = 0;
            W2.IsConnected = false;
            W2.AddComponentToManager();
            W2.Graphics.Visible = false;
            W2.Initialize();
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
            Components.Graphics.DoubleSwitchGraphics.LoadContentStatic();
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

            W1.Remove();
            W2.Remove();
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
            return "Double Switch";
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
                    return new int[] { JointLocs90cw[0], JointLocs90cw[1], JointLocs90cw[2], JointLocs90cw[3], JointLocs90cw[4], JointLocs90cw[5] };
                case Rotation.cw180:
                    return new int[] { JointLocs180cw[0], JointLocs180cw[1], JointLocs180cw[2], JointLocs180cw[3], JointLocs180cw[4], JointLocs180cw[5] };
                case Rotation.cw270:
                    return new int[] { JointLocs270cw[0], JointLocs270cw[1], JointLocs270cw[2], JointLocs270cw[3], JointLocs270cw[4], JointLocs270cw[5] };
                default:
                    return new int[0];
            }
        }

        public override void Reset()
        {
            base.Reset();

            W1.IsConnected = connection == Connection.Connection1;
            W2.IsConnected = connection == Connection.Connection2;
        }

        public override Joint[] FindAccessibleJoints(Joint from)
        {
            if (from == Joints[0])
                return new Joint[] { W1.IsConnected ? Joints[1] : Joints[2] };
            else if ((from == Joints[1] && W1.IsConnected) || (from == Joints[2] && W2.IsConnected))
                return new Joint[] { Joints[0] };
            return new Joint[0];
        }

        //============================================================LOGICS========================================================


        //============================================================INPUT=========================================================

        public override void OnMouseClick(InputEngine.MouseArgs e)
        {
            if (connection == Connection.Connection1)
            {
                connection = Connection.Connection2;
                W1.IsConnected = false;
                W2.IsConnected = true;
            }
            else
            {
                connection = Connection.Connection1;
                W1.IsConnected = true;
                W2.IsConnected = false;
            }
        }
        
        //==============================================================IO==========================================================


        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);

            Compound.Add("J0", Joints[0].ID);
            Compound.Add("J1", Joints[1].ID);
            Compound.Add("J2", Joints[2].ID);
            Compound.Add("W1", W1.ID);
            Compound.Add("W2", W2.ID);
            Compound.Add("Connection", connection.GetHashCode());
        }

        private int j0, j1, j2, w1, w2;

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            j0 = Compound.GetInt("J0");
            j1 = Compound.GetInt("J1");
            j2 = Compound.GetInt("J2");
            w1 = Compound.GetInt("W1");
            w2 = Compound.GetInt("W2");

            connection = (Connection)Compound.GetInt("Connection");
        }

        public override void PostLoad()
        {
            base.PostLoad();

            Joints[0] = (Joint)Components.ComponentsManager.GetComponent(j0);
            Joints[1] = (Joint)Components.ComponentsManager.GetComponent(j1);
            Joints[2] = (Joint)Components.ComponentsManager.GetComponent(j2);
            W1 = (Wire)Components.ComponentsManager.GetComponent(w1);
            W2 = (Wire)Components.ComponentsManager.GetComponent(w2);

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].ContainingComponents.Add(this);
            }
        }
    }
}