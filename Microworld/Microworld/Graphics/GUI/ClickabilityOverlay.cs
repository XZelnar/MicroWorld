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
using MicroWorld.Utilities;

namespace MicroWorld.Graphics.GUI
{
    unsafe static class ClickabilityOverlay
    {
        static List<Rectangle> allowed = new List<Rectangle>();
        static List<RectangleF> allowedScalable = new List<RectangleF>();
        static RenderTarget2D fbo = null, oldfbo = null, globalfbo = null, fboGlow = null;
        static float opacity = 0f, oldopacity = 0f;
        static bool changed = false;

        static Texture2D cw0, cw45, cw90, cw135, cw180, cw225, cw270, cw315;
        static Texture2D glow;

        static List<IProvidesClickabilityAreas> extensions = new List<IProvidesClickabilityAreas>();

        #region API
        public static void RegisterExtension(IProvidesClickabilityAreas t)
        {
            extensions.Add(t);
        }

        public static void RemoveExtension(IProvidesClickabilityAreas t)
        {
            extensions.Remove(t);
        }
        #endregion

        public static void allowedAdd(Rectangle r, bool scalable)
        {
            allowedAdd(new RectangleF(r.X, r.Y, r.Width, r.Height), scalable);
        }

        public static void allowedAdd(RectangleF r, bool scalable)
        {
            if (scalable)
            {
                allowedScalable.Add(new RectangleF(r));
            }
            else
            {
                if (r.X + r.Width > Main.WindowWidth && r.X < Main.WindowWidth) r.Width = Main.WindowWidth - r.X;
                if (r.Y + r.Height > Main.WindowHeight && r.Y < Main.WindowHeight) r.Height = Main.WindowHeight - r.Y;
                if (r.X < 0 && r.X + r.Width > 0)
                {
                    r.Width += r.X;
                    r.X = 0;
                }
                if (r.Y < 0 && r.Y + r.Height > 0)
                {
                    r.Height += r.Y;
                    r.Y = 0;
                }
                allowed.Add((Rectangle)r);
            }

            changed = true;
        }

        public static void allowedRemoveAt(int index)
        {
            if (index < 0) return;
            if (index >= allowed.Count) return;
            allowed.RemoveAt(index);

            changed = true;
        }

        public static void allowedClear()
        {
            allowed.Clear();
            allowedScalable.Clear();
            for (int i = 0; i < extensions.Count; i++)
                extensions[i].ClearClickableAreas();

            changed = true;
        }

        public static void Initialize()
        {
            GlobalEvents.onResolutionChanged += new GlobalEvents.ResolutionEventHandler(GlobalEvents_onResolutionChanged);
            globalfbo = new RenderTarget2D(Shortcuts.renderer.GraphicsDevice, Main.windowWidth, Main.windowHeight);
            fboGlow = new RenderTarget2D(Shortcuts.renderer.GraphicsDevice, Main.windowWidth, Main.windowHeight);
        }

        public static void LoadContent()
        {
            cw0 = ResourceManager.Load<Texture2D>("GUI/Arrows/ArrowRight");
            cw45 = ResourceManager.Load<Texture2D>("GUI/Arrows/ArrowRightDown");
            cw90 = ResourceManager.Load<Texture2D>("GUI/Arrows/ArrowDown");
            cw135 = ResourceManager.Load<Texture2D>("GUI/Arrows/ArrowDownLeft");
            cw180 = ResourceManager.Load<Texture2D>("GUI/Arrows/ArrowLeft");
            cw225 = ResourceManager.Load<Texture2D>("GUI/Arrows/ArrowLeftUp");
            cw270 = ResourceManager.Load<Texture2D>("GUI/Arrows/ArrowUp");
            cw315 = ResourceManager.Load<Texture2D>("GUI/Arrows/ArrowUpRight");

            glow = ResourceManager.Load<Texture2D>("GUI/RadialGradient");
        }

        static void GlobalEvents_onResolutionChanged(int w, int h, int oldw, int oldh)
        {
            if (fbo != null)
            {
                fbo.Dispose();
                fbo = null;
            }

            if (globalfbo != null)
                globalfbo.Dispose();
            globalfbo = new RenderTarget2D(Shortcuts.renderer.GraphicsDevice, Main.windowWidth, Main.windowHeight);
            if (fboGlow != null)
                fboGlow.Dispose();
            fboGlow = new RenderTarget2D(Shortcuts.renderer.GraphicsDevice, Main.windowWidth, Main.windowHeight);
        }

