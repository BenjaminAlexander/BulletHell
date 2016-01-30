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
using MyGame.PlayerControllers;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Bullets;
using MyGame.GameServer;
using MyGame.GameClient;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    abstract public class Ship : MovingGameObject 
    {
        private IntegerGameObjectMember health;
        private FloatGameObjectMember maxSpeed;
        private FloatGameObjectMember acceleration;
        private FloatGameObjectMember maxAgularSpeed;
        private IntegerGameObjectMember shipsKilled;
        private Vector2GameObjectMember targetVelocity;

        private ControlState controller;

        public Ship(Game1 game)
            : base(game)
        {
            health = new IntegerGameObjectMember(this, 40);
            maxSpeed = new FloatGameObjectMember(this, 300);
            acceleration = new FloatGameObjectMember(this, 300);
            maxAgularSpeed = new FloatGameObjectMember(this, 0.5f);
            shipsKilled = new IntegerGameObjectMember(this, 0);
            targetVelocity = new Vector2GameObjectMember(this, new Vector2(0));
        }

        public static void ServerInitialize(Ship ship, Vector2 position, Vector2 velocity, float direction, int health, float maxSpeed, float acceleration, float maxAgularSpeed, ControlState controller)
        {
            MovingGameObject.ServerInitialize(ship, position, new Vector2(0), direction, 0, 0);
            ship.health.Value = health;
            ship.maxSpeed.Value = maxSpeed;
            ship.acceleration.Value = acceleration;
            ship.maxAgularSpeed.Value = maxAgularSpeed;

            ship.controller = controller;
        }

        public static float MaxRadius
        {
            //TODO: can't we compute this for each texture at load time?
            get { return 600; }
        }

        public ControlState GetController()
        {
            return controller;
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

        public override void ServerOnlyUpdate(float seconds)
        {
            base.ServerOnlyUpdate(seconds);

            ControlState controller = this.GetController();
            if (controller != null)
            {
                this.TargetVelocity = Utils.Vector2Utils.ConstructVectorFromPolar(this.MaxSpeed * controller.MovementControl, this.WorldDirection());
                this.TargetAngle = controller.TargetAngle;
                this.AngularSpeed = this.MaxAgularSpeed * controller.AngleControl;
            }
        }

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);

            this.Velocity = PhysicsUtils.MoveTowardBounded(this.Velocity, this.TargetVelocity, this.Acceleration * seconds);
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

            if (this.Health <= 0)
            {
                this.Destroy();
            }
        }
    }
}
