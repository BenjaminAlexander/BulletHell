using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.GameServer;
using MyGame.GameClient;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.GameState;
using MyGame.Engine.GameState.FieldValues;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects
{
    public abstract class MovingGameObject : CompositePhysicalObject
    {
        private Field<Vector2Value> velocity;
        private Field<FloatValue> angularSpeed;
        private Field<FloatValue> targetAngle;

        public MovingGameObject()
        {
        }

        public MovingGameObject(Game1 game)
            : base(game)
        {
        }

        internal override void DefineFields(InitialInstant instant)
        {
            base.DefineFields(instant);
            velocity = new Field<Vector2Value>(instant);
            angularSpeed = new Field<FloatValue>(instant);
            targetAngle = new Field<FloatValue>(instant);
        }

        public static void ServerInitialize(MovingGameObject obj, Vector2 position, Vector2 velocity, float direction, float angularVelocity, float targetAngle)
        {
            CompositePhysicalObject.ServerInitialize(obj, position, direction);
            obj.velocity[new NextInstant(new Instant(0))] = velocity;
            obj.angularSpeed[new NextInstant(new Instant(0))] = angularVelocity;
            obj.targetAngle[new NextInstant(new Instant(0))] = targetAngle;
        }

        public Vector2 Velocity
        {
            get { return velocity[new NextInstant(new Instant(0))]; }
            protected set { velocity[new NextInstant(new Instant(0))] = value; }
        }

        public float AngularSpeed
        {
            get { return angularSpeed[new NextInstant(new Instant(0))]; }
            protected set { angularSpeed[new NextInstant(new Instant(0))] = value; }
        }

        public float TargetAngle
        {
            get { return targetAngle[new NextInstant(new Instant(0))]; }
            protected set { targetAngle[new NextInstant(new Instant(0))] = value; }
        }

        public override void Update(CurrentInstant current, NextInstant next)
        {
            this.Position = this.Position + (this.Velocity * secondsElapsed);

            if (this.TargetAngle <= Math.PI * 2 && this.TargetAngle >= 0)
            {
                float changeInAngle = (float)(secondsElapsed * this.AngularSpeed);
                this.Direction = PhysicsUtils.AngularMoveTowardBounded(this.Direction, this.TargetAngle, changeInAngle);
            }
            else
            {
                this.Direction = this.Direction + (float)(secondsElapsed * this.AngularSpeed);
            }

        }
    }
}
