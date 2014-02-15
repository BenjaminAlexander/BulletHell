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
            this.worldSize = worldSize;
            foreach (NetworkPlayerController controller in StaticNetworkPlayerManager.NetworkPlayerControllerList())
            {
                Ship ship = new Ship(new Vector2(0), new Vector2(0, 0), inputManager, controller);
                StaticGameObjectCollection.Collection.Add(ship);
            }
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            StaticControllerFocus.SendUpdateMessages();
        }

        private Vector2 RandomPosition()
        {
            return new Vector2((float)(random.NextDouble() * worldSize.X), (float)(random.NextDouble() * worldSize.Y));
        }
    }
}
