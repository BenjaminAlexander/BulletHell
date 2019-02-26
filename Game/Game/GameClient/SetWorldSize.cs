using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameClient;
using System.Net.Sockets;
using MyGame.Networking;
using MyGame.Engine.Networking;

namespace MyGame.GameClient
{
    public class SetWorldSize : TcpMessage
    {
        private Vector2 worldSize;
        public Vector2 WorldSize
        {
            get { return worldSize; }
        }

        public SetWorldSize(Vector2 worldSize)
            : base(new GameTime())
        {
            this.worldSize = worldSize;
            this.Append(worldSize);
        }

        public SetWorldSize(UdpTcpPair pair)
            : base(pair)
        {
            this.ResetReader();
            worldSize = this.ReadVector2();
            this.AssertMessageEnd();
        }
    }
}
