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

namespace MicroWorld.Graphics
{
    public static class GraphicsEngine
    {
        public static readonly RasterizerState s_Regular = new RasterizerState();
        public static readonly RasterizerState s_ScissorsOn = new RasterizerState() { ScissorTestEnable = true };

        internal static Effect ComponentFadeEffect = null;

        public static Camera camera = new Camera();

        public static Texture2D pixel = null, circle = null, FadeTriangle = null, dottedPattern = null, dottedPatternBig = null, bg = null;

        internal static bool isSelectedGlowPass = false;
        public static bool IsSelectedGlowPass
        {
            get { return GraphicsEngine.isSelectedGlowPass; }
        }

        private static Graphics.Renderer renderer;
        public static Graphics.Renderer Renderer
        {
            get { return GraphicsEngine.renderer; }
            internal set
            {
                if (GraphicsEngine.renderer != null)
                    GraphicsEngine.renderer.GraphicsDevice.DeviceReset -= new EventHandler<EventArgs>(GlobalEvents.OnGraphicsDeviceReset);
                GraphicsEngine.renderer = value;
                if (GraphicsEngine.renderer != null)
                    GraphicsEngine.renderer.GraphicsDevice.DeviceReset += new EventHandler<EventArgs>(GlobalEvents.OnGraphicsDeviceReset);
            }
        }

        public static void Initialize()
        {
            Main.LoadingDetails = "Initializing GUI...";
            IO.Log.Write("    Initializing GUI");
            Graphics.GUI.GUIEngine.Init();
            GUI.ClickabilityOverlay.Initialize();
            RenderHelper.Init();

            Main.LoadingDetails = "Initializing particles...";
            IO.Log.Write("    Initializing particles");
            ParticleManager.Initialize();

            Main.LoadingDetails = "Initializing overlays...";
            IO.Log.Write("    Initializing overlays");
            OverlayManager.Initialize();

            Main.LoadingDetails = "Initializing effects...";
            IO.Log.Write("    Initializing effects");
            Effects.Effects.Initialize();

            Main.LoadingDetails = "Initializing cursors...";
            IO.Log.Write("    Initializing cursors");
            Graphics.GUI.Cursors.CursorManager.Initialize();
        }

        public static void LoadContent()
        {
            pixel = ResourceManager.Load<Texture2D>("pixel");
            circle = ResourceManager.Load<Texture2D>("circle");
            FadeTriangle = ResourceManager.Load<Texture2D>("FadeTriangle");
            dottedPattern = ResourceManager.Load<Texture2D>("GUI/HUD/HighlightPattern");
            dottedPatternBig = ResourceManager.Load<Texture2D>("GUI/HUD/HighlightPatternBig");
            bg = ResourceManager.Load<Texture2D>("GUI/bg");
            GUI.GridDraw.CursorTexture = ResourceManager.Load<Texture2D>("GUI/CursorGrid");
            ComponentFadeEffect = ResourceManager.Load<Effect>("Shaders/ComponentFadeEffect");
            Components.GUI.GeneralProperties.TitleFont = ResourceManager.Load<SpriteFont>("Fonts/LiberationSans_16_b");

            Graphics.GUI.GUIEngine.LoadContent();
            Components.ComponentsManager.LoadContent();
            ParticleManager.LoadContent();
            OverlayManager.LoadContent();
            GUI.Cursors.CursorManager.LoadContent();

            Effects.Effects.LoadContent();
        }

        public static void Update()
        {
            GUI.Cursors.CursorManager.Update();
            Graphics.GUI.GUIEngine.Update();
            ParticleManager.Update();
            OverlayManager.Update();
            if (!IO.SaveEngine.IsLoading)
                Effects.Effects.Update();
        }

        public static void Draw()
        {
            //GUI
            Main.renderer.End();
            GUI.GUIEngine.Draw();
            Main.renderer.Begin();
            //Particles
            if (GUI.GUIEngine.curScene != GUI.GUIEngine.s_game)
                ParticleManager.Draw();
            //Overlays
            Main.renderer.End();
            Main.renderer.BeginUnscaled();
            OverlayManager.Draw();
            Main.renderer.End();
            //cursor
            Main.renderer.BeginUnscaled();
            GUI.Cursors.CursorManager.Draw(Main.renderer);
        }

        public static bool CanSeeGrid(int x, int y)
        {
            if (GUI.GUIEngine.IsIn((int)(x), (int)(y))) 
                return false;
            x = (int)(x / Settings.GameScale - Settings.GameOffset.X);
            y = (int)(y / Settings.GameScale - Settings.GameOffset.Y);
            if (!Components.ComponentsManager.CanSeeGrid(x, y)) 
                return false;
            return true;
        }

        public static bool CanDrawGhostComponent(int x, int y, int w, int h)
        {
            //if (y > 0)
            //{
            //    y++;
            //    h--;
            //}

            bool b;
            var a = Components.ComponentsManager.VisibilityMap.IgnoreJointsCoords;
            Components.ComponentsManager.VisibilityMap.IgnoreJointsCoords = Logics.GameInputHandler.ghostJointsPos.Clone() as int[];
            for (int i = 0; i < Components.ComponentsManager.VisibilityMap.IgnoreJointsCoords.Length; i += 2)
            {
                Components.ComponentsManager.VisibilityMap.IgnoreJointsCoords[i] += x + 1;
                Components.ComponentsManager.VisibilityMap.IgnoreJointsCoords[i + 1] += y + 1;
                if (!Logics.PlacableAreasManager.IsPlacable(Components.ComponentsManager.VisibilityMap.IgnoreJointsCoords[i],
                    Components.ComponentsManager.VisibilityMap.IgnoreJointsCoords[i + 1]))
                {
                    Components.ComponentsManager.VisibilityMap.IgnoreJointsCoords = a;
                    return false;
                }
            }

            b = GUI.GUIEngine.s_componentSelector.SelectedComponent.component.instance.CanPlace(x, y, w, h);

            Components.ComponentsManager.VisibilityMap.IgnoreJointsCoords = a;
            return b;
        }

        public static bool CanDrawGhostComponent(ref int x, ref int y, int w, int h, bool CheckNear = true)
        {
            bool b = CanDrawGhostComponent(x, y, w, h);

            if (CheckNear && !b)
            {
                //check right
                x += 8;
                if (CanDrawGhostComponent(x, y, w, h))
                    return true;

                //check left
                x -= 16;
                if (CanDrawGhostComponent(x, y, w, h))
                    return true;

                //check up
                x += 8;
                y -= 8;
                if (CanDrawGhostComponent(x, y, w, h))
                    return true;

                //check down
                y += 16;
                if (CanDrawGhostComponent(x, y, w, h))
                    return true;

                //check down left
                x -= 8;
                if (CanDrawGhostComponent(x, y, w, h))
                    return true;

                //check up left
                y -= 16;
                if (CanDrawGhostComponent(x, y, w, h))
                    return true;

                //check up right
                x += 16;
                if (CanDrawGhostComponent(x, y, w, h))
                    return true;

                //check down right
                y += 16;
                return CanDrawGhostComponent(x, y, w, h);
            }
            else
                return b;
        }
    }
}
