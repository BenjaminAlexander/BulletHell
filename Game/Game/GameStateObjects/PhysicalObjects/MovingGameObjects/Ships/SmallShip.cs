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
using MyGame.AIControllers;
using MyGame.Engine.GameState.Instants;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    class SmallShip : Ship
    {
        public static SmallShip SmallShipFactory(NextInstant next, ServerGame game, Ship target)
        {
            Rectangle spawnRect = new Rectangle((int)(game.WorldSize.X - 1000), 0, 1000, (int)(game.WorldSize.Y));
            EvilAI c = new EvilAI(game, target);
            SmallShip ship3 = game.GameObjectCollection.NewGameObject<SmallShip>(next);
            SmallShip.ServerInitialize(next, game, ship3, Utils.RandomUtils.RandomVector2(spawnRect), new Vector2(0, 0), c, c);
            c.Focus = ship3;
            return ship3;
        }

        public static void ServerInitialize(NextInstant next, Game1 game, SmallShip smallShip, Vector2 position, Vector2 velocity, ControlState controller1, ControlState controller4)
        {
            Ship.ServerInitialize(smallShip, position, velocity, 0, 40, 800, 1800, 1f, controller1);
            Turret t3 = game.GameObjectCollection.NewGameObject<Turret>(next);
            Turret.ServerInitialize(t3, smallShip, new Vector2(25, 25) - TextureLoader.GetTexture("Enemy").CenterOfMass, (float)(0), (float)(Math.PI * 3), controller4);
        }

        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Enemy"), Color.White, TextureLoader.GetTexture("Enemy").CenterOfMass, .9f);
        public override Collidable Collidable
        {
            get { return collidable; }
        }
    }
}