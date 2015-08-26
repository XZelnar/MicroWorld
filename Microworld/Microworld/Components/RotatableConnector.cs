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
    public class RotatableConnector : Component, Properties.IDragDropPlacable
    {
        private Component _connectedComponent1, _connectedComponent2;

        public Component ConnectedComponent1
        {
            get { return _connectedComponent1; }
            set
            {
                _connectedComponent1 = value;
            }
        }
        public Component ConnectedComponent2
        {
            get { return _connectedComponent2; }
            set
            {
                _connectedComponent2 = value;
            }
        }

        #region IDragDropPlacable
        public bool CanStart(int x, int y)
        {
            var a = Components.ComponentsManager.GetComponent(x, y);
            return a != null && (a is Components.Properties.IRotatable || a is Components.Properties.IRotator);
        }

        public void DrawGhost(MicroWorld.Graphics.Renderer renderer, int x1, int y1, int x2, int y2)
        {
            var c = ComponentsManager.GetComponent(x1, y1);
            if (c != null)
            {
                x1 = (int)(c.Graphics.Position.X + c.Graphics.GetSize().X / 2);
                y1 = (int)(c.Graphics.Position.Y + c.Graphics.GetSize().Y / 2);
            }
            else
                return;
            var c2 = ComponentsManager.GetComponent(x2, y2);
            if (c2 != null)
            {
                x2 = (int)(c2.Graphics.Position.X + c2.Graphics.GetSize().X / 2);
                y2 = (int)(c2.Graphics.Position.Y + c2.Graphics.GetSize().Y / 2);
            }
            if (c2 == null || c2 is Properties.IRotator || c2 is Properties.IRotatable)
            {
                (Graphics as Graphics.RotatableConnectorGraphics).DrawConnector(x1, y1, x2, y2, renderer, 0.6f);
                Components.Graphics.RotatableConnectorGraphics.DrawBorder(new Vector2(x1, y1), new Vector2(x2, y2), renderer);
            }
        }

        public bool CanEnd(int x1, int y1, int x2, int y2)
        {
            var c2 = Components.ComponentsManager.GetComponent(x2, y2);
            if (!(c2 is Components.Properties.IRotator || c2 is Components.Properties.IRotatable))
            {
                return false;
            }
            var c = Components.ComponentsManager.GetComponent(x1, y1);
            if (c == null) return false;
            if (c2 == c) return false;
            if (c is Components.Properties.IRotatable)
            {
                if (!(c as Components.Properties.IRotatable).CanConnect(c2))
                {
                    return false;
                }
            }
            else
            {
                if (!(c as Components.Properties.IRotator).CanConnect(c2))
                {
                    return false;
                }
            }
            if (c2 is Components.Properties.IRotatable)
            {
                if (!(c2 as Components.Properties.IRotatable).CanConnect(c))
                {
                    return false;
                }
            }
            else
            {
                if (!(c2 as Components.Properties.IRotator).CanConnect(c))
                {
                    return false;
                }
            }
            return true;
        }

        public void Place(int x1, int y1, int x2, int y2)
        {
            var c = Components.ComponentsManager.GetComponent(x1, y1);
            var c2 = Components.ComponentsManager.GetComponent(x2, y2);
            Components.RotatableConnector rc = new Components.RotatableConnector(c, c2);
            rc.Initialize();
            rc.AddComponentToManager();
            rc.OnPlaced();
        }
        #endregion

        private void constructor()
        {
            Logics = new Logics.RotatableConnectorLogics();
            Graphics = new Graphics.RotatableConnectorGraphics();
            Graphics.parent = this;
            Logics.parent = this;

            //Graphics.Size = new Vector2(32, 32);
            //ToolTip = new GUI.LEDToolTip();
        }

        public RotatableConnector()
        {
            constructor();
        }

        public RotatableConnector(Component c1, Component c2)
        {
            _connectedComponent1 = c1;
            _connectedComponent2 = c2;
            constructor();
        }

        public override void SetComponentOnVisibilityMap()
        {
        }

        public override void RemoveComponentFromVisibilityMap()
        {
        }

        public override void Update()
        {
            base.Update();
        }

        public override void NonGameUpdate()
        {
            base.NonGameUpdate();

            if (MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent == this)
            {
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.AutoUpdatePosition = false;
                MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.Position =
                    new Microsoft.Xna.Framework.Vector2(clickedx - 25 / Settings.GameScale, clickedy + 10) * Settings.GameScale;
            }
        }

        public override void InitAddChildComponents()
        {
        }

        public override void AddComponentToManager()
        {
            ID = ComponentsManager.GetFreeID();
            base.AddComponentToManager();
        }

        public static void LoadContentStatic()
        {
            Components.Graphics.RotatableConnectorGraphics.LoadContentStatic();
        }

        //used to prevent stackoverflow at Disconnect
        bool IgnoreRemove = false;
        public override void Remove()
        {
            if (!IsRemovable) return;
            if (IgnoreRemove) return;
            IgnoreRemove = true;
            Disconnect();
            IgnoreRemove = false;
            base.Remove();
        }

        public override bool CanDragDrop()
        {
            return false;
        }

        public override void OnPlaced()
        {
            if (ConnectedComponent1 is Properties.IRotatable)
            {
                (ConnectedComponent1 as Properties.IRotatable).Connect(ConnectedComponent2, this);
            }
            else if (ConnectedComponent1 is Properties.IRotator)
            {
                (ConnectedComponent1 as Properties.IRotator).Connect(ConnectedComponent2, this);
            }
            if (ConnectedComponent2 is Properties.IRotatable)
            {
                (ConnectedComponent2 as Properties.IRotatable).Connect(ConnectedComponent1, this);
            }
            else if (ConnectedComponent2 is Properties.IRotator)
            {
                (ConnectedComponent2 as Properties.IRotator).Connect(ConnectedComponent1, this);
            }

            GlobalEvents.OnComponentPlacedByPlayer(this);
            Statistics.ElementsPlaced++;
        }

        public void Disconnect()
        {
            if (ConnectedComponent1 is Properties.IRotator)
                (ConnectedComponent1 as Properties.IRotator).Disconnect();
            if (ConnectedComponent1 is Properties.IRotatable)
                (ConnectedComponent1 as Properties.IRotatable).Disconnect();
            if (ConnectedComponent2 is Properties.IRotator)
                (ConnectedComponent2 as Properties.IRotator).Disconnect();
            if (ConnectedComponent2 is Properties.IRotatable)
                (ConnectedComponent2 as Properties.IRotatable).Disconnect();
            ConnectedComponent1 = null;
            ConnectedComponent2 = null;
            Remove();
        }

        public override string GetName()
        {
            return "RotatableConnector";
        }

        public override bool isIn(int x, int y)
        {
            if (!Graphics.Visible) return false;
            if (ConnectedComponent1 == null || ConnectedComponent2 == null) return false;
            var a1 = ConnectedComponent1.Graphics.Position + ConnectedComponent1.Graphics.GetSize() / 2;
            var a2 = ConnectedComponent2.Graphics.Position + ConnectedComponent2.Graphics.GetSize() / 2;
            var d = MicroWorld.Logics.MathHelper.DistancePointToLineSegment(new Microsoft.Xna.Framework.Vector2(x, y), a1, a2);
            return d <= 8;
        }

        public override bool isIn(int x, int y, int w, int h)
        {
            if (!Graphics.Visible) return false;
            //var a1 = ConnectedComponent1.Graphics.Position + ConnectedComponent1.Graphics.GetSize() / 2;
            //var a2 = ConnectedComponent2.Graphics.Position + ConnectedComponent2.Graphics.GetSize() / 2;
            return ConnectedComponent1.isIn(x, y, w, h) || ConnectedComponent2.isIn(x, y, w, h);
        }

        public override bool Intersects(int x, int y, int w, int h)
        {
            if (!Graphics.Visible) return false;
            return ConnectedComponent1.Intersects(x, y, w, h) || ConnectedComponent2.Intersects(x, y, w, h);
        }

        //============================================================LOGICS========================================================

        public override void Reset()
        {
            base.Reset();
        }

        int clickedx, clickedy;
        public override void OnMouseClick(InputEngine.MouseArgs e)
        {
            base.OnMouseClick(e);

            clickedx = e.curState.X;
            clickedy = e.curState.Y;
        }

        public override void OnMouseMove(InputEngine.MouseMoveArgs e)
        {
            if (Graphics.Visible)
            {
                if (isIn(e.curState.X, e.curState.Y) && !ToolTip.StayOpened)
                {
                    ToolTip.GameOffset = new Vector2(e.curState.X, e.curState.Y) - new Vector2(25, ToolTip.size.Y / 4);
                }
            }

            base.OnMouseMove(e);
        }
        
        //==============================================================IO==========================================================


        public override void SaveAll(IO.ComponentData Compound)
        {
            base.SaveAll(Compound);

            Compound.Add("C1", ConnectedComponent1.ID);
            Compound.Add("C2", ConnectedComponent2.ID);
        }

        int c1, c2;
        public override void LoadAll(IO.ComponentData Compound)
        {
            base.LoadAll(Compound);

            c1 = Compound.GetInt("C1");
            c2 = Compound.GetInt("C2");
        }

        public override void PostLoad()
        {
            base.PostLoad();

            ConnectedComponent1 = ComponentsManager.GetComponent(c1);
            ConnectedComponent2 = ComponentsManager.GetComponent(c2);
        }
    }
}
