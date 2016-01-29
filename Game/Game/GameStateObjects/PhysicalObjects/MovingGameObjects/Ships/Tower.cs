using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.Networking;
using Microsoft.Xna.Framework;
using MyGame.PlayerControllers;
using MyGame.IO;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.PhysicalObjects.MemberPhysicalObjects;
using MyGame.GameServer;
using MyGame.GameClient;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    class Tower : Ship
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Moon"), Color.White, TextureLoader.GetTexture("Moon").CenterOfMass, .9f);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        public Tower(Game1 game)
            : base(game)
        {
        }

        public void TowerInit(Vector2 position, float direction)
        {
            Ship.ServerInitialize(this, position, new Vector2(0), direction, 10000, 0, 0, 0.0f, null);
        }
    }
}
