using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects;
using MyGame.PlayerControllers;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameStateObjects.PhysicalObjects.CompositePhysicalObjects;
using MyGame.GameServer;
using MyGame.AIControllers;

namespace MyGame
{
    public class ServerLogic
    {
        private List<BigShip> aiBigShips = new List<BigShip>();

        public ServerLogic(ServerGame game, Lobby lobby, Vector2 worldSize)
        {
            ControllerFocusObject controllerFocusObject = new ControllerFocusObject(game);
            ControllerFocusObject.ServerInitialize(controllerFocusObject, lobby.Clients.Count);
            game.GameObjectCollection.Add(controllerFocusObject);

            for (int j = 0; j < 4; j++)
            {
                Tower.TowerFactory(game);
            }
        }

        public void Update(ServerGame game, Lobby lobby)
        {
            foreach (BigShip ship in aiBigShips.ToArray())
            {
                if (ship.IsDestroyed)
                {
                    aiBigShips.Remove(ship);
                }
            }

            while (game.GameObjectCollection.GetMasterList().GetList<SmallShip>().Count < 20)
            {
                SmallShip.SmallShipFactory(game);
            }

            while (game.GameObjectCollection.GetMasterList().GetList<BigShip>().Count < 5)
            {
                aiBigShips.Add(BigShip.BigShipFactory(game));
            }

            ControllerFocusObject controllerFocusObject = game.GameObjectCollection.GetMasterList().GetList<ControllerFocusObject>()[0];
            foreach (Player player in lobby.Clients)
            {
                if (controllerFocusObject.GetFocus(player.Id) == null || controllerFocusObject.GetFocus(player.Id).IsDestroyed)
                {
                    BigShip playerShip = BigShip.BigShipFactory(game, player);
                    CircleBigShips(playerShip.Position);
                }
            }
        }

        public void CircleBigShips(Vector2 position)
        {
            int i = 0;
            foreach (BigShip ship in aiBigShips)
            {
                ((GoodAI)ship.GetController()).TargetPosition = position + Utils.Vector2Utils.ConstructVectorFromPolar(1500, ((float)i / aiBigShips.Count) * (2 * Math.PI));
                i++;
            }
        }
    }
}
