using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.IO;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    abstract class Turret : MemberPhysicalObject
    {
        private float turretDirectionRelativeToSelf = 0;
        private float range;
        private float angularSpeed = 100;
        private Drawable drawable = new Drawable(Textures.Gun, new Vector2(0), Color.White, 0, new Vector2(2.5f, 5), 1);
        private List<Gun> gunList = new List<Gun>();
        private Vector2 target = new Vector2(0);
        private Boolean interleave = false;
        private int interleaveCooldown = 0;
        private int currentGun = 0;
        public Boolean Interleave
        {
            get { return interleave;  }
            set
            {
                Boolean can = true;
                int cooldown = 0;
                if (gunList.Count >= 1)
                {
                    cooldown = gunList[0].CooldownTime;

                    foreach (Gun gun in gunList)
                    {
                        if (cooldown != gun.CooldownTime)
                        {
                            can = false;
                        }
                    }
                }

                if (can)
                {
                    interleave = value;
                    interleaveCooldown = cooldown;
                    currentGun = 0;
                }
                else
                {
                    interleave = false;
                }
            }
        }

        public override void Add(MemberPhysicalObject obj)
        {
            base.Add(obj);
            if (obj is Gun)
            {
                gunList.Add((Gun)obj);
            }
            Interleave = Interleave;
            currentGun = 0;
        }

        public Turret(int id)
            : base(id)
        {
            
        }

        public Turret(PhysicalObject parent, Vector2 positionRelativeToParent, float directionRelativeToParent, float range)
        : base(parent, positionRelativeToParent, directionRelativeToParent)
        {
            this.range = range;
        }

        public Vector2 Target
        {
            set { target = value; }
            get { return target; }
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            drawable.Position = this.WorldPosition();
            drawable.Rotation = this.WorldDirection();
            drawable.Draw(graphics);
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            base.UpdateSubclass(gameTime);
            TurnTowards(gameTime, target);
        }

        public void Fire()
        {
            //gun.Fire();
            if (interleave)
            {
                if (gunList[currentGun].CooldownTimeRemaining < (float)interleaveCooldown - (float)interleaveCooldown / gunList.Count)
                {
                    int nextGun = currentGun + 1;
                    if (nextGun >= gunList.Count)
                    {
                        nextGun = 0;
                    }
                    if (gunList[nextGun].ReadyToFire())
                    {
                        currentGun = nextGun;
                        gunList[currentGun].Fire();
                    }
                }
            }
            else
            {
                foreach (Gun gun in gunList)
                {
                    gun.Fire();
                }
            }
        }

        public override float WorldDirection()
        {
            return Parent.WorldDirection() + turretDirectionRelativeToSelf + base.DirectionRelativeToParent;
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

        private void TurnTowards(GameTime gameTime, Vector2 target)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            float targetAngle = GetClosestPointAtAngleInRange(target);
            float currentAngle = Vector2Utils.MinimizeMagnitude(TurretDirectionRelativeToSelf);
            float maxAngleChange = secondsElapsed * angularSpeed;
            
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

        public void PointAt(Vector2 target)
        {
            this.TurretDirectionRelativeToSelf = GetClosestPointAtAngleInRange(target);
        }

        public float GetClosestPointAtAngleInRange(Vector2 target)
        {
            float worldDirection = Vector2Utils.Vector2Angle(target - this.WorldPosition());
            float targetAngleRelativeToParent = worldDirection - Parent.WorldDirection() - this.DirectionRelativeToParent;
            return MathUtils.ClosestInRange(Vector2Utils.MinimizeMagnitude(targetAngleRelativeToParent), range, -range);
        }

        public Boolean IsPointedAt(Vector2 target, float errorDistance)
        {
            Vector2 worldPosition = this.WorldPosition();
            float angle = Vector2Utils.ShortestAngleDistance(Vector2Utils.Vector2Angle(target - worldPosition), this.WorldDirection());
            float distanceToTarget = Vector2.Distance(target, worldPosition);
            return Math.Abs(angle) <= Math.PI/2 && Math.Abs((float)(Math.Sin(angle) * distanceToTarget)) < errorDistance;
        }

        //using MyGame.Networking;
        public override void UpdateMemberFields(GameObjectUpdate message)
        {
            base.UpdateMemberFields(message);
            turretDirectionRelativeToSelf = message.ReadFloat();
            range = message.ReadFloat();
            angularSpeed = message.ReadFloat();
            gunList = message.ReadGameObjectList().Cast<Gun>().ToList();
            target = message.ReadVector2();
            Interleave = message.ReadBoolean();
            interleaveCooldown = message.ReadInt();
            currentGun = message.ReadInt();
        }

        public override GameObjectUpdate MemberFieldUpdateMessage(GameObjectUpdate message)
        {
            message = base.MemberFieldUpdateMessage(message);
            message.Append(turretDirectionRelativeToSelf);
            message.Append(range);
            message.Append(angularSpeed);
            message.Append(gunList.Cast<GameObject>().ToList());
            message.Append(target);
            message.Append(interleave);
            message.Append(interleaveCooldown);
            message.Append(currentGun);
            return message;
        }
    }
}
