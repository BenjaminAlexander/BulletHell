using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLibrary
{
    public class HelloMessage : TCPMessage
    {
        protected override void InitializeFromBuffer(byte[] b, int lenght)
        {
            base.InitializeFromBuffer(b, lenght);
        }
    }
}
