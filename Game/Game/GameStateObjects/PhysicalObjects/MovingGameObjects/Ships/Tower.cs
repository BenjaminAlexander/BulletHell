using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.PhysicalObjects.MemberPhysicalObjects;
using MyGame.GameServer;
using MyGame.GameClient;
using MyGame.Utils;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    class Tower : Ship
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Moon"), Color.White, TextureLoader.GetTexture("Moon").CenterOfMass, .9f);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        public Tower()
        {
        }

        public Tower(Game1 game)
            : base(game)
        {
        }

        public static void ServerInitialize(Tower obj, Vector2 position, float direction)
        {
            Ship.ServerInitialize(obj, position, new Vector2(0), direction, 10000, 0, 0, 0.0f, null);
        }

        public static void TowerFactory(ServerGame game)
        {
            Tower t = new Tower(game);
            Tower.ServerInitialize(t, 
                RandomUtils.RandomVector2(new Rectangle(0, 0, 5000, 5000)) + game.WorldSize / 2, 
                (float)(RandomUtils.random.NextDouble() * Math.PI * 2));
            game.GameObjectCollection.Add(t);
        }
    }
}
