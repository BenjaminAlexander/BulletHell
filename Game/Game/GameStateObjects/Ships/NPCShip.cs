using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.AI;

namespace MyGame.GameStateObjects.Ships
{
    public class NPCShip : Ship
    {
        Gun gun;
        public NPCShip(Vector2 position, Random random)
            : base(position, new Collidable(Textures.Enemy, position, Color.White, 0, new Vector2(30, 25), .9f), 600 + random.Next(0, 100))
        {
            gun = new Gun(this, new Vector2(70, 0), 0);
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            if (this.flyingStrategy == null)
            {
                PlayerShip player = GameState.GetPlayerShip();
                if (player != null)
                {
                    this.flyingStrategy = new NPCBasicAttackStrategy(this, player);
                }
            }
            else
            {
                this.flyingStrategy.ExecuteStrategy(gameTime);
            }
            base.UpdateSubclass(gameTime);
        }

        public void FireCoaxialWeapon()
        {
            //gun.Fire();
        }
        FlyingStrategy flyingStrategy;
    }
}
