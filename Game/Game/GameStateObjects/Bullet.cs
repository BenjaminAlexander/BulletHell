using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public class Bullet : MovingGameObject
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Bullet"), Color.White, new Vector2(20, 5), 1);
        private static float speed = 100;
        public static float MAX_RADIUS
        {
            get { return 50;}
        }

        public new class State : MovingGameObject.State
        {

            private int damage = 10;
            private Vector2 start;
            private float range = 3000;

            public State(GameObject obj) : base(obj) { }

            public override void ApplyMessage(GameObjectUpdate message)
            {
                base.ApplyMessage(message);
                this.damage = message.ReadInt();
                this.start = message.ReadVector2();
                this.range = message.ReadFloat();
            }

            public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
            {
                message = base.ConstructMessage(message);
                message.Append(this.damage);
                message.Append(this.start);
                message.Append(this.range);
                return message;
            }

            public override void Interpolate(GameObject.State d, GameObject.State s, float smoothing, GameObject.State blankState)
            {
                base.Interpolate(d, s, smoothing, blankState);
                Bullet.State myS = (Bullet.State)s;
                Bullet.State myBlankState = (Bullet.State)blankState;

                myBlankState.damage = myS.damage;
                myBlankState.start = myS.start;
                myBlankState.range = myS.range;
            }

            public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
            {
                if (this.Velocity.Length() < speed - 100)
                {
                    int i = 1;

                }
                else
                {

                }
                collidable.Draw(graphics, this.Position, Direction);
            }

            protected override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
            {
                if (Game1.IsServer)
                {
                    this.Destroy();
                }
            }
        }


        protected override GameObject.State BlankState(GameObject obj)
        {
            return new Bullet.State(obj);
        }

        public Bullet(GameObjectUpdate message) : base(message) { }

        public Bullet(Vector2 position, float direction)
            : base(position, Utils.Vector2Utils.ConstructVectorFromPolar(speed, direction), direction, 0, direction)
        {
        } 
    }
}
