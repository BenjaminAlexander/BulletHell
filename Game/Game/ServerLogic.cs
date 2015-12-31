using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;
using MyGame.Networking;
using MyGame.IO;
using MyGame.PlayerControllers;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameStateObjects.PhysicalObjects.CompositePhysicalObjects;
using MyGame.GameServer;
namespace MyGame
{
    public class ServerLogic
    {
        public ServerLogic(ServerGame game)
        {
            Random random = new Random(5);
            Rectangle spawnRect = new Rectangle((int)(game.WorldSize.X - 1000), 0, 1000, (int)(game.WorldSize.Y));
            IController[] controllers = new IController[] { new AIController(), new AIController(), new AIController(), new AIController() };

            //foreach (NetworkPlayerController controller in StaticNetworkPlayerManager.NetworkPlayerControllerList())
            int i = 0;
            for (; i < StaticNetworkPlayerManager.NetworkPlayerControllerList().Count; i++)
            {
                controllers[i % 4] = StaticNetworkPlayerManager.NetworkPlayerControllerList()[i];

                if (i % 4 == 3)
                {
                    BigShip ship = new BigShip(game, game.WorldSize / 2, new Vector2(0, 0), controllers[0], controllers[1], controllers[2], controllers[3]);
                    StaticGameObjectCollection.Collection.Add(ship);
                    controllers = new IController[] { new AIController(), new AIController(), new AIController(), new AIController() };
                }
            }

            if (i % 4 != 0)
            {
                BigShip ship2 = new BigShip(game, game.WorldSize / 2, new Vector2(0, 0), controllers[0], controllers[1], controllers[2], controllers[3]);
                StaticGameObjectCollection.Collection.Add(ship2);
            }

            StaticGameObjectCollection.Collection.Add(new Tower(game,
                Utils.RandomUtils.RandomVector2(new Rectangle(0, 0, 1000, 1000)) + game.WorldSize / 2
                , (float)(random.NextDouble() * Math.PI * 2)));

            StaticGameObjectCollection.Collection.Add(new Tower(game,
                Utils.RandomUtils.RandomVector2(new Rectangle(1500, 1500, 1000, 1000)) + game.WorldSize / 2
                , (float)(random.NextDouble() * Math.PI * 2)));

            StaticGameObjectCollection.Collection.Add(new Tower(game,
                Utils.RandomUtils.RandomVector2(new Rectangle(0, 1500, 1000, 1000)) + game.WorldSize / 2
                , (float)(random.NextDouble() * Math.PI * 2)));

            StaticGameObjectCollection.Collection.Add(new Tower(game,
                Utils.RandomUtils.RandomVector2(new Rectangle(1500, 0, 1000, 1000)) + game.WorldSize / 2
                , (float)(random.NextDouble() * Math.PI * 2)));

            for (int j = 0; j < 20; j++)
            {
                AIController c = new AIController();
                SmallShip ship3 = new SmallShip(game, Utils.RandomUtils.RandomVector2(spawnRect), new Vector2(0, 0), c, c);
                StaticGameObjectCollection.Collection.Add(ship3);
            }
        }
    }
}
