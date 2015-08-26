using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;
using Lua511;

namespace MicroWorld.Logics.LUA
{
    public class LUALevelEngine
    {
        public static List<Object> ExternalCommands = new List<object>();

        private Lua pLuaVM = null;
        private Hashtable pLuaFuncs = null;
        bool terminate = false;

        public bool IsExecuting
        {
            get
            {
                if (pLuaVM != null) return pLuaVM.IsExecuting;
                return false;
            }
        }


        public void Init()
        {
            pLuaVM = new Lua();
            pLuaFuncs = new Hashtable();
            registerLuaFunctions(new LUALevelCommands());
            for (int i = 0; i < ExternalCommands.Count; i++)
            {
                try
                {
                    registerLuaFunctions(ExternalCommands[i]);
                }
                catch { }
            }
            terminate = false;
        }

        public void Dispose()
        {
            if (AsyncThread != null)
            {
                AsyncThread.Abort();
            }
            if (pLuaVM == null) return;
            terminate = true;
            System.Threading.Thread.Sleep(50);
            /*
            try
            {
                //To hell with it. I tried being nice - didn't work. So FUCK THAT!!!
                //GC will probably handle this shit.
                //pLuaVM.Close();
            }
            catch { }//*/
            System.Threading.Thread.Sleep(10);
            //pLuaVM.Dispose();
            pLuaFuncs.Clear();
        }

        System.Threading.Thread AsyncThread;
        public void LoadFileAsync(String fn)
        {
            if (AsyncThread != null)
            {
                AsyncThread.Abort();
            }
            AsyncThread = new System.Threading.Thread(delegate()
            {
                pLuaVM.DoFile(fn);
            });
            AsyncThread.Start();
        }

        public void DoFileAsync(String fn)
        {
            if (AsyncThread != null)
            {
                AsyncThread.Abort();
            }
            AsyncThread = new System.Threading.Thread(delegate()
            {
                String s = "";
                System.IO.StreamReader tsr = new System.IO.StreamReader(fn);
                if (tsr.Peek() == 8)
                {
                    tsr.Read();
                    tsr.Close();
                    IO.SaveReader sr = new IO.SaveReader(fn);
                    s = sr.ReadToEnd();
                    sr.Close();
                }
                else
                {
                    //tsr.Read();
                    s = tsr.ReadToEnd();
                    tsr.Close();
                }
                //pLuaVM.DoFile(fn);
                try
                {
                    pLuaVM.DoString(s);
                }
                catch (Exception e)
                {
                    IO.Log.Write("Exception in LUA engine at DoFileAsync");
                    IO.Log.Write(e.Message);
                    IO.Log.Write(e.Source);
                    OutputEngine.WriteLine("LUA error: " + e.Message);
                }
            });
            AsyncThread.Start();
        }

        public void DoString(String s)
        {
            if (pLuaVM != null)
                try
                {
                    pLuaVM.DoString(s);
                }
                catch { }
        }

        public void CallFunction(String name, params object[] args)
        {
            if (pLuaVM != null)
            {
                LuaFunction a = null;
                try
                {
                    a = pLuaVM.GetFunction(name);
                }
                catch { }
                if (a != null)
                {
                    try
                    {
                        a.Call(args);
                    }
                    catch { }
                }
            }
        }

        public void registerLuaFunctions(Object pTarget)
        {
            // Sanity checks
            if (pLuaVM == null || pLuaFuncs == null)
                return;

            // Get the target type
            Type pTrgType = pTarget.GetType();

            // ... and simply iterate through all it's methods
            foreach (MethodInfo mInfo in pTrgType.GetMethods())
            {
                // ... then through all this method's attributes
                foreach (Attribute attr in Attribute.GetCustomAttributes(mInfo))
                {
                    // and if they happen to be one of our AttrLuaFunc attributes
                    if (attr.GetType() == typeof(AttrLuaFunc))
                    {
                        AttrLuaFunc pAttr = (AttrLuaFunc)attr;
                        Hashtable pParams = new Hashtable();

                        // Get the desired function name and doc string, along with parameter info
                        String strFName = pAttr.getFuncName();
                        String strFDoc = pAttr.getFuncDoc();
                        String[] pPrmDocs = pAttr.getFuncParams();

                        // Now get the expected parameters from the MethodInfo object
                        ParameterInfo[] pPrmInfo = mInfo.GetParameters();

                        // If they don't match, someone forgot to add some documentation to the
                        // attribute, complain and go to the next method
                        if (pPrmDocs != null && (pPrmInfo.Length != pPrmDocs.Length))
                        {
                            Console.WriteLine("Function " + mInfo.Name + " (exported as " +
                                              strFName + ") argument number mismatch. Declared " +
                                              pPrmDocs.Length + " but requires " +
                                              pPrmInfo.Length + ".");
                            break;
                        }

                        // Build a parameter <-> parameter doc hashtable
                        for (int i = 0; i < pPrmInfo.Length; i++)
                        {
                            pParams.Add(pPrmInfo[i].Name, pPrmDocs[i]);
                        }

                        // Get a new function descriptor from this information
                        LuaFuncDescriptor pDesc = new LuaFuncDescriptor(strFName, strFDoc, pParams, pParams);

                        // Add it to the global hashtable
                        pLuaFuncs.Add(strFName, pDesc);

                        // And tell the VM to register it.
                        pLuaVM.RegisterFunction(strFName, pTarget, mInfo);
                    }
                }
            }
        }

        //=============================================================API=====================================================

        public double GetNumber(String name)
        {
            try
            {
                object o = pLuaVM[name];
                if (o is double || o is int || o is short || o is float || o is byte)
                    return (double)o;
                else
                    return Double.NaN;
            }
            catch { return Double.NaN; };
        }

        public string GetString(String name)
        {
            try
            {
                object o = pLuaVM[name];
                if (o == null) return null;
                if (o is string)
                    return (string)o;
                else
                    return o.ToString();
            }
            catch { return null; };
        }

        public void SetVar(String name, object value)
        {
            while (true)
            {
                try
                {
                    pLuaVM[name] = value;
                    break;
                }
                catch { }
            }
        }
    }
}
