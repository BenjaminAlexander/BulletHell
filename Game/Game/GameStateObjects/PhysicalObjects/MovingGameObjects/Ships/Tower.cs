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

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    class Tower : Ship
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Moon"), Color.White, TextureLoader.GetTexture("Moon").CenterOfMass, .9f);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        public Tower(GameObjectUpdate message) : base(message) { }

        public Tower(Vector2 position, float direction)
            : base(position, new Vector2(0), direction, 10000, 0, 0, 0.0f, null)
        {
        }
    }
}
