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
        private int waveSize = 1;
        private Random random = new Random(5);
        private Vector2 worldSize;
        private Rectangle spawnRect;

        public ServerLogic(Vector2 worldSize, InputManager inputManager)
        {
            this.worldSize = worldSize;
            spawnRect = new Rectangle((int)(worldSize.X - 1000), 0, 1000, (int)(worldSize.Y));
            IController[] controllers = new IController[] { new AIController(), new AIController(), new AIController(), new AIController() };

            //foreach (NetworkPlayerController controller in StaticNetworkPlayerManager.NetworkPlayerControllerList())
            int i = 0;
            for (; i < StaticNetworkPlayerManager.NetworkPlayerControllerList().Count; i++)
            {
                controllers[i % 4] = StaticNetworkPlayerManager.NetworkPlayerControllerList()[i];

                if (i % 4 == 3)
                {
                    BigShip ship = new BigShip(worldSize/2, new Vector2(0, 0), controllers[0], controllers[1], controllers[2], controllers[3]);
                    StaticGameObjectCollection.Collection.Add(ship);
                    controllers = new IController[] { new AIController(), new AIController(), new AIController(), new AIController() };
                }
            }

            if (i % 4 != 0)
            {
                BigShip ship2 = new BigShip(worldSize / 2, new Vector2(0, 0), controllers[0], controllers[0], controllers[0], controllers[0]);
                StaticGameObjectCollection.Collection.Add(ship2);
            }

            //SmallShip ship3 = new SmallShip(new Vector2(100), new Vector2(0, 0), new AIController(), new AIController());
            //StaticGameObjectCollection.Collection.Add(ship3);

            StaticGameObjectCollection.Collection.Add(new Tower(
                Utils.RandomUtils.RandomVector2(new Rectangle(0, 0, 1000, 1000)) + worldSize / 2
                , (float)(random.NextDouble() * Math.PI * 2)));

            StaticGameObjectCollection.Collection.Add(new Tower(
                Utils.RandomUtils.RandomVector2(new Rectangle(1500, 1500, 1000, 1000)) + worldSize / 2
                , (float)(random.NextDouble() * Math.PI * 2)));

            StaticGameObjectCollection.Collection.Add(new Tower(
                Utils.RandomUtils.RandomVector2(new Rectangle(0, 1500, 1000, 1000)) + worldSize / 2
                , (float)(random.NextDouble() * Math.PI * 2)));

            StaticGameObjectCollection.Collection.Add(new Tower(
                Utils.RandomUtils.RandomVector2(new Rectangle(1500, 0, 1000, 1000)) + worldSize / 2
                , (float)(random.NextDouble() * Math.PI * 2)));
        }

        public void Update(float secondsElapsed)
        {
            StaticControllerFocus.SendUpdateMessages();

            if (StaticGameObjectCollection.Collection.GetMasterList().GetList<SmallShip>().Count == 0)
            {
                waveSize = waveSize + 5;
                while (StaticGameObjectCollection.Collection.GetMasterList().GetList<SmallShip>().Count < waveSize)
                {
                    AIController c = new AIController();
                    SmallShip ship3 = new SmallShip(Utils.RandomUtils.RandomVector2(spawnRect), new Vector2(0, 0), c, c);
                    StaticGameObjectCollection.Collection.Add(ship3);
                }
            }

            
        }

        private Vector2 RandomPosition()
        {
            return Utils.RandomUtils.RandomVector2(worldSize);
        }

    }
}
