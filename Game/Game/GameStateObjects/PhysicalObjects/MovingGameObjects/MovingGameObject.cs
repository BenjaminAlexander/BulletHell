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

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects
{
    public abstract class MovingGameObject : CompositePhysicalObject
    {

        public abstract new class State : CompositePhysicalObject.State
        {
            private Vector2GameObjectMember velocity = new Vector2GameObjectMember(new Vector2(0));
            private FloatGameObjectMember angularSpeed = new FloatGameObjectMember(0);
            private FloatGameObjectMember targetAngle = new FloatGameObjectMember(0);

            protected override void InitializeFields()
            {
                base.InitializeFields();
                this.AddField(velocity);
                this.AddField(angularSpeed);
                this.AddField(targetAngle);
            }

            public State(GameObject obj) : base(obj) {}

            public void Initialize(Vector2 velocity, float angularVelocity, float targetAngle)
            {
                Game1.AsserIsServer();
                this.velocity.Value = velocity;
                this.angularSpeed.Value = angularVelocity;
                this.targetAngle.Value = targetAngle;
            }

            public override void UpdateState(float seconds)
            {
                base.UpdateState(seconds);
                this.Position = this.Position + (this.velocity.Value * seconds);

                if (targetAngle.Value <= Math.PI * 2 && targetAngle.Value >= 0)
                {
                    float changeInAngle = (float)(seconds * angularSpeed.Value);
                    Direction = Physics.PhysicsUtils.AngularMoveTowardBounded(Direction, targetAngle.Value, changeInAngle);
                }
                else
                {
                    Direction = Direction + (float)(seconds * angularSpeed.Value);
                }
            }

            public Vector2 Velocity
            {
                get { return velocity.Value; }
                protected set { velocity.Value = value; }
            }

            public float AngularSpeed
            {
                get { return angularSpeed.Value; }
                protected set { angularSpeed.Value = value; }
            }

            public float TargetAngle
            {
                get { return targetAngle.Value; }
                protected set { targetAngle.Value = value; }
            }
        }

        public MovingGameObject(GameObjectUpdate message) : base(message) { }

        public MovingGameObject(Vector2 position, Vector2 velocity, float direction, float angularVelocity, float targetAngle)
            : base(position, direction)
        {
            MovingGameObject.State myState = this.PracticalState<MovingGameObject.State>();
            myState.Initialize(velocity, angularVelocity, targetAngle);
        }
    }
}
