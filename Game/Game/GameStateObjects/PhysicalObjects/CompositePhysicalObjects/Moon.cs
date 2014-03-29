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

        public new class State : CompositePhysicalObject.State
        {
            public virtual void UpdateState(float seconds)
            {
                //base.UpdateState(seconds);

                if (Game1.IsServer)
                {
                    Moon thisMoon = this.GetObject<Moon>();

                    foreach (GameObject obj in StaticGameObjectCollection.Collection.Tree.GetObjectsInCircle(this.GetObject<Moon>().WorldPosition(), Moon.MaxRadius + Bullet.MaxRadius))
                    {
                        if (obj is Bullet)
                        {

                            Bullet bullet = (Bullet)obj;
                            Bullet.State bulletState = bullet.PracticalState<Bullet.State>();
                            if (thisMoon.CollidesWith(bullet))
                            {      
                                bullet.Destroy();
                            }
                        }
                    }

                }
            }

            public void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
            {
            }

            public State(GameObject obj) : base(obj) { }
        }

        public Moon(GameObjectUpdate message) : base(message) { }
        public Moon(Vector2 position, float direction)
            : base(position, direction)
        {
        }

        protected override GameObject.State BlankState(GameObject obj)
        {
            return new Moon.State(obj);
        }

        public override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
        {
            this.PracticalState<Moon.State>().MoveOutsideWorld(position, movePosition);
        }

        public override void UpdateSub(float seconds)
        {
            base.UpdateSub(seconds);
            this.PracticalState<Moon.State>().UpdateState(seconds);
        }
    }
}
