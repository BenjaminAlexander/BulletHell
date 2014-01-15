using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
namespace MyGame.GameStateObjects.Ships
{
    public class NPCShip : Ship
    {
        /*private Vector2 followPoint;

        public Vector2 FollowPoint
        {
            get { return followPoint; }
            set { followPoint = value; }
        }*/

        public NPCShip(GameState gameState, Vector2 position)
            : base(gameState, position, new Drawable(Textures.Enemy, position, Color.White, 0, new Vector2(30, 25), 1))
        {
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

        public void FireWeapon()
        {
            this.GameState.AddGameObject(new Bullet(this.GameState, this, this.Position, this.Direction));
        }

        /*public override void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            graphics.DrawSolidRectangle(Position, new Vector2(90, 30), new Vector2(45, 15), Direction, Color.Red, 1);
        }*/

        public override Boolean IsNPCShip
        {
            get { return true; }
        }

        FlyingStrategy flyingStrategy;
    }
}
