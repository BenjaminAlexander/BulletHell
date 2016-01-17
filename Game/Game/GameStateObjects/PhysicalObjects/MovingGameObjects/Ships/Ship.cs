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
using MyGame.GameServer;
using MyGame.GameClient;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    abstract public class Ship : MovingGameObject 
    {
        public static float MaxRadius
        {
            get { return 600; }
        }
        private ControlState controller;

        private IntegerGameObjectMember health;
        private FloatGameObjectMember maxSpeed;
        private FloatGameObjectMember acceleration;
        private FloatGameObjectMember maxAgularSpeed;
        private IntegerGameObjectMember shipsKilled;
        private Vector2GameObjectMember targetVelocity;

        protected override void InitializeFields()
        {
            base.InitializeFields();

            health = this.AddIntegerGameObjectMember(40);
            maxSpeed = this.AddFloatGameObjectMember(300);
            acceleration = this.AddFloatGameObjectMember(300);
            maxAgularSpeed = this.AddFloatGameObjectMember(0.5f);
            shipsKilled = this.AddIntegerGameObjectMember(0);
            targetVelocity = this.AddVector2GameObjectMember(new Vector2(0));
        }

        public ControlState GetController()
        {
            return controller;
        }

        public void ShipInit(ServerGame game, Vector2 position, Vector2 velocity, int health, float maxSpeed, float acceleration, float maxAgularSpeed, ControlState controller)
        {
            this.ShipInit(game, position, velocity, 0, health, maxSpeed, acceleration, maxAgularSpeed, controller);
        }

        public void ShipInit(ServerGame game, Vector2 position, Vector2 velocity, float direction, int health, float maxSpeed, float acceleration, float maxAgularSpeed, ControlState controller)
        {
            base.MovingGameObjectInit(game, position, new Vector2(0), direction, 0, 0);
            this.health.Value = health;
            this.maxSpeed.Value = maxSpeed;
            this.acceleration.Value = acceleration;
            this.maxAgularSpeed.Value = maxAgularSpeed;

            this.controller = controller;
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
            this.Velocity = Physics.PhysicsUtils.MoveTowardBounded(this.Velocity, this.TargetVelocity, this.Acceleration * seconds);
            if (this.Health <= 0)
            {
                this.Destroy();
            }
        }

        public override void SimulationStateOnlyUpdate(float seconds)
        {
            base.SimulationStateOnlyUpdate(seconds);
            foreach (GameObject obj in this.Game.GameObjectCollection.Tree.GetObjectsInCircle(this.WorldPosition(), Ship.MaxRadius + Bullet.MaxRadius))
            {
                if (obj is Bullet)
                {
                    Bullet bullet = (Bullet)obj;
                    if (!bullet.BelongsTo(this) && this.CollidesWith(bullet))
                    {
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

        public override void ServerOnlyUpdate(float seconds)
        {
            base.ServerOnlyUpdate(seconds);
            ControlState controller = this.GetController();

            //this.Velocity = this.Velocity + controller.CurrentState.Move * 10;
            if (controller != null)
            {
                this.TargetVelocity = Utils.Vector2Utils.ConstructVectorFromPolar(this.MaxSpeed * controller.MovementControl, this.WorldDirection());
                this.TargetAngle = controller.TargetAngle;
                this.AngularSpeed = this.MaxAgularSpeed * controller.AngleControl;
            }
        }
    }
}
