using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.IO;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.Networking;
using MyGame.PlayerControllers;
using MyGame.GameStateObjects.PhysicalObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameServer;
using MyGame.GameClient;

namespace MyGame.GameStateObjects.PhysicalObjects.MemberPhysicalObjects
{
    public class Turret : MemberPhysicalObject
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Gun"), Color.White, new Vector2(13, TextureLoader.GetTexture("Gun").Texture.Height / 2), 1);
        private IController controller;

        private GameObjectReferenceListField<Gun> gunList;
        private InterpolatedAngleGameObjectMember turretDirectionRelativeToSelf;
        private FloatGameObjectMember range;
        private FloatGameObjectMember angularSpeed;
        private Vector2GameObjectMember target;

        internal GameObjectReferenceListField<Gun> GunList
        {
            get { return gunList;}
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            gunList = new GameObjectReferenceListField<Gun>(this, new List<GameObjectReference<Gun>>(), this.Game.GameObjectCollection);
            turretDirectionRelativeToSelf = new InterpolatedAngleGameObjectMember(this, 0);
            range = new FloatGameObjectMember(this, 0);
            angularSpeed = new FloatGameObjectMember(this, 50);
            target = new Vector2GameObjectMember(this, new Vector2(1000));

            AddField(gunList);
            AddField(turretDirectionRelativeToSelf);
            AddField(range);
            AddField(angularSpeed);
            AddField(target);
        }

        public override void Add(MemberPhysicalObject obj)
        {
            base.Add(obj);
            if (obj is Gun)
            {
                gunList.Value.Add(new GameObjectReference<Gun>((Gun)obj, this.Game.GameObjectCollection));
            }
        }

        public Turret(ClientGame game, GameObjectUpdate message) : base(game, message) { }

        public Turret(ServerGame game, PhysicalObject parent, Vector2 position, float direction, float range, IController controller)
            : base(game, parent, position, direction)
        {
            this.Range = range;

            this.controller = controller;

            Gun gun = new Gun(game, this, new Vector2(37, 0), 0);
            game.GameObjectCollection.Add(gun);
        }

        public IController GetController()
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

        public override void DrawSub(GameTime gameTime, MyGraphicsClass graphics)
        {
            base.DrawSub(gameTime, graphics);
            Vector2 pos = this.WorldPosition();
            float dr = this.WorldDirection();
            collidable.Draw(graphics, pos, dr);
        }

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);
        }

        public override void ServerOnlyUpdate(float seconds)
        {
            base.ServerOnlyUpdate(seconds);
            IController controller = this.GetController();

            Ship rootShip = (Ship)(this.Root());
            if (controller != null && rootShip != null)
            {
                controller.Update(seconds);

                this.Target = rootShip.Position + controller.CurrentState.Aimpoint;

                if (controller.CurrentState.Fire)
                {
                    this.Fire();
                }
            }
            this.TurnTowards(seconds, this.Target);
        }

        public float TurretDirectionRelativeToSelf
        {
            get
            {
                return turretDirectionRelativeToSelf.Value;
            }
            protected set
            {
                float rValue = Vector2Utils.RestrictAngle(value);
                if (Vector2Utils.ShortestAngleDistance(rValue, 0) <= this.Range)
                {
                    turretDirectionRelativeToSelf.Value = value;
                }
            }
        }

        public float Range
        {
            get { return range.Value; }
            set { range.Value = value; }
        }

        public float AngularSpeed
        {
            get { return angularSpeed.Value; }
        }

        public Vector2 Target
        {
            set { target.Value = value; }
            get { return target.Value; }
        }

        public void Fire()
        {
            foreach (GameObjectReference<Gun> gunRef in this.GunList.Value)
            {
                Gun gun = gunRef.Dereference();
                if (gun != null && this.IsPointedAt(this.Target, 50))
                {
                    gun.Fire();
                }
            }
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
                this.TurretDirectionRelativeToSelf = Physics.PhysicsUtils.AngularMoveTowardBounded(currentAngle, targetAngle, maxAngleChange);
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
