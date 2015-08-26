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
    class Button : Component, Properties.IDrawBorder
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            -4, 12,
            52, 12
        };
        public static int[] JointLocs90cw = new int[]{
            12, -4,
            12, 52
        };
        #endregion

        public MicroWorld.Components.Joint[] Joints = new Joint[2];
        public Wire W;

        private void constructor()
        {
            Logics = new Logics.ButtonLogics();
            Graphics = new Graphics.ButtonGraphics();
            Graphics.parent = this;
            Logics.parent = this;
        }

        public Button()
        {
            constructor();
        }

        public Button(float x, float y)
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
            Components.Graphics.ButtonGraphics.LoadContentStatic();
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

        public override void Reset()
        {
            var a = InputEngine.GetCursorGameCoords();
            W.IsConnected = InputEngine.curMouse.LeftButton == ButtonState.Pressed && isIn((int)a.X, (int)a.Y);
            base.Reset();
        }

        public override string GetName()
        {
            return "Button";
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

        public override void NonGameUpdate()
        {
            if (W.IsConnected && InputEngine.curMouse.LeftButton == ButtonState.Released)
            {
                W.IsConnected = false;
            }
        }

        //============================================================INPUT=========================================================

        public override void OnMouseDown(InputEngine.MouseArgs e)
        {
            //e.SetHandled();
            W.IsConnected = e.curState.LeftButton == ButtonState.Pressed;
        }
        
        //==============================================================IO==========================================================


        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);
            
            W.IsConnected = false;

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
