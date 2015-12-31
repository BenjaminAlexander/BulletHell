using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.Networking;

namespace MyGame.GameServer
{
    public abstract class ServerUpdate : GameMessage
    {
        //public abstract void Apply(GameObjectCollection collection);
        public abstract void Apply(ServerGame game);
        public ServerUpdate(byte[] b, int lenght)
            : base(b, lenght)
        {
        }

        public ServerUpdate(GameTime currentGameTime)
            : base(currentGameTime)
        {
        }
    }
}
