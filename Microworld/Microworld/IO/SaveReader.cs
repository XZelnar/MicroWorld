using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MicroWorld.IO
{
    unsafe class SaveReader : StreamReader
    {
        public SaveReader(String fn) : base(fn)
        {
        }

        public override int Read()
        {
            return base.Read() ^ 639;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            var a = base.Read(buffer, index, count);
            for (int i = index; i < index + count; i++)
            {
                buffer[i] = (char)(buffer[i] ^ 639);
            }
            return a;
        }

        public override string ReadLine()
        {
            char[] s = base.ReadLine().ToCharArray();
            String r = "";
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = (char)(s[i] ^ 639);
                r += s[i];
            }
            return r;
        }

        public override string ReadToEnd()
        {
            char[] s = base.ReadToEnd().ToCharArray();
            String r = "";
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = (char)(s[i] ^ 639);
                r += s[i];
            }
            return r;
        }
    }
}
