using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
namespace MyGame.GameStateObjects
{
    public abstract class FlyingGameObject : CompositePhysicalObject
    {
        private float speed = 00.0f;
        private float maxSpeed = 200.0f;
        private float maxAngularSpeed = 1.0f;
        private float acceleration = 0;
        private float maxAcceleration = 100;
        private Drawable drawObject;
        protected FlyingGameObject(GameState gameState, Drawable drawObject, Vector2 position, float direction, float speed, float maxSpeed, float acceleration, float maxAcceleration, float maxAngularSpeed)
            : base(gameState, position, direction)
        {
            this.speed = speed;
            this.maxSpeed = maxSpeed;
            this.maxAngularSpeed = maxAngularSpeed;
            this.acceleration = acceleration;
            this.maxAcceleration = maxAcceleration;
            this.drawObject = drawObject;
        }

        public float Speed
        {
            get { return speed; }
            set
            {
                if (value > maxSpeed)
                {
                    speed = maxSpeed;
                }
                else if (value < -maxSpeed)
                {
                    speed = -maxSpeed;
                }
                else
                {
                    speed = value;
                }
            }
        }

        public float Acceleration
        {
            get { return acceleration; }
            set
            {
                if (value > maxAcceleration)
                {
                    acceleration = maxAcceleration;
                }
                else if (value < -maxAcceleration)
                {
                    acceleration = -maxAcceleration;
                }
                else
                {
                    acceleration = value;
                }
            }
        }

        public float MaxSpeed
        {
            get { return maxSpeed; }
            private set
            {
                maxSpeed = value;
                // We need to reset the speed to comply with the new max.
                Speed = Speed;
            }
        }

        public float MaxAngularSpeed
        {
            get { return maxAngularSpeed; }
        }

        public float MaxAcceleration
        {
            get { return maxAcceleration; }
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (acceleration != 0)
            {
                Speed = Speed + acceleration * secondsElapsed;
            }
            else
            {
                //simulate drag
                if (Speed > 0)
                {
                    Speed = Math.Max(0, Speed - maxAcceleration * secondsElapsed);
                }
                else
                {
                    Speed = Math.Min(0, Speed + maxAcceleration * secondsElapsed);
                }
            }

            Position = Position + Vector2Utils.ConstructVectorFromPolar((float)(secondsElapsed * Speed), Direction);
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            drawObject.Position = this.Position;
            drawObject.Rotation = this.Direction;
            drawObject.Draw(graphics);
        }

        public Boolean CollidesWith(FlyingGameObject other)
        {
            drawObject.Position = this.Position;
            drawObject.Rotation = this.Direction;

            other.drawObject.Position = other.Position;
            other.drawObject.Rotation = other.Direction;
            return this.drawObject.CollidesWith(other.drawObject);
        }

        public virtual void TurnRight(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            float changeInAngle = (float)(secondsElapsed * maxAngularSpeed);
            Direction = Direction + changeInAngle;
        }

        public virtual void TurnLeft(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            float changeInAngle = (float)(secondsElapsed * maxAngularSpeed);
            Direction = Direction - changeInAngle;
        }

        private void TurnTowardsDirection(GameTime gameTime, float directionSetpoint)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            float changeInAngle = (float)(secondsElapsed * maxAngularSpeed);
            float angleDifference = Vector2Utils.RestrictAngle(directionSetpoint - Direction);
            if (angleDifference < Math.PI)
            {
                Direction = (changeInAngle > angleDifference) ? directionSetpoint : Direction + changeInAngle;
            }
            else
            {
                Direction = (-changeInAngle < (angleDifference - Math.PI * 2)) ? directionSetpoint : Direction - changeInAngle;
            }
        }

        public virtual void TurnTowards(GameTime gameTime, Vector2 target)
        {
            float directionSetpoint = Vector2Utils.Vector2Angle(target - Position);
            this.TurnTowardsDirection(gameTime, directionSetpoint);
        }

        public virtual void TurnAway(GameTime gameTime, Vector2 target)
        {
            float directionSetpoint = Vector2Utils.RestrictAngle(Vector2Utils.Vector2Angle(target - Position) + Math.PI);
            this.TurnTowardsDirection(gameTime, directionSetpoint);
        }

        public override Boolean IsFlyingGameObject
        {
            get { return true; }
        }

        public virtual Boolean IsShip
        {
            get { return false; }
        }

        public virtual Boolean IsBullet
        {
            get { return false; }
        }   

    }
}
