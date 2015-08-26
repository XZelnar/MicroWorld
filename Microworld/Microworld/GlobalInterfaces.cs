using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MicroWorld
{
    /// <summary>
    /// Instances have also to be registered in MicroWorld.Graphics.GUI.ClickabilityOverlay
    /// </summary>
    public interface IProvidesClickabilityAreas
    {
        Rectangle[] GetClickabilityRectangles();
        bool HasClickableRectangles();
        void ClearClickableAreas();
    }
}
