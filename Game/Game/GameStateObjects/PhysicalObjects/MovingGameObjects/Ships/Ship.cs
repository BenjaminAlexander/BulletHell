using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.PlayerControllers;
using MyGame.GameServer;
using MyGame.GameClient;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.GameState;
using MyGame.Engine.GameState.FieldValues;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    abstract public class Ship : MovingGameObject 
    {
        //TODO: make these fields
        private Field<FloatValue> maxSpeed;
        private Field<FloatValue> acceleration;
        private Field<FloatValue> maxAgularSpeed;
        private Field<IntegerValue> shipsKilled;
        private Field<Vector2Value> targetVelocity;

        private ControlState controller;

        internal override void DefineFields(InitialInstant instant)
        {
            base.DefineFields(instant);
            targetVelocity = new Field<Vector2Value>(instant);
            maxSpeed = new Field<FloatValue>(instant);
            acceleration = new Field<FloatValue>(instant);
            maxAgularSpeed = new Field<FloatValue>(instant);
            shipsKilled = new Field<IntegerValue>(instant);

            //maxSpeed[new NextInstant(new Instant(0))] = 300;
            //acceleration[new NextInstant(new Instant(0))] = 300;
            //maxAgularSpeed[new NextInstant(new Instant(0))] = 0.5f;
        }

        public static void ServerInitialize(NextInstant next, Ship ship, Vector2 position, Vector2 velocity, float direction, int health, float maxSpeed, float acceleration, float maxAgularSpeed, ControlState controller)
        {
            MovingGameObject.ServerInitialize(next, ship, position, new Vector2(0), direction, 0, 0);
            ship.maxSpeed[next] = maxSpeed;
            ship.acceleration[next] = acceleration;
            ship.maxAgularSpeed[next] = maxAgularSpeed;

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

        public override void Update(CurrentInstant current, NextInstant next)
        {
            base.Update(current, next);
            ControlState controller = this.GetController();
            if (controller != null)
            {
                targetVelocity[next] = Utils.Vector2Utils.ConstructVectorFromPolar(maxSpeed[current] * controller.MovementControl, this.WorldDirection(current));
                this.TargetAngle[next] = controller.TargetAngle;
                this.AngularSpeed[next] = maxAgularSpeed[current] * controller.AngleControl;
            }
            this.Velocity[next] = PhysicsUtils.MoveTowardBounded(this.Velocity[current], targetVelocity[current], acceleration[current] * secondsElapsed);
        }
    }
}
