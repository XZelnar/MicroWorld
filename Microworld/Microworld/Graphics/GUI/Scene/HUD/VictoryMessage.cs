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
using MicroWorld.Graphics.GUI.Elements;

namespace MicroWorld.Graphics.GUI.Scene
{
    class VictoryMessage : HUDScene
    {
        Texture2D bg, title;
        SpriteFont font;

        //float opacity = 0f;

        public Vector2 position = new Vector2(), size = new Vector2();

        MenuButton menu, replay, next, stay;
        Label time, elements;

        public override void Initialize()
        {
            font = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_18");

            Layer = 5000;
            ShouldBeScaled = false;

            size = new Vector2(Main.WindowWidth / 4, Main.WindowHeight / 4);
            size = new Vector2(602, 338);
            position = new Vector2((Main.WindowWidth - size.X) / 2, (Main.WindowHeight - size.Y) / 2);

            menu = new MenuButton((int)position.X, (int)(position.Y + size.Y - size.Y / 3), 
                (int)(size.X / 4), 30, "Menu");
            menu.Font = font;
            menu.TextOffset = new Vector2(0, 3);
            menu.onClicked += new Button.ClickedEventHandler(menu_onClicked);
            controls.Add(menu);

            replay = new MenuButton((int)(position.X + size.X / 4), (int)(position.Y + size.Y - size.Y / 3),
                (int)(size.X / 4), 30, "Replay");
            replay.Font = font;
            replay.TextOffset = menu.TextOffset;
            replay.onClicked += new Button.ClickedEventHandler(replay_onClicked);
            controls.Add(replay);

            stay = new MenuButton((int)(position.X + 2 * size.X / 4), (int)(position.Y + size.Y - size.Y / 3),
                (int)(size.X / 4), 30, "Stay");
            stay.Font = font;
            stay.TextOffset = menu.TextOffset;
            stay.onClicked += new Button.ClickedEventHandler(stay_onClicked);
            controls.Add(stay);

            next = new MenuButton((int)(position.X + 3 * size.X / 4), (int)(position.Y + size.Y - size.Y / 3),
                (int)(size.X / 4), 30, "Next");
            next.Font = font;
            next.TextOffset = menu.TextOffset;
            next.onClicked += new Button.ClickedEventHandler(next_onClicked);
            controls.Add(next);

            time = new Label((int)position.X + 5, (int)position.Y + 5,      "Time:                     ");
            time.foreground = Color.White;
            controls.Add(time);

            elements = new Label((int)position.X + 5, (int)position.Y + 35, "Components placed:        ");
            elements.foreground = Color.White;
            controls.Add(elements);

            base.Initialize();
        }

        public override void LoadContent()
        {
            bg = ResourceManager.Load<Texture2D>("GUI/bg");
            title = ResourceManager.Load<Texture2D>("GUI/HUD/VictoryScreen/Title");
            base.LoadContent();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            //size = new Vector2(Main.WindowWidth / 3, Main.WindowHeight / 3);
            size = new Vector2(602, 338);
            position = new Vector2((Main.WindowWidth - size.X) / 2, (Main.WindowHeight - size.Y) / 2);

            menu.Size = new Vector2((int)(size.X / 4), 30);
            replay.Size = menu.Size;
            stay.Size = menu.Size;
            next.Size = menu.Size;

            menu.position = new Vector2((int)position.X, (int)(position.Y + size.Y - 30));
            replay.position = new Vector2((int)(position.X + size.X / 4), (int)(position.Y + size.Y - 30));
            stay.position = new Vector2((int)(position.X + 2 * size.X / 4), (int)(position.Y + size.Y - 30));
            next.position = new Vector2((int)(position.X + 3 * size.X / 4), (int)(position.Y + size.Y - 30));
            time.position = new Vector2((int)position.X + 120, (int)position.Y + 187);
            elements.position = new Vector2((int)position.X + 120, (int)position.Y + 217);

            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void OnGraphicsDeviceReset()
        {
            menu.WasInitiallyDrawn = false;
            replay.WasInitiallyDrawn = false;
            stay.WasInitiallyDrawn = false;
            next.WasInitiallyDrawn = false;

            base.OnGraphicsDeviceReset();
        }

        public int GetUserElementCount()
        {
            int c = 0;
            for (int i = 0; i < Components.ComponentsManager.Components.Count; i++)
            {
                if (Components.ComponentsManager.Components[i].IsRemovable &&
                    !(Components.ComponentsManager.Components[i] is Components.EmptyComponent) &&
                    Components.ComponentsManager.Components[i].Graphics.Visible) c++;
            }
            return c;
        }

        void next_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Logics.LevelEngine.GameUpdates = 0;
            Logics.LevelEngine.Stop();
            GUIEngine.s_levelSelection.RefreshCompletionState();
            Sound.SoundPlayer.PlayButtonClick();
            Logics.GameLogicsHelper.GameStop();
            Logics.GameLogicsHelper.OnLevelChange();
            GUIEngine.s_levelSelection.StartNextLevel();
            GUIEngine.RemoveHUDScene(this);
        }

        void stay_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Sound.SoundPlayer.PlayButtonClick();
            GUIEngine.RemoveHUDScene(this);
        }

