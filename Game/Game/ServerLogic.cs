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
using MyGame.GameServer;

namespace MyGame
{
    public class ServerLogic
    {
        List<AIController> aiControlerList;

        public ServerLogic(ServerGame game, Lobby lobby, Vector2 worldSize)
        {
            Random random = new Random(5);
            Rectangle spawnRect = new Rectangle((int)(worldSize.X - 1000), 0, 1000, (int)(worldSize.Y));
            aiControlerList = new List<AIController>();

            ControllerFocusObject controllerFocusObject = new ControllerFocusObject(game);
            ControllerFocusObject.ServerInitialize(controllerFocusObject, lobby.Clients.Count);
            game.GameObjectCollection.Add(controllerFocusObject);

            foreach (Player player in lobby.Clients)
            {
                BigShip ship = new BigShip(game);
                ship.BigShipInit(worldSize / 2, new Vector2(0, 0),
                    player.Controller,
                    player.Controller,
                    player.Controller,
                    player.Controller);

                game.GameObjectCollection.Add(ship);
                controllerFocusObject.SetFocus(player, ship);
            }

            Tower t = new Tower(game);
            t.TowerInit(Utils.RandomUtils.RandomVector2(new Rectangle(0, 0, 1000, 1000)) + worldSize / 2
                , (float)(random.NextDouble() * Math.PI * 2));
            game.GameObjectCollection.Add(t);

            Tower t1 = new Tower(game);
            t1.TowerInit(Utils.RandomUtils.RandomVector2(new Rectangle(1500, 1500, 1000, 1000)) + worldSize / 2
                , (float)(random.NextDouble() * Math.PI * 2));
            game.GameObjectCollection.Add(t1);

            Tower t2 = new Tower(game);
            t2.TowerInit(Utils.RandomUtils.RandomVector2(new Rectangle(0, 1500, 1000, 1000)) + worldSize / 2
                , (float)(random.NextDouble() * Math.PI * 2));
            game.GameObjectCollection.Add(t2);

            Tower t3 = new Tower(game);
            t3.TowerInit(Utils.RandomUtils.RandomVector2(new Rectangle(1500, 0, 1000, 1000)) + worldSize / 2
                , (float)(random.NextDouble() * Math.PI * 2));
            game.GameObjectCollection.Add(t3);

            for (int j = 0; j < 20; j++)
            {
                AIController c = new AIController(game);
                aiControlerList.Add(c);
                SmallShip ship3 = new SmallShip(game);
                SmallShip.ServerInitialize(ship3, Utils.RandomUtils.RandomVector2(spawnRect), new Vector2(0, 0), c, c);
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