        public static void Update()
        {
            if (changed)
            {
                if (oldopacity <= 0)
                {
                    oldfbo = fbo;
                    oldopacity = opacity;
                    fbo = null;
                }
                changed = false;
            }

            if (oldopacity > 0)
            {
                oldopacity -= 0.05f;
                if (oldopacity <= 0)
                {
                    oldopacity = 0;
                    if (oldfbo != null)
                    {
                        oldfbo.Dispose();
                        oldfbo = null;
                    }
                }
            }

            bool hasDrawEx = false;
            for (int i = 0; i < extensions.Count && !hasDrawEx; i++)
                hasDrawEx = extensions[i].HasClickableRectangles();

            if ((allowed == null || allowed.Count == 0) && (allowedScalable == null || allowedScalable.Count == 0) && !hasDrawEx)
            {
                opacity -= 0.05f;
                if (opacity < 0)
                    opacity = 0;
                if (opacity == 0 && fbo != null)
                {
                    fbo.Dispose();
                    fbo = null;
                }
                goto PostUpdate;
            }
            if (Main.curState.StartsWith("GUI") || Graphics.GUI.GUIEngine.ContainsHUDScene(Graphics.GUI.GUIEngine.s_mainMenu))
                goto PostUpdate;

            if (fbo == null)
                fbo = Main.renderer.CreateFBO(Main.graphics.PreferredBackBufferWidth, Main.graphics.PreferredBackBufferHeight);

            opacity += 0.05f;
            if (opacity > 1)
                opacity = 1;
            drawOverlay();

        PostUpdate: ;
            drawFBO();
            DrawRectangleGlow();
        }

        public static void DrawOverlay()
        {
            if (!Main.curState.StartsWith("GAME") || GUIEngine.curScene != GUIEngine.s_game) 
                return;
            Shortcuts.renderer.BeginUnscaled(SpriteSortMode.Immediate, Shortcuts.renderer.Multiply, SamplerState.PointWrap, null, null);
            Shortcuts.renderer.Draw(globalfbo, new Rectangle(0, 0, Main.graphics.PreferredBackBufferWidth, Main.graphics.PreferredBackBufferHeight),
                Color.White * 0.7f);
            Shortcuts.renderer.End();

            Shortcuts.renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, null, null);
            Shortcuts.renderer.Draw(fboGlow, Vector2.Zero, Color.White);
            Shortcuts.renderer.End();
            //drawFBO();
        }

        private static void DrawRectangleGlow()
        {
            Shortcuts.renderer.EnableFBO(fboGlow);
            Shortcuts.renderer.Clear(new Color(1f, 1f, 1f, 0f));
            BlendState b = new BlendState();
            b.AlphaBlendFunction = BlendFunction.Max;
            b.AlphaSourceBlend = Blend.One;
            b.AlphaDestinationBlend = Blend.One;
            b.ColorBlendFunction = BlendFunction.Min;
            b.ColorSourceBlend = Blend.One;
            b.ColorDestinationBlend = Blend.One;
            Shortcuts.renderer.BeginUnscaled(SpriteSortMode.Immediate, b, null, null, null);
            Color col = Color.White;
            float t = Main.Ticks % 120;
            if (t >= 60)
                t = 120 - t;
            col *= (t / 120f + 0.5f);

            RectangleF tpr;
            if (allowed.Count > 0 || allowedScalable.Count > 0)
            {
                for (int c = 0; c < allowed.Count; c++)
                    DrawGlowUnscaled(allowed[c], col);

                //Shortcuts.renderer.End();
                //Shortcuts.renderer.Begin();

                for (int c = 0; c < allowedScalable.Count; c++)
                {
                    tpr = allowedScalable[c];
                    Utilities.Tools.GameToScreenCoords(ref tpr);
                    DrawGlowUnscaled((Rectangle)tpr, col);
                }

                //Shortcuts.renderer.End();
                //Shortcuts.renderer.BeginUnscaled();
            }

            for (int q = 0; q < extensions.Count; q++)
            {
                Rectangle[] tr = extensions[q].GetClickabilityRectangles();
                for (int c = 0; c < tr.Length; c++)
                    DrawGlowUnscaled(tr[c], col);
            }

            Shortcuts.renderer.End();
            Shortcuts.renderer.DisableFBO();
        }

        private static void DrawGlowUnscaled(Rectangle r, Color c)
        {
            if (r.Width > 0)
                RenderHelper.SmartDrawRectangle(glow, 16, r.X - 15, r.Y - 15, r.Width + 30, r.Height + 30, c, GraphicsEngine.Renderer);
        }

