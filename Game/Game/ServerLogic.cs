using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;
using MyGame.IO;
using MyGame.PlayerControllers;
namespace MyGame
{
    class ServerLogic : GameStateObjects.IUpdateable
    {
        private Random random = new Random(5);
        private Vector2 worldSize;
        public ServerLogic(Vector2 worldSize, InputManager inputManager)
        {
            SimpleShip ship = new SimpleShip(new Vector2(0), new Vector2(0, 0), inputManager, StaticNetworkPlayerManager.GetController(1));
            SimpleShip ship2 = new SimpleShip(new Vector2(0), new Vector2(0, 0), inputManager, StaticNetworkPlayerManager.GetController(2));
            StaticGameObjectCollection.Collection.Add(ship);
            StaticGameObjectCollection.Collection.Add(ship2);
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
        }

        private Vector2 RandomPosition()
        {
            return new Vector2((float)(random.NextDouble() * worldSize.X), (float)(random.NextDouble() * worldSize.Y));
        }
    }
}
