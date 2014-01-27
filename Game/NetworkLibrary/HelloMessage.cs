using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLibrary
{
    public class HelloMessage : TCPMessage
    {
        public HelloMessage(int i, string s, float f)
        {
            this.Append(i);
            this.Append(s);
            this.Append(f);
        }

        public HelloMessage(byte[] b, int lenght) : base(b, lenght)
        {
            this.ResetReader();
            i = this.ReadInt();
            s =  this.ReadString();
            f = this.ReadFloat();
        }

        public int i;
        public string s;
        public float f;
    }
}
