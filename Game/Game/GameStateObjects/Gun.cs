using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.Ships;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public class Gun : MemberPhysicalObject
    {
        private Boolean fire = false;
        private const int COOLDOWN_TIME = 250;
        private int cooldownTimer = 0;

        public Gun(int id)
            : base(id)
        {
            
        }

        public Gun(PhysicalObject parent, Vector2 positionRelativeToParent, float directionRelativeToParent)
            : base(parent, positionRelativeToParent, directionRelativeToParent)
        {
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            base.UpdateSubclass(gameTime);
            if (cooldownTimer <= 0)
            {
                if (fire && this.Root() is Ship && this.GameState.GetWorldRectangle().Contains(this.WorldPosition()))
                {
                    Bullet bullet = new Bullet((Ship)this.Root(), this.WorldPosition(), this.WorldDirection());
                    this.GameState.AddBullet(bullet);
                    cooldownTimer = COOLDOWN_TIME + 50;// GameState.random.Next(0, 100);
                }
            }
            else
            {
                cooldownTimer = cooldownTimer - gameTime.ElapsedGameTime.Milliseconds;
            }
            fire = false;
        }

        public override void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
        }

        public virtual void Fire()
        {
            fire = true;
        }

        public Boolean ReadyToFire()
        {
            return cooldownTimer <= 0;
        }

        public int CooldownTime
        {
            get { return COOLDOWN_TIME; }
        }

        public int CooldownTimeRemaining
        {
            get { return cooldownTimer; }
        }

        
    }
}
