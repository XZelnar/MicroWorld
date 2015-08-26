﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MicroWorld.Graphics.GUI.Scene.MapOverlays
{
    class MagnetOverlay : HUDScene
    {
        public static Texture2D arrow;

        public class ArrowedLine
        {
            VertexPositionColorTexture p1, p2;
            Components.Component c1, c2;
            float opacity = 0f;
            VertexPositionColorTexture[] arr = new VertexPositionColorTexture[2];
            FadeState fs = FadeState.FadeIn;
            public bool IsDead = false;
            Matrix arrowMatrix;
            float length = 0;
            float range = 0;

            public ArrowedLine(VertexPositionColorTexture cp1, VertexPositionColorTexture cp2, Components.Component cc1, Components.Component cc2)
            {
                p1 = cp1;
                p2 = cp2;
                c1 = cc1;
                c2 = cc2;
                arrowMatrix = Matrix.CreateRotationZ(-(float)Math.Atan2(c2.Graphics.Center.X - c1.Graphics.Center.X, c2.Graphics.Center.Y - c1.Graphics.Center.Y));
                length = Math.Abs((c2.Graphics.Center - c1.Graphics.Center).Length());
            }

            public void FadeOut()
            {
                fs = FadeState.FadeOut;
            }

            public void Update()
            {
                if (fs == FadeState.FadeIn)
                {
                    opacity += 0.1f;
                    if (opacity >= 1f)
                    {
                        opacity = 1f;
                        fs = FadeState.None;
                    }
                }

                if (fs == FadeState.FadeOut)
                {
                    opacity -= 0.05f;
                    if (opacity <= 0f)
                    {
                        opacity = 0f;
                        fs = FadeState.None;
                        IsDead = true;
                    }
                }

                range = (c1 as Components.Properties.IMagnetic).GetRadius();
                if (range > 0)
                    p1.Color = Color.Red;
                else
                    p1.Color = Color.LightBlue;
                p2.Color = p1.Color * (c2 as Components.Properties.IUsesMagnetism).GetMapLineOpacity(c1 as Components.Properties.IMagnetic);

                p1.Position = new Vector3(c1.Graphics.Center, 0);
                p2.Position = new Vector3(c2.Graphics.Center, 0);
                arrowMatrix = Matrix.CreateRotationZ(-(float)Math.Atan2(c2.Graphics.Center.X - c1.Graphics.Center.X, c2.Graphics.Center.Y - c1.Graphics.Center.Y));
            }

            public void Draw(Renderer renderer)
            {
                arr[0] = p1;
                arr[1] = p2;
                if (opacity <= 1)
                {
                    arr[0].Color *= opacity;
                    arr[1].Color *= opacity;
                }
                renderer.DrawLinesList(arr);
            }

            float r, t;
            Vector2 a;
            Matrix m;
            public void DrawArrow(Renderer renderer)
            {
                if (p1.Position == p2.Position)
                    return;
                r = (c1 as Components.Properties.IMagnetic).GetRadius();
                t = (float)Main.Ticks % length;
                if (r > 0)
                    a = Vector2.Lerp(c1.Graphics.Center, c2.Graphics.Center, t / length);
                else
                    a = Vector2.Lerp(c2.Graphics.Center, c1.Graphics.Center, t / length);
                m = Matrix.CreateTranslation(new Vector3(-16, -16, 0)) * Matrix.CreateScale(0.15f, 0.2f, 1f) * 
                    arrowMatrix * Matrix.CreateRotationZ(r > 0 ? 0 : (float)Math.PI) * Matrix.CreateTranslation(new Vector3(a, 0));
                renderer.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                    null, m, true);
                renderer.Draw(arrow, new Vector2(), (r > 0 ? Color.Red : Color.LightBlue) * opacity * (t < 20 ? t / 20f : t > length - 20 ? (-t + length) / 20f : 1f));
                renderer.End();
            }
        }

        private List<OverlayToolTip> tooltips = new List<OverlayToolTip>();
        private List<ArrowedLine> lines = new List<ArrowedLine>(64);
        private List<ArrowedLine> fadeOutLines = new List<ArrowedLine>();

        private RenderTarget2D fbo;
        public bool ScheduledReGen = false;
        private Components.Component selectedComponent = null;
        public float opacity = 0f;
        private FadeState fadeState = FadeState.None;
        private bool isActive = false;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (value)
                {
                    fadeState = FadeState.FadeIn;
                    isActive = true;
                    MagnetAOE.Visible = true;
                }
                else
                {
                    fadeState = FadeState.FadeOut;
                }
            }
        }
        public Components.Component SelectedComponent
        {
            get { return selectedComponent; }
            set
            {
                selectedComponent = value;
                ReGenAll();
            }
        }



        public void ReGenFBO()
        {
            if (fbo != null)
                fbo.Dispose();
            fbo = new RenderTarget2D(GraphicsEngine.Renderer.GraphicsDevice, Main.WindowWidth, Main.WindowHeight);
        }

        public void GenMap()
        {
            while (tooltips.Count > 0)
            {
                tooltips[0].Dispose();
                tooltips.RemoveAt(0);
            }

            var t = selectedComponent is Components.Properties.IMagnetic ? selectedComponent : null;
            foreach (var c in Components.ComponentsManager.Components)
            {
                if (c is Components.Properties.IUsesMagnetism)
                {
                    tooltips.Add(new OverlayToolTip(c as Components.Properties.IUsesMagnetism, t));
                }
            }
        }

        public void GenLines()
        {
            List<Components.Component> emitters = new List<Components.Component>(), users = new List<Components.Component>();

            foreach (var c in Components.ComponentsManager.Components)
            {
                if (c is Components.Properties.IUsesMagnetism)
                {
                    users.Add(c);
                }
                if (c is Components.Properties.IMagnetic)
                {
                    emitters.Add(c);
                }
            }

            fadeOutLines.AddRange(lines);
            for (int i = 0; i < fadeOutLines.Count; i++)
            {
                fadeOutLines[i].FadeOut();
            }

            lines.Clear();
            VertexPositionColorTexture e, u;
            Color col;
            for (int i = 0; i < emitters.Count; i++)
            {
                if (selectedComponent != null && selectedComponent is Components.Properties.IMagnetic && selectedComponent != emitters[i])
                    continue;

                col = (emitters[i] as Components.Properties.IMagnetic).GetRadius() > 0 ? Color.Red : Color.Blue;
                e = new VertexPositionColorTexture(new Vector3(emitters[i].Graphics.Position + emitters[i].Graphics.GetSize() / 2, 0), 
                    col, new Vector2());
                for (int j = 0; j < users.Count; j++)
                {
                    if (selectedComponent != null && selectedComponent is Components.Properties.IUsesMagnetism && selectedComponent != users[j])
                        continue;

                    if ((emitters[i] as Components.Properties.IMagnetic).IsInRange(users[j]))
                    {
                        u = new VertexPositionColorTexture(new Vector3(users[j].Graphics.Position + users[j].Graphics.GetSize() / 2, 0),
                            col * (users[j] as Components.Properties.IUsesMagnetism).GetMapLineOpacity(emitters[i] as Components.Properties.IMagnetic), new Vector2());
                        lines.Add(new ArrowedLine(e, u, emitters[i], users[j]));
                    }
                    else
                    {
                        lines.Add(new ArrowedLine(e, e, emitters[i], users[j]));
                    }
                }
            }
        }

        public void ReGenAll()
        {
            GenMap();
            GenLines();
        }

        public override void Initialize()
        {
            for (int i = 0; i < tooltips.Count; i++)
            {
                tooltips[i].Initialize();
            }

            GlobalEvents.onComponentPlacedByPlayer += new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentPlacedByPlayer);
            GlobalEvents.onComponentRemovedByPlayer += new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentRemovedByPlayer);
            GlobalEvents.onComponentMoved += new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentMoved);
            GlobalEvents.onComponentSelected += new GlobalEvents.ComponentEventHandler(GlobalEvents_onComponentSelected);

            ReGenFBO();

            base.Update();
        }

        public override void LoadContent()
        {
            arrow = ResourceManager.Load<Texture2D>("GUI/HUD/MapOverlays/Arrow");

            base.LoadContent();
        }

        void GlobalEvents_onComponentSelected(Components.Component sender)
        {
            if (sender is Components.Properties.IUsesMagnetism|| sender is Components.Properties.IMagnetic)
                SelectedComponent = sender;
            else
                SelectedComponent = null;
        }

        void GlobalEvents_onComponentMoved(Components.Component sender)
        {
            ScheduledReGen = true;
        }

        void GlobalEvents_onComponentRemovedByPlayer(Components.Component sender)
        {
            ScheduledReGen = true;
        }

        void GlobalEvents_onComponentPlacedByPlayer(Components.Component sender)
        {
            ScheduledReGen = true;
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            ReGenFBO();

            base.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void onShow()
        {
            ReGenAll();

            base.onShow();
        }

        public override void Update()
        {
            if (!isActive)
                return;

            if (ScheduledReGen)
            {
                ScheduledReGen = false;
                ReGenAll();
            }

            for (int i = 0; i < tooltips.Count; i++)
            {
                tooltips[i].Update();
            }

            if (selectedComponent != null && !selectedComponent.GetComponentToolTip().isVisible)
            {
                SelectedComponent = null;
            }

            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].Update();
            }
            for (int i = 0; i < fadeOutLines.Count; i++)
            {
                fadeOutLines[i].Update();
                if (fadeOutLines[i].IsDead)
                {
                    fadeOutLines.RemoveAt(i);
                    i--;
                }
            }

            #region Fades
            if (fadeState == FadeState.FadeIn)
            {
                opacity += 0.07f;
                if (opacity >= 1)
                {
                    opacity = 1;
                    fadeState = FadeState.None;
                }
            }
            if (fadeState == FadeState.FadeOut)
            {
                opacity -= 0.07f;
                if (opacity <= 0)
                {
                    opacity = 0;
                    fadeState = FadeState.None;
                    isActive = false;
                    MagnetAOE.Visible = false;
                }
            }
            MagnetAOE.FadeOpacity = opacity;
            #endregion

            var r = GraphicsEngine.Renderer;
            r.EnableFBO(fbo);
            r.GraphicsDevice.Clear(Color.Transparent);
            r.Begin();
            DrawToFBO(r);
            r.End();
            r.DisableFBO();

            base.Update();
        }

        public override void Draw(Renderer renderer)
        {
            if (!isActive)
                return;

            renderer.Draw(fbo, new Vector2(), Color.White * opacity);

            base.Draw(renderer);
        }

        public void DrawToFBO(Renderer renderer)
        {
            if (lines.Count > 0 || fadeOutLines.Count > 0)
            {
                renderer.End();
                renderer.Begin();
                for (int i = 0; i < lines.Count; i++)
                {
                    renderer.GraphicsDevice.Textures[0] = GraphicsEngine.pixel;
                    lines[i].Draw(renderer);
                }
                for (int i = 0; i < fadeOutLines.Count; i++)
                {
                    renderer.GraphicsDevice.Textures[0] = GraphicsEngine.pixel;
                    fadeOutLines[i].Draw(renderer);
                }
                renderer.End();

                for (int i = 0; i < lines.Count; i++)
                {
                    lines[i].DrawArrow(renderer);
                }
                for (int i = 0; i < fadeOutLines.Count; i++)
                {
                    fadeOutLines[i].DrawArrow(renderer);
                }

                renderer.BeginUnscaled();
            }

            for (int i = 0; i < tooltips.Count; i++)
            {
                tooltips[i].Draw(renderer);
            }
        }
    }
}