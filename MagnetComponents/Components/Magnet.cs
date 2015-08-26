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
    public class Magnet : Component, Properties.IDrawBorder, Properties.IMagnetic, Properties.IRotatable
    {
        internal MagnetPole pole = MagnetPole.N;
        private float fieldRadius = 200f;
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
        public float FieldRadiusAbs
        {
            get { return Math.Abs(FieldRadius); }
        }
        private Vector2 range = new Vector2(1, 800);

        Vector2? origPos = null;
        public Component connectedComponent;
        public RotatableConnector connector;

        //IMagnetic
        #region IMagnetic
        public Vector2 GetFieldForce(float x, float y)
        {
            float dx = x - (Graphics.Position.X + Graphics.Size.X / 2),
                dy = y - (Graphics.Position.Y + Graphics.Size.Y / 2);
            double l = Math.Sqrt(dx * dx + dy * dy);
            if (l >= FieldRadiusAbs) return new Vector2();

            Vector2 t = new Vector2(dx, dy);
            t.Normalize();
            t *= FieldRadiusAbs;
            t *= (float)((FieldRadiusAbs - l) / FieldRadiusAbs);
            if (pole == MagnetPole.S) 
                t *= -1;
            return t;
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
                             Math.Pow(c.Graphics.Position.Y + c.Graphics.GetSize().Y / 2 - Graphics.Position.Y - Graphics.GetSize().Y / 2, 2)) < FieldRadiusAbs;
        }

        public float GetRadius()
        {
            return fieldRadius;
        }

        public int GetPolarity()
        {
            return pole.GetHashCode();
        }

        public void SetPolarity(MagnetPole p)
        {
            pole = p;
        }
        #endregion

        //IRotatable
        #region IRotatable
        public void Rotate(Vector2 origin, float da)
        {
            Vector2 t = Graphics.Position + Graphics.GetSize() / 2 - origin;
            double a = Math.Atan2(t.Y, t.X);
            a += da;
            Graphics.Position = new Vector2((float)Math.Cos(a) * t.Length() + origin.X,
                (float)Math.Sin(a) * t.Length() + origin.Y) - Graphics.GetSize() / 2;
        }

        public void ResetPosition()
        {
            if (origPos.HasValue)
            {
                Graphics.Position = origPos.Value;
            }
        }

        public void Connect(Component c, RotatableConnector rc)
        {
            connector = rc;
            connectedComponent = c;
        }

        public bool CanConnect(Component c)
        {
            return connectedComponent == null;
        }

        public void Disconnect()
        {
            connector = null;
            connectedComponent = null;
        }

        public RotatableConnector GetConnector()
        {
            return connector;
        }
        #endregion



        private void constructor()
        {
            Logics = new Logics.MagnetLogics();
            Graphics = new Graphics.MagnetGraphics();
            Graphics.parent = this;
            Logics.parent = this;

            Graphics.Size = new Vector2(32, 32);
            ToolTip = new GUI.MagnetProperties();
        }

        public Magnet()
        {
            constructor();
        }

        public Magnet(float x, float y)
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
            var g = Graphics as Graphics.MagnetGraphics;
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
            Components.Graphics.MagnetGraphics.LoadContentStatic();
        }

        public override void Remove()
        {
            if (!IsRemovable) return;
            var a = connector;
            connectedComponent = null;
            if (a != null) a.Disconnect();//leave this as is. prevents stack overflow
            base.Remove();
        }

        public override bool CanDragDrop()
        {
            return true;
        }

        public override void OnMove(int dx, int dy)
        {
            base.OnMove(dx, dy);
            if (origPos.HasValue)
            {
                origPos = Graphics.Position;
            }
        }

        public override string GetName()
        {
            return "Magnet";
        }

        //============================================================LOGICS========================================================

        public override void Reset()
        {
            ResetPosition();
            base.Reset();
        }

        public override void Start()
        {
            origPos = Graphics.Position;
            base.Start();
        }
        
        //==============================================================IO==========================================================


        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);

            Compound.Add("Field", fieldRadius);
            Compound.Add("Pole", pole.GetHashCode());

            if (connectedComponent == null)
            {
                Compound.Add("ConComp", -1);
                Compound.Add("Connector", -1);
            }
            else
            {
                Compound.Add("ConComp", connectedComponent.ID);
                Compound.Add("Connector", connector.ID);
            }
        }

        int con, com;
        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            fieldRadius = (float)Compound.GetDouble("Field");
            pole = (MagnetPole)Compound.GetInt("Pole");

            con = Compound.GetInt("Connector");
            com = Compound.GetInt("ConComp");
        }

        public override void PostLoad()
        {
            base.PostLoad();

            if (con != Int32.MinValue && con != -1)
            {
                connectedComponent = ComponentsManager.GetComponent(com);
                connector = ComponentsManager.GetComponent(con) as RotatableConnector;
            }
        }
    }
}
