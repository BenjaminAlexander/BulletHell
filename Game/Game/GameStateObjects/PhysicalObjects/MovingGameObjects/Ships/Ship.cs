using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.PlayerControllers;
using MyGame.GameServer;
using MyGame.GameClient;
using MyGame.Engine.GameState.Instants;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    abstract public class Ship : MovingGameObject 
    {
        //TODO: make these fields
        private FloatGameObjectMember maxSpeed;
        private FloatGameObjectMember acceleration;
        private FloatGameObjectMember maxAgularSpeed;
        private IntegerGameObjectMember shipsKilled;
        private Vector2GameObjectMember targetVelocity;

        private ControlState controller;

        public Ship()
        {
            maxSpeed = new FloatGameObjectMember(this, 300);
            acceleration = new FloatGameObjectMember(this, 300);
            maxAgularSpeed = new FloatGameObjectMember(this, 0.5f);
            shipsKilled = new IntegerGameObjectMember(this, 0);
            targetVelocity = new Vector2GameObjectMember(this, new Vector2(0));
        }

        public Ship(Game1 game)
            : base(game)
        {
            maxSpeed = new FloatGameObjectMember(this, 300);
            acceleration = new FloatGameObjectMember(this, 300);
            maxAgularSpeed = new FloatGameObjectMember(this, 0.5f);
            shipsKilled = new IntegerGameObjectMember(this, 0);
            targetVelocity = new Vector2GameObjectMember(this, new Vector2(0));
        }

        public static void ServerInitialize(Ship ship, Vector2 position, Vector2 velocity, float direction, int health, float maxSpeed, float acceleration, float maxAgularSpeed, ControlState controller)
        {
            MovingGameObject.ServerInitialize(ship, position, new Vector2(0), direction, 0, 0);
            ship.maxSpeed[new NextInstant(new Instant(0))] = maxSpeed;
            ship.acceleration[new NextInstant(new Instant(0))] = acceleration;
            ship.maxAgularSpeed[new NextInstant(new Instant(0))] = maxAgularSpeed;

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

        public float MaxSpeed
        {
            get { return maxSpeed[new NextInstant(new Instant(0))]; }
        }

        public float Acceleration
        {
            get { return acceleration[new NextInstant(new Instant(0))]; }
        }

        public float MaxAgularSpeed
        {
            get { return maxAgularSpeed[new NextInstant(new Instant(0))]; }
        }

        public Vector2 TargetVelocity
        {
            protected set { targetVelocity[new NextInstant(new Instant(0))] = value; }
            get { return targetVelocity[new NextInstant(new Instant(0))]; }
        }

        public void AddKill()
        {
            this.shipsKilled[new NextInstant(new Instant(0))] = this.shipsKilled[new NextInstant(new Instant(0))] + 1;
        }

        public int Kills()
        {
            return this.shipsKilled[new NextInstant(new Instant(0))];
        }

        public override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
        {
            Velocity = new Vector2(0);
        }

        public override void Update(CurrentInstant current, NextInstant next)
        {
            base.Update(current, next);
            ControlState controller = this.GetController();
            if (controller != null)
            {
                this.TargetVelocity = Utils.Vector2Utils.ConstructVectorFromPolar(this.MaxSpeed * controller.MovementControl, this.WorldDirection());
                this.TargetAngle = controller.TargetAngle;
                this.AngularSpeed = this.MaxAgularSpeed * controller.AngleControl;
            }
            this.Velocity = PhysicsUtils.MoveTowardBounded(this.Velocity, this.TargetVelocity, this.Acceleration * secondsElapsed);
        }
    }
}
