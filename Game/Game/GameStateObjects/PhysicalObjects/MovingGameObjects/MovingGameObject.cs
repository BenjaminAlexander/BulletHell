using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
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

        internal override void DefineFields(InitialInstant instant)
        {
            base.DefineFields(instant);
            velocity = new Field<Vector2Value>(instant);
            angularSpeed = new Field<FloatValue>(instant);
            targetAngle = new Field<FloatValue>(instant);
        }

        public static void ServerInitialize(NextInstant next, MovingGameObject obj, Vector2 position, Vector2 velocity, float direction, float angularVelocity, float targetAngle)
        {
            CompositePhysicalObject.ServerInitialize(next, obj, position, direction);
            obj.velocity[next] = velocity;
            obj.angularSpeed[next] = angularVelocity;
            obj.targetAngle[next] = targetAngle;
        }

        public Field<Vector2Value> Velocity
        {
            get { return velocity; }
        }

        public Field<FloatValue> AngularSpeed
        {
            get { return angularSpeed; }
        }

        public Field<FloatValue> TargetAngle
        {
            get { return targetAngle; }
        }

        public override void Update(CurrentInstant current, NextInstant next)
        {
            this.Position[next] = this.Position[current] + ((Vector2)this.Velocity[current] * secondsElapsed);

            if (this.TargetAngle[current] <= Math.PI * 2 && this.TargetAngle[current] >= 0)
            {
                float changeInAngle = (float)(secondsElapsed * this.AngularSpeed[current]);
                this.Direction[next] = PhysicsUtils.AngularMoveTowardBounded(this.Direction[current], this.TargetAngle[current], changeInAngle);
            }
            else
            {
                this.Direction[next] = this.Direction[current] + (float)(secondsElapsed * this.AngularSpeed[current]);
            }

        }
    }
}
