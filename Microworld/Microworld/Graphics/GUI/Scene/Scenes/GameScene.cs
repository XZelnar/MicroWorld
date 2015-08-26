using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MicroWorld.Graphics.GUI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MicroWorld.Graphics.GUI.Scene
{
    class GameScene : Scene
    {
        public override void Initialize()
        {
            Layer = -1000;
            ShouldBeScaled = false;
            base.Initialize();

            LightAOE.Initialize();
            MagnetAOE.Initialize();
        }

        public override void LoadContent()
        {
            GridDraw.LoadContent();
            base.LoadContent();

            LightAOE.LoadContent();
            MagnetAOE.LoadContent();
        }

        public override void OnResolutionChanged(int w, int h, int oldw, int oldh)
        {
            base.OnResolutionChanged(w, h, oldw, oldh);

            LightAOE.OnResolutionChanged(w, h, oldw, oldh);
            MagnetAOE.OnResolutionChanged(w, h, oldw, oldh);
        }

        public override void Update()
        {
            base.Update();

            LightAOE.Update();
            MagnetAOE.Update();
            Logics.GameInputHandler.Update();
        }

        public override void Draw(Renderer renderer)
        {
            if (Main.curState.StartsWith("GAME"))
            {
                //Grid
                #region Grid
                MicroWorld.Graphics.GUI.GridDraw.DrawGrid();
                #endregion


                //AoE-s
                #region AoE-s
                renderer.End();
                renderer.BeginUnscaled(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone);
                LightAOE.Draw(renderer);
                MagnetAOE.Draw(renderer);
                renderer.End();
                renderer.Begin();
                #endregion


                //Components
                #region Components
                Components.ComponentsManager.Draw();
                Components.ComponentsManager.PostDraw();
                #endregion


                //SelectedGhost
                #region SelectedGhost
                if (MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponent.Text != "Cursor" &&
                    InputEngine.curMouse.X > GUI.Scene.ComponentSelector.CSTile.SIZE_X && !Logics.GameInputHandler.isLine)// &&
                //!MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponent.IsComponentOfType<Components.Properties.IDragDropPlacable>())
                {
                    int x = (int)((InputEngine.curMouse.X) / Settings.GameScale - Settings.GameOffset.X);
                    int y = (int)((InputEngine.curMouse.Y) / Settings.GameScale - Settings.GameOffset.Y);
                    Components.Component.Rotation rot = Logics.GameInputHandler.GhostRotation;
                    Vector2 size = MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponentGraphics.GetSizeRotated(rot);
                    Logics.GridHelper.GridCoords(ref x, ref y);
                    Logics.PlacableAreasManager.MakePlacable(ref x, ref y, (int)size.X, (int)size.Y);
                    Logics.GridHelper.GridCoords(ref x, ref y);
                    Vector2 so = MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponentGraphics.GetCenter(rot);
                    int nx = x - (int)so.X, ny = y - (int)so.Y;

                    bool b = GraphicsEngine.CanDrawGhostComponent(ref nx, ref ny, (int)size.X, (int)size.Y);

                    Color c = Main.renderer.Overlay;
                    if (!b)
                        renderer.Overlay = Color.Red * 0.5f;
                    if (InputEngine.curMouse.LeftButton == ButtonState.Pressed &&
                        GUI.GUIEngine.s_componentSelector.SelectedComponent.Avalable == 0)
                        Main.renderer.Overlay = Main.Ticks % 40 < 20 ? Color.Red : c;
                    MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponentGraphics.DrawGhost(
                        nx,
                        ny,
                        Main.renderer, Logics.GameInputHandler.GhostRotation);
                    Main.renderer.Overlay = c;

                    #region Dotted Lines
                    MicroWorld.Graphics.GUI.GUIEngine.s_componentSelector.SelectedComponentGraphics.DrawBorder(
                        nx, ny, Logics.GameInputHandler.GhostRotation, renderer);
                    #endregion
                }
                #endregion


                //RemovingComponentsVisuals
                #region RemovingComponentsVisuals
                MicroWorld.Graphics.Effects.Effects.DrawRemovingVisuals();
                #endregion


                //MouseOverComponent
                #region MouseOverComponent
                if (Logics.GameInputHandler.MouseOverComponent != null)
                {
                    Logics.GameInputHandler.MouseOverComponent.Graphics.DrawBorder(renderer);
                }
                #endregion


                //wire
                #region Wire
                if (Logics.GameInputHandler.isLine)
                {
                    lock (Logics.GameInputHandler.pendingWirePath)
                        Components.Graphics.WireGraphics.DrawWire(Logics.GameInputHandler.pendingWirePath);

                    #region Dotted Lines
                    if (Logics.GameInputHandler.pendingWirePath != null && Logics.GameInputHandler.pendingWirePath.Count > 0)
                    {
                        //optimization
                        List<Vector2> l = new List<Vector2>();
                        Vector2 v1 = new Vector2(), v2 = new Vector2();
                        lock (Logics.GameInputHandler.pendingWirePath)
                        {
                            l.Add(new Vector2(Logics.GameInputHandler.pendingWirePath[0].X, Logics.GameInputHandler.pendingWirePath[0].Y));
                            for (int i = 1; i < Logics.GameInputHandler.pendingWirePath.Count - 1; i++)
                            {
                                v1 = new Vector2(Logics.GameInputHandler.pendingWirePath[i].X - Logics.GameInputHandler.pendingWirePath[i - 1].X,
                                                 Logics.GameInputHandler.pendingWirePath[i].Y - Logics.GameInputHandler.pendingWirePath[i - 1].Y);
                                v2 = new Vector2(Logics.GameInputHandler.pendingWirePath[i + 1].X - Logics.GameInputHandler.pendingWirePath[i].X,
                                                 Logics.GameInputHandler.pendingWirePath[i + 1].Y - Logics.GameInputHandler.pendingWirePath[i].Y);

                                if (((v1.X != 0) != (v2.X != 0)) ||
                                    (v1.Y != 0) != (v2.Y != 0))
                                {
                                    l.Add(new Vector2(Logics.GameInputHandler.pendingWirePath[i].X, Logics.GameInputHandler.pendingWirePath[i].Y));
                                }
                            }
                            l.Add(new Vector2(Logics.GameInputHandler.pendingWirePath[Logics.GameInputHandler.pendingWirePath.Count - 1].X,
                                              Logics.GameInputHandler.pendingWirePath[Logics.GameInputHandler.pendingWirePath.Count - 1].Y));
                        }
                        //draw
                        Components.Graphics.WireGraphics.DrawBorder(l, renderer);
                    }
                    #endregion
                }
                #endregion


                //dnd
                #region DnD
                if (Logics.GameInputHandler.isComponentDnD)
                {
                    int x = (int)((InputEngine.curMouse.X) / Settings.GameScale - Settings.GameOffset.X);
                    int y = (int)((InputEngine.curMouse.Y) / Settings.GameScale - Settings.GameOffset.Y);
                    Logics.PlacableAreasManager.MakePlacable(ref x, ref y);
                    MicroWorld.Logics.GridHelper.GridCoords(ref x, ref y);
                    Logics.GameInputHandler.DnDComponent.DrawGhost(Main.renderer,
                        (int)Logics.GameInputHandler.pendingWireP1.X, (int)Logics.GameInputHandler.pendingWireP1.Y, x, y);

                    #region Dotted Lines
                    (Logics.GameInputHandler.DnDComponent as Components.Component).Graphics.DrawBorder(renderer);
                    #endregion
                }
                #endregion


                //Pending placable area
                #region PlacableAreas
                if (PlacableAreasCreator.create)
                {
                    int x = InputEngine.curMouse.X, y = InputEngine.curMouse.Y;
                    Utilities.Tools.ScreenToGameCoords(ref x, ref y);
                    Logics.GridHelper.GridCoords(ref x, ref y);
                    RenderHelper.DrawDottedLinesToBorders(new Point[] { new Point(x, y) }, Color.White, renderer, true);
                }

                if (Logics.GameInputHandler.isPlacableAreaPending)
                {
                    int x = (int)Logics.GameInputHandler.pendingWireP1.X;
                    int y = (int)Logics.GameInputHandler.pendingWireP1.Y;
                    int w = InputEngine.curMouse.X;
                    int h = InputEngine.curMouse.Y;
                    Utilities.Tools.ScreenToGameCoords(ref w, ref h);
                    Logics.GridHelper.GridCoords(ref w, ref h);
                    if (w < x)
                    {
                        int t = w;
                        w = x;
                        x = t;
                    }
                    if (h < y)
                    {
                        int t = h;
                        h = y;
                        y = t;
                    }
                    w -= x;
                    h -= y;
                    Main.renderer.Draw(GraphicsEngine.pixel, new Rectangle((int)x, (int)y, (int)w, (int)h), Color.Yellow * 0.4f);
                    RenderHelper.DrawDottedLinesToBorders(
                        new Point[] { 
                            new Point(x, y),
                            new Point(x + w, y),
                            new Point(x, y + h),
                            new Point(x + w, y + h)
                        },
                        Color.White, renderer, true);
                }
                else
                {
                    //highlight deleting area
                    if (Graphics.GUI.Scene.PlacableAreasCreator.delete)
                    {
                        Rectangle r;
                        for (int i = 0; i < Logics.PlacableAreasManager.areas.Count; i++)
                        {
                            r = Logics.PlacableAreasManager.areas[i];
                            int x = InputEngine.curMouse.X;
                            int y = InputEngine.curMouse.Y;
                            Utilities.Tools.ScreenToGameCoords(ref x, ref y);

                            if (r.Contains(x, y))
                            {
                                Main.renderer.Draw(GraphicsEngine.pixel, new Rectangle(r.X, r.Y, r.Width, r.Height), Color.Yellow * 0.4f);
                                RenderHelper.DrawDottedLinesToBorders(
                                    new Point[] { 
                                        new Point(r.X, r.Y),
                                        new Point(r.X + r.Width, r.Y),
                                        new Point(r.X, r.Y + r.Height),
                                        new Point(r.X + r.Width, r.Y + r.Height)
                                    },
                                    Color.White, renderer, true);
                                break;
                            }
                        }
                    }
                }
                #endregion


                //Particles
                #region Particles
                ParticleManager.Draw();
                #endregion
            }
        }

        public override void onButtonUp(InputEngine.MouseArgs e)
        {
            if (GUIEngine.ContainsHUDScene(GUIEngine.s_mainMenu))
                return;
            base.onButtonUp(e);
        }

    }
}
