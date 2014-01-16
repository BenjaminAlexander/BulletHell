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
        private int ammo = 3;

        public int Ammo
        {
            get { return ammo; }
            set { ammo = value; }
        }

        public Gun(GameState gameState,Ship parent, Vector2 positionRelativeToParent, float directionRelativeToParent)
            : base(gameState, parent, positionRelativeToParent, directionRelativeToParent)
        {
            
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            base.UpdateSubclass(gameTime);
            if (cooldownTimer <= 0)
            {
                if (ammo > 0 && fire && this.Root() is Ship)
                {
                    this.GameState.AddGameObject(new Bullet(this.GameState, (Ship)this.Root(), this.WorldPosition(), this.WorldDirection()));
                    cooldownTimer = COOLDOWN_TIME;
                    ammo = ammo - 1;
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

        public void Fire()
        {
            fire = true;
        }

        
    }
}
