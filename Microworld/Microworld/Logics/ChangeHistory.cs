using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MicroWorld.Components;
using MicroWorld.Components.Properties;

namespace MicroWorld.Logics
{
    public static class ChangeHistory
    {
        private const int HISTORY_LENGTH = 50;

        internal static bool IsHistoryEnabled = true;

        internal static List<Component>[] Components = new List<Component>[HISTORY_LENGTH];
        internal static List<ILightEmitting>[] LightEmittingComponents = new List<ILightEmitting>[HISTORY_LENGTH];
        internal static List<IMagnetic>[] MagnetComponents = new List<IMagnetic>[HISTORY_LENGTH];
        internal static List<int>[] Layers = new List<int>[HISTORY_LENGTH];
        internal static VisibilityMap[] MapVisibility = new VisibilityMap[HISTORY_LENGTH];
        internal static List<Graphics.GUI.Scene.ComponentSelector.CSComponent>[] ComponentSelectorComponents =
            new List<Graphics.GUI.Scene.ComponentSelector.CSComponent>[HISTORY_LENGTH];
        internal static List<Components.Colliders.Collider>[] Colliders = new List<Components.Colliders.Collider>[HISTORY_LENGTH];

        public static void Reset()
        {
            Components = new List<Component>[HISTORY_LENGTH];
            LightEmittingComponents = new List<ILightEmitting>[HISTORY_LENGTH];
            MagnetComponents = new List<IMagnetic>[HISTORY_LENGTH];
            Layers = new List<int>[HISTORY_LENGTH];
            MapVisibility = new VisibilityMap[HISTORY_LENGTH];
            ComponentSelectorComponents = new List<Graphics.GUI.Scene.ComponentSelector.CSComponent>[HISTORY_LENGTH];
            Colliders = new List<Components.Colliders.Collider>[HISTORY_LENGTH];
            IsHistoryEnabled = true;
        }

        public static void Push()
        {
            if (!IsHistoryEnabled) return;
            for (int i = HISTORY_LENGTH - 1; i > 0; i--)
            {
                Components[i] = Components[i - 1];
                LightEmittingComponents[i] = LightEmittingComponents[i - 1];
                MagnetComponents[i] = MagnetComponents[i - 1];
                Layers[i] = Layers[i - 1];
                MapVisibility[i] = MapVisibility[i - 1];
                ComponentSelectorComponents[i] = ComponentSelectorComponents[i - 1];
                Colliders[i] = Colliders[i - 1];
            }
            Components[0] = new List<Component>(ComponentsManager.Components);
            LightEmittingComponents[0] = new List<ILightEmitting>(ComponentsManager.LightEmittingComponents);
            MagnetComponents[0] = new List<IMagnetic>(ComponentsManager.MagnetComponents);
            Layers[0] = new List<int>(ComponentsManager.Layers);
            MapVisibility[0] = new VisibilityMap(ComponentsManager.VisibilityMap);
            ComponentSelectorComponents[0] = MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.CloneTiles();
            Colliders[0] = MicroWorld.Components.ComponentsManager.collidersManager.PushColliders();
        }

        public static void Pop()
        {
            if (!IsHistoryEnabled) return;
            if (Components[0] == null) return;
            //if (Settings.GameState != Settings.GameStates.Stopped)
            //{
            //    Graphics.OverlayManager.HighlightStop();
            //    return;
            //}

            ComponentsManager.Components = Components[0];
            ComponentsManager.LightEmittingComponents = LightEmittingComponents[0];
            ComponentsManager.MagnetComponents = MagnetComponents[0];
            ComponentsManager.Layers = Layers[0];
            ComponentsManager.mapVisibility = MapVisibility[0];
            MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.PopState(ComponentSelectorComponents[0]);
            MicroWorld.Components.ComponentsManager.collidersManager.PopColliders(Colliders[0]);

            for (int i = 0; i < HISTORY_LENGTH - 2; i++)
            {
                Components[i] = Components[i + 1];
                LightEmittingComponents[i] = LightEmittingComponents[i + 1];
                MagnetComponents[i] = MagnetComponents[i + 1];
                Layers[i] = Layers[i + 1];
                MapVisibility[i] = MapVisibility[i + 1];
                ComponentSelectorComponents[i] = ComponentSelectorComponents[i + 1];
                Colliders[i] = Colliders[i + 1];
            }

            Components[HISTORY_LENGTH - 1] = null;
            LightEmittingComponents[HISTORY_LENGTH - 1] = null;
            MagnetComponents[HISTORY_LENGTH - 1] = null;
            Layers[HISTORY_LENGTH - 1] = null;
            MapVisibility[HISTORY_LENGTH - 1] = null;
            ComponentSelectorComponents[HISTORY_LENGTH - 1] = null;
            Colliders[HISTORY_LENGTH - 1] = null;
        }
    }
}
