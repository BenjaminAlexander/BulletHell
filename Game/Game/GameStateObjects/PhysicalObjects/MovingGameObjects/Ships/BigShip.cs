using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.PlayerControllers;
using MyGame.DrawingUtils;
using MyGame.GameServer;
using MyGame.GameClient;
using Microsoft.Xna.Framework.Graphics;
using MyGame.AIControllers;
using MyGame.Engine.GameState.Instants;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    class BigShip : Ship
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Ship"), Color.White, TextureLoader.GetTexture("Ship").CenterOfMass, .9f);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        public static void ServerInitialize(NextInstant next, Game1 game, BigShip obj, Vector2 position, Vector2 velocity, ControlState controller1, ControlState controller2, ControlState controller3, ControlState controller4)
        {
            Ship.ServerInitialize(next, obj, position, velocity, 0, 500, 600, 300, 0.5f, controller1);
        }

        public static BigShip BigShipFactory(NextInstant next, ServerGame game, ControllerFocusObject controllerFocusObject, Player player)
        {
            BigShip ship = game.GameObjectCollection.NewGameObject<BigShip>(next);
            BigShip.ServerInitialize(next, game, ship, game.WorldSize / 2, new Vector2(0, 0),
                player.Controller,
                player.Controller,
                player.Controller,
                player.Controller);

            controllerFocusObject.SetFocus(next, player, ship);
            return ship;
        }

        public static BigShip BigShipFactory(NextInstant next, ServerGame game, Ship target)
        {
            GoodAI controller1 = new GoodAI(game, target);
            GoodGunnerAI controller2 = new GoodGunnerAI(game, target);
            GoodGunnerAI controller3 = new GoodGunnerAI(game, target);
            GoodGunnerAI controller4 = new GoodGunnerAI(game, target);

            BigShip ship = game.GameObjectCollection.NewGameObject<BigShip>(next);
            Vector2 position = Utils.RandomUtils.RandomVector2(new Vector2(500)) + game.WorldSize / 2;
            BigShip.ServerInitialize(next, game, ship, position, new Vector2(0, 0),
                controller1,
                controller2,
                controller3,
                controller4);

            controller1.Focus = ship;
            controller1.TargetPosition = position;
            controller2.Focus = ship;
            controller3.Focus = ship;
            controller4.Focus = ship;
            return ship;
        }
    }
}