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

namespace MicroWorld.Components
{
    public static class ComponentsManager
    {
        internal static List<Component> Components = new List<Component>();
        internal static List<Properties.ILightEmitting> LightEmittingComponents = new List<Properties.ILightEmitting>();
        internal static List<Properties.IMagnetic> MagnetComponents = new List<Properties.IMagnetic>();
        internal static List<Type> RegisteredComponents = new List<Type>();
        internal static List<int> Layers = new List<int>();
        internal static VisibilityMap mapVisibility = new VisibilityMap();
        internal static Colliders.CollidersManager collidersManager = new Colliders.CollidersManager();
        internal static Dictionary<Type, short> TypeIDs = new Dictionary<Type, short>(32);
        public static Colliders.CollidersManager CollidersManager
        {
            get { return ComponentsManager.collidersManager; }
        }
        public static VisibilityMap VisibilityMap
        {
            get { return ComponentsManager.mapVisibility; }
        }

        internal static List<GUI.GeneralProperties> closingProperties = new List<GUI.GeneralProperties>();

        public static bool ContainsVisibleComponents//TODO OPTIMIZE!!!
        {
            get
            {
                if (Components.Count == 0) return false;
                for (int i = 0; i < Components.Count; i++)
                {
                    if (Components[i].Graphics.Visible) return true;
                }
                return false;
            }
        }

        public static void Add(Component c)
        {
            Components.Add(c);

            if (c is Properties.ICore)
            {
                (c as Properties.ICore).onFinishedRecieving += new Properties.OnRecievedEventHandler(ComponentsManager_onFinishedRecieving);
            }
            if (c is Properties.ILightEmitting)
            {
                LightEmittingComponents.Add(c as Properties.ILightEmitting);
            }
            if (c is Properties.IMagnetic)
            {
                MagnetComponents.Add(c as Properties.IMagnetic);
            }

            if (Settings.GameState != Settings.GameStates.Stopped)
                c.Start();
        }

