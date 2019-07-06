﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.PlayerControllers;
using MyGame.GameStateObjects.PhysicalObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameServer;
using MyGame.GameClient;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.GameState;
using MyGame.Engine.GameState.FieldValues;

namespace MyGame.GameStateObjects.PhysicalObjects.MemberPhysicalObjects
{
    public class Turret : MemberPhysicalObject
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Gun"), Color.White, new Vector2(13, TextureLoader.GetTexture("Gun").Texture.Height / 2), 1);
        private ControlState controller;

        private Field<FloatValue> turretDirectionRelativeToSelf;
        private Field<FloatValue> range;
        private Field<FloatValue> angularSpeed;
        private Field<Vector2Value> target;

        public Turret()
        {
        }

        public Turret(Game1 game)
            : base(game)
        {
        }

        internal override void DefineFields(InitialInstant instant)
        {
            base.DefineFields(instant);
            turretDirectionRelativeToSelf = new Field<FloatValue>(instant);
            target = new Field<Vector2Value>(instant);
            range = new Field<FloatValue>(instant);
            angularSpeed = new Field<FloatValue>(instant);

            target[new NextInstant(new Instant(0))] = new Vector2(1000);
            angularSpeed[new NextInstant(new Instant(0))] = 50;
        }

        public static void ServerInitialize(Turret obj, PhysicalObject parent, Vector2 position, float direction, float range, ControlState controller)
        {
            MemberPhysicalObject.ServerInitialize(obj, parent, position, direction);
            obj.Range = range;

            obj.controller = controller;
        }

        public ControlState GetController()
        {
            return controller;
        }

        public override float WorldDirection(CurrentInstant current)
        {
            PhysicalObject parent = ((PhysicalObject)(this.Parent));
            if (parent != null)
            {
                return parent.WorldDirection(current) + this.TurretDirectionRelativeToSelf + this.DirectionRelativeToParent;
            }
            else
            {
                return 0;
            }
        }

        public override void Draw(CurrentInstant current, MyGraphicsClass graphics)
        {
            base.Draw(current, graphics);
            Vector2 pos = this.WorldPosition(current);
            float dr = this.WorldDirection(current);
            collidable.Draw(graphics, pos, dr);
        }

        public override void Update(CurrentInstant current, NextInstant next)
        {
            ControlState controller = this.GetController();

            Ship rootShip = (Ship)(this.Root());
            if (controller != null && rootShip != null)
            {
                this.Target = controller.Aimpoint + rootShip.Position[current];
            }
            //TODO: we need to standardize how controller ultimatly effect the game
            this.TurnTowards(current, secondsElapsed, this.Target);
        }

        public float TurretDirectionRelativeToSelf
        {
            get
            {
                return turretDirectionRelativeToSelf[new NextInstant(new Instant(0))];
            }
            protected set
            {
                float rValue = Vector2Utils.RestrictAngle(value);
                if (Vector2Utils.ShortestAngleDistance(rValue, 0) <= this.Range)
                {
                    turretDirectionRelativeToSelf[new NextInstant(new Instant(0))] = value;
                }
            }
        }

        public float Range
        {
            get { return range[new NextInstant(new Instant(0))]; }
            set { range[new NextInstant(new Instant(0))] = value; }
        }

        public float AngularSpeed
        {
            get { return angularSpeed[new NextInstant(new Instant(0))]; }
        }

        public Vector2 Target
        {
            set { target[new NextInstant(new Instant(0))] = value; }
            get { return target[new NextInstant(new Instant(0))]; }
        }

        public Boolean IsPointedAt(CurrentInstant current, Vector2 target, float errorDistance)
        {
            Vector2 worldPosition = this.WorldPosition(current);
            float angle = Vector2Utils.ShortestAngleDistance(Vector2Utils.Vector2Angle(target - worldPosition), this.WorldDirection(current));
            float distanceToTarget = Vector2.Distance(target, worldPosition);
            return Math.Abs(angle) <= Math.PI / 2 && Math.Abs((float)(Math.Sin(angle) * distanceToTarget)) < errorDistance;
        }

        public float GetClosestPointAtAngleInRange(CurrentInstant current, Vector2 target)
        {
            PhysicalObject parent = ((PhysicalObject)(this.Parent));
            if (parent != null)
            {
                float worldDirection = Vector2Utils.Vector2Angle(target - this.WorldPosition(current));
                float targetAngleRelativeToParent = worldDirection - parent.WorldDirection(current) - this.DirectionRelativeToParent;
                return MathUtils.ClosestInRange(Vector2Utils.MinimizeMagnitude(targetAngleRelativeToParent), this.Range, -this.Range);
            }
            else
            {
                return 0;
            }
        }

        private void TurnTowards(CurrentInstant current, float seconds, Vector2 target)
        {
            float targetAngle = this.GetClosestPointAtAngleInRange(current, target);
            float currentAngle = Vector2Utils.MinimizeMagnitude(this.TurretDirectionRelativeToSelf);
            float maxAngleChange = seconds * this.AngularSpeed;

            if (this.Range >= Math.PI)
            {
                this.TurretDirectionRelativeToSelf = PhysicsUtils.AngularMoveTowardBounded(currentAngle, targetAngle, maxAngleChange);
            }
            else
            {

                if (Math.Abs(currentAngle - targetAngle) <= maxAngleChange)
                {
                    this.TurretDirectionRelativeToSelf = targetAngle;
                }
                else if (targetAngle < currentAngle)
                {
                    this.TurretDirectionRelativeToSelf = currentAngle - maxAngleChange;
                }
                else
                {
                    this.TurretDirectionRelativeToSelf = currentAngle + maxAngleChange;
                }
            }
        }

    }
}
