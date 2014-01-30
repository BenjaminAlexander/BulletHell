using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetworkLibrary
{
    public class LaterMessage : TCPMessage
    {
        public LaterMessage(byte[] b, int lenght) : base(b, lenght)
        {
        }
    }
}
