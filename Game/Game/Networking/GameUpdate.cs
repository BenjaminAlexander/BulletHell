using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.DataStuctures;

namespace MyGame.Networking
{
    public abstract class GameUpdate : GameMessage
    {
        //public abstract void Apply(GameObjectCollection collection);
        public abstract void Apply(Game1 game);
        public GameUpdate(byte[] b, int lenght)
            : base(b, lenght)
        {
        }

        public GameUpdate()
        {
        }
    }
}
