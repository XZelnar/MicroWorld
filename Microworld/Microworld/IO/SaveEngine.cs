using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.IO
{
    public static class SaveEngine
    {
        public static readonly String SavesVersion = "MicroWorld Save v1.1 CBT";
        public static bool IsVersionCompatable(String version)
        {
            return version == "MicroWorld Save v1.1 CBT";
        }

        public enum SaveType
        {
            Default = 0,
            Levels = 1,
            LevelsSave = 2,
            LevelDesigner = 3
        }

        public static List<ComponentData> datas = new List<ComponentData>();

        internal static bool scriptLocFromFile = false;
        internal static String LastLoadedFile = "";

        private static bool isLoading = false;
        public static bool IsLoading
        {
            get { return isLoading; }
        }

        #region Saves
        public static void SaveAll(String filename)
        {
            SaveAll(filename, SaveType.Default);
        }

        public static void SaveAll(String filename, SaveType type)
        {
            new System.IO.FileInfo(filename);
            switch (type)
            {
                case SaveType.Default:
                    saveAllDefault(filename);
                    break;
                case SaveType.LevelsSave:
                case SaveType.Levels:
                    saveAllLevels(filename);
                    break;
                case SaveType.LevelDesigner:
                    saveAllLevels(filename);
                    break;
                default:
                    break;
            }
        }

        #region Saves
        private static void saveAllDefault(String filename)
        {
            try
            {
                datas.Clear();
                Components.ComponentsManager.PreSave();
                Components.ComponentsManager.SaveAll();
                Components.ComponentsManager.PostSave();

                SaveWriter sw = new SaveWriter(filename);
                sw.WriteLine(SavesVersion);
                SaveCameraInfo(ref sw);
                Logics.PlacableAreasManager.Save(ref sw);
                for (int i = 0; i < datas.Count; i++)
                {
                    datas[i].Compile();
                    datas[i].WriteToFile(sw);
                }
                sw.Close();
                sw.Dispose();
            }
            catch (Exception e)
            {
                Graphics.GUI.Scene.OKMessageBox.Show("Error occured during saving.\r\nGame has not been saved.");
                IO.Log.Write(e, 0, false, false);
                try
                {
                    if (System.IO.File.Exists(filename))
                        System.IO.File.Delete(filename);
                }
                catch { }
            }
            GlobalEvents.OnLevelSaved();
        }

        private static void saveAllLevels(String filename)
        {
            try
            {
                datas.Clear();
                Components.ComponentsManager.PreSave();
                Components.ComponentsManager.SaveAll();
                Components.ComponentsManager.PostSave();

                SaveWriter sw = new SaveWriter(filename);
                sw.WriteLine(SavesVersion);
                SaveCameraInfo(ref sw);
                sw.WriteLine("l");
                Graphics.GUI.GUIEngine.s_levelSelection.WriteSaveInfo(ref sw);
                Logics.LevelEngine.Save(ref sw);
                Logics.PlacableAreasManager.Save(ref sw);
                for (int i = 0; i < datas.Count; i++)
                {
                    datas[i].Compile();
                    datas[i].WriteToFile(sw);
                }
                sw.Close();

                var sww = new SaveWriter(filename.Substring(0, filename.Length - 3) + "lua");
                sww.Write(((char)8).ToString() + MicroWorld.Graphics.GUI.GUIEngine.s_scriptEditor.Text);
                sww.Close();

                sww.Dispose();
                sw.Dispose();
            }
            catch (Exception e)
            {
                Graphics.GUI.Scene.OKMessageBox.Show("Error occured during saving.\r\nGame has not been saved.");
                IO.Log.Write(e, 0, false, false);
            }
            GlobalEvents.OnLevelSaved();
        }
        #endregion

        private static void SaveCameraInfo(ref SaveWriter sw)
        {
            sw.WriteLine("2");//scale

            sw.WriteLine("0");//centerX
            sw.WriteLine("0");//centerY

            sw.WriteLine("0");//boundsX
            sw.WriteLine("0");//boundsY
            sw.WriteLine("-1");//boundsW
            sw.WriteLine("-1");//boundsH
        }

        public static void SaveNode(ComponentData d)
        {
            datas.Add(d);
        }

        //private static void SavePlacableAreas(ref SaveWriter sw)
        //{
        //    Logics.PlacableAreasManager.SavePlacableAreas(ref sw);
        //    //String r = "";
        //    //foreach (var rec in Logics.GameInputHandler.PlacableAreas)
        //    //{
        //    //    r = r + rec.X.ToString() + ";" + rec.Y.ToString() + ";" + rec.Width.ToString() + ";" + rec.Height.ToString() + "\r\n";
        //    //}
        //    //sw.WriteLine(r.Length.ToString());
        //    //sw.WriteLine(r);
        //}
        #endregion



        #region Loads
        public static void LoadAll(String filename)
        {
            LastLoadedFile = filename;

            LoadAll(filename, SaveType.Default);
        }

        public static void LoadAll(String filename, SaveType type)
        {
            LastLoadedFile = filename;

            Components.ComponentsManager.VisibilityMap.ignorePlacableRestriction = true;
            switch (type)
            {
                case SaveType.Default:
                    loadAllDefault(filename);
                    break;
                case SaveType.LevelsSave:
                    scriptLocFromFile = true;
                    loadAllLevels(filename);
                    break;
                case SaveType.Levels:
                    scriptLocFromFile = false;
                    loadAllLevels(filename);
                    break;
                case SaveType.LevelDesigner:
                    loadAllLevelDesigner(filename);
                    break;
                default:
                    break;
            }
            Components.ComponentsManager.VisibilityMap.ignorePlacableRestriction = false;
        }

        #region Loads
        public static void loadAllDefault(String filename)
        {
            LastLoadedFile = filename;

            if (!System.IO.File.Exists(filename)) return;
            isLoading = true;

            Graphics.GUI.GUIEngine.s_loading.LoadingMainText = "Reading components from file...";

            SaveReader sr = new SaveReader(filename);

            //version check
            String tvc = sr.ReadLine();
            if (!IsVersionCompatable(tvc))
            {
                Graphics.GUI.Scene.OKMessageBox.Show("Incompatible saves file!");
                isLoading = false;
                return;
            }

            ReadCameraInfo(sr);

            LoadPlacableAreas(ref sr);
            Components.ComponentsManager.Clear();
            int c = 0;
            while (sr.Peek() > -1)
            {
                System.Threading.Thread.Sleep(1);
                c++;
                Graphics.GUI.GUIEngine.s_loading.LoadingDescriptiveText = c.ToString() + " Component(s) found...";
                ReadComponent(sr);
            }
            Graphics.GUI.GUIEngine.s_loading.LoadingDescriptiveText = "Post-Loading components...";
            Components.ComponentsManager.PostLoad();
            Components.ComponentsManager.InitAllComponents();
            Components.ComponentsManager.PostPostLoad();
            isLoading = false;
            GlobalEvents.OnLevelLoaded();
            Logics.GameLogicsHelper.GameStart();
        }

        public static void loadAllLevels(String filename)
        {
            LastLoadedFile = filename;

            if (!System.IO.File.Exists(filename)) return;
            isLoading = true;

            Graphics.GUI.GUIEngine.s_loading.LoadingMainText = "Reading components from file...";

            SaveReader sr = new SaveReader(filename);

            //version check
            String tvc = sr.ReadLine();
            if (!IsVersionCompatable(tvc))
            {
                Graphics.GUI.Scene.OKMessageBox.Show("Incompatible saves file!");
                isLoading = false;
                return;
            }

            ReadCameraInfo(sr);

            if (sr.ReadLine() != "l")
            {
                sr.Close();
                Main.Close();
                isLoading = false;
                return;
            }
            if (scriptLocFromFile)
            {
                if (!Graphics.GUI.GUIEngine.s_levelSelection.ReadSaveInfo(ref sr, true))
                {
                    isLoading = false;
                    return;
                }
            }
            else
            {
                if (!Graphics.GUI.GUIEngine.s_levelSelection.BlankReadSaveInfo(ref sr, true))
                {
                    isLoading = false;
                    return;
                }
            }
            Logics.LevelEngine.Load(ref sr);
            LoadPlacableAreas(ref sr);
            Components.ComponentsManager.Clear();
            int c = 0;
            while (sr.Peek() > -1)
            {
                System.Threading.Thread.Sleep(1);
                c++;
                Graphics.GUI.GUIEngine.s_loading.LoadingDescriptiveText = c.ToString() + " Component(s) found...";
                ReadComponent(sr);
            }
            Graphics.GUI.GUIEngine.s_loading.LoadingDescriptiveText = "Post-Loading components...";
            Components.ComponentsManager.PostLoad();
            Components.ComponentsManager.InitAllComponents();
            Components.ComponentsManager.PostPostLoad();
            isLoading = false;
            GlobalEvents.OnLevelLoaded();
            Logics.GameLogicsHelper.GameStart();
        }

        public static void loadAllLevelDesigner(String filename)
        {
            LastLoadedFile = filename;

            if (!System.IO.File.Exists(filename)) return;
            isLoading = true;

            Graphics.GUI.GUIEngine.s_loading.LoadingMainText = "Reading components from file...";

            SaveReader sr = new SaveReader(filename);

            //version check
            String tvc = sr.ReadLine();
            if (!IsVersionCompatable(tvc))
            {
                Graphics.GUI.Scene.OKMessageBox.Show("Incompatible saves file!");
                isLoading = false;
                return;
            }

            ReadCameraInfo(sr);
            Shortcuts.camera.AllowedVisibleRectangle = null;

            if (sr.ReadLine() != "l")
            {
                sr.Close();
                Main.Close();
                isLoading = false;
                return;
            }
            if (!Graphics.GUI.GUIEngine.s_levelSelection.ReadSaveInfo(ref sr, false))
            {
                isLoading = false;
                return;
            }
            else
            {
                if (System.IO.File.Exists(filename.Substring(0, filename.Length - 4) + ".lua"))
                {
                    SaveReader tssr = new SaveReader(filename.Substring(0, filename.Length - 4) + ".lua");
                    Graphics.GUI.GUIEngine.s_scriptEditor.Text = tssr.ReadToEnd();
                    tssr.Close();
                }
                Logics.LevelEngine.Stop();
            }
            Logics.LevelEngine.Load(ref sr);
            LoadPlacableAreas(ref sr);
            Components.ComponentsManager.Clear();
            int c = 0;
            while (sr.Peek() > -1)
            {
                System.Threading.Thread.Sleep(1);
                c++;
                Graphics.GUI.GUIEngine.s_loading.LoadingDescriptiveText = c.ToString() + " Component(s) found...";
                ReadComponent(sr);
            }
            Graphics.GUI.GUIEngine.s_loading.LoadingDescriptiveText = "Post-Loading components...";
            Components.ComponentsManager.PostLoad();
            Components.ComponentsManager.InitAllComponents();
            Components.ComponentsManager.PostPostLoad();
            isLoading = false;
            GlobalEvents.OnLevelLoaded();
            Logics.GameLogicsHelper.GameStart();
        }
        #endregion

        private static void ReadCameraInfo(SaveReader sr)
        {
            Shortcuts.camera.Scale = (float)Convert.ToDouble(sr.ReadLine());

            Shortcuts.camera.Center = new Microsoft.Xna.Framework.Vector2((float)Convert.ToDouble(sr.ReadLine()), (float)Convert.ToDouble(sr.ReadLine()));

            int x = Convert.ToInt32(sr.ReadLine()),
                y = Convert.ToInt32(sr.ReadLine()),
                w = Convert.ToInt32(sr.ReadLine()),
                h = Convert.ToInt32(sr.ReadLine());
            if (w < 0 || h < 0)
                Shortcuts.camera.AllowedVisibleRectangle = null;
            else
                Shortcuts.camera.AllowedVisibleRectangle = new Microsoft.Xna.Framework.Rectangle(x, y, w, h);
        }

        private static void ReadComponent(SaveReader sr)
        {
            String s = sr.ReadLine();
            if (s == "") return;
            int l = Convert.ToInt32(s.Substring(0, s.Length));
            if (l == 0) return;
            s = ReadBlock(sr, l + 2);
            //important
            //important
            if (s == "") return;
            ComponentData c = new ComponentData(s);
            s = sr.ReadLine();
            //s = sr.ReadLine();
            c.Load();
            System.Threading.Thread.Sleep(1);//TODO mb remove. for decoration
        }

        internal static String ReadBlock(SaveReader sr, int n)
        {
            char[] buf = new char[n];
            sr.ReadBlock(buf, 0, n);
            String r = "";
            for (int i = 0; i < n; i++)
            {
                r += buf[i];
            }
            return r;
        }

        private static void LoadPlacableAreas(ref SaveReader sr)//TODO move to PlacableAreasManager
        {
            Logics.PlacableAreasManager.Clear();
            String s = sr.ReadLine();
            if (s == "0") return;
            s = ReadBlock(sr, Convert.ToInt32(s));
            var a = s.Split('\n');
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i].Length < 2) continue;
                var b = a[i].Split(';');
                Logics.PlacableAreasManager.Add(new Microsoft.Xna.Framework.Rectangle(
                    Convert.ToInt32(b[0]), Convert.ToInt32(b[1]), Convert.ToInt32(b[2]), Convert.ToInt32(b[3])));
            }
        }
        #endregion

    }
}
