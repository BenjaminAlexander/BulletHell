using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.Geometry;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public abstract class MovingGameObject : CompositePhysicalObject
    {

        public abstract new class State : CompositePhysicalObject.State
        {
            private Vector2 velocity = new Vector2(0);

            private float angularSpeed = 0;
            private float targetAngle = 0;

            public State(GameObject obj) : base(obj) {}

            public void Initialize(Vector2 velocity, float angularVelocity, float targetAngle)
            {
                Game1.AssertIsServer();
                this.velocity = velocity;
                this.angularSpeed = angularVelocity;
                this.targetAngle = targetAngle;
            }

            public override void ApplyMessage(GameObjectUpdate message)
            {
                base.ApplyMessage(message);
                velocity = message.ReadVector2();
                angularSpeed = message.ReadFloat();
                targetAngle = message.ReadFloat();
            }

            public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
            {
                message = base.ConstructMessage(message);
                message.Append(velocity);
                message.Append(angularSpeed);
                message.Append(targetAngle);
                return message;
            }

            public override void UpdateState(float seconds)
            {
                base.UpdateState(seconds);
                Position = Position + (velocity * seconds);

                float changeInAngle = seconds * angularSpeed;
                Direction = Physics.PhysicsUtils.AngularMoveTowardBounded(Direction, targetAngle, changeInAngle);
            }

            public override void Interpolate(GameObject.State d, GameObject.State s, float smoothing, GameObject.State blankState)
            {
                base.Interpolate(d, s, smoothing, blankState);
                State myS = (State)s;
                State myBlankState = (State)blankState;

                myBlankState.velocity = myS.velocity;
                myBlankState.targetAngle = myS.targetAngle;
                myBlankState.angularSpeed = myS.angularSpeed;
            }

            public Vector2 Velocity
            {
                get { return velocity; }
                protected set { velocity = value; }
            }

            public float AngularSpeed
            {
                get { return angularSpeed; }
                protected set { angularSpeed = value; }
            }

            public float TargetAngle
            {
                get { return targetAngle; }
                protected set { targetAngle = value; }
            }
        }

        public MovingGameObject(GameObjectUpdate message) : base(message) { }

        public MovingGameObject(Vector2 position, Vector2 velocity, float direction, float angularVelocity, float targetAngle)
            : base(position, direction)
        {
            State myState = (State)GetState();
            myState.Initialize(velocity, angularVelocity, targetAngle);
        }
    }
}
