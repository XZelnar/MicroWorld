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
    class PulseCore : Component, Properties.IDrawBorder, Properties.ICore
    {
        #region JointLocs
        public static int[] JointLocs0cw = new int[]{
            -4, 12,
            16, 16
        };
        public static int[] JointLocs90cw = new int[]{
            12, -4,
            16, 16
        };
        public static int[] JointLocs180cw = new int[]{
            52, 12,
            16, 16
        };
        public static int[] JointLocs270cw = new int[]{
            12, 52,
            16, 16
        };
        #endregion

        public MicroWorld.Components.Joint[] Joints = new Joint[2];
        public Wire W;

        #region Event
        public event Properties.OnRecievedEventHandler onFinishedRecieving;
        #endregion

        #region ICore
        public bool IsCorrect()
        {
            return (Logics as Logics.PulseCoreLogics).IsCorrect();
        }

        public void InvokeRecievedFinished()
        {
            if (onFinishedRecieving != null)
            {
                onFinishedRecieving.Invoke(IsCorrect());
            }
        }
        #endregion

        private void constructor()
        {
            Logics = new Logics.PulseCoreLogics();
            Graphics = new Graphics.PulseCoreGraphics();
            Graphics.parent = this;
            Logics.parent = this;

            ToolTip = new GUI.PulseCoreProperties();
        }

        public PulseCore()
        {
            constructor();
        }

        public PulseCore(float x, float y)
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
                Joints[i].CanRemove = false;
                Joints[i].ContainingComponents.Add(this);
            }
            Joints[1].Graphics.Visible = false;
            Joints[1].CanBeGround = true;
            Joints[1].IsGround = true;

            for (int i = 0; i < Joints.Length; i++)
            {
                Joints[i].Initialize();
            }

            W = new Wire(Joints[0], Joints[1]);
            W.Resistance = 300;
            W.Direction = Wire.WireDirection.J1ToJ2;
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
            Components.Graphics.PulseCoreGraphics.LoadContentStatic();
        }

        public override void Remove()
        {
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
            return "Pulse Core";
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
                    return new int[] { JointLocs0cw[0], JointLocs0cw[1] };
                case Rotation.cw90:
                    return new int[] { JointLocs90cw[0], JointLocs90cw[1] };
                case Rotation.cw180:
                    return new int[] { JointLocs180cw[0], JointLocs180cw[1] };
                case Rotation.cw270:
                    return new int[] { JointLocs270cw[0], JointLocs270cw[1] };
                default:
                    return new int[0];
            }
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
            Compound.Add("RequiredLength", (Logics as Logics.PulseCoreLogics).RequiredActivity);
        }

        private int j0, j1,w;

        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            j0 = Compound.GetInt("J0");
            j1 = Compound.GetInt("J1");
            w = Compound.GetInt("W");
            (Logics as Logics.PulseCoreLogics).RequiredActivity = Compound.GetInt("RequiredLength");
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
