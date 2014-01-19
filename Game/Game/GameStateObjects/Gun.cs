using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.Ships;

namespace MyGame.GameStateObjects
{
    public class Gun : MemberPhysicalObject
    {
        private Boolean fire = false;
        private const int COOLDOWN_TIME = 100;
        private int cooldownTimer = 0;

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
                    this.GameState.AddBullet(new Bullet((Ship)this.Root(), this.WorldPosition(), this.WorldDirection()));
                    cooldownTimer = COOLDOWN_TIME + GameState.random.Next(0, 100);
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

        
    }
}
