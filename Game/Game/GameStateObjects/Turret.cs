using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.IO;
using MyGame.Utils;
using MyGame.DrawingUtils;
namespace MyGame.GameStateObjects
{
    abstract class Turret : MemberPhysicalObject
    {
        float turretDirectionRelativeToSelf = 0;
        float range;
        float angularSpeed = 10;
        Drawable drawable = new Drawable(Textures.Gun, new Vector2(0), Color.White, 0, new Vector2(2.5f, 5), 1);
        Gun gun;
        Vector2 target = new Vector2(0);

        public Turret(PhysicalObject parent, Vector2 positionRelativeToParent, float directionRelativeToParent, float range)
            : base(parent, positionRelativeToParent, directionRelativeToParent)
        {
            this.range = range;
            gun = new Gun(this, new Vector2(50f, 0), 0);
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
            gun.Fire();
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
    }
}
