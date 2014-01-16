using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.IO;
using MyGame.Utils;
using MyGame.DrawingUtils;
namespace MyGame.GameStateObjects.Ships
{
    abstract class Turret : MemberPhysicalObject
    {
        float worldDirection = 0;
        Drawable drawable = new Drawable(Textures.Gun, new Vector2(0), Color.White, 0, new Vector2(2.5f, 5), 1);
        Gun gun;

        public Turret(GameState gameState, PhysicalObject parent, Vector2 positionRelativeToParent)
            : base(gameState, parent, positionRelativeToParent, 0)
        {
            gun = new Gun(gameState, this, new Vector2(50f, 0), 0);
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            drawable.Position = this.WorldPosition();
            drawable.Rotation = this.WorldDirection();
            drawable.Draw(graphics);
        }

        public override float WorldDirection()
        {
            return worldDirection;
        }

        public void Fire()
        {
            gun.Fire();
        }

        public void PointAt(Vector2 target)
        {
            worldDirection = Vector2Utils.Vector2Angle(target - this.WorldPosition());
        }
    }
}
