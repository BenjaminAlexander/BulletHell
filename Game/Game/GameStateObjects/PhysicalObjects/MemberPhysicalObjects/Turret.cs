using System;
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

namespace MyGame.GameStateObjects.PhysicalObjects.MemberPhysicalObjects
{
    public class Turret : MemberPhysicalObject
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Gun"), Color.White, new Vector2(13, TextureLoader.GetTexture("Gun").Texture.Height / 2), 1);
        private ControlState controller;

        private GameObjectReferenceListField<Gun> gunList;
        private InterpolatedAngleGameObjectMember turretDirectionRelativeToSelf;
        private FloatGameObjectMember range;
        private FloatGameObjectMember angularSpeed;
        private Vector2GameObjectMember target;

        internal GameObjectReferenceListField<Gun> GunList
        {
            get { return gunList;}
        }

        public Turret()
        {
            gunList = new GameObjectReferenceListField<Gun>(this); ;
            turretDirectionRelativeToSelf = new InterpolatedAngleGameObjectMember(this, 0);
            range = new FloatGameObjectMember(this, 0);
            angularSpeed = new FloatGameObjectMember(this, 50);
            target = new Vector2GameObjectMember(this, new Vector2(1000));
        }

        public Turret(Game1 game)
            : base(game)
        {
            gunList = new GameObjectReferenceListField<Gun>(this); ;
            turretDirectionRelativeToSelf = new InterpolatedAngleGameObjectMember(this, 0);
            range = new FloatGameObjectMember(this, 0);
            angularSpeed = new FloatGameObjectMember(this, 50);
            target = new Vector2GameObjectMember(this, new Vector2(1000));
        }

        public override void Add(MemberPhysicalObject obj)
        {
            base.Add(obj);
            if (obj is Gun)
            {
                gunList.Value.Add((Gun)obj);
            }
        }

        public static void ServerInitialize(Turret obj, PhysicalObject parent, Vector2 position, float direction, float range, ControlState controller)
        {
            MemberPhysicalObject.ServerInitialize(obj, parent, position, direction);
            obj.Range = range;

            obj.controller = controller;

            Gun gun = new Gun(obj.Game);
            Gun.ServerInitialize(gun, obj, new Vector2(37, 0), 0);
            obj.Game.GameObjectCollection.Add(gun);
        }

        public ControlState GetController()
        {
            return controller;
        }

        public override float WorldDirection()
        {
            PhysicalObject parent = ((PhysicalObject)(this.Parent));
            if (parent != null)
            {
                return parent.WorldDirection() + this.TurretDirectionRelativeToSelf + this.DirectionRelativeToParent;
            }
            else
            {
                return 0;
            }
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);
            Vector2 pos = this.WorldPosition();
            float dr = this.WorldDirection();
            collidable.Draw(graphics, pos, dr);
        }

        public override void Update(CurrentInstant current, NextInstant next)
        {
            base.Update(current, next);
            ControlState controller = this.GetController();

            Ship rootShip = (Ship)(this.Root());
            if (controller != null && rootShip != null)
            {
                this.Target = controller.Aimpoint + rootShip.Position;
            }
            //TODO: we need to standardize how controller ultimatly effect the game
            this.TurnTowards(secondsElapsed, this.Target);
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

        public Boolean IsPointedAt(Vector2 target, float errorDistance)
        {
            Vector2 worldPosition = this.WorldPosition();
            float angle = Vector2Utils.ShortestAngleDistance(Vector2Utils.Vector2Angle(target - worldPosition), this.WorldDirection());
            float distanceToTarget = Vector2.Distance(target, worldPosition);
            return Math.Abs(angle) <= Math.PI / 2 && Math.Abs((float)(Math.Sin(angle) * distanceToTarget)) < errorDistance;
        }

        public float GetClosestPointAtAngleInRange(Vector2 target)
        {
            PhysicalObject parent = ((PhysicalObject)(this.Parent));
            if (parent != null)
            {
                float worldDirection = Vector2Utils.Vector2Angle(target - this.WorldPosition());
                float targetAngleRelativeToParent = worldDirection - parent.WorldDirection() - this.DirectionRelativeToParent;
                return MathUtils.ClosestInRange(Vector2Utils.MinimizeMagnitude(targetAngleRelativeToParent), this.Range, -this.Range);
            }
            else
            {
                return 0;
            }
        }

        private void TurnTowards(float seconds, Vector2 target)
        {
            float targetAngle = this.GetClosestPointAtAngleInRange(target);
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
