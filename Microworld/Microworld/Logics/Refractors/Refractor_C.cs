using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroWorld.Logics.Refractors
{
    public class Refractor_C : Refractor
    {
        public override void RefractorString(ref string s)
        {
            int t;
            while ((t = s.IndexOf("while (")) >= 0)//look for cycle
            {
                int c = 1;
                for (int i = t + 8; i < s.Length; i++)//look for closing condition
                {
                    if (s[i] == '(') c++;
                    if (s[i] == ')') c--;
                    if (c == 0)
                    {
                        for (int j = i+1; j < s.Length; j++)//look for {
                        {
                            if (s[j] == '{')
                            {
                                s = s.Substring(0, j) + s.Substring(j + 1);
                                bool foundEnd = false;
                                for (int k = j; k < s.Length; k++)//found {. Looking for }
                                {
                                    if (s[k] == '}')
                                    {
                                        s = s.Substring(0, k) + "end" + s.Substring(k + 1);
                                        foundEnd = true;
                                        break;
                                    }
                                }
                                if (!foundEnd)//haven't found }. Refractor sad... ;(
                                {
                                    throw new Exception("Never ending cycle");
                                }
                                break;
                            }
                            if (s[j] != ' ' && s[j] != '\r' && s[j] != '\n')//1-op cycle w/o {}
                            {//looking for a place to end it all
                                bool inserted = false;
                                for (; j < s.Length; j++)
                                {
                                    if (s[j] == '\r')
                                    {
                                        s.Insert(j, " end");
                                        inserted = true;
                                        break;
                                    }
                                }
                                if (!inserted)
                                {
                                    s = s + " end";
                                }
                                break;
                            }
                        }
                        s = s.Substring(0, i) + " do" + s.Substring(i + 1, s.Length - i - 1);
                        s = s.Substring(0, t + 6) + s.Substring(t + 7);
                        //s.Insert(i + 1, "then");
                        break;
                    }
                }
            }
            base.RefractorString(ref s);
        }
    }
}
