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

namespace MicroWorld.Components.Graphics
{
    class Electron
    {
        internal static Texture2D texture;

        internal float lerpState = 0f, lerpSpeed = 0f;
        internal int curind = 0;
    }

    unsafe class WireGraphics : GraphicalComponent
    {
        const int ElectronFrequency = 8;

        public bool IgnoreNextPathFinder = false;
        public List<Vector2> DrawPath = new List<Vector2>();
        internal List<Electron> electrons = new List<Electron>();

        public WireGraphics()
        {
            Layer = -1;
        }

        internal static void LoadContentStatic()
        {
            Electron.texture = ResourceManager.Load<Texture2D>("Circle16");
        }

        public override string GetIconName()
        {
            return "Components/Icons/Wire";
        }

        public override string GetCSToolTip()
        {
            return "Wire";
        }

        public override string GetComponentSelectorPath()
        {
            return null;
        }

        public override string GetHandbookFile()
        {
            return "Components/Wire.edf";
        }

        public override void Initialize()
        {
            base.Initialize();

            if (Visible)
            {
                if (IgnoreNextPathFinder)
                {
                    IgnoreNextPathFinder = false;
                    return;
                }
                var w = parent as Wire;

                ComponentsManager.IgnoreAS1 = w.J1;
                ComponentsManager.IgnoreAS2 = w.J2;

                var path = RunPathFinder((int)w.J1.Graphics.Position.X + 4, (int)w.J1.Graphics.Position.Y + 4,
                                         (int)w.J2.Graphics.Position.X + 4, (int)w.J2.Graphics.Position.Y + 4);

                DrawPath.Clear();
                if (path == null) return;
                for (int i = 0; i < path.Count; i++)
                {
                    DrawPath.Add(new Vector2(path[i].X, path[i].Y));
                }
                OptimizeDrawPath(ref DrawPath);

                OnDrawPathChanged();

                ComponentsManager.IgnoreAS1 = null;
                ComponentsManager.IgnoreAS2 = null;

                GenerateElectrons();
            }
        }

        public static bool CanFindPath(Joint J1, Joint J2, int sx, int sy, int ex, int ey)
        {
            ComponentsManager.IgnoreAS1 = J1;
            ComponentsManager.IgnoreAS2 = J2;

            var path = RunPathFinder(sx, sy, ex, ey);

            if (path == null) return false;

            ComponentsManager.IgnoreAS1 = null;
            ComponentsManager.IgnoreAS2 = null;
            return true;
        }

        public static List<Point> RunPathFinder(int sx, int sy, int ex, int ey)
        {
            return new MicroWorld.Logics.PathFinding.PathFinder().FindPath(new Point(sx, sy), new Point(ex, ey));
        }

        internal static void OptimizeDrawPath(ref List<Vector2> l)
        {
            Vector2 v1 = new Vector2(), v2 = new Vector2();
            for (int i = 1; i < l.Count - 1; i++)
            {
                v1 = l[i] - l[i - 1];
                v2 = l[i + 1] - l[i];

                if (((v1.X != 0) != (v2.X != 0)) ||
                    (v1.Y != 0) != (v2.Y != 0))
                    continue;
                l.RemoveAt(i);
                i--;
            }
        }

        public bool Intersects(Rectangle r)
        {
            for (int i = 1; i < DrawPath.Count; i++)
                if (DrawPath[i].X == DrawPath[i - 1].X)
                {
                    if (DrawPath[i].X < r.X || DrawPath[i].X > r.X + r.Width)
                        continue;
                    if ((Math.Sign(DrawPath[i].Y - r.Y) != Math.Sign(DrawPath[i - 1].Y - r.Y)) ||
                        (Math.Sign(DrawPath[i].Y - r.Y - r.Height) != Math.Sign(DrawPath[i - 1].Y - r.Y - r.Height)))
                        return true;
                }
                else
                {
                    if (DrawPath[i].Y < r.Y || DrawPath[i].Y > r.Y + r.Height)
                        continue;
                    if ((Math.Sign(DrawPath[i].X - r.X) != Math.Sign(DrawPath[i - 1].X - r.X)) ||
                        (Math.Sign(DrawPath[i].X - r.X - r.Width) != Math.Sign(DrawPath[i - 1].X - r.X - r.Width)))
                        return true;
                }
            return false;
        }

        public override object PushPosition()
        {
            return new List<Vector2>(DrawPath);
        }

        public override void PopPosition(object o)
        {
            DrawPath = o as List<Vector2>;
        }

