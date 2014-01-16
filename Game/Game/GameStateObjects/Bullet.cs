using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.Ships;
namespace MyGame.GameStateObjects
{
    class Bullet : FlyingGameObject
    {

        private static float speed = 1600;
        private int damage = 10;
        private Ship owner;
        private Vector2 start;
        private float range = 3000;

        public Bullet(GameState gameState, Ship owner, Vector2 position, float direction)
            : base(gameState, new Drawable(Textures.Bullet, position, Color.White, 0, new Vector2(20, 5), 1), position, direction, speed, speed, 0, 0, 0)
        {
            this.start = position;
            this.owner = owner;
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            base.UpdateSubclass(gameTime);

            if (!GameState.GetWorldRectangle().Contains(this.Position))
            {
                GameState.RemoveGameObject(this);
            }

            if (Vector2.Distance(start, this.Position) > range)
            {
                GameState.RemoveGameObject(this);
            }
            else
            {
                foreach (Ships.Ship ship in GameState.GetShips())
                {
                    if (owner != ship && this.CollidesWith(ship))
                    {
                        GameState.RemoveGameObject(this);
                        ship.DoDamage(damage);
                    }
                }
            }
        }

        public override Boolean IsBullet
        {
            get { return true; }
        }
    }
}
