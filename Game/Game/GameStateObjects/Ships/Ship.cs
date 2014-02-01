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
using MyGame.Networking;

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
        public Ship(int id)
            : base(id)
        {

        }

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

        //using MyGame.Networking;
        public override void UpdateMemberFields(GameObjectUpdate message)
        {
            base.UpdateMemberFields(message);
            health = message.ReadInt();

        }

        public override GameObjectUpdate MemberFieldUpdateMessage(GameObjectUpdate message)
        {
            message = base.MemberFieldUpdateMessage(message);
            message.Append(health);
            return message;
        }
    }
}
