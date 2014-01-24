using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.Ships;
using MyGame.GameStateObjects.QuadTreeUtils;

namespace MyGame.GameStateObjects
{
    public class Bullet : FlyingGameObject
    {
        public static float MAX_RADIUS
        {
            get { return 50;}
        }

        private static float speed = 1000;
        private int damage = 10;
        private Ship owner;
        private Vector2 start;
        private float range = 3000;

        public Bullet(Ship owner, Vector2 position, float direction)
            : base(new Collidable(Textures.Bullet, position, Color.White, 0, new Vector2(20, 5), 1), position, direction, Vector2Utils.ConstructVectorFromPolar(speed, direction), speed, 0, 0)
        {
            this.start = position;
            this.owner = owner;
        }

        protected override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
        {
            GameState.RemoveGameObject(this);
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            Vector2 prePosition = this.Position;
            base.UpdateSubclass(gameTime);

            if (!GameState.GetWorldRectangle().Contains(this.Position))
            {
                GameState.RemoveGameObject(this);
            }

            if (Vector2.Distance(start, this.Position) > range)
            {
                GameState.RemoveGameObject(this);
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
    }
}
