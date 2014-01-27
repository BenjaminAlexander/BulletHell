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
        public Ship(Vector2 position, Collidable drawable, float maxSpeed)
            : base(drawable, position, 0, new Vector2(0), maxSpeed, 500, 1.0f)
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

                if (!SpeedUp && !SlowDown)
                {
                    TargetVelocity = new Vector2(0);
                }
                

                base.UpdateSubclass(gameTime);

                if (!GameState.GetWorldRectangle().Contains(this.Position))
                {
                    this.Position = preUpdatePosition;
                    this.Velocity = new Vector2(0);
                    //this.Acceleration = 0;
                }

                //Bullet hits

                Circle boundingCircle = this.BoundingCircle();
                Circle dangerCircle = new Circle(boundingCircle.Center, Math.Max(boundingCircle.Radius + Bullet.MAX_RADIUS, boundingCircle.Radius + Mine.MAX_RADIUS));
                foreach (CompositePhysicalObject obj in GameState.GetObjectsInCircle(dangerCircle))
                {
                    if(obj is Bullet)
                    {
                        Bullet bullet = (Bullet) obj;
                        if (bullet.Owner != this && this.CollidesWith(bullet))
                        //if (owner != ship && ship.Contains(this.Position))
                        {
                            //GameState.RemoveGameObject(bullet);
                            this.DoDamage(bullet.Damage);
                            bullet.Destroy();
                        }
                    }

                    if(!(this is NPCShip) && obj is Mine)
                    {
                        Mine mine = (Mine) obj;
                        if (this.CollidesWith(mine.Collidable))
                        //if (owner != ship && ship.Contains(this.Position))
                        {
                            //GameState.RemoveGameObject(mine);
                            this.DoDamage(mine.Damage);
                            mine.Destroy();
                        }
                    }
                }
                
            }
            else if(GameState != null)
            {
                //GameState.RemoveGameObject(this);
                this.Destroy();
            }
        }

        public class ShipOutOfBoundsException : Exception
        {
        }
    }
}
