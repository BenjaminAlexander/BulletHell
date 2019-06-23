using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.DrawingUtils;
using Microsoft.Xna.Framework;
using MyGame.GameServer;
using MyGame.GameClient;

namespace MyGame.GameStateObjects.PhysicalObjects.CompositePhysicalObjects
{
    class Moon : CompositePhysicalObject
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Moon"), Color.White, TextureLoader.GetTexture("Moon").CenterOfMass, .9f);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        public static float MaxRadius
        {
            get { return 600; }
        }

        public Moon()
        {
        }

        public Moon(Game1 game)
            : base(game)
        {
        }

        /*
        public Moon(Vector2 position, float direction)
        {
            base.CompositePhysicalObjectInit(position, direction);
        }*/

        public override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
        {
        }
    }
}
