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
using MyGame.GameServer;
using MyGame.GameClient;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects
{
    public abstract class MovingGameObject : CompositePhysicalObject
    {
        private InterpolatedVector2GameObjectMember velocity;
        private FloatGameObjectMember angularSpeed;
        private FloatGameObjectMember targetAngle;
        protected override void InitializeFields()
        {
            base.InitializeFields();

            velocity = new InterpolatedVector2GameObjectMember(this, new Vector2(0));
            angularSpeed = new FloatGameObjectMember(this, 0);
            targetAngle = new FloatGameObjectMember(this, 0);

            this.AddField(velocity);
            this.AddField(angularSpeed);
            this.AddField(targetAngle);
        }

        public MovingGameObject(ClientGame game, GameObjectUpdate message) : base(game, message) { }

        public MovingGameObject(ServerGame game, Vector2 position, Vector2 velocity, float direction, float angularVelocity, float targetAngle)
            : base(game, position, direction)
        {
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

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);

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
