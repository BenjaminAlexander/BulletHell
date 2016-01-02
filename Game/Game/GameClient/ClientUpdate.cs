using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;
using MyGame.Networking;

namespace MyGame.GameClient
{
    public abstract class ClientUpdate : GameMessage
    {
        //public abstract void Apply(GameObjectCollection collection);
        public abstract void Apply(ClientGame game, GameTime gameTime);
        public ClientUpdate(byte[] b, int lenght)
            : base(b, lenght)
        {
        }

        public ClientUpdate(GameTime currentGameTime)
            : base(currentGameTime)
        {
        }
    }
}