        public override void MoveVisually(Vector2 d)
        {
            for (int i = 0; i < DrawPath.Count; i++)
            {
                DrawPath[i] += d;
            }
        }

        internal bool step = false;
        public override void Update()
        {
            base.Update();

            if (step && Visible && (parent as Wire).VoltageDropAbs > 0.0001)
                StepElectrons();
        }

        public override void Draw(MicroWorld.Graphics.Renderer renderer)
        {
            //renderer.DrawString(MicroWorld.Graphics.GUI.GUIEngine.font, Math.Round((parent as Wire).Current, 3).ToString(), (parent as Wire).J1.Graphics.Center, Color.Red, MicroWorld.Graphics.Renderer.TextAlignment.Left);

            if (!Visible)
                return;
            if (!CanDraw()) return;

            Wire p = (Wire)parent;
            Color cs = new Color(0.8f, 0.8f, 0.8f);
            if (Settings.GameState != Settings.GameStates.Stopped)
                cs = p.VoltageDropAbs > 0.001 ? new Color(1f, 0.8f, 0.8f) : Color.White;

            //renderer.DrawString(MicroWorld.Graphics.GUI.GUIEngine.font, electrons.Count.ToString(), DrawPath[0], Color.Red, MicroWorld.Graphics.Renderer.TextAlignment.Left);

            Color c = p.IsBurnt ? Color.Red : cs;
            for (int j = 0; j < DrawPath.Count - 1; j++)
            {
                renderer.Draw(Shortcuts.pixel, new Rectangle((int)DrawPath[j].X - 2, (int)DrawPath[j].Y - 2,
                        (int)(DrawPath[j + 1].X - DrawPath[j].X) + 4, (int)(DrawPath[j + 1].Y - DrawPath[j].Y) + 4), c);
                renderer.Draw(Shortcuts.pixel, new Rectangle((int)DrawPath[j].X - 2, (int)DrawPath[j].Y - 2, 4, 4), c);
            }

            float scale = 0.5f + (float)p.J1.Voltage / 10f;
            scale = (scale > 1f ? 1f : scale < 0 ? 0 : scale) * 2;
            float scale2 = scale * 2;
            Vector2 tv;
            try
            {
                for (int i = 0; i < electrons.Count; i++)
                {
                    tv = Vector2.Lerp(DrawPath[electrons[i].curind], DrawPath[electrons[i].curind + 1], electrons[i].lerpState);
                    renderer.Draw(Electron.texture, tv.X - scale, tv.Y - scale, scale2, scale2, Color.Orange);
                }
            }
            catch
            {
                GenerateElectrons();
            }
        }

        internal override void PostDraw(MicroWorld.Graphics.Renderer renderer)
        {
            Wire p = (Wire)parent;
            if (!Visible && Settings.DrawInvisibleWires)
            {
                renderer.GraphicsDevice.Textures[0] = Shortcuts.pixel;
                renderer.DrawLine(p.J1.Graphics.Center.X, p.J1.Graphics.Center.Y, p.J2.Graphics.Center.X, p.J2.Graphics.Center.Y, Color.Lime, Color.Lime);
            }
        }

        public void OnDrawPathChanged()
        {
            GenerateElectrons();
        }
        
        internal void GenerateElectrons()
        {
            electrons.Clear();
            int cur = 8;
            int l = 0;
            for (int i = 0; i < DrawPath.Count - 1; i++)
            {
                l = (int)(DrawPath[i + 1] - DrawPath[i]).Length();
                while (cur < l)
                {
                    Electron e = new Electron();
                    e.curind = i;
                    e.lerpSpeed = 1.0f / l;
                    e.lerpState = (float)cur / l;
                    electrons.Add(e);
                    cur += ElectronFrequency;
                }
                cur -= l;
            }
        }

