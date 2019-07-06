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
using MyGame.Engine.GameState.Instants;
using MyGame.Utils;

namespace MyGame
{
    public class ServerLogic
    {
        private List<BigShip> aiBigShips = new List<BigShip>();

        public ServerLogic(ServerGame game, Lobby lobby, Vector2 worldSize)
        {
            NextInstant next = (new Instant(0)).AsNext;

            ControllerFocusObject controllerFocusObject = game.GameObjectCollection.NewGameObject<ControllerFocusObject>(next);
            ControllerFocusObject.ServerInitialize(controllerFocusObject, lobby.Clients.Count);

            List<Ship> targetsForEvil = new List<Ship>();

            for (int j = 0; j < 4; j++)
            {
                targetsForEvil.Add(Tower.TowerFactory(game, next));
            }

            for (int j = 0; j < 5; j++)
            {
                BigShip newShip = BigShip.BigShipFactory(next, game, targetsForEvil[RandomUtils.random.Next(targetsForEvil.Count)]);
                aiBigShips.Add(newShip);
                targetsForEvil.Add(newShip);
            }

            foreach (Player player in lobby.Clients)
            {
                if (controllerFocusObject.GetFocus(player.Id) == null)
                {
                    BigShip playerShip = BigShip.BigShipFactory(next, game, controllerFocusObject, player);
                    CircleBigShips(game.WorldSize / 2);
                }
            }

            for (int j = 0; j < 20; j++)
            {
                SmallShip.SmallShipFactory(next, game, targetsForEvil[RandomUtils.random.Next(targetsForEvil.Count)]);
            }
        }

        public void Update(CurrentInstant current, NextInstant next, ServerGame game, Lobby lobby)
        {

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
