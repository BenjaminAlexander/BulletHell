using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.PlayerControllers;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.PhysicalObjects.MemberPhysicalObjects;
using MyGame.GameServer;
using MyGame.GameClient;
using Microsoft.Xna.Framework.Graphics;
using MyGame.AIControllers;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    class BigShip : Ship
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Ship"), Color.White, TextureLoader.GetTexture("Ship").CenterOfMass, .9f);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        public BigShip(Game1 game)
            : base(game)
        {
        }

        public static void ServerInitialize(BigShip obj, Vector2 position, Vector2 velocity, ControlState controller1, ControlState controller2, ControlState controller3, ControlState controller4)
        {
            Ship.ServerInitialize(obj, position, velocity, 0, 500, 600, 300, 0.5f, controller1);
            Turret t = new Turret(obj.Game);
            Turret.ServerInitialize(t, obj, new Vector2(119, 95) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(Math.PI / 2), (float)(Math.PI / 3), controller2);
            Turret t2 = new Turret(obj.Game);
            Turret.ServerInitialize(t2, obj, new Vector2(119, 5) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(-Math.PI / 2), (float)(Math.PI / 3), controller3);
            Turret t3 = new Turret(obj.Game);
            Turret.ServerInitialize(t3, obj, new Vector2(145, 50) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(0), (float)(Math.PI / 3), controller1);
            Turret t4 = new Turret(obj.Game);
            Turret.ServerInitialize(t4, obj, new Vector2(20, 50) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(-Math.PI), (float)(Math.PI / 3), controller4);
            obj.Game.GameObjectCollection.Add(t);
            obj.Game.GameObjectCollection.Add(t2);
            obj.Game.GameObjectCollection.Add(t3);
            obj.Game.GameObjectCollection.Add(t4);
        }

        public static BigShip BigShipFactory(ServerGame game, Player player)
        {
            BigShip ship = new BigShip(game);
            BigShip.ServerInitialize(ship, game.WorldSize / 2, new Vector2(0, 0),
                player.Controller,
                player.Controller,
                player.Controller,
                player.Controller);

            game.GameObjectCollection.Add(ship);
            ControllerFocusObject controllerFocusObject = game.GameObjectCollection.GetMasterList().GetList<ControllerFocusObject>()[0];
            controllerFocusObject.SetFocus(player, ship);
            return ship;
        }

        public static BigShip BigShipFactory(ServerGame game)
        {
            GoodAI controller1 = new GoodAI(game);
            GoodGunnerAI controller2 = new GoodGunnerAI(game);
            GoodGunnerAI controller3 = new GoodGunnerAI(game);
            GoodGunnerAI controller4 = new GoodGunnerAI(game);

            BigShip ship = new BigShip(game);
            BigShip.ServerInitialize(ship, Utils.RandomUtils.RandomVector2(new Vector2(500))+game.WorldSize / 2, new Vector2(0, 0),
                controller1,
                controller2,
                controller3,
                controller4);

            game.GameObjectCollection.Add(ship);
            controller1.Focus = ship;
            controller1.TargetPosition = ship.Position;
            controller2.Focus = ship;
            controller3.Focus = ship;
            controller4.Focus = ship;
            return ship;
        }
    }
}