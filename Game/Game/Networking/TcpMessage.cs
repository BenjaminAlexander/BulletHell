using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Net.Sockets;
using MyGame.Engine.Networking;

namespace MyGame.Networking
{
    public abstract class TcpMessage : GameMessage
    {
        public TcpMessage(GameTime gameTime)
            : base(gameTime)
        {
        }

        public TcpMessage(UdpTcpPair pair) : base(pair.ReadTCP())
        {
        }

        public override void Send(UdpTcpPair pair)
        {
            pair.SendTCP(this.MessageBuffer, this.Size);
        }
    }
}