        internal void StepElectrons()
        {
            if (parent.ID == 189)
            {
                int asf = 0;
            }
            Wire p = parent as Wire;
            float mul = Math.Sign(p.J2.Voltage - p.J1.Voltage);
            if (mul == 0)
                return;

            //update
            for (int i = 0; i < electrons.Count; i++)
            {
                electrons[i].lerpState += electrons[i].lerpSpeed * mul;
                if (electrons[i].lerpState < 0)
                {
                    if (electrons[i].curind == 0)
                        electrons.RemoveAt(i--);
                    else
                    {
                        //get overshot distance
                        int l = (int)(Vector2.Lerp(DrawPath[electrons[i].curind], DrawPath[electrons[i].curind + 1], electrons[i].lerpState) - 
                            DrawPath[electrons[i].curind]).Length();

                        electrons[i].curind--;
                        float tl = (DrawPath[electrons[i].curind + 1] - DrawPath[electrons[i].curind]).Length();
                        electrons[i].lerpState = (tl - l) / tl;
                        electrons[i].lerpSpeed = 1.0f / tl;
                    }
                }
                else if (electrons[i].lerpState > 1)
                {
                    if (electrons[i].curind == DrawPath.Count - 2)
                        electrons.RemoveAt(i--);
                    else
                    {
                        //get overshot distance
                        int l = (int)(Vector2.Lerp(DrawPath[electrons[i].curind], DrawPath[electrons[i].curind + 1], electrons[i].lerpState) -
                            DrawPath[electrons[i].curind + 1]).Length();

                        electrons[i].curind++;
                        float tl = (DrawPath[electrons[i].curind + 1] - DrawPath[electrons[i].curind]).Length();
                        electrons[i].lerpState = l / tl;
                        electrons[i].lerpSpeed = 1.0f / tl;
                    }
                }
            }

            //add
            float ll = (DrawPath[1] - DrawPath[0]).Length();
            if (electrons[0].curind != 0 || electrons[0].lerpState > (float)ElectronFrequency / ll)
            {
                Electron e = new Electron();
                e.curind = 0;
                e.lerpSpeed = 1.0f / ll;
                //e.lerpState = electrons[0].lerpState - (float)ElectronFrequency / ll;
                e.lerpState = 0f;
                electrons.Insert(0, e);
            }
            ll = (DrawPath[DrawPath.Count - 1] - DrawPath[DrawPath.Count - 2]).Length();
            if (electrons[electrons.Count - 1].curind != DrawPath.Count - 2 || electrons[electrons.Count - 1].lerpState < (ll - (float)ElectronFrequency) / ll)
            {
                Electron e = new Electron();
                e.curind = DrawPath.Count - 2;
                e.lerpSpeed = 1.0f / ll;
                //e.lerpState = electrons[electrons.Count - 1].lerpState + (float)ElectronFrequency / ll;
                e.lerpState = 1.0f;
                electrons.Add(e);
            }
        }

        internal static void DrawWire(int x1, int y1, int x2, int y2)
        {
            if (x1 == x2 && y1 == y2) return;
            var DrawPath = new List<Vector2>();
            var path = RunPathFinder(x1, y1, x2, y2);

            DrawPath.Clear();
            if (path == null) return;
            for (int i = 0; i < path.Count; i++)
            {
                DrawPath.Add(new Vector2(path[i].X, path[i].Y));
            }
            OptimizeDrawPath(ref DrawPath);

            //Main.renderer.Draw(global::MicroWorld.Graphics.GraphicsEngine.pixel, new Rectangle(0, 0, 0, 0), Color.White);
            //Main.renderer.End();
            /*
            for (float i = -0.5f; i < 0.5f; i += 0.2f)
            {
                Main.renderer.DrawLine(x1 + i, y1,
                                  x2 + i, y2,
                                  Color.Lime, Color.Lime);
                Main.renderer.DrawLine(x1, y1 + i,
                                  x2, y2 + i,
                                  Color.Lime, Color.Lime);
            }
            //*/
            Color c = Color.Lime;
            for (int j = 0; j < DrawPath.Count - 1; j++)
            {
                Shortcuts.renderer.Draw(Shortcuts.pixel, new Rectangle((int)DrawPath[j].X - 2, (int)DrawPath[j].Y - 2,
                        (int)(DrawPath[j + 1].X - DrawPath[j].X) + 4, (int)(DrawPath[j + 1].Y - DrawPath[j].Y) + 4), c);
                Shortcuts.renderer.Draw(Shortcuts.pixel, new Rectangle((int)DrawPath[j].X - 2, (int)DrawPath[j].Y - 2, 4, 4), c);
            }
            //Main.renderer.Begin();
        }

