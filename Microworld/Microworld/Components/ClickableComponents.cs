using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MicroWorld.Components
{
    internal class ClickableComponents : IProvidesClickabilityAreas
    {
        private static ClickableComponents instance = new ClickableComponents();
        public static ClickableComponents Instance
        {
            get { return instance; }
        }

        List<int> ClickableIDs = new List<int>();
        double[] rt = new double[4];

        private ClickableComponents() { }

        #region Interface
        public Rectangle[] GetClickabilityRectangles()
        {
            Vector2 s;
            Rectangle[] r = new Rectangle[ClickableIDs.Count];
            for (int i = 0; i < r.Length; i++)
            {
                var a = Components.ComponentsManager.GetComponent(ClickableIDs[i]);
                if (a == null)
                    r[i] = new Rectangle();
                else
                {
                    if (a is Wire)
                    {
                        rt[0] = Math.Min((a as Wire).J1.Graphics.Position.X, (a as Wire).J2.Graphics.Position.X);
                        rt[1] = Math.Min((a as Wire).J1.Graphics.Position.Y, (a as Wire).J2.Graphics.Position.Y);
                        rt[2] = Math.Max((a as Wire).J1.Graphics.Position.X, (a as Wire).J2.Graphics.Position.X) - rt[0] + 8;
                        rt[3] = Math.Max((a as Wire).J1.Graphics.Position.Y, (a as Wire).J2.Graphics.Position.Y) - rt[1] + 8;
                    }
                    else
                    {
                        s = a.Graphics.GetSizeRotated(a.ComponentRotation);
                        rt[0] = a.Graphics.Position.X;
                        rt[1] = a.Graphics.Position.Y;
                        rt[2] = s.X;
                        rt[3] = s.Y;
                    }
                    Utilities.Tools.GameToScreenCoords(rt);
                    if (a is Joint)
                    {
                        rt[0] -= 12;
                        rt[1] -= 12;
                        rt[2] += 24;
                        rt[3] += 24;
                    }
                    else
                    {
                        rt[0] -= 4;
                        rt[1] -= 4;
                        rt[2] += 9;
                        rt[3] += 9;
                    }
                    r[i] = new Rectangle((int)rt[0], (int)rt[1], (int)rt[2], (int)rt[3]);
                }
            }
            return r;
        }

        public bool HasClickableRectangles()
        {
            return ClickableIDs.Count != 0;
        }

        public void ClearClickableAreas()
        {
            ClickableIDs.Clear();
        }
        #endregion

        #region API
        public void AddClickableComponent(int id)
        {
            if (id < 0 || id >= ComponentsManager.Components.Count)
                return;
            if (!ClickableIDs.Contains(id))
                ClickableIDs.Add(id);
        }

        public void RemoveClickableComponent(int id)
        {
            ClickableIDs.Remove(id);
        }
        #endregion
    }
}
