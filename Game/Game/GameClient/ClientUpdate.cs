using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.Networking;
using System.Net.Sockets;

namespace MyGame.GameClient
{
    public abstract class ClientUpdate : GameMessage
    {
        public abstract void Apply(ClientGame game, GameTime gameTime);

        public ClientUpdate(GameTime currentGameTime)
            : base(currentGameTime)
        {
        }

        public ClientUpdate(UdpClient udpClient)
            : base(udpClient)
        {
        }

        public ClientUpdate(NetworkStream networkStream)
            : base(networkStream)
        {
        }
    }
}