        public override void DrawBorder(MicroWorld.Graphics.Renderer renderer)
        {
            #region Dotted Lines
            if (DrawPath != null && DrawPath.Count > 0)
            {
                MicroWorld.Graphics.RenderHelper.DrawDottedLinesToBorders(
                    DrawPath.ToArray(), Color.White, renderer, true);
            }
            else
            {
                if (Size.X == -1)
                {
                    MicroWorld.Graphics.RenderHelper.DrawDottedLines(
                        new float[] { Position.X, Position.X },
                        new float[] { Position.Y, Position.Y },
                        new float[] { float.MaxValue, Position.X },
                        new float[] { Position.Y, float.MaxValue },
                        Color.White, renderer, true);
                }
                else
                {
                    MicroWorld.Graphics.RenderHelper.DrawDottedLines(
                        new float[] { Position.X, Position.X, Size.X, Size.X },
                        new float[] { Position.Y, Position.Y, Size.Y, Size.Y },
                        new float[] { float.MaxValue, Position.X, Size.X, float.MaxValue },
                        new float[] { Position.Y, float.MaxValue, float.MaxValue, Size.Y },
                        Color.White, renderer, true);
                }
            }
            #endregion
        }

        public override void DrawBorder(float x, float y, Component.Rotation rot, MicroWorld.Graphics.Renderer renderer)
        {
            int xt = InputEngine.curMouse.X;
            int yt = InputEngine.curMouse.Y;
            Utilities.Tools.ScreenToGameCoords(ref xt, ref yt);
            MicroWorld.Logics.GridHelper.GridCoords(ref xt, ref yt);
            var b = Position;
            Position = new Vector2(xt, yt);
            var a = Size;
            Size = new Vector2(-1, -1);
            DrawBorder(renderer);
            Size = a;
            Position = b;
        }

        public static void DrawBorder(List<Vector2> p, MicroWorld.Graphics.Renderer renderer)
        {
            #region Dotted Lines
            MicroWorld.Graphics.RenderHelper.DrawDottedLinesToBorders(
                p.ToArray(), Color.White, renderer, true);
            #endregion
        }

        internal static void DrawWire(List<Point> path)
        {
            var DrawPath = new List<Vector2>();
            if (path == null) return;
            for (int i = 0; i < path.Count; i++)
            {
                DrawPath.Add(new Vector2(path[i].X, path[i].Y));
            }
            OptimizeDrawPath(ref DrawPath);

            //Main.renderer.Draw(MicroWorld.Graphics.GraphicsEngine.pixel, new Rectangle(0, 0, 0, 0), Color.White);
            //Main.renderer.End();
            Color c = Color.White;
            for (int j = 0; j < DrawPath.Count - 1; j++)
            {
                Shortcuts.renderer.Draw(Shortcuts.pixel, new Rectangle((int)DrawPath[j].X - 2, (int)DrawPath[j].Y - 2,
                        (int)(DrawPath[j + 1].X - DrawPath[j].X) + 4, (int)(DrawPath[j + 1].Y - DrawPath[j].Y) + 4), c);
                Shortcuts.renderer.Draw(Shortcuts.pixel, new Rectangle((int)DrawPath[j].X - 2, (int)DrawPath[j].Y - 2, 4, 4), c);
            }
            //Main.renderer.Begin();
        }

        public override bool IntersectsWith(int x, int y, int w, int h)
        {
            for (int i = 0; i < DrawPath.Count - 1; i++)
            {
                if (MicroWorld.Logics.MathHelper.LineIntersectsRect(DrawPath[i], DrawPath[i + 1], new Rectangle(x, y, w, h))) return true;
            }
            return false;
        }

        public override bool CanDraw()
        {
            Wire p = (Wire)parent;
            Position = new Vector2(
                Math.Min(p.J1.Graphics.Position.X, p.J2.Graphics.Position.X),
                Math.Min(p.J1.Graphics.Position.Y, p.J2.Graphics.Position.Y));
            Size = new Vector2(p.J1.Graphics.Position.X + p.J2.Graphics.Position.X - Position.X * 2,
                               p.J1.Graphics.Position.Y + p.J2.Graphics.Position.Y - Position.Y * 2);
            return base.CanDraw();
        }

        internal Vector2 GetPosSubButtons()
        {
            var a = parent as Wire;
            float x = (a.J1.Graphics.Position.X + a.J2.Graphics.Position.X) / 2f;
            for (int i = 0; i < DrawPath.Count - 1; i++)
            {
                if ((DrawPath[i].X > x && DrawPath[i + 1].X < x) ||
                    (DrawPath[i].X < x && DrawPath[i + 1].X > x))
                {
                    return new Vector2(x, DrawPath[i].Y);
                }
            }
            return new Vector2(x, (float)Math.Max(a.J1.Graphics.Position.Y, a.J2.Graphics.Position.Y));
        }
    }
}
