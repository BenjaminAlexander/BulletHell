using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;

namespace MyGame.GameStateObjects
{
    public abstract class FlyingGameObject : GameObject
    {
        Vector2 position = new Vector2(0);
        float direction = 0;
        private float speed = 00.0f;
        private float maxSpeed = 200.0f;
        private float maxAngularSpeed = 1.0f;
        private float acceleration = 0;
        private float maxAcceleration = 100;

        protected FlyingGameObject(Vector2 position, float direction, float speed, float maxSpeed, float acceleration, float maxAcceleration, float maxAngularSpeed)
        {
            this.position = position;
            this.direction = direction;
            this.speed = speed;
            this.maxSpeed = maxSpeed;
            this.maxAngularSpeed = maxAngularSpeed;
            this.acceleration = acceleration;
            this.maxAcceleration = maxAcceleration;
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

        public Vector2 Position
        {
            get { return position; }
            protected set { position = value; }
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

        public float Direction
        {
            get { return direction; }
            private set { direction = Vector2Utils.RestrictAngle(value); }
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

        public virtual void TurnTowards(GameTime gameTime, Vector2 target)
        {
            float directionSetpoint = Vector2Utils.Vector2Angle(target - Position);
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
