using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MicroWorld.Logics
{
    internal class ClickablePlacableAreas : IProvidesClickabilityAreas
    {
        private static ClickablePlacableAreas instance = new ClickablePlacableAreas();
        public static ClickablePlacableAreas Instance
        {
            get { return instance; }
        }

        List<int> ClickableIDs = new List<int>();
        Rectangle a;
        double[] ra = new double[4];

        private ClickablePlacableAreas() { }

        #region Interface
        public Rectangle[] GetClickabilityRectangles()
        {
            int index;
            Rectangle[] r = new Rectangle[ClickableIDs.Count];
            for (int i = 0; i < r.Length; i++)
            {
                index = ClickableIDs[i];
                if (index < 0 || index >= PlacableAreasManager.areas.Count)
                {
                    r[i] = new Rectangle();
                    continue;
                }
                else
                {
                    a = PlacableAreasManager.areas[index];
                    ra[0] = a.X;
                    ra[1] = a.Y;
                    ra[2] = a.Width;
                    ra[3] = a.Height;
                    Utilities.Tools.GameToScreenCoords(ra);
                    r[i] = new Rectangle((int)ra[0], (int)ra[1], (int)ra[2], (int)ra[3]);
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
        public void AddClickablePlacableArea(int id)
        {
            if (id < 0 || id >= PlacableAreasManager.areas.Count)
                return;
            if (!ClickableIDs.Contains(id))
                ClickableIDs.Add(id);
        }

        public void RemoveClickablePlacableArea(int id)
        {
            ClickableIDs.Remove(id);
        }
        #endregion
    }
}
