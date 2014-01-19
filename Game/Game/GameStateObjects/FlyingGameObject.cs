using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.Geometry;
using MyGame.GameStateObjects.QuadTreeUtils;

namespace MyGame.GameStateObjects
{
    public abstract class FlyingGameObject : CompositePhysicalObject
    {
        private float speed = 00.0f;
        private float maxSpeed = 200.0f;
        private float maxAngularSpeed = 1.0f;
        private float acceleration = 0;
        private float maxAcceleration = 100;
        private Collidable collidable;
        protected FlyingGameObject(Collidable drawObject, Vector2 position, float direction, float speed, float maxSpeed, float acceleration, float maxAcceleration, float maxAngularSpeed)
            : base(position, direction)
        {
            this.speed = speed;
            this.maxSpeed = maxSpeed;
            this.maxAngularSpeed = maxAngularSpeed;
            this.acceleration = acceleration;
            this.maxAcceleration = maxAcceleration;
            this.collidable = drawObject;
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
            Speed = Speed + acceleration * secondsElapsed;
            Position = Position + Vector2Utils.ConstructVectorFromPolar((float)(secondsElapsed * Speed), Direction);
            base.UpdateSubclass(gameTime);
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);
            collidable.Position = this.Position;
            collidable.Rotation = this.Direction;
            collidable.Draw(graphics);
        }

        public Boolean CollidesWith(FlyingGameObject other)
        {
            collidable.Position = this.Position;
            collidable.Rotation = this.Direction;

            other.collidable.Position = other.Position;
            other.collidable.Rotation = other.Direction;
            return this.collidable.CollidesWith(other.collidable);
        }
/*
        public Boolean CollidesWith(Vector2 point1, Vector2 point2)
        {
            drawObject.Position = this.Position;
            drawObject.Rotation = this.Direction;

            return this.drawObject.CollidesWith(point1, point2);
        }*/

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

        public Boolean Contains(Vector2 point)
        {
            return collidable.Contains(point);
        }

        public Circle BoundingCircle()
        {
            return collidable.BoundingCircle();
        }
    }
}
