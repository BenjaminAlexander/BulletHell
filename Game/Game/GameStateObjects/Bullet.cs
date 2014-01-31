using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.Ships;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public class Bullet : FlyingGameObject
    {
        public static float MAX_RADIUS
        {
            get { return 50;}
        }

        private static float speed = 1000;
        private static int damage = 10;
        private Ship owner;
        private Vector2 start;
        private static float range = 3000;

        public Bullet(int id)
            : base(id)
        {
            this.Collidable = new Collidable(Textures.Bullet, new Vector2(0), Color.White, 0, new Vector2(20, 5), 1);
        }

        public Bullet(Ship owner, Vector2 position, float direction)
            : base(new Collidable(Textures.Bullet, position, Color.White, 0, new Vector2(20, 5), 1), position, direction, Vector2Utils.ConstructVectorFromPolar(speed, direction), speed, 0, 0)
        {
            this.start = position;
            this.owner = owner;
            
        }

        protected override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
        {
            //GameState.RemoveGameObject(this);
            this.Destroy();
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            Vector2 prePosition = this.Position;
            base.UpdateSubclass(gameTime);

            if (!GameState.GetWorldRectangle().Contains(this.Position))
            {
                //GameState.RemoveGameObject(this);
                this.Destroy();
            }

            if (Vector2.Distance(start, this.Position) > range)
            {
                //GameState.RemoveGameObject(this);
                this.Destroy();
            }
            
        }

        public Ship Owner
        {
            get { return owner; }
        }

        public int Damage
        {
            get { return damage; }
        }

        //using MyGame.Networking;
        public override void UpdateMemberFields(GameObjectUpdate message)
        {
            base.UpdateMemberFields(message);
            owner = (Ship)message.ReadGameObject();
            start = message.ReadVector2();
        }

        public override GameObjectUpdate MemberFieldUpdateMessage(GameObjectUpdate message)
        {
            message = base.MemberFieldUpdateMessage(message);
            message.Append(owner);
            message.Append(start);
            return message;
        }
    }
}
