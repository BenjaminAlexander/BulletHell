using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;
using MyGame.GameServer;
using MyGame.GameClient;
using MyGame.Utils;
using MyGame.Engine.GameState.Instants;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    class Tower : Ship
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Moon"), Color.White, TextureLoader.GetTexture("Moon").CenterOfMass, .9f);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        public static void ServerInitialize(NextInstant next, Tower obj, Vector2 position, float direction)
        {
            Ship.ServerInitialize(next, obj, position, new Vector2(0), direction, 10000, 0, 0, 0.0f, null);
        }

        public static Tower TowerFactory(ServerGame game, NextInstant next)
        {
            Tower t = game.GameObjectCollection.NewGameObject<Tower>(next);
            Tower.ServerInitialize(next, t, 
                RandomUtils.RandomVector2(new Rectangle(0, 0, 5000, 5000)) + game.WorldSize / 2, 
                (float)(RandomUtils.random.NextDouble() * Math.PI * 2));
            return t;
        }
    }
}
