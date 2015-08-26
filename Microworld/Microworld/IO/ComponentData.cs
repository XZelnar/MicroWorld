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
using System.Reflection;

namespace MicroWorld.IO
{
    public class ComponentData
    {
        private String type;
        private String assemblyName;
        public String Data = "";
        private String[] lines = null;//name,length,data,...


        public ComponentData() { }

        public ComponentData(String s)
        {
            var a = s.Split('\n');
            type = a[0].Substring(0, a[0].Length - 1);
            int assemblyDescLength = 0;
            if (Utilities.Tools.IsRunningOnMono())
            {
                String an = a[1].Substring(0, a[1].Length - 1);
                assemblyDescLength = an.Length;
                if (an == "Microworld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3dafb17035af824f")
                {
                    an = "Microworld, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                }
                assemblyName = an;
            }
            else
            {
                assemblyName = a[1].Substring(0, a[1].Length - 1);
                assemblyDescLength = assemblyName.Length;
            }
            Data = s.Substring(4 + type.Length + assemblyDescLength);
            updateLines();
        }


        public void Add(String name, String value)
        {
            Data += name + "\r\n" + value.Length.ToString() + "\r\n" + value + "\r\n";
            updateLines();
        }
        public void Add(String name, int value)
        {
            Add(name, value.ToString());
        }
        public void Add(String name, double value)
        {
            Add(name, value.ToString());
        }
        public void Add(String name, Vector2 value)
        {
            Add(name, value.X.ToString() + ";" + value.Y.ToString());
        }
        public void Add(String name, bool value)
        {
            Add(name, value.ToString());
        }

        public bool Contains(String name)
        {
            for (int i = 0; i < lines.Length; i += 3)
            {
                if (lines[i] == name)
                {
                    return true;
                }
            }
            return false;
        }

        public String GetString(String name)
        {
            for (int i = 0; i < lines.Length; i += 3)
            {
                if (lines[i] == name)
                {
                    //return lines[i + 2].Substring(0, lines[i + 2].Length - 1);
                    return lines[i + 2];
                }
            }
            return null;
        }
        public int GetInt(String name)
        {
            String s = GetString(name);
            if (s != null)
            {
                int t = 0;
                if (Int32.TryParse(s, out t))
                {
                    return t;
                }
            }
            return Int32.MinValue;
        }
        public double GetDouble(String name)
        {
            String s = GetString(name);
            if (s != null)
            {
                double t = 0;
                if (Double.TryParse(s, out t))
                {
                    return t;
                }
            }
            return Double.NaN;
        }
        public Vector2 GetVector2(String name)
        {
            String s = GetString(name);
            if (s != null && s.IndexOf(";") >= 0)
            {
                var a = s.Split(';');
                if (a.Length != 2)
                {
                    return new Vector2();
                }
                double x, y;
                if (Double.TryParse(a[0], out x) && Double.TryParse(a[1], out y))
                {
                    return new Vector2((float)x, (float)y);
                }
            }
            return new Vector2();
        }
        public bool GetBool(String name)
        {
            String s = GetString(name);
            if (s != null)
            {
                bool t = false;
                if (Boolean.TryParse(s, out t))
                {
                    return t;
                }
            }
            return false;
        }


        public void SetType(Type t)
        {
            type = t.ToString();
            assemblyName = t.Assembly.FullName;
        }

        public Components.Component GetComponentFromType()
        {
            if (type != null && assemblyName != null)
            {
                Type t = null;
                if (assemblyName.StartsWith("Microworld, "))
                {
                    t = Type.GetType(type + ", " + assemblyName, true);
                }
                else
                {
                    try
                    {
                        t = Assembly.LoadFrom("Components/" + assemblyName.Substring(0, assemblyName.IndexOf(",")) + ".dll").GetType(type);
                    }
                    catch (Exception e)
                    {
                        Log.Write(Log.State.ERROR, "Error in creating type for: " + type + "\r\n Assembly: " + assemblyName + "\r\n");
                        Log.Write(Log.State.ERROR, "Error message: \r\n" + e.Message);
                    }
                }
                return (Components.Component)((object)Activator.CreateInstance(t));
            }
            return null;
        }

        private void updateLines()
        {
            var a = Data.Split('\n');
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != "" && a[i][a[i].Length - 1] == '\r') a[i] = a[i].Substring(0, a[i].Length - 1);
            }
            int items = 0, c = 0, n = 0;
            //sanity check and items count
            for (int i = 0; i < a.Length - 2; i += 3)
            {
                if (a[i] != "" && a[i] != "\r")
                {
                    items++;
                    i++;
                    n = Convert.ToInt32(a[i]);
                    c = 0;
                    while (c < n+1)
                    {
                        if (++i < a.Length)
                        {
                            c += a[i].Length + 2;
                        }
                        else
                        {
                            throw new InvalidOperationException("String appears to be broken");
                        }
                    }
                    i-=2;
                }
            }
            //actually update lines
            lines = new String[items * 3];
            items = -1;
            for (int i = 0; i < a.Length - 2; i += 3)
            {
                if (a[i] != "" && a[i] != "\r")
                {
                    items++;
                    lines[items * 3] = a[i];
                    i++;
                    lines[items * 3 + 1] = a[i]; 
                    n = Convert.ToInt32(a[i]);
                    c = 0;
                    String s = "";
                    while (c < n + 1)
                    {
                        if (++i < a.Length)
                        {
                            c += a[i].Length + 2;
                            s += a[i] + "\r\n";
                        }
                        else
                        {
                            throw new InvalidOperationException("String appears to be broken");
                        }
                    }
                    s = s.Substring(0, s.Length - 2);
                    //if (s[s.Length - 1] == '\r') s = s.Substring(0, s.Length - 1);
                    i-=2;
                    lines[items * 3 + 2] = s;
                }
            }
        }

        public void Compile()
        {
            if (type == null || assemblyName == null)
            {
                throw new InvalidOperationException("Cannot save component without type");
            }
        }

        public void WriteToFile(System.IO.StreamWriter sw)
        {
            sw.WriteLine(Data.Length + type.Length + assemblyName.Length);
            sw.WriteLine(type + "\r\n" + assemblyName + "\r\n" + Data);
        }

        public void Load()
        {
            var c = GetComponentFromType();
            c.LoadAll(this);
            c.AddComponentToManager();
            //c.Initialize();
        }//create obj and stuff

    }
}
