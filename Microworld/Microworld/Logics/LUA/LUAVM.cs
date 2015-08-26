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
    public class LUAVM
    {
        private Lua pLuaVM = null;
        private Hashtable pLuaFuncs = null;
        public int CommandsPerUpdate = 1;
        public String CurrentCode = "";
        private System.Threading.Thread AsyncThread;

        private bool IsSyncThreadRunning = false;

        private bool mayAsyncContinue = false;
        private Type baseType;
        private object baseClass;
        private MethodInfo baseCallback;

        private Refractors.Refractor refractor = new Refractors.Refractor();

        private object[] returns = new object[0];
        private bool isProcessing = false;


        public LUAVM()
        {
            VMWatcher.VMs.Add(this);
        }

        public void SetRefractor(Refractors.Refractor r)
        {
            if (r != null) refractor = r;
        }

        public Refractors.Refractor GetRefractor()
        {
            return refractor;
        }

        public void Init(object cbase)
        {
            if (pLuaVM != null)
            {
                pLuaVM.DebugHook -= new EventHandler<DebugHookEventArgs>(pLuaVM_DebugHook);
                pLuaVM.RemoveDebugHook();
                try
                {
                    pLuaVM.Close();
                }
                catch { }
                System.Threading.Thread.Sleep(1);

                if (AsyncThread != null) Terminate();
            }

            baseClass = cbase;
            baseType = cbase.GetType();
            baseCallback = baseType.GetMethod("Callback");
            pLuaVM = new Lua();
            pLuaFuncs = new Hashtable();
            registerLuaFunctions(new LUAVMCommands());
            pLuaVM.SetDebugHook(EventMasks.LUA_MASKCOUNT, CommandsPerUpdate);
            pLuaVM.DebugHook += new EventHandler<DebugHookEventArgs>(pLuaVM_DebugHook);
        }

        public void Update()
        {
            mayAsyncContinue = true;
        }

        public String doString(String s)
        {
            IsSyncThreadRunning = true;
            try
            {
                RefractorString(ref s);
                CurrentCode = s;
                object[] o = pLuaVM.DoString(s);

                String r = "";
                if (o != null)
                    for (int i = 0; i < o.Length; i++)
                    {
                        r += o[i].ToString();
                    }
                IsSyncThreadRunning = false;
                return r;
            }
            catch (AccessViolationException e)
            {
                IsSyncThreadRunning = false;
                return "";
            }
            catch (Exception e)
            {
                IsSyncThreadRunning = false;
                return e.Message;
            }
        }

        public void WaitForSyncIDLE()
        {
            while (IsSyncThreadRunning) System.Threading.Thread.Sleep(1);
        }

        public void doStringAsync(String s)
        {
            try
            {
                initDoStr();
                RefractorString(ref s);
                CurrentCode = s;

                if (AsyncThread != null)
                {
                    if (AsyncThread.ThreadState == System.Threading.ThreadState.Running)
                    {
                        AsyncThread.Abort();
                    }
                }
                AsyncThread = new System.Threading.Thread(new System.Threading.ThreadStart(_doStringAsync));
                AsyncThread.Start();
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        private unsafe void initDoStr()
        {
            Terminate();
        }

        private void _doStringAsync()
        {
            isProcessing = true;
            try
            {
                returns = pLuaVM.DoString(CurrentCode);
            }
            catch (Exception e)
            {
                return;
            }

            String r = "";
            if (returns != null)
                for (int i = 0; i < returns.Length; i++)
                {
                    r += returns[i].ToString();
                }
            return;
        }

        private void pLuaVM_DebugHook(object sender, DebugHookEventArgs e)
        {
            //callback
            if (baseCallback != null)
            {
                baseCallback.Invoke(baseClass, new object[0]);
            }

            if (AsyncThread != null)
            {
                mayAsyncContinue = false;
                isProcessing = false;
                while (!mayAsyncContinue && AsyncThread != null) System.Threading.Thread.Sleep(1);
                isProcessing = true;
            }
        }

        public void WaitForProcessingToFinish()
        {
            while (isProcessing) System.Threading.Thread.Sleep(1);
        }

        public void RefractorString(ref String s)
        {
            refractor.RefractorString(ref s);
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

        public void Terminate()
        {
            if (AsyncThread != null)
            {
                AsyncThread.Abort();
                AsyncThread = null;
                isProcessing = false;
                mayAsyncContinue = true;
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
