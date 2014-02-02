using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.Networking
{
    public abstract class GameObjectCollectionUpdate : TCPMessage
    {
        public abstract void Apply(GameObjectCollection collection);

        public GameObjectCollectionUpdate(byte[] b, int lenght)
            : base(b, lenght)
        {
        }

        public GameObjectCollectionUpdate()
        {
        }
    }
}
