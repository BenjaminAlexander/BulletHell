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
            IController[] controllers = new IController[] { new AIController(), new AIController(), new AIController(), new AIController() };

            //foreach (NetworkPlayerController controller in StaticNetworkPlayerManager.NetworkPlayerControllerList())
            int i = 0;
            for (; i < StaticNetworkPlayerManager.NetworkPlayerControllerList().Count; i++)
            {
                controllers[i % 4] = StaticNetworkPlayerManager.NetworkPlayerControllerList()[i];

                if (i % 4 == 3)
                {
                    Ship ship = new Ship(new Vector2(0), new Vector2(0, 0), inputManager, controllers[0], controllers[1], controllers[2], controllers[3]);
                    StaticGameObjectCollection.Collection.Add(ship);
                    controllers = new IController[] { new AIController(), new AIController(), new AIController(), new AIController() };
                }
            }

            if (i % 4 != 0)
            {
                Ship ship2 = new Ship(new Vector2(0), new Vector2(0, 0), inputManager, controllers[0], controllers[1], controllers[2], controllers[3]);
                StaticGameObjectCollection.Collection.Add(ship2);
            }

            Ship ship3 = new Ship(new Vector2(1000), new Vector2(0, 0), inputManager, new AIController(), new AIController(), new AIController(), new AIController());
            StaticGameObjectCollection.Collection.Add(ship3);
        }

        public void Update(float secondsElapsed)
        {
            StaticControllerFocus.SendUpdateMessages();
        }

        private Vector2 RandomPosition()
        {
            return new Vector2((float)(random.NextDouble() * worldSize.X), (float)(random.NextDouble() * worldSize.Y));
        }
    }
}
