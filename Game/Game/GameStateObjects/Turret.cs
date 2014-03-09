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

namespace MyGame.GameStateObjects
{
    public class Turret : MemberPhysicalObject
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Gun"), Color.White, new Vector2(0, TextureLoader.GetTexture("Gun").Texture.Height / 2), 1);
        private NetworkPlayerController controller;

        public new class State : MemberPhysicalObject.State
        {
            private float turretDirectionRelativeToSelf = 0;
            private float range;
            private float angularSpeed = 2;
            private List<GameObjectReference<Gun>> gunList = new List<GameObjectReference<Gun>>();
            private Vector2 target = new Vector2(1000);

            public State(GameObject obj) : base(obj) { }

            public override void ApplyMessage(GameObjectUpdate message)
            {
                base.ApplyMessage(message);
                turretDirectionRelativeToSelf = message.ReadFloat();
                range = message.ReadFloat();
                angularSpeed = message.ReadFloat();
                gunList = message.ReadGameObjectReferenceList<Gun>();
                target = message.ReadVector2();
            }

            public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
            {
                message = base.ConstructMessage(message);
                message.Append(turretDirectionRelativeToSelf);
                message.Append(range);
                message.Append(angularSpeed);
                message.Append(gunList);
                message.Append(target);
                return message;
            }

            public override void Interpolate(GameObject.State d, GameObject.State s, float smoothing, GameObject.State blankState)
            {
                base.Interpolate(d, s, smoothing, blankState);
                Turret.State myS = (Turret.State)s;
                Turret.State myD = (Turret.State)d;
                Turret.State myBlankState = (Turret.State)blankState;

                float direction = Utils.Vector2Utils.MinimizeMagnitude(Utils.Vector2Utils.Lerp(myS.turretDirectionRelativeToSelf, myD.turretDirectionRelativeToSelf, smoothing));

                myBlankState.turretDirectionRelativeToSelf = direction;
                myBlankState.range = myS.range;
                myBlankState.angularSpeed = myS.angularSpeed;
                myBlankState.gunList = myS.gunList;
                myBlankState.target = myS.target;
            }

            public override void Add(MemberPhysicalObject obj)
            {
                base.Add(obj);
                if (obj is Gun)
                {
                    gunList.Add(new GameObjectReference<Gun>((Gun)obj));
                }
            }

            public float Range
            {
                get { return range; }
                set { range = value; }
            }

            public Vector2 Target
            {
                set { target = value; }
                get { return target; }
            }

            public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
            {
                Vector2 pos = this.WorldPosition();
                float dr = this.WorldDirection();
                collidable.Draw(graphics, pos, dr);
            }

            public override void UpdateState(float seconds)
            {
                base.UpdateState(seconds);

                if (Game1.IsServer)
                {
                    Turret myself = (Turret)this.Object;

                    NetworkPlayerController controller = myself.GetController();

                    Ship rootShip = (Ship)(myself.Root());
                    if (controller != null)
                    {
                        this.target = rootShip.Position + controller.CurrentState.Aimpoint;
                        Console.WriteLine(controller.CurrentState.Aimpoint.ToString());

                        if (controller.CurrentState.Fire)
                        {
                            this.Fire();
                        }
                    }
                    TurnTowards(seconds, target);
                }

                
            }

            public override void ServerUpdate(float seconds)
            {
                base.ServerUpdate(seconds);
                
            }

            public override float WorldDirection()
            {
                PhysicalObject parent = ((PhysicalObject)(this.Parent));
                if (parent != null)
                {
                    return ((PhysicalObject.State)(parent.PracticalState)).WorldDirection() + turretDirectionRelativeToSelf + base.DirectionRelativeToParent;
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
                    return turretDirectionRelativeToSelf;
                }
                protected set
                {
                    float rValue = Vector2Utils.RestrictAngle(value);
                    if (Vector2Utils.ShortestAngleDistance(rValue, 0) <= range)
                    {
                        turretDirectionRelativeToSelf = value;
                    }
                }
            }

            private void TurnTowards(float seconds, Vector2 target)
            {
                float targetAngle = GetClosestPointAtAngleInRange(target);
                float currentAngle = Vector2Utils.MinimizeMagnitude(TurretDirectionRelativeToSelf);
                float maxAngleChange = seconds * angularSpeed;

                if (range >= Math.PI)
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
                PhysicalObject parent = ((PhysicalObject)(this.Parent));
                if (parent != null)
                {
                    float worldDirection = Vector2Utils.Vector2Angle(target - this.WorldPosition());
                    float targetAngleRelativeToParent = worldDirection - ((PhysicalObject.State)(parent.PracticalState)).WorldDirection() - this.DirectionRelativeToParent;
                    return MathUtils.ClosestInRange(Vector2Utils.MinimizeMagnitude(targetAngleRelativeToParent), range, -range);
                }
                else
                {
                    return 0;
                }
            }

            public Boolean IsPointedAt(Vector2 target, float errorDistance)
            {
                Vector2 worldPosition = this.WorldPosition();
                float angle = Vector2Utils.ShortestAngleDistance(Vector2Utils.Vector2Angle(target - worldPosition), this.WorldDirection());
                float distanceToTarget = Vector2.Distance(target, worldPosition);
                return Math.Abs(angle) <= Math.PI / 2 && Math.Abs((float)(Math.Sin(angle) * distanceToTarget)) < errorDistance;
            }

            public void Fire()
            {
                foreach (GameObjectReference<Gun> gunRef in gunList)
                {
                    Gun gun = gunRef.Dereference();
                    if(gun != null && this.IsPointedAt(target, 50))
                    {
                        gun.Fire();
                    }
                }
            }
        }

        public Turret(GameObjectUpdate message) : base(message) { }

        public Turret(PhysicalObject parent, Vector2 position, float direction, float range, NetworkPlayerController controller)
            : base(parent, position, direction)
        {
            Turret.State myState = (Turret.State)this.PracticalState;
            myState.Range = range;

            this.controller = controller;

            Gun gun = new Gun(this, new Vector2(75, 0), 0);
            StaticGameObjectCollection.Collection.Add(gun);
            //TODO: add gun
        }

        protected override GameObject.State BlankState(GameObject obj)
        {
            return new Turret.State(obj);
        }

        public NetworkPlayerController GetController()
        {
            return controller;
        }
        
        

        

        

        

        
    }
}