        public static Component Get(Component c)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i] == c) return Components[i];
            }
            return null;
        }

        public static void Replace(Component c1, Component c2)
        {
            if (c1 is Properties.ICore)
            {
                (c1 as Properties.ICore).onFinishedRecieving -= new Properties.OnRecievedEventHandler(ComponentsManager_onFinishedRecieving);
            }
            if (c1 is Properties.ILightEmitting)
            {
                LightEmittingComponents.Remove(c1 as Properties.ILightEmitting);
            }
            if (c2 is Properties.ILightEmitting)
            {
                LightEmittingComponents.Add(c2 as Properties.ILightEmitting);
            }
            if (c1 is Properties.IMagnetic)
            {
                MagnetComponents.Remove(c1 as Properties.IMagnetic);
            }
            if (c2 is Properties.IMagnetic)
            {
                MagnetComponents.Add(c2 as Properties.IMagnetic);
            }
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i] == c1)
                {
                    Components[i] = c2;
                    break;
                }
            }

            for (int i = Components.Count - 1; i >= 0; i--)
            {
                if (Components[i] is EmptyComponent)
                    Components.RemoveAt(i);
                else
                    break;
            }
        }

        public static void Remove(Component c)
        {
            /*
            Components.Remove(c);
            if (c is Properties.ICore)
            {
                (c as Properties.ICore).onFinishedRecieving -= new Properties.OnRecievedEventHandler(ComponentsManager_onFinishedRecieving);
            }
            if (c is Properties.ILightEmitting)
            {
                LightEmittingComponents.Remove(c as Properties.ILightEmitting);
            }
            if (c is Properties.IMagnetic)
            {
                MagnetComponents.Remove(c as Properties.IMagnetic);
            }//*/
            Replace(c, new EmptyComponent(c.ID));
        }

        static void ComponentsManager_onFinishedRecieving(bool correct)
        {
            if (Main.curState == "GAMELevels")
            {
                foreach (var c in Components)
                {
                    if (c is Properties.ICore)
                    {
                        if (!(c as Properties.ICore).IsCorrect())
                            return;
                    }
                }

                MicroWorld.Logics.CampaingProgress.SetCurrentCompleted();
                MicroWorld.Graphics.GUI.GUIEngine.AddHUDScene(MicroWorld.Graphics.GUI.GUIEngine.s_victoryMessage);
            }
        }

        public static void Initialize()
        {
            GlobalEvents.onGraphicsDeviceReset += new GlobalEvents.GraphicDeviceEventHandler(OnGraphicsDeviceReset);
            Colliders.ColliderGroupManager.Initialize();
            LoadDLLs();
        }

        internal static void CircuitInitialized()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Logics.CircuitInitialized();
            }
        }

        internal static void InitAllComponents()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Initialize();
            }
        }

        public static void LoadContent()
        {
            Joint.LoadContentStatic();
            Core.LoadContentStatic();
            PulseCore.LoadContentStatic();
            Wire.LoadContentStatic();
            //PipeJoint.LoadContentStatic();
            //Pipe.LoadContentStatic();

            for (int i = 0; i < RegisteredComponents.Count; i++)
            {
                var m = RegisteredComponents[i].GetMethod("LoadContentStatic");
                if (m != null) m.Invoke(RegisteredComponents[i], new object[0]);
            }
            
            //ATmega8.LoadContentStatic();
        }

        public static void CircuitUpdate()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Logics.CircuitUpdate();
            }
        }

        public static void LastCircuitUpdate()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Logics.LastCircuitUpdate();
            }
        }

        public static void OnResolutionChange(int w, int h, int oldw, int oldh)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].CreateSelectionFBO(w, h);
            }
        }

        public static void PreUpdate()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Logics.PreUpdate();
            }
        }

        public static void Update()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Update();
            }

            collidersManager.Update();
        }

        public static void NonGameUpdate()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].NonGameUpdate();
            }

            for (int i = 0; i < closingProperties.Count; i++)
            {
                closingProperties[i].Update();
                if (closingProperties[i].Opacity <= 0)
                {
                    closingProperties.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void DrawComponent(Component c)
        {
            var vr = MicroWorld.Graphics.GraphicsEngine.camera.VisibleRectangle;
            if (c is Properties.IDrawBorder)
            {
                int x = InputEngine.curMouse.X;
                int y = InputEngine.curMouse.Y;
                Utilities.Tools.ScreenToGameCoords(ref x, ref y);

                if (c.Graphics.Visible && c.isIn(x, y) &&
                    !(MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.isVisible && //no subbottons
                    MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent == c))//or another component
                {
                    c.Graphics.DrawBorder(Main.renderer);
                }
            }
            if (c.TicksSincePlacement > 20)
            {
                if (c.selectionShaderState != 0)
                {
                    MicroWorld.Graphics.Effects.Effects.DrawSelectedBackground(c);
                }
                else
                {
                    c.Draw(Main.renderer);
                }
            }
            else
            {
                Main.renderer.End();
                Main.renderer.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, null, RasterizerState.CullNone,
                    MicroWorld.Graphics.GraphicsEngine.ComponentFadeEffect);

                Matrix projection = Matrix.CreateOrthographicOffCenter(vr.X, vr.X + vr.Width, vr.Y + vr.Height, vr.Y, 0, 1);
                MicroWorld.Graphics.GraphicsEngine.ComponentFadeEffect.Parameters["MatrixTransform"].SetValue(projection);
                MicroWorld.Graphics.GraphicsEngine.ComponentFadeEffect.Parameters["halfpixel"].SetValue(
                    new float[] { 0.5f / vr.Width, 0.5f / vr.Height });
                MicroWorld.Graphics.GraphicsEngine.ComponentFadeEffect.Parameters["Opacity"].SetValue(
                    (float)(c.TicksSincePlacement < 10 ? (float)c.TicksSincePlacement / 10f :
                                                       (float)(10 - (c.TicksSincePlacement - 10)) / 10f));
                MicroWorld.Graphics.GraphicsEngine.ComponentFadeEffect.Parameters["Drawtex"].SetValue(
                    c.TicksSincePlacement >= 10);

                c.Draw(Main.renderer);
                Main.renderer.End();
                Main.renderer.Begin();
            }
        }

        public static void Draw()
        {
            var vr = MicroWorld.Graphics.GraphicsEngine.camera.VisibleRectangle;
            for (int l = 0; l < Layers.Count; l++)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    if (Components[i].Graphics.Layer == Layers[l])
                    {
                        if (Components[i].Graphics.IgnoreNextDraw)
                        {
                            Components[i].Graphics.IgnoreNextDraw = false;
                        }
                        else
                        {
                            if (Components[i] is Properties.IDrawBorder)
                            {
                                int x = InputEngine.curMouse.X;
                                int y = InputEngine.curMouse.Y;
                                Utilities.Tools.ScreenToGameCoords(ref x, ref y);

                                if (Components[i].Graphics.Visible && Components[i].isIn(x, y) &&
                                    !(MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.isVisible && //no subbottons
                                    MicroWorld.Graphics.GUI.GUIEngine.s_subComponentButtons.SelectedComponent == Components[i]))//or another component
                                {
                                    Components[i].Graphics.DrawBorder(Main.renderer);
                                }
                            }
                            if (Components[i].TicksSincePlacement > 20)
                            {
                                if (Components[i].selectionShaderState != 0)
                                {
                                    MicroWorld.Graphics.Effects.Effects.DrawSelectedBackground(Components[i]);
                                }
                                else
                                {
                                    Components[i].Draw(Main.renderer);
                                }
                            }
                            else
                            {
                                Main.renderer.End();
                                Main.renderer.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, null, RasterizerState.CullNone,
                                    MicroWorld.Graphics.GraphicsEngine.ComponentFadeEffect);

                                Matrix projection = Matrix.CreateOrthographicOffCenter(vr.X, vr.X + vr.Width, vr.Y + vr.Height, vr.Y, 0, 1);
                                MicroWorld.Graphics.GraphicsEngine.ComponentFadeEffect.Parameters["MatrixTransform"].SetValue(projection);
                                MicroWorld.Graphics.GraphicsEngine.ComponentFadeEffect.Parameters["halfpixel"].SetValue(
                                    new float[] { 0.5f / vr.Width, 0.5f / vr.Height });
                                MicroWorld.Graphics.GraphicsEngine.ComponentFadeEffect.Parameters["Opacity"].SetValue(
                                    (float)(Components[i].TicksSincePlacement < 10 ? (float)Components[i].TicksSincePlacement / 10f :
                                                                       (float)(10 - (Components[i].TicksSincePlacement - 10)) / 10f));
                                MicroWorld.Graphics.GraphicsEngine.ComponentFadeEffect.Parameters["Drawtex"].SetValue(
                                    Components[i].TicksSincePlacement >= 10);

                                Components[i].Draw(Main.renderer);
                                Main.renderer.End();
                                Main.renderer.Begin();
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < closingProperties.Count; i++)
            {
                closingProperties[i].Draw(MicroWorld.Graphics.GraphicsEngine.Renderer);
            }
            /*
            Joint a;
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i] is Joint && Components[i].Graphics.Visible)
                {
                    a = Components[i] as Joint;
                    Graphics.JointState.Draw((float)a.Voltage, (int)a.Graphics.Position.X, (int)a.Graphics.Position.Y - 4);
                }
            }//*/
        }

        internal static void PostDraw()//TODO rm
        {
            for (int l = 0; l < Layers.Count; l++)
                for (int i = 0; i < Components.Count; i++)
                    if (Components[i].Graphics.Layer == Layers[l])
                        Components[i].Graphics.PostDraw(Shortcuts.renderer);
        }

        public static int GetFreeID()
        {
            int j = 0;
            for (j = 0; ; j++)
            {
                bool free = true;
                for (int i = 0; i < Components.Count && free; i++)
                {
                    if (Components[i].ID == j)
                    {
                        free = false;
                        break;
                    }
                }
                if (free) return j;
            }
        }

        public static Component GetComponent(int id)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].ID == id)
                {
                    return Components[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Ignores wires and joints
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Component GetComponent(int x, int y)
        {
            Component c = null;
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].isIn(x, y))
                {
                    if (c == null)
                        c = Components[i];
                    else
                        if (c.Graphics.Layer < Components[i].Graphics.Layer)
                            c = Components[i];
                }
            }
            return c;
        }

        public static Component GetVisibleComponent(int x, int y)
        {
            return GetTopVisibleComponent(GetVisibleComponents(x, y));
        }

        public static Component GetTopVisibleComponent(Component[] c)
        {
            if (c.Length == 0)
                return null;
            Component r = c[0];
            for (int i = 1; i < c.Length; i++)
                if (c[i].Graphics.Layer > r.Graphics.Layer)
                    r = c[i];
            return r;
        }

        public static Component[] GetVisibleComponents(int x, int y)
        {
            List<Component> components = new List<Component>();
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].Graphics.Visible && !(Components[i] is Properties.IUnselecrable) && Components[i].isIn(x, y))
                {
                    components.Add(Components[i]);
                    //if (Components[i] is Properties.IContainer)
                    //    container = Components[i];
                    //else
                    //    return Components[i];
                }
            }
            return components.ToArray();
        }

        public static List<Component> GetComponents(int x, int y)
        {
            List<Component> r = new List<Component>();
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].isIn(x, y))
                {
                    r.Add(Components[i]);
                }
            }
            return r;
        }

        public static List<Component> GetComponents(int x, int y, float rad)
        {
            List<Component> r = new List<Component>();
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].Graphics.Visible && Components[i].isIn(x, y, rad))
                {
                    r.Add(Components[i]);
                }
            }
            return r;
        }

        public static List<Component> GetComponents(int x, int y, int w, int h)
        {
            List<Component> r = new List<Component>();
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].Graphics.Visible && Components[i].isIn(x, y, w, h))
                {
                    r.Add(Components[i]);
                }
            }
            return r;
        }

        public static List<Component> GetComponents(float x, float y, float w, float h)
        {
            return GetComponents((int)x, (int)y, (int)w, (int)h);
        }

        public static List<Component> GetIntersectingComponents(int x, int y, int w, int h)
        {
            List<Component> r = new List<Component>();
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].Graphics.Visible && Components[i].Intersects(x, y, w, h))
                {
                    r.Add(Components[i]);
                }
            }
            return r;
        }

        public static List<Component> GetIntersectingComponents(float x, float y, float w, float h)
        {
            return GetIntersectingComponents((int)x, (int)y, (int)w, (int)h);
        }

        public static List<T> GetComponents<T>(int x, int y)
        {
            List<T> r = new List<T>();
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i] is T && Components[i].Graphics.Visible && Components[i].isIn(x, y))
                {
                    r.Add((T)((object)Components[i]));
                }
            }
            return r;
        }

        public static List<T> GetComponents<T>(int x, int y, int w, int h)
        {
            List<T> r = new List<T>();
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i] is T)// && Components[i].Graphics.Visible && Components[i].isIn(x, y, w, h))
                {
                    if (Components[i].Graphics.Visible && Components[i].isIn(x, y, w, h))
                        r.Add((T)((object)Components[i]));
                }
            }
            return r;
        }

        public static List<T> GetComponents<T>(int x, int y, float rad)
        {
            List<T> r = new List<T>();
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i] is T && Components[i].Graphics.Visible && Components[i].isIn(x, y, rad))
                {
                    r.Add((T)((object)Components[i]));
                }
            }
            return r;
        }

        public static int GetComponentID(Component c)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i] == c) return i;
            }
            return -1;
        }
        
        public static Texture2D LoadTexture(String path)
        {
            return ResourceManager.Load<Texture2D>(path);
        }

        public static bool CanSeeGrid(int x, int y)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].isIn(x, y)) return false;
            }
            return true;
        }

        public static bool IntersectsWith(int x, int y, int w, int h)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].Graphics.IntersectsWith(x, y, w, h)) 
                    return true;
            }
            return false;
        }

        public static void LoadDLLs()
        {
            IO.Log.Write("    Loading Component DLLs");
            if (!System.IO.Directory.Exists("Components"))
            {
                IO.Log.Write("        Components direction not found");
                System.IO.Directory.CreateDirectory("Components");
                return;
            }
            String[] files = System.IO.Directory.GetFiles("Components/", "*.dll", System.IO.SearchOption.AllDirectories);
            OutputEngine.WriteLine("Attempting to load components. " + files.Length.ToString() + " located");
            IO.Log.Write("        " + files.Length.ToString() + " file(s) found");
            if (files.Length != 0)
            for (int i = 0; i < files.Length; i++)
            {
                OutputEngine.WriteLine("Attemting to load from /" + files[i]);
                IO.Log.Write("        Attempting to load " + files[i]);
                try
                {
                    System.Reflection.Assembly a = System.Reflection.Assembly.LoadFrom(files[i]);
                    Type[] types = a.GetTypes();
                    bool hasComponents = false;
                    for(int j = 0; j < types.Length; j++)
                    {
                        if (types[j].ToString().StartsWith("MicroWorld.Components.") &&
                            !types[j].ToString().StartsWith("MicroWorld.Components.GUI") &&
                            !types[j].ToString().StartsWith("MicroWorld.Components.Graphics") &&
                            !types[j].ToString().StartsWith("MicroWorld.Components.Logics"))
                        {
                            String name = types[j].ToString().Substring(22);
                            if (name != "Component" && name != "ComponentsManager" && name.IndexOf(".") == -1 && //name check
                                Activator.CreateInstance(types[j]) is Components.Component) //actual type check
                            {
                                hasComponents = true;
                                OutputEngine.WriteLine("Found component: " + name);
                                IO.Log.Write("            Component found: " + name);
                                Main.LoadingDetails = "Found component: " + name;
                                RegisteredComponents.Add(types[j]);
                            }
                        }
                        //TODO MOVE!!!
                        if (types[j].IsSubclassOf(typeof(Modding.BaseMod)))
                        {
                            OutputEngine.WriteLine("Found mod: " + types[j].ToString());
                            IO.Log.Write("            Mod found: " + types[j].ToString());
                            Modding.ModdingLogics.registeredMods.Add(Activator.CreateInstance(types[j]) as Modding.BaseMod);
                            OutputEngine.WriteLine("Mod registered successfully!");
                            IO.Log.Write("            Mod registered successfully!");
                        }
                    }

                    if (!hasComponents)
                    {
                        OutputEngine.WriteLine("File /" + files[i] + " contains no components");
                        IO.Log.Write("        No components found");
                    }
                    else
                    {
                        Utilities.Reflection.RegisterAssembly(a);
                        OutputEngine.WriteLine("Successfully loaded /" + files[i]);
                        IO.Log.Write("        Succressfully loaded from " + files[i]);
                    }
                }
                catch (Exception e)
                {
                    OutputEngine.WriteLine("Error while loading /" + files[i]);
                    IO.Log.Write("        Error while loading " + files[i]);
                    IO.Log.Write(IO.Log.State.ERROR, "       \r\n" + e.Message);
                    IO.Log.Write(IO.Log.State.ERROR, "       \r\n" + e.StackTrace);
                    Console.WriteLine(e.Message);
                }
                OutputEngine.WriteLine("");
            }
        }

        public static void Clear()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].Graphics.Visible)
                    Components[i].IsRemovable = true;
            }
            //foreach (var c in Components)
            int oldc = 0;
            while(Components.Count > 0)
            {
                for (int i = 0; i < Components.Count; i++)
                {
                    if (Components[i] is Properties.ICore)
                    {
                        (Components[i] as Properties.ICore).onFinishedRecieving -= new Properties.OnRecievedEventHandler(ComponentsManager_onFinishedRecieving);
                    }

                    oldc = Components.Count;
                    try
                    {
                        Components[i].Remove();
                    }
                    catch (Exception e)
                    {
                        Components.RemoveAt(i);
                    }

                    if (oldc != Components.Count)
                        i--;
                }
            }
            Layers.Clear();
            Components.Clear();
            LightEmittingComponents.Clear();
            MagnetComponents.Clear();
            mapVisibility.Clear();
        }

        public static void AddLayer(int l)
        {
            for (int i = 0; i < Layers.Count; i++)
            {
                if (Layers[i] == l) return;
            }
            //sort
            if (Layers.Count == 0)
            {
                Layers.Add(l);
                return;
            }
            if (Layers.Count >= 1 && Layers[0] > l)
            {
                Layers.Insert(0, l);
                return;
            }
            if (Layers.Count == 1 && Layers[0] < l)
            {
                Layers.Insert(1, l);
                return;
            }
            for (int i = 1; i < Layers.Count; i++)
            {
                if (Layers[i] > l)
                {
                    Layers.Insert(i, l);
                    return;
                }
            }
            Layers.Add(l);
        }

        public static void Reset()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Reset();
            }
            collidersManager.Reset();
        }

        public static void Start()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Start();
                //if (Components[i].Logics is Logics.Microcontrollers.MicrocontrollersLogics)
                //{
                //    (Components[i].Logics as Logics.Microcontrollers.MicrocontrollersLogics).Start();
                //}
            }
        }

        /// <summary>
        /// Returns max brightness at a specific point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double GetBrightness(float x, float y)
        {
            double d = 0;
            for (int i = 0; i < LightEmittingComponents.Count; i++)
            {
                d += LightEmittingComponents[i].GetBrightness(x, y);
            }
            d = d > 1 ? 1 : d < 0 ? 0 : d;
            return d;
        }

        /// <summary>
        /// Returns max magnetic field at a specific point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Vector2 GetMagneticField(float x, float y)
        {
            Vector2 d = new Vector2();
            for (int i = 0; i < MagnetComponents.Count; i++)
            {
                d += MagnetComponents[i].GetFieldForce(x, y);
            }
            return d;
        }

        /// <summary>
        /// Returns max magnetic field at a specific point
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="blacklist">Component to ignore</param>
        /// <returns></returns>
        public static Vector2 GetMagneticField(float x, float y, Properties.IMagnetic blacklist)
        {
            Vector2 d = new Vector2();
            for (int i = 0; i < MagnetComponents.Count; i++)
            {
                if (MagnetComponents[i] != blacklist)
                    d += MagnetComponents[i].GetFieldForce(x, y);
            }
            return d;
        }

        /// <summary>
        /// Used in wires to ignore wire's joints in A* algorythm
        /// </summary>
        internal static Component IgnoreAS1 = null, IgnoreAS2 = null;
        /// <summary>
        /// Returns point's cost for A* algorythm
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal unsafe static int GetLocationCost(int x, int y)
        {
            return mapVisibility.GetAStarValue(x, y);
        }

        //======================================================EVENTS=================================================================

        public static void OnGraphicsDeviceReset()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].OnGraphicsDeviceReset();
            }
        }

        public static void OnMouseClick(InputEngine.MouseArgs e)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].isIn(e.curState.X, e.curState.Y))
                {
                    Components[i].OnMouseClick(e);
                }
            }
        }

        public static void OnMouseDown(InputEngine.MouseArgs e)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].isIn(e.curState.X, e.curState.Y))
                {
                    Components[i].OnMouseDown(e);
                }
            }
        }

        public static void OnMouseUp(InputEngine.MouseArgs e)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                //if (Components[i].isIn(e.curState.X, e.curState.Y))
                {
                    Components[i].OnMouseUp(e);
                }
            }
        }

        public static void OnMouseMove(InputEngine.MouseMoveArgs e)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                //if (Components[i].isIn(e.curState.X, e.curState.Y))
                {
                    Components[i].OnMouseMove(e);
                    if (MicroWorld.Graphics.GUI.GUIEngine.s_statusStrip.TextLeft == "" && 
                        Components[i].Graphics.Visible && Components[i].isIn(e.curState.X, e.curState.Y))
                        Shortcuts.SetInGameStatus(Components[i].Graphics.GetCSToolTip(), "<Right click> - remove, <Middle click> - properties");
                }
            }
        }

        public static void OnMouseWheel(InputEngine.MouseWheelMoveArgs e)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i].isIn(e.curState.X, e.curState.Y))
                {
                    Components[i].OnMouseWheel(e);
                }
            }
        }







        public static void PreSave()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].PreSave();
            }
        }

        public static void PostSave()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].PostSave();
            }
        }

        public static void SaveAll()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                IO.ComponentData d = new IO.ComponentData();
                Components[i].SaveAll(d);
                IO.SaveEngine.SaveNode(d);
            }
        }

        public static void PostLoad()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].PostLoad();
            }
        }

        public static void PostPostLoad()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].PostPostLoad();
            }
        }

    }
}
