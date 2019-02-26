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
    public abstract class UdpMessage : GameMessage
    {
        public UdpMessage(GameTime gameTime)
            : base(gameTime)
        {
        }

        public UdpMessage(UdpTcpPair pair)
            : base(pair.ReadUDP())
        {
        }

        public override void Send(UdpTcpPair pair)
        {
            pair.SendUDP(this.MessageBuffer, this.Size);
        }
    }
}
