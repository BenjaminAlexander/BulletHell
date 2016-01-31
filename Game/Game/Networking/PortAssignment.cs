using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Net;
using System.Net.Sockets;

namespace MyGame.Networking
{
    class ClientID : GameMessage
    {
        private int id;
        public int ID
        {
            get { return id; }
        }

        public ClientID(int port)
            : base(new GameTime())
        {
            this.id = port;
            this.Append(port);
        }

        public ClientID(NetworkStream networkStream) : base(networkStream)
        {
            this.ResetReader();
            id = this.ReadInt();
            this.AssertMessageEnd();
        }
    }
}
