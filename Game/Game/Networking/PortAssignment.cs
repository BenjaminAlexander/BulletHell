using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.Networking
{
    public class ClientID : GameMessage
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

        public ClientID(byte[] b, int lenght)
            : base(b, lenght)
        {
            this.ResetReader();
            id = this.ReadInt();
            this.AssertMessageEnd();
        }
    }
}
