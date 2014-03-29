using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.IO;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.Geometry;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.Networking;
using MyGame.PlayerControllers;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Bullets;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    abstract public class Ship : MovingGameObject 
    {
        public static float MaxRadius
        {
            get { return 600; }
        }
        private IController controller;

        private IntegerGameObjectMember health;
        private FloatGameObjectMember maxSpeed;
        private FloatGameObjectMember acceleration;
        private FloatGameObjectMember maxAgularSpeed;
        private IntegerGameObjectMember shipsKilled;
        private Vector2GameObjectMember targetVelocity;

        protected override void InitializeFields()
        {
            base.InitializeFields();

            health = new IntegerGameObjectMember(this, 40);
            maxSpeed = new FloatGameObjectMember(this, 300);
            acceleration = new FloatGameObjectMember(this, 300);
            maxAgularSpeed = new FloatGameObjectMember(this, 0.5f);
            shipsKilled = new IntegerGameObjectMember(this, 0);
            targetVelocity = new Vector2GameObjectMember(this, new Vector2(0));

            this.AddField(health);
            this.AddField(maxSpeed);
            this.AddField(acceleration);
            this.AddField(maxAgularSpeed);
            this.AddField(shipsKilled);
            this.AddField(targetVelocity);
        }

        public IController GetController()
        {
            return controller;
        }

        public Ship(GameObjectUpdate message) : base(message) { }

        public Ship(Vector2 position, Vector2 velocity, int health, float maxSpeed, float acceleration, float maxAgularSpeed,  IController controller)
            : base(position, new Vector2(0), 0, 0, 0)
        {
            this.health.Value = health;
            this.maxSpeed.Value = maxSpeed;
            this.acceleration.Value = acceleration;
            this.maxAgularSpeed.Value = maxAgularSpeed;

            this.controller = controller;

            if (this.controller != null)
            {
                this.controller.Focus = this;
            }
        }

        public int Health
        {
            protected set { health.Value = value; }
            get { return health.Value; }
        }

        public float MaxSpeed
        {
            get { return maxSpeed.Value; }
        }

        public float Acceleration
        {
            get { return acceleration.Value; }
        }

        public float MaxAgularSpeed
        {
            get { return maxAgularSpeed.Value; }
        }

        public Vector2 TargetVelocity
        {
            protected set { targetVelocity.Value = value; }
            get { return targetVelocity.Value; }
        }

        public void AddKill()
        {
            this.shipsKilled.Value++;
        }

        public int Kills()
        {
            return this.shipsKilled.Value;
        }

        public override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
        {
            Velocity = new Vector2(0);
        }

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);
            if (Game1.IsServer)
            {
                IController controller = this.GetController();
                controller.Update(seconds);

                //this.Velocity = this.Velocity + controller.CurrentState.Move * 10;
                if (controller != null)
                {
                    this.TargetVelocity = Utils.Vector2Utils.ConstructVectorFromPolar(this.MaxSpeed * controller.CurrentState.MovementControl, this.WorldDirection());
                    this.TargetAngle = controller.CurrentState.TargetAngle;
                    this.AngularSpeed = this.MaxAgularSpeed * controller.CurrentState.AngleControl;
                }

                foreach (GameObject obj in StaticGameObjectCollection.Collection.Tree.GetObjectsInCircle(this.WorldPosition(), Ship.MaxRadius + Bullet.MaxRadius))
                {
                    if (obj is Bullet)
                    {
                        Bullet bullet = (Bullet)obj;
                        if (!bullet.BelongsTo(this) && this.CollidesWith(bullet))
                        {
                            //if(thisShip is SmallShip)
                            this.Health = this.Health - bullet.Damage;
                            if (this.Health <= 0)
                            {
                                bullet.Hit();
                            }
                            bullet.Destroy();
                        }
                    }
                }


            }

            this.Velocity = Physics.PhysicsUtils.MoveTowardBounded(this.Velocity, this.TargetVelocity, this.Acceleration * seconds);
            if (this.Health <= 0)
            {
                this.Destroy();
            }
        }
    }
}
