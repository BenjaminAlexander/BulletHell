using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.IO;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.Geometry;
using MyGame.GameStateObjects.QuadTreeUtils;

namespace MyGame.GameStateObjects.Ships
{
    public abstract class Ship : FlyingGameObject  //simple player ship
    {

        public static float MAX_RADIUS
        {
            get { return 500; }
        }

        private int health = 40;
        protected int Health
        {
            set { health = value; }
            get { return health; }
        }

        protected override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
        {
           
        }


        // All ships have a position and a direction (speed).
        public Ship(Vector2 position, Collidable drawable)
            : base(drawable, position, 0, 00.0f, 400.0f, 0, 1200, 2.0f)
        {

        }

        public void DoDamage(int damage)
        {
            health = health - damage;
        }

        public Boolean IsAlive
        {
            get { return health > 0; }
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {

            if (!GameState.GetWorldRectangle().Contains(this.Position))
            {
                throw new ShipOutOfBoundsException();
            }
            if (IsAlive)
            {

                Vector2 preUpdatePosition = this.Position;

                if (Acceleration == 0)
                {
                    float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
                    //simulate drag
                    if (Speed > 0)
                    {
                        Speed = Math.Max(0, Speed - MaxAcceleration * secondsElapsed);
                    }
                    else
                    {
                        Speed = Math.Min(0, Speed + MaxAcceleration * secondsElapsed);
                    }
                }

                base.UpdateSubclass(gameTime);
                if (!GameState.GetWorldRectangle().Contains(this.Position))
                {
                    this.Position = preUpdatePosition;
                    this.Speed = 0;
                    this.Acceleration = 0;
                }

                //Bullet hits

                Circle boundingCircle = this.BoundingCircle();

                foreach (Bullet bullet in GameState.GetBullets(boundingCircle.Center, boundingCircle.Radius + Bullet.MAX_RADIUS))
                {
                    if (bullet.Owner != this && this.CollidesWith(bullet))
                    //if (owner != ship && ship.Contains(this.Position))
                    {
                        GameState.RemoveGameObject(bullet);
                        this.DoDamage(bullet.Damage);
                    }
                }
                
            }
            else if(GameState != null)
            {
                GameState.RemoveGameObject(this);
            }
        }

        public virtual Boolean IsPlayerShip
        {
            get{ return false; }
        }

        public virtual Boolean IsNPCShip
        {
            get{ return false; }
        }

        public override Boolean IsShip
        {
            get { return true; }
        }

        public class ShipOutOfBoundsException : Exception
        {
        }
    }
}
