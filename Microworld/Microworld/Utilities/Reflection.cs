using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MicroWorld.Utilities
{
    public static class Reflection
    {
        public enum Types
        {
            Fields = 1,
            Methods = 2
        }

        public class ReflectionReturn : IComparable
        {
            public Types type = Types.Fields;
            public String name = "";
            public String value = "";
            public String arguments = "";
            public String returnType = "";

            public ReflectionReturn() { }

            public ReflectionReturn(String n, String v, Types t)
            {
                type = t;
                name = n;
                value = v;
            }

            public ReflectionReturn(String n, String v, Types t, String par, String ret)
            {
                type = t;
                name = n;
                value = v;
                arguments = par;
                returnType = ret;
            }

            public int CompareTo(object o)
            {
                if (o is ReflectionReturn)
                    return name.CompareTo((o as ReflectionReturn).name);
                return 0;
            }
        }

        public class ReflectionObject
        {
            public String nmSpace = "";
            public String name = "";
            public Types type = Types.Fields;
            public Type[] parameters = null;
        }

        static List<Assembly> assemblies = new List<Assembly>();

        public static void RegisterAssembly(Assembly a)
        {
            for (int i = 0; i < assemblies.Count; i++)
            {
                if (assemblies[i] == a)
                    return;
            }
            assemblies.Add(a);
        }

        public static List<ReflectionReturn> GetAllNames(Object parent, String mask, Types t)
        {
            mask = mask.ToLower();
            List<ReflectionReturn> r = new List<ReflectionReturn>();
            if ((t.GetHashCode() & Types.Fields.GetHashCode()) != 0)
            {
                r.InsertRange(0, GetAllFieldsNames(parent, mask));
            }
            if ((t.GetHashCode() & Types.Methods.GetHashCode()) != 0)
            {
                if (r.Count == 0)
                    r.InsertRange(0, GetAllMethodsNames(parent, mask));
                else
                    r.InsertRange(r.Count, GetAllMethodsNames(parent, mask));
            }
            return r;
        }

        public static List<ReflectionReturn> GetAllMethodsNames(Object parent, String mask)
        {
            String s = "";
            List<ReflectionReturn> t = new List<ReflectionReturn>();
            if (parent == null)
            {
                foreach (var ab in assemblies)
                {
                    var a = ab.GetTypes();
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (!a[i].Attributes.HasFlag(System.Reflection.TypeAttributes.Public)) continue;
                        var b = a[i].GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                        for (int j = 0; j < b.Length; j++)
                        {
                            s = a[i].FullName + "." + b[j].Name;
                            if (s.ToLower().StartsWith(mask) || s.StartsWith(mask))// && b[j].GetParameters().Length == 0)
                            {
                                String par = "";
                                var pars = b[j].GetParameters();
                                for (int k = 0; k < pars.Length; k++)
                                {
                                    par = par + ", " + pars[k].ParameterType.ToString();
                                }
                                if (par.Length == 0)
                                    par = " ";
                                else
                                    par = par.Substring(2);
                                t.Add(new ReflectionReturn(s + "()", "", Types.Methods, par, b[j].ReturnType.ToString()));
                            }
                        }
                    }
                }
            }
            else
            {
                var b = parent.GetType().GetMethods();
                for (int j = 0; j < b.Length; j++)
                {
                    if (b[j].Name.ToLower().StartsWith(mask) || b[j].Name.StartsWith(mask))// && b[j].GetParameters().Length == 0)
                    {
                        String par = "";
                        var pars = b[j].GetParameters();
                        for (int k = 0; k < pars.Length; k++)
                        {
                            par = par + ", " + pars[k].ParameterType.ToString();
                        }
                        if (par.Length == 0)
                            par = " ";
                        else
                            par = par.Substring(2);
                        t.Add(new ReflectionReturn(b[j].Name + "()", "", Types.Methods, par, b[j].ReturnType.ToString()));
                    }
                }
            }
            t.Sort();
            return t;
        }

        public static List<ReflectionReturn> GetAllFieldsNames(Object parent, String mask)
        {
            String s = "";
            List<ReflectionReturn> t = new List<ReflectionReturn>();
            Object v;
            if (parent == null)
            {
                foreach (var ab in assemblies)
                {
                    var a = ab.GetTypes();
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (!a[i].Attributes.HasFlag(System.Reflection.TypeAttributes.Public)) continue;
                        var b = a[i].GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                        for (int j = 0; j < b.Length; j++)
                        {
                            s = a[i].FullName + "." + b[j].Name;
                            if (s.ToLower().StartsWith(mask) || s.StartsWith(mask))
                            {
                                v = b[j].GetValue(null);
                                if (v == null) v = "[null]";
                                t.Add(new ReflectionReturn(s, v.ToString(), Types.Fields));
                            }
                        }
                        var c = a[i].GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                        for (int j = 0; j < c.Length; j++)
                        {
                            s = a[i].FullName + "." + c[j].Name;
                            if (s.ToLower().StartsWith(mask) || s.StartsWith(mask))
                            {
                                try
                                {
                                    v = c[j].GetValue(null, null);
                                    if (v == null) v = "[null]";
                                    t.Add(new ReflectionReturn(s, v.ToString(), Types.Fields));
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            else
            {
                var b = parent.GetType().GetFields();
                for (int j = 0; j < b.Length; j++)
                {
                    if (b[j].Name.ToLower().StartsWith(mask) || b[j].Name.StartsWith(mask))
                    {
                        v = b[j].GetValue(parent);
                        if (v == null) v = "[null]";
                        t.Add(new ReflectionReturn(b[j].Name, v.ToString(), Types.Fields));
                    }
                }
                var c = parent.GetType().GetProperties();
                for (int j = 0; j < c.Length; j++)
                {
                    if (c[j].Name.ToLower().StartsWith(mask) || c[j].Name.StartsWith(mask))
                    {
                        try
                        {
                            v = c[j].GetValue(parent, null);
                            if (v == null) v = "[null]";
                            t.Add(new ReflectionReturn(c[j].Name, v.ToString(), Types.Fields));
                        }
                        catch { }
                    }
                }
            }
            t.Sort();
            return t;
        }

        public static object GetObject(Object parent, String name, Type[] parameters = null)
        {
            var o = ParseString(name);
            if (parent != null)
            {
                if (o.type == Types.Methods)//object method
                    return parent.GetType().GetMethod(o.name, parameters);
                else//object field
                {
                    var b = parent.GetType().GetField(o.name);
                    if (b != null) return b;
                    return parent.GetType().GetProperty(o.name);
                }
            }
            else
            {
                if (o.type == Types.Methods)//static method
                {
                    foreach (var ab in assemblies)
                    {
                        var a = ab.GetTypes();
                        for (int i = 0; i < a.Length; i++)
                        {
                            if (!a[i].Attributes.HasFlag(System.Reflection.TypeAttributes.Public)) continue;
                            try
                            {
                                var b = a[i].GetMethod(o.name, parameters);
                                if (b != null) return b;
                            }
                            catch { }
                        }
                    }
                }
                else//static field
                {
                    foreach (var ab in assemblies)
                    {
                        var a = ab.GetTypes();
                        for (int i = 0; i < a.Length; i++)
                        {
                            if (!a[i].Attributes.HasFlag(System.Reflection.TypeAttributes.Public)) continue;
                            try
                            {
                                var b = a[i].GetField(o.name);
                                if (b != null) return b;
                            }
                            catch { }
                            try
                            {
                                var c = a[i].GetProperty(o.name);
                                if (c != null) return c;
                            }
                            catch { }
                        }
                    }
                }
            }
            return null;
        }

        public static object GetObjectValue(Object parent, String name, Type[] parameters = null)
        {
            var o = ParseString(name);
            if (parent != null)
            {
                if (o.type == Types.Methods)//object method
                    return parent.GetType().GetMethod(o.name, parameters);
                else//object field
                {
                    var b = parent.GetType().GetField(o.name);
                    if (b != null) return b;
                    return parent.GetType().GetProperty(o.name);
                }
            }
            else
            {
                if (o.type == Types.Methods)//static method
                {
                    foreach (var ab in assemblies)
                    {
                        var a = ab.GetTypes();
                        for (int i = 0; i < a.Length; i++)
                        {
                            if (!a[i].Attributes.HasFlag(System.Reflection.TypeAttributes.Public)) continue;
                            try
                            {
                                var b = a[i].GetMethod(o.name, parameters);
                                if (b != null) return b;
                            }
                            catch { }
                        }
                    }
                }
                else//static field
                {
                    foreach (var ab in assemblies)
                    {
                        var a = ab.GetTypes();
                        for (int i = 0; i < a.Length; i++)
                        {
                            if (!a[i].Attributes.HasFlag(System.Reflection.TypeAttributes.Public)) continue;
                            try
                            {
                                var b = a[i].GetField(o.name);
                                if (b != null)
                                    return b.GetValue(null);
                            }
                            catch { }
                            try
                            {
                                var c = a[i].GetProperty(o.name);
                                if (c != null)
                                    return c.GetValue(null, null);
                            }
                            catch { }
                        }
                    }
                }
            }
            return null;
        }

        public static ReflectionObject ParseString(String s)
        {
            if (s == null || s == "") return null;
            ReflectionObject o = new ReflectionObject();
            if (s.EndsWith(")")) o.type = Types.Methods;
            else o.type = Types.Fields;
            int i = s.LastIndexOf('.');
            if (i == -1)
            {
                o.nmSpace = "";
                if (o.type == Types.Fields)
                    o.name = s;
                else
                    o.name = s.Substring(0, s.IndexOf('('));
            }
            else
            {
                o.nmSpace = s.Substring(0, i);
                i++;
                if (o.type == Types.Fields)
                {
                    o.name = s.Substring(i, s.Length - i);
                }
                else
                {
                    o.name = s.Substring(i, s.IndexOf('(') - i);
                }
            }
            return o;
        }


        public static MethodInfo GetMethod(Object parent, String name, Type[] parameters)
        {
            var o = ParseString(name);
            if (parent == null)
            {
                foreach (var ab in assemblies)
                {
                    var a = ab.GetTypes();
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (!a[i].Attributes.HasFlag(System.Reflection.TypeAttributes.Public)) continue;
                        if (a[i].Namespace + "." + a[i].Name != o.nmSpace) continue;
                        var b = a[i].GetMethod(o.name, parameters);
                        if (b != null) 
                            return b;
                    }
                }
            }
            else
            {
                var b = parent.GetType().GetMethod(o.name, parameters);
                if (b != null) return b;
            }
            return null;
        }

        public static object GetField(Object parent, String name)
        {
            var a = GetObjectValue(parent, name);
            if (a == null) return null;
            if (a is FieldInfo)
                return (a as FieldInfo).GetValue(parent);
            else if (a is PropertyInfo)
                return (a as PropertyInfo).GetValue(parent, null);
            return null;
        }

        public static String SetField(Object parent, String name, object value)
        {
            var a = GetObject(parent, name);
            if (a == null) return "Couldn't find field \"" + name + "\"";
            try
            {
                if (a is FieldInfo)
                {
                    value = ConvertToType(value, (a as FieldInfo).FieldType);
                    (a as FieldInfo).SetValue(parent, value);
                }
                else if (a is PropertyInfo)
                {
                    value = ConvertToType(value, (a as PropertyInfo).PropertyType);
                    (a as PropertyInfo).SetValue(parent, value, null);
                }
            }
            catch { return "Couldn't set \"" + value.ToString() + "\" to \"" + name + "\""; }
            return "";
        }

        public static object ConvertToType(Object o, Type t)
        {
            try
            {
                if (t == typeof(Int32))
                    return Convert.ToInt32(o);
                if (t == typeof(Int16))
                    return Convert.ToInt16(o);
                if (t == typeof(Byte))
                    return Convert.ToByte(o);
                if (t == typeof(Char))
                    return Convert.ToChar(o);
                if (t == typeof(float))
                    return Convert.ToSingle(o);
                if (t == typeof(String))
                    return Convert.ToString(o);
                if (t == typeof(Boolean))
                    return Convert.ToBoolean(o);
                if (t == typeof(Double))
                    return Convert.ToDouble(o);
            }
            catch { }
            return o;
        }


    }
}