        void replay_onClicked(object sender, InputEngine.MouseArgs e)
        {
            //Logics.LevelEngine.GameUpdates = 0;
            Sound.SoundPlayer.PlayButtonClick();
            //Logics.GameLogicsHelper.GameStop();
            //GUIEngine.s_levelSelection.StartLevel();
            GUIEngine.RemoveHUDScene(this);
            GUIEngine.s_mainMenu.igRestart_onClicked(null, null);
        }

        void menu_onClicked(object sender, InputEngine.MouseArgs e)
        {
            Logics.LevelEngine.GameUpdates = 0;
            Sound.SoundPlayer.PlayButtonClick();
            Logics.GameLogicsHelper.GameStop();
            GUIEngine.s_levelSelection.RefreshCompletionState();

            GUIEngine.s_mainMenu.InitMenuFor(GUIEngine.s_mainMenu.items, "MENU", true);
            
            GUIEngine.ChangeScene(GUIEngine.s_mainMenu, "GUIMainMenu");
            //GUIEngine.s_levelSelection.CurTab = GUIEngine.s_levelSelection.SelectedTab;
            GUIEngine.ClearHUDs();
        }

        public override void Update()
        {
            stay.isEnabled = GUIEngine.s_levelSelection.folder != "Levels/Tut/";

            if (fadeopacity < 0.6f) fadeopacity += 0.01f;
            if (OffsetX < 0)
                OffsetX += 15;
            if (OffsetX > 0)
                OffsetX = 0;

            base.Update();
        }

        int OffsetX = 0;
        public override void Draw(Renderer renderer)//TODO animation
        {
            renderer.Draw(GraphicsEngine.pixel,
                new Rectangle(0, 0, Main.WindowWidth, Main.WindowHeight),
                Shortcuts.BG_COLOR * fadeopacity);
            bool b = OffsetX != 0;
            if (b)
            {
                renderer.SetScissorRectangle(position.X, position.Y, size.X, size.Y, false);
                renderer.End();
                renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, null,
                    null, Graphics.GraphicsEngine.s_ScissorsOn, null, Matrix.CreateTranslation(OffsetX, 0, 0));
            }

            RenderHelper.SmartDrawRectangle(bg, 5,
                (int)position.X - 2, (int)position.Y - 2, (int)size.X + 4, (int)size.Y - 30 + 4,
                Color.White, renderer);

            base.Draw(renderer);

            renderer.Draw(title, position + new Vector2(64, 64), Color.White);

            renderer.Draw(GraphicsEngine.pixel, 
                new Rectangle((int)(position.X + size.X / 4), (int)(position.Y + size.Y - 30), 1, 30),
                Color.White);
            renderer.Draw(GraphicsEngine.pixel,
                new Rectangle((int)(position.X + size.X / 2), (int)(position.Y + size.Y - 30), 1, 30),
                Color.White);
            renderer.Draw(GraphicsEngine.pixel,
                new Rectangle((int)(position.X + 3 * size.X / 4), (int)(position.Y + size.Y - 30), 1, 30),
                Color.White);

            if (b)
                renderer.ResetScissorRectangle();
        }

        #region fades
        float fadeopacity = 0f;

        public override void onShow()
        {
            OffsetX = -(int)size.X;
            fadeopacity = 0f;

            int t = Logics.LevelEngine.GameUpdates / 60;
            String r = "";
            String p = "";
            p = (t % 60).ToString();
            if (p.Length == 1)
                p = "0" + p;
            r += p;
            t = t / 60;
            p = (t % 60).ToString();
            if (p.Length == 1)
                p = "0" + p;
            r = p + ":" + r;
            t = t / 60;
            p = t.ToString();
            if (p.Length == 1)
                p = "0" + p;
            r = p + ":" + r;

            time.text =     "Time:                       " + r;
            elements.text = "Components:                 " + GetUserElementCount().ToString();
        }
        #endregion

        #region IO
        public override void onButtonDown(InputEngine.MouseArgs e)
        {
            base.onButtonDown(e);
            e.Handled = true;
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            base.onButtonUp(e);
            e.Handled = true;
        }

        public override void onButtonClick(InputEngine.MouseArgs e)
        {
            base.onButtonClick(e);
            e.Handled = true;
        }

        public override void onMouseMove(InputEngine.MouseMoveArgs e)
        {
            base.onMouseMove(e);
            e.Handled = true;
        }

        public override void onMouseWheelMove(InputEngine.MouseWheelMoveArgs e)
        {
            base.onMouseWheelMove(e);
            e.Handled = true;
        }

        public override void onKeyDown(InputEngine.KeyboardArgs e)
        {
            base.onKeyDown(e);
            e.Handled = true;
        }

        public override void onKeyUp(InputEngine.KeyboardArgs e)
        {
            base.onKeyUp(e);
            e.Handled = true;
        }

        public override void onKeyPressed(InputEngine.KeyboardArgs e)
        {
            if (e.key == Keys.Escape.GetHashCode())
            {
                menu_onClicked(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.Enter.GetHashCode() || e.key == Keys.Space.GetHashCode())
            {
                next_onClicked(null, null);
                e.Handled = true;
                return;
            }
            if (e.key == Keys.Back.GetHashCode())
            {
                replay_onClicked(null, null);
                e.Handled = true;
                return;
            }
            base.onKeyPressed(e);
            e.Handled = true;
        }
        #endregion

        public override bool IsIn(int x, int y)
        {
            return x >= position.X && x < position.X + size.X
                && y >= position.Y && y < position.Y + size.Y;
        }

    }
}
