using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MicroWorld.IO
{
    unsafe class SaveWriter : StreamWriter
    {
        public SaveWriter(String fn)
            : base(fn)
        {
        }

        public override void WriteLine(string value)
        {
            String r = "";
            for (int i = 0; i < value.Length; i++)
            {
                r += (char)(value[i] ^ 639);
            }
            base.WriteLine(r);
        }

        public override void WriteLine(int value)
        {
            base.WriteLine(value ^ 639);
        }

        public override void Write(int value)
        {
            base.Write(value ^ 639);
        }

        public override void Write(string value)
        {
            String r = "";
            for (int i = 0; i < value.Length; i++)
                r += (char)(value[i] ^ 639);
            base.Write(r);
        }

    }
}
