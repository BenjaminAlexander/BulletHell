using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;
using MyGame.IO;
using MyGame.PlayerControllers;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameStateObjects.PhysicalObjects.CompositePhysicalObjects;
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
                    BigShip ship = new BigShip(new Vector2(0), new Vector2(0, 0), controllers[0], controllers[1], controllers[2], controllers[3]);
                    StaticGameObjectCollection.Collection.Add(ship);
                    controllers = new IController[] { new AIController(), new AIController(), new AIController(), new AIController() };
                }
            }

            if (i % 4 != 0)
            {
                BigShip ship2 = new BigShip(new Vector2(0), new Vector2(0, 0), controllers[0], controllers[1], controllers[2], controllers[3]);
                StaticGameObjectCollection.Collection.Add(ship2);
            }

            //SmallShip ship3 = new SmallShip(new Vector2(100), new Vector2(0, 0), new AIController(), new AIController());
            //StaticGameObjectCollection.Collection.Add(ship3);
        }

        public void Update(float secondsElapsed)
        {
            StaticControllerFocus.SendUpdateMessages();
            if (StaticGameObjectCollection.Collection.GetMasterList().GetList<SmallShip>().Count < 10)
            {
                SmallShip ship3 = new SmallShip(new Vector2((float)(random.NextDouble() * worldSize.X), (float)(random.NextDouble() * worldSize.Y)), new Vector2(0, 0), new AIController(), new AIController());
                StaticGameObjectCollection.Collection.Add(ship3);
            }

            if (StaticGameObjectCollection.Collection.GetMasterList().GetList<Moon>().Count < 15)
            {
                //Moon moon = new Moon(RandomPosition(), 4);
                //StaticGameObjectCollection.Collection.Add(moon);
            }
        }

        private Vector2 RandomPosition()
        {
            return new Vector2((float)(random.NextDouble() * worldSize.X), (float)(random.NextDouble() * worldSize.Y));
        }
    }
}
