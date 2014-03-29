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

        public MovingGameObject(GameObjectUpdate message) : base(message) { }

        public MovingGameObject(Vector2 position, Vector2 velocity, float direction, float angularVelocity, float targetAngle)
            : base(position, direction)
        {
            MovingGameObject.State myState = this.PracticalState<MovingGameObject.State>();
            this.velocity.Value = velocity;
            this.angularSpeed.Value = angularVelocity;
            this.targetAngle.Value = targetAngle;
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

        public override void UpdateSub(float seconds)
        {
            base.UpdateSub(seconds);

            this.Position = this.Position + (this.Velocity * seconds);

            if (this.TargetAngle <= Math.PI * 2 && this.TargetAngle >= 0)
            {
                float changeInAngle = (float)(seconds * this.AngularSpeed);
                this.Direction = Physics.PhysicsUtils.AngularMoveTowardBounded(this.Direction, this.TargetAngle, changeInAngle);
            }
            else
            {
                this.Direction = this.Direction + (float)(seconds * this.AngularSpeed);
            }

        }
    }
}
