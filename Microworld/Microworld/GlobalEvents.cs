using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MicroWorld
{
    public static class GlobalEvents
    {
        #region Events
        public delegate void ComponentEventHandler(Components.Component sender);
        public static event ComponentEventHandler onComponentPlacedByPlayer;
        public static event ComponentEventHandler onComponentRemovedByPlayer;
        public static event ComponentEventHandler onComponentRemoved;
        public static event ComponentEventHandler onComponentSelected;
        public static event ComponentEventHandler onComponentMoved;
        public delegate void ResolutionEventHandler(int w, int h, int oldw, int oldh);
        public static event ResolutionEventHandler onResolutionChanged;
        public delegate void LevelEventHandler();
        public static event LevelEventHandler onLevelExited;
        public delegate void CameraEvent(float scale, float oldscale, Vector2 deltaOffset);
        public static event CameraEvent onCameraMoved;
        public static event CameraEvent onCameraScaled;
        public delegate void GraphicDeviceEventHandler();
        public static event GraphicDeviceEventHandler onGraphicsDeviceReset;
        public delegate void IOEngineEvent();
        public static event IOEngineEvent onLevelLoaded;
        public static event IOEngineEvent onLevelSaved;
        #endregion

        internal static void OnComponentRemovedByPlayer(Components.Component c)
        {
            if (onComponentRemovedByPlayer != null)
            {
                onComponentRemovedByPlayer.Invoke(c);
            }
        }

        internal static void OnComponentRemoved(Components.Component c)
        {
            if (c is Components.Properties.ICollidable)
                (c as Components.Properties.ICollidable).UnRegisterColliders();

            if (onComponentRemoved != null)
            {
                onComponentRemoved.Invoke(c);
            }
        }

        internal static void OnComponentPlacedByPlayer(Components.Component c)
        {
            if (c is Components.Properties.ICollidable)
                (c as Components.Properties.ICollidable).RegisterColliders();
            Sound.SoundPlayer.ComponentPlaced();

            if (onComponentPlacedByPlayer != null)
            {
                onComponentPlacedByPlayer.Invoke(c);
            }
        }

        internal static void OnComponentSelected(Components.Component c)
        {
            if (onComponentSelected != null)
            {
                onComponentSelected.Invoke(c);
            }
        }

        internal static void OnComponentMoved(Components.Component c)
        {
            if (onComponentMoved != null)
            {
                onComponentMoved.Invoke(c);
            }
        }

        internal static void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            if (onResolutionChanged != null)
            {
                onResolutionChanged.Invoke(w, h, oldw, oldh);
            }
        }

        internal static void OnLevelExited()
        {
            if (onLevelExited != null)
            {
                onLevelExited.Invoke();
            }
        }

        internal static void OnCameraMoved(Vector2 delta)
        {
            if (onCameraMoved != null)
            {
                onCameraMoved.Invoke(Graphics.GraphicsEngine.camera.Scale, Graphics.GraphicsEngine.camera.Scale, delta);
            }
        }

        internal static void OnCameraScaled(float old, Vector2 delta)
        {
            if (onCameraScaled != null)
            {
                onCameraScaled.Invoke(Graphics.GraphicsEngine.camera.Scale, old, delta);
            }
        }

        internal static void OnGraphicsDeviceReset(object sender, EventArgs e)
        {
            //System.Threading.Thread.Sleep(100);
            if (onGraphicsDeviceReset != null)
            {
                onGraphicsDeviceReset.Invoke();
            }
        }

        internal static void OnLevelLoaded()
        {
            if (onLevelLoaded != null)
            {
                onLevelLoaded.Invoke();
            }
        }

        internal static void OnLevelSaved()
        {
            if (onLevelSaved != null)
            {
                onLevelSaved.Invoke();
            }
        }
    }
}
