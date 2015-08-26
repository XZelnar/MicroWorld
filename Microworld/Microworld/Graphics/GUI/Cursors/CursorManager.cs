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

namespace MicroWorld.Graphics.GUI.Cursors
{
    public static class CursorManager
    {
        private static Cursor curCursor = null;
        public static Cursor CurCursor
        {
            get { return curCursor; }
            set
            {
                if (curCursor != value)
                {
                    curCursor.OnClose();
                    curCursor = value;
                    curCursor.OnShow();
                }
            }
        }

        #region Cursors
        public static Cursor normal;
        public static Cursor move;
        public static Cursor moveVertical;
        public static Cursor moveHorizontal;
        public static Cursor moveRightDown;
        public static Cursor moveRightUp;
        #endregion


        public static void Initialize()
        {
            normal = new NormalCursor();
            normal.Initialize();
            move = new MoveCursor();
            move.Initialize();
            moveVertical = new MoveCursorVertical();
            moveVertical.Initialize();
            moveHorizontal = new MoveCursorHorizontal();
            moveHorizontal.Initialize();
            moveRightDown = new MoveRightDown();
            moveRightDown.Initialize();
            moveRightUp = new MoveRightUp();
            moveRightUp.Initialize();

            curCursor = normal;
            curCursor.OnShow();
        }

        public static void LoadContent()
        {
            normal.LoadContent();
            move.LoadContent();
            moveVertical.LoadContent();
            moveHorizontal.LoadContent();
            moveRightDown.LoadContent();
            moveRightUp.LoadContent();
        }

        public static void Update()
        {
            int x = InputEngine.curMouse.X, y = InputEngine.curMouse.Y;
            Utilities.Tools.ScreenToGameCoords(ref x, ref y);
            if (Logics.GameInputHandler.isDragDrop || 
                (GUIEngine.s_subComponentButtons.isVisible && GUIEngine.s_subComponentButtons.SelectedComponent is Components.Wire &&
                    GUIEngine.s_subComponentButtons.SelectedComponent.isIn(x, y) && Components.Wire.DnDState == 0))
                CurCursor = move;
            else if (Components.Wire.DnDState == 1)
                CurCursor = moveVertical;
            else if (Components.Wire.DnDState == 2)
                CurCursor = moveHorizontal;
            else if (Logics.GameInputHandler.resizeType != Direction.None)
            {
                if (Logics.GameInputHandler.resizeType == Direction.Left || Logics.GameInputHandler.resizeType == Direction.Right)
                    CurCursor = moveHorizontal;
                else if (Logics.GameInputHandler.resizeType == Direction.Up || Logics.GameInputHandler.resizeType == Direction.Down)
                    CurCursor = moveVertical;
                else if (Logics.GameInputHandler.resizeType == Direction.RightUp|| Logics.GameInputHandler.resizeType == Direction.LeftDown)
                    CurCursor = moveRightUp;
                else if (Logics.GameInputHandler.resizeType == Direction.RightDown || Logics.GameInputHandler.resizeType == Direction.LeftUp)
                    CurCursor = moveRightDown;
            }
            else
                CurCursor = normal;

            curCursor.Update();
        }

        public static void Draw(Renderer renderer)
        {
            curCursor.Draw(renderer, new Vector2(InputEngine.curMouse.X, InputEngine.curMouse.Y));
        }

    }
}
