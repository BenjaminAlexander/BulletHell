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

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects
{
    public abstract class MovingGameObject : CompositePhysicalObject
    {
        private InterpolatedVector2GameObjectMember velocity;
        private FloatGameObjectMember angularSpeed;
        private FloatGameObjectMember targetAngle;

        public MovingGameObject()
        {
            velocity = new InterpolatedVector2GameObjectMember(this, new Vector2(0));
            angularSpeed = new FloatGameObjectMember(this, 0);
            targetAngle = new FloatGameObjectMember(this, 0);
        }

        public MovingGameObject(Game1 game)
            : base(game)
        {
            velocity = new InterpolatedVector2GameObjectMember(this, new Vector2(0));
            angularSpeed = new FloatGameObjectMember(this, 0);
            targetAngle = new FloatGameObjectMember(this, 0);
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
            base.Update(current, next);

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
