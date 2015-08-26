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
    class ReedSwitch : Component, Properties.IDrawBorder, Properties.IUsesMagnetism
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            -4, 12,
            44, 12
        };
        public static int[] JointLocs90cw = new int[]{
            12, -4,
            12, 44
        };
        #endregion

        public MicroWorld.Components.Joint[] Joints = new Joint[2];
        public Wire W;


        #region IUsesMagnetism
        public String GetMapOverlayToolTip(Component c = null)
        {
            if (c == null)
            {
                return W.IsConnected ? "Connected" : "Disconnected";
            }
            else
            {
                if (c is Properties.IMagnetic)
                {
                    return (c as Properties.IMagnetic).GetFieldForce(Graphics.Center.X, Graphics.Center.Y).Length() >
                        (Logics as Logics.ReedSwitchLogics).RequiredField ? "Connected" : "Disconnected";
                }
                else
                {
                    return "Disconnected";
                }
            }
        }

        public float GetMapLineOpacity(Properties.IMagnetic c = null)
        {
            if (c == null)
            {
                return W.IsConnected ? 1f : 0f;
            }
            else
            {
                return (c as Properties.IMagnetic).GetFieldForce(Graphics.Center.X, Graphics.Center.Y).Length() >
                    (Logics as Logics.ReedSwitchLogics).RequiredField ? 1f : 0f;
            }
        }
        #endregion


        private void constructor()
        {
            Logics = new Logics.ReedSwitchLogics();
            Graphics = new Graphics.ReedSwitchGraphics();
            Graphics.parent = this;
            Logics.parent = this;

            ToolTip = new GUI.ReedSwitchProperties();
        }

        public ReedSwitch()
        {
            constructor();
        }

        public ReedSwitch(float x, float y)
        {
            constructor();
            Graphics.Position = new Vector2(x, y);
        }

        public override void SetRotation(Component.Rotation rot)
        {
            if (rot.GetHashCode() > 1) rot = (Component.Rotation)(rot.GetHashCode() - 2);
            base.SetRotation(rot);
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
            W.Resistance = 0;
            W.IsConnected = false;
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
            Components.Graphics.ReedSwitchGraphics.LoadContentStatic();
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
            return "Reed Switch";
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
            if (W.IsConnected)
                return new Joint[] { from == Joints[0] ? Joints[1] : Joints[0] };
            return new Joint[0];
        }

        //============================================================LOGICS========================================================

        
        //==============================================================IO==========================================================


        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);

            Compound.Add("J0", Joints[0].ID);
            Compound.Add("J1", Joints[1].ID);
            Compound.Add("W", W.ID);
        }

        private int j0, j1, w;

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            j0 = Compound.GetInt("J0");
            j1 = Compound.GetInt("J1");
            w = Compound.GetInt("W");
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