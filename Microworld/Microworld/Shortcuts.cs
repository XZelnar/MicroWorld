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

namespace MicroWorld
{
    public static class Shortcuts
    {
        #region CameraImplementations
        public static float GameScale
        {
            get { return Graphics.GraphicsEngine.camera.Scale; }
            set
            {
                Graphics.GraphicsEngine.camera.Scale = value;
            }
        }
        public static Vector2 GameOffset
        {
            get { return Graphics.GraphicsEngine.camera.BottomRight; }
        }
        #endregion

        public static Texture2D pixel
        {
            get { return Graphics.GraphicsEngine.pixel; }
        }
        public static Graphics.Renderer renderer
        {
            get { return Graphics.GraphicsEngine.Renderer; }
        }
        public static Graphics.Camera camera
        {
            get { return Graphics.GraphicsEngine.camera; }
        }
        public static Components.Component MouseOverComponent
        {
            get { return Logics.GameInputHandler.MouseOverComponent; }
        }
        public static Components.Component[] MouseOverComponents
        {
            get { return Logics.GameInputHandler.MouseOverComponents; }
        }

        public static int GridSize
        {
            get { return Graphics.GUI.GridDraw.Step; }
        }

        public static readonly Color BG_COLOR = new Color(45, 57, 107);


        public static void SetInGameStatus(String left, String right)
        {
            Graphics.GUI.GUIEngine.s_statusStrip.TextLeft = left;
            Graphics.GUI.GUIEngine.s_statusStrip.TextRight = right;
        }

        public static void ShowToolTip(Vector2 pos, String text, 
            Graphics.GUI.Scene.ArrowLineDirection forcedDirection = Graphics.GUI.Scene.ArrowLineDirection.None)
        {
            Graphics.GUI.GUIEngine.s_toolTip.Position = pos;
            Graphics.GUI.GUIEngine.s_toolTip.ForcedDirection = forcedDirection;
            Graphics.GUI.GUIEngine.s_toolTip.Text = text;
            Graphics.GUI.GUIEngine.s_toolTip.Show();
            Graphics.GUI.GUIEngine.s_toolTip.isVisible = true;
        }

        public static void ShowToolTipNoAnimation(Vector2 pos, String text,
            Graphics.GUI.Scene.ArrowLineDirection forcedDirection = Graphics.GUI.Scene.ArrowLineDirection.None)
        {
            Graphics.GUI.GUIEngine.s_toolTip.Position = pos;
            Graphics.GUI.GUIEngine.s_toolTip.ForcedDirection = forcedDirection;
            Graphics.GUI.GUIEngine.s_toolTip.Text = text;
            Graphics.GUI.GUIEngine.s_toolTip.Show();
            Graphics.GUI.GUIEngine.s_toolTip.isVisible = true;
            Graphics.GUI.GUIEngine.s_toolTip.SkipAnimation();
        }

        public static void CloseToolTip()
        {
            Graphics.GUI.GUIEngine.s_toolTip.Close();
        }

        public static short GetComponentTypeID(Type t)
        {
            for (int i = 0; i < Graphics.GUI.GUIEngine.s_componentSelector.components.Count; i++)
                if (Graphics.GUI.GUIEngine.s_componentSelector.components[i].instance.GetType() == t)
                    return Graphics.GUI.GUIEngine.s_componentSelector.components[i].instance.typeID;
            return 0;
        }

        public static void ProcessException(Exception e, String descriptionLog = "An error has occured", String descriptionConsole = "Error: ")
        {
            IO.Log.Write(descriptionLog);
            IO.Log.Write(e.Message);
            IO.Log.Write(e.Source);
            OutputEngine.WriteLine(descriptionConsole + e.Message);
        }



        #region temp
        //TODO rm
        public static void SetAllComponentsUnremovable()
        {
            for (int i = 0; i < Components.ComponentsManager.Components.Count; i++)
                if (Components.ComponentsManager.Components[i].Graphics.Visible && !(Components.ComponentsManager.Components[i] is Components.Joint))
                    Components.ComponentsManager.Components[i].IsRemovable = false;
        }
        #endregion
    }
}