        private static void drawFBO()
        {
            Shortcuts.renderer.EnableFBO(globalfbo);
            Shortcuts.renderer.GraphicsDevice.Clear(Color.Transparent);

            if (fbo == null && oldfbo == null)
            {
                Shortcuts.renderer.DisableFBO();
                return;
            }

            Main.renderer.BeginUnscaled();

            if (oldopacity > 0)
            {
                if (fbo == null)
                {
                    Main.renderer.Draw(oldfbo,
                        new Rectangle(0, 0, Main.graphics.PreferredBackBufferWidth, Main.graphics.PreferredBackBufferHeight),
                        Color.White * oldopacity);
                }
                else
                {
                    Main.renderer.Draw(oldfbo,
                        new Rectangle(0, 0, Main.graphics.PreferredBackBufferWidth, Main.graphics.PreferredBackBufferHeight),
                        Color.White * 1f);
                    Main.renderer.Draw(fbo,
                        new Rectangle(0, 0, Main.graphics.PreferredBackBufferWidth, Main.graphics.PreferredBackBufferHeight),
                        Color.White * (1f - oldopacity));
                }
            }
            else
            {
                Main.renderer.Draw(fbo,
                    new Rectangle(0, 0, Main.graphics.PreferredBackBufferWidth, Main.graphics.PreferredBackBufferHeight),
                    Color.White * opacity);
            }

            //draw arrows on areas outside screen
            DrawArrows();

            Main.renderer.End();
        }

        private static void DrawArrows()
        {
            float arrowOpacity = (float)(Math.Abs((Main.Ticks % 60) - 30)) / 30f;
            Color col = Color.LightBlue * arrowOpacity;
            Rectangle a;
            for (int i = 0; i < allowed.Count; i++)
            {
                a = allowed[i];
                if (a.X + a.Width <= 0)//left
                {
                    if (a.Y + a.Height <= 0)//top
                    {
                        Main.renderer.Draw(cw225, new Vector2(0, 0), col);
                    }
                    else if (a.Y >= Main.WindowHeight)//bottom
                    {
                        Main.renderer.Draw(cw135, new Vector2(0, Main.WindowHeight - 40), col);
                    }
                    else//center
                    {
                        Main.renderer.Draw(cw180, new Vector2(0, a.Y + (a.Height - 40) / 2), col);
                    }
                }
                else if (a.X >= Main.WindowWidth)//right
                {
                    if (a.Y + a.Height <= 0)//top
                    {
                        Main.renderer.Draw(cw315, new Vector2(Main.WindowWidth - 40, 0), col);
                    }
                    else if (a.Y >= Main.WindowHeight)//bottom
                    {
                        Main.renderer.Draw(cw45, new Vector2(Main.WindowWidth - 40, Main.WindowHeight - 40), col);
                    }
                    else//center
                    {
                        Main.renderer.Draw(cw0, new Vector2(Main.WindowWidth - 40, a.Y + (a.Height - 40) / 2), col);
                    }
                }
                else//center
                {
                    if (a.Y + a.Height <= 0)//top
                    {
                        Main.renderer.Draw(cw270, new Vector2(a.X + (a.Width - 40) / 2, 0), col);
                    }
                    else if (a.Y >= Main.WindowHeight)//bottom
                    {
                        Main.renderer.Draw(cw90, new Vector2(a.X + (a.Width - 40) / 2, Main.WindowHeight - 40), col);
                    }
                }
            }

            RectangleF an;
            for (int i = 0; i < allowedScalable.Count; i++)
            {
                an = allowedScalable[i];
                Utilities.Tools.GameToScreenCoords(ref an);
                if (an.X + an.Width <= 0)//left
                {
                    if (an.Y + an.Height <= 0)//top
                    {
                        Main.renderer.Draw(cw225, new Vector2(0, 0), col);
                    }
                    else if (an.Y >= Main.WindowHeight)//bottom
                    {
                        Main.renderer.Draw(cw135, new Vector2(0, Main.WindowHeight - 40), col);
                    }
                    else//center
                    {
                        Main.renderer.Draw(cw180, new Vector2(0, an.Y + (an.Height - 40) / 2), col);
                    }
                }
                else if (an.X >= Main.WindowWidth)//right
                {
                    if (an.Y + an.Height <= 0)//top
                    {
                        Main.renderer.Draw(cw315, new Vector2(Main.WindowWidth - 40, 0), col);
                    }
                    else if (an.Y >= Main.WindowHeight)//bottom
                    {
                        Main.renderer.Draw(cw45, new Vector2(Main.WindowWidth - 40, Main.WindowHeight - 40), col);
                    }
                    else//center
                    {
                        Main.renderer.Draw(cw0, new Vector2(Main.WindowWidth - 40, an.Y + (an.Height - 40) / 2), col);
                    }
                }
                else//center
                {
                    if (an.Y + an.Height <= 0)//top
                    {
                        Main.renderer.Draw(cw270, new Vector2(an.X + (an.Width - 40) / 2, 0), col);
                    }
                    else if (an.Y >= Main.WindowHeight)//bottom
                    {
                        Main.renderer.Draw(cw90, new Vector2(an.X + (an.Width - 40) / 2, Main.WindowHeight - 40), col);
                    }
                }
            }
        }

