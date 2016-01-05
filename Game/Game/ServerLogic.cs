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
        List<AIController> aiControlerList;

        public ServerLogic(ServerGame game, Vector2 worldSize)
        {
            Random random = new Random(5);
            Rectangle spawnRect = new Rectangle((int)(worldSize.X - 1000), 0, 1000, (int)(worldSize.Y));
            aiControlerList = new List<AIController>();

            foreach (int id in game.NetworkPlayerManager.ControllersIDs)
            {
                //AIController ai1 = new AIController(game);
                //AIController ai2 = new AIController(game);
                //AIController ai3 = new AIController(game);

                //aiControlerList.Add(ai1);
                //aiControlerList.Add(ai2);
                //aiControlerList.Add(ai3);

                BigShip ship = new BigShip(game, worldSize / 2, new Vector2(0, 0),
                    game.NetworkPlayerManager[id],
                    game.NetworkPlayerManager[id],
                    game.NetworkPlayerManager[id],
                    game.NetworkPlayerManager[id]);

                game.ControllerFocus.SetFocus(id, ship, game.GameObjectCollection);

                //ai1.Focus = ship;
                //ai2.Focus = ship;
                //ai3.Focus = ship;

                game.GameObjectCollection.Add(ship);
            }

            game.GameObjectCollection.Add(new Tower(game,
                Utils.RandomUtils.RandomVector2(new Rectangle(0, 0, 1000, 1000)) + worldSize / 2
                , (float)(random.NextDouble() * Math.PI * 2)));

            game.GameObjectCollection.Add(new Tower(game,
                Utils.RandomUtils.RandomVector2(new Rectangle(1500, 1500, 1000, 1000)) + worldSize / 2
                , (float)(random.NextDouble() * Math.PI * 2)));

            game.GameObjectCollection.Add(new Tower(game,
                Utils.RandomUtils.RandomVector2(new Rectangle(0, 1500, 1000, 1000)) + worldSize / 2
                , (float)(random.NextDouble() * Math.PI * 2)));

            game.GameObjectCollection.Add(new Tower(game,
                Utils.RandomUtils.RandomVector2(new Rectangle(1500, 0, 1000, 1000)) + worldSize / 2
                , (float)(random.NextDouble() * Math.PI * 2)));

            for (int j = 0; j < 20; j++)
            {
                AIController c = new AIController(game);
                aiControlerList.Add(c);
                SmallShip ship3 = new SmallShip(game, Utils.RandomUtils.RandomVector2(spawnRect), new Vector2(0, 0), c, c);
                c.Focus = ship3;
                game.GameObjectCollection.Add(ship3);
            }
        }

        public void Update(float seconds)
        {
            foreach (AIController c in aiControlerList)
            {
                c.Update(seconds);
            }
        }
    }
}
