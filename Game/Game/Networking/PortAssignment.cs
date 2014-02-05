using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.Networking
{
    public class ClientID : TCPMessage
    {
        private int id;
        public int ID
        {
            get { return id; }
        }

        public ClientID(int port)
        {
            this.id = port;
            this.Append(port);
        }

        public ClientID(byte[] b, int lenght)
            : base(b, lenght)
        {
            this.ResetReader();
            id = this.ReadInt();
            this.AssertMessageEnd();
        }
    }
}