        private static void drawOverlay()
        {
            bool hasAreas = allowed.Count > 0 || allowedScalable.Count > 0;

            Shortcuts.renderer.EnableFBO(fbo);
            Shortcuts.renderer.GraphicsDevice.Clear(Color.Transparent);
            Shortcuts.renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointWrap, null, null);

            Shortcuts.renderer.Draw(Shortcuts.pixel, new Rectangle(0, 0, Main.graphics.PreferredBackBufferWidth, Main.graphics.PreferredBackBufferHeight), 
                Color.Black);

            if (hasAreas)
            {
                for (int c = 0; c < allowed.Count; c++)
                    Shortcuts.renderer.Draw(Shortcuts.pixel, allowed[c], Color.White);

                Shortcuts.renderer.End();
                Shortcuts.renderer.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointWrap, null, null);

                for (int c = 0; c < allowedScalable.Count; c++)
                    Shortcuts.renderer.Draw(Shortcuts.pixel, (Rectangle)allowedScalable[c], Color.White);

                Shortcuts.renderer.End();
                Shortcuts.renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointWrap, null, null);
            }

            for (int q = 0; q < extensions.Count; q++)
            {
                Rectangle[] tr = extensions[q].GetClickabilityRectangles();
                for (int c = 0; c < tr.Length; c++)
                {
                    hasAreas = true;
                    Shortcuts.renderer.Draw(Shortcuts.pixel, tr[c], Color.White);
                }
            }

            if (!hasAreas)
                Shortcuts.renderer.Draw(Shortcuts.pixel, new Rectangle(0, 0, Main.graphics.PreferredBackBufferWidth, Main.graphics.PreferredBackBufferHeight), 
                    Color.Transparent);

            Shortcuts.renderer.End();
            Shortcuts.renderer.DisableFBO();
        }

        public static bool IsClickable(int x, int y)
        {
            int exc = 0;
            if (extensions != null)
                for (int i = 0; i < extensions.Count; i++)
                    exc += extensions[i].GetClickabilityRectangles().Length;

            if ((allowed == null || allowed.Count == 0) && (allowedScalable == null || allowedScalable.Count == 0) && exc == 0) 
                return true;
            if (Main.curState.StartsWith("GUI") || Graphics.GUI.GUIEngine.ContainsHUDScene(Graphics.GUI.GUIEngine.s_mainMenu)) 
                return true;
            if (MicroWorld.Graphics.GUI.GUIEngine.s_tutorial.isVisible && MicroWorld.Graphics.GUI.GUIEngine.s_tutorial.IsIn(x, y))
                return true;
            if (MicroWorld.Graphics.GUI.GUIEngine.s_dialog.isVisible && MicroWorld.Graphics.GUI.GUIEngine.s_dialog.IsIn(x, y))
                return true;

            for (int c = 0; c < allowed.Count; c++)
            {
                if (allowed[c].Contains(x, y)) 
                    return true;
            }
            RectangleF tr;
            for (int c = 0; c < allowedScalable.Count; c++)
            {
                tr = allowedScalable[c];
                Utilities.Tools.GameToScreenCoords(ref tr);
                if (tr.Contains(x, y))
                    return true;
            }
            for (int i = 0; i < extensions.Count; i++)
            {
                var t = extensions[i].GetClickabilityRectangles();
                for (int c = 0; c < t.Length; c++)
                {
                    if (t[c].Contains(x, y)) 
                        return true;
                }
            }
            return Graphics.GUI.GUIEngine.s_tutorial.IsIn(x, y);
            //return false;
        }

        public static bool IsClickable(InputEngine.MouseArgs e)
        {
            return IsClickable(e.curState.X, e.curState.Y);
        }
    }
}
