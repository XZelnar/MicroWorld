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

namespace MicroWorld.Graphics.GUI.Background
{
    class ComponentsBackground : Background
    {
        List<Components.Component> components = new List<Components.Component>();
        int ticksSinceLastComponentAdd = 0;

        public override void Initialize()
        {
            components.Clear();
        }

        public override void Update()
        {
            return;
            if (ticksSinceLastComponentAdd > 30)
            {
                GenNewComponent();
            }
            ticksSinceLastComponentAdd++;
        }

        public void GenNewComponent()
        {
            Random r = new Random();
            var a = GraphicsEngine.camera.VisibleRectangle;
            int type = r.Next(3);//0 = component, 1,2 = wire
            if (type == 0)//component
            {
                int ci = r.Next(0, GUIEngine.s_componentSelector.components.Count);
                GUIEngine.s_componentSelector.components[ci].avalable = 1;
                var c = GUIEngine.s_componentSelector.components[ci].GetNewInstance() as Components.Component;
                if (c == null || c is Components.Cursor || c is Components.Wire || c is Components.Properties.IDragDropPlacable ||
                    c is Components.Properties.ICore)
                    return;
                var rots = c.Graphics.GetPossibleRotations();
                if (rots != null && rots.Length > 0)
                {
                    int rot = r.Next(0, rots.Length);
                    c.ComponentRotation = rots[rot];
                }
                int x = r.Next(a.X - 100, a.X + a.Width + 100);
                int y = r.Next(a.Y - 100, a.Y + a.Height + 100);
                if (!GraphicsEngine.CanDrawGhostComponent(x-16, y-16, (int)c.Graphics.GetSize().X+32, (int)c.Graphics.GetSize().Y+32))
                    return;
                c.Graphics.Position = new Vector2(x, y);
                //components.Add(c);
                c.Initialize();
                c.InitAddChildComponents();
                c.Tag = 0;
                Components.ComponentsManager.Add(c);
            }
            else//wire
            {
                if (Components.ComponentsManager.Components.Count == 0)
                    return;
                var c1 = Components.ComponentsManager.GetComponent(r.Next(0, Components.ComponentsManager.Components.Count));
                var c2 = Components.ComponentsManager.GetComponent(r.Next(0, Components.ComponentsManager.Components.Count));
                if (c1 == null || c2 == null)
                    return;
                var cj1 = c1.getJoints();
                var cj2 = c2.getJoints();
                if (cj1 == null || cj2 == null || cj1.Length == 0 || cj2.Length == 0)
                    return;
                var j1 = Components.ComponentsManager.GetComponent(cj1[r.Next(cj1.Length)]);
                var j2 = Components.ComponentsManager.GetComponent(cj2[r.Next(cj2.Length)]);
                if (j1 == null || j2 == null || !(j1 is Components.Joint) || !(j2 is Components.Joint) ||
                    !(j1 as Components.Joint).Graphics.Visible || !(j2 as Components.Joint).Graphics.Visible)
                    return;
                Components.Wire w = new Components.Wire(j1 as Components.Joint, j2 as Components.Joint);
                w.Initialize();
                w.InitAddChildComponents();
                w.Tag = 0;
                Components.ComponentsManager.Add(w);
            }
            ticksSinceLastComponentAdd = 0;
        }

        public override void Draw(Renderer renderer)
        {
            renderer.End();
            renderer.Begin();
            try
            {
                Components.ComponentsManager.Draw();
            }
            catch { }
            renderer.End();
            renderer.BeginUnscaled();
            renderer.Draw(GraphicsEngine.pixel, new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight), new Color(45, 57, 107) * 0.75f);
        }
    }
}
