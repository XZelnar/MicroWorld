using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Components.Logics
{
    class MoverLogics : LogicalComponent
    {
        public override void Update()
        {
            var p = parent as Mover;
            float d = (p.W.VoltageDropAbs > 1 ? (p.Joints[0].Voltage > p.Joints[1].Voltage ? 1 : -1) : 0);// *0.2f;
            (p.Graphics as Graphics.MoverGraphics).CogRotation += 9 * d / 180f * (float)Math.PI;
            List<Properties.IMovable> a;
            Component t;
            switch (parent.ComponentRotation)
            {
                case Component.Rotation.cw0:
                    a = ComponentsManager.GetComponents<Properties.IMovable>((int)parent.Graphics.Position.X + 16, (int)parent.Graphics.Position.Y - 16);
                    for (int i = 0; i < a.Count; i++)
                    {
                        t = a[i] as Component;
                        if (t.Graphics.Position.Y + t.Graphics.GetSize().Y == parent.Graphics.Position.Y - 8)
                        {
                            a[i].Move(new Microsoft.Xna.Framework.Vector2(d, 0));
                        }
                    }
                    break;
                case Component.Rotation.cw90:
                    a = ComponentsManager.GetComponents<Properties.IMovable>((int)parent.Graphics.Position.X + 48, (int)parent.Graphics.Position.Y + 16);
                    for (int i = 0; i < a.Count; i++)
                    {
                        t = a[i] as Component;
                        if (t.Graphics.Position.X == parent.Graphics.Position.X + 40)
                        {
                            a[i].Move(new Microsoft.Xna.Framework.Vector2(0, d));
                        }
                    }
                    break;
                case Component.Rotation.cw180:
                    a = ComponentsManager.GetComponents<Properties.IMovable>((int)parent.Graphics.Position.X + 16, (int)parent.Graphics.Position.Y + 48);
                    for (int i = 0; i < a.Count; i++)
                    {
                        t = a[i] as Component;
                        if (t.Graphics.Position.Y == parent.Graphics.Position.Y + 40)
                        {
                            a[i].Move(new Microsoft.Xna.Framework.Vector2(-d, 0));
                        }
                    }
                    break;
                case Component.Rotation.cw270:
                    a = ComponentsManager.GetComponents<Properties.IMovable>((int)parent.Graphics.Position.X- 16, (int)parent.Graphics.Position.Y + 16);
                    for (int i = 0; i < a.Count; i++)
                    {
                        t = a[i] as Component;
                        if (t.Graphics.Position.X + t.Graphics.GetSize().X == parent.Graphics.Position.X - 8)
                        {
                            a[i].Move(new Microsoft.Xna.Framework.Vector2(0, -d));
                        }
                    }
                    break;
                default:
                    break;
            }

            base.Update();
        }
    }
}
