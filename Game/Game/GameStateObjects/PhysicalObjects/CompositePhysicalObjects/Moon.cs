using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.DrawingUtils;
using Microsoft.Xna.Framework;
using MyGame.Networking;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Bullets;

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

        public Moon(GameObjectUpdate message) : base(message) { }
        public Moon(Vector2 position, float direction)
            : base(position, direction)
        {
        }

        public override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
        {
        }

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);
            if (Game1.IsServer)
            {
                foreach (GameObject obj in StaticGameObjectCollection.Collection.Tree.GetObjectsInCircle(this.WorldPosition(), Moon.MaxRadius + Bullet.MaxRadius))
                {
                    if (obj is Bullet)
                    {

                        Bullet bullet = (Bullet)obj;
                        if (this.CollidesWith(bullet))
                        {
                            bullet.Destroy();
                        }
                    }
                }

            }
        }
    }
}
