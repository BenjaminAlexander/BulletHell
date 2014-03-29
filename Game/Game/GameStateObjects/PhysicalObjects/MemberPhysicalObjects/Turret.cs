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

namespace MyGame.GameStateObjects.PhysicalObjects.MemberPhysicalObjects
{
    public class Turret : MemberPhysicalObject
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Gun"), Color.White, new Vector2(13, TextureLoader.GetTexture("Gun").Texture.Height / 2), 1);
        private IController controller;

        private GameObjectReferenceListField<Gun> gunList = new GameObjectReferenceListField<Gun>(new List<GameObjectReference<Gun>>());
        internal GameObjectReferenceListField<Gun> GunList
        {
            get { return gunList;}
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            AddField(gunList);
        }

        public new class State : MemberPhysicalObject.State
        {
            private InterpolatedAngleGameObjectMember turretDirectionRelativeToSelf = new InterpolatedAngleGameObjectMember(0);
            private FloatGameObjectMember range = new FloatGameObjectMember(0);
            private FloatGameObjectMember angularSpeed = new FloatGameObjectMember(5);
            
            private Vector2GameObjectMember target = new Vector2GameObjectMember(new Vector2(1000));

            protected override void InitializeFields()
            {
                base.InitializeFields();
                AddField(turretDirectionRelativeToSelf);
                AddField(range);
                AddField(angularSpeed);
            }

            public State(GameObject obj) : base(obj) { }

            public float Range
            {
                get { return range.Value; }
                set { range.Value = value; }
            }

            public Vector2 Target
            {
                set { target.Value = value; }
                get { return target.Value; }
            }

            public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
            {
                Vector2 pos = this.GetObject<Turret>().WorldPosition();
                float dr = this.WorldDirection();
                collidable.Draw(graphics, pos, dr);
            }

            public override void UpdateState(float seconds)
            {
                base.UpdateState(seconds);

                if (Game1.IsServer)
                {
                    Turret myself = this.GetObject<Turret>();

                    IController controller = myself.GetController();
                    

                    Ship rootShip = (Ship)(myself.Root());
                    if (controller != null && rootShip != null)
                    {
                        controller.Update(seconds);

                        this.target.Value = rootShip.Position + controller.CurrentState.Aimpoint;

                        if (controller.CurrentState.Fire)
                        {
                            this.Fire();
                        }
                    }
                    TurnTowards(seconds, target.Value);
                }

                
            }

            public float WorldDirection()
            {
                PhysicalObject parent = ((PhysicalObject)(this.GetObject<Turret>().Parent));
                if (parent != null)
                {
                    return parent.WorldDirection() + turretDirectionRelativeToSelf.Value + this.GetObject<Turret>().DirectionRelativeToParent;
                }
                else
                {
                    return 0;
                }
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
                    if (Vector2Utils.ShortestAngleDistance(rValue, 0) <= range.Value)
                    {
                        turretDirectionRelativeToSelf.Value = value;
                    }
                }
            }

            private void TurnTowards(float seconds, Vector2 target)
            {
                float targetAngle = GetClosestPointAtAngleInRange(target);
                float currentAngle = Vector2Utils.MinimizeMagnitude(TurretDirectionRelativeToSelf);
                float maxAngleChange = seconds * angularSpeed.Value;

                if (range.Value >= Math.PI)
                {
                    TurretDirectionRelativeToSelf = Physics.PhysicsUtils.AngularMoveTowardBounded(currentAngle, targetAngle, maxAngleChange);
                }
                else
                {

                    if (Math.Abs(currentAngle - targetAngle) <= maxAngleChange)
                    {
                        TurretDirectionRelativeToSelf = targetAngle;
                    }
                    else if (targetAngle < currentAngle)
                    {
                        TurretDirectionRelativeToSelf = currentAngle - maxAngleChange;
                    }
                    else
                    {
                        TurretDirectionRelativeToSelf = currentAngle + maxAngleChange;
                    }
                }
            }

            public void PointAt(Vector2 target)
            {
                this.TurretDirectionRelativeToSelf = GetClosestPointAtAngleInRange(target);
            }

            public float GetClosestPointAtAngleInRange(Vector2 target)
            {
                PhysicalObject parent = ((PhysicalObject)(this.GetObject<Turret>().Parent));
                if (parent != null)
                {
                    float worldDirection = Vector2Utils.Vector2Angle(target - this.GetObject<Turret>().WorldPosition());
                    float targetAngleRelativeToParent = worldDirection - parent.WorldDirection() - this.GetObject<Turret>().DirectionRelativeToParent;
                    return MathUtils.ClosestInRange(Vector2Utils.MinimizeMagnitude(targetAngleRelativeToParent), range.Value, -range.Value);
                }
                else
                {
                    return 0;
                }
            }

            public Boolean IsPointedAt(Vector2 target, float errorDistance)
            {
                Vector2 worldPosition = this.GetObject<Turret>().WorldPosition();
                float angle = Vector2Utils.ShortestAngleDistance(Vector2Utils.Vector2Angle(target - worldPosition), this.WorldDirection());
                float distanceToTarget = Vector2.Distance(target, worldPosition);
                return Math.Abs(angle) <= Math.PI / 2 && Math.Abs((float)(Math.Sin(angle) * distanceToTarget)) < errorDistance;
            }

            public void Fire()
            {
                foreach (GameObjectReference<Gun> gunRef in this.GetObject<Turret>().GunList.Value)
                {
                    Gun gun = gunRef.Dereference();
                    if (gun != null && this.IsPointedAt(target.Value, 50))
                    {
                        gun.Fire();
                    }
                }
            }
        }

        public override void Add(MemberPhysicalObject obj)
        {
            base.Add(obj);
            if (obj is Gun)
            {
                gunList.Value.Add(new GameObjectReference<Gun>((Gun)obj));
            }
        }

        public Turret(GameObjectUpdate message) : base(message) { }

        public Turret(PhysicalObject parent, Vector2 position, float direction, float range, IController controller)
            : base(parent, position, direction)
        {
            Turret.State myState = this.PracticalState<Turret.State>();
            myState.Range = range;

            this.controller = controller;

            Gun gun = new Gun(this, new Vector2(37, 0), 0);
            StaticGameObjectCollection.Collection.Add(gun);
        }

        protected override GameObject.State BlankState(GameObject obj)
        {
            return new Turret.State(obj);
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
                return parent.WorldDirection() + this.PracticalState<Turret.State>().TurretDirectionRelativeToSelf + this.DirectionRelativeToParent;
            }
            else
            {
                return 0;
            }
        }
    }
}
