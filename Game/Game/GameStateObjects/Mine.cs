using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.Ships;
using MyGame.DrawingUtils;


namespace MyGame.GameStateObjects
{
    public class Mine : CompositePhysicalObject

    {
        private Collidable collidable;
        public Mine(Vector2 position) : base(position, 0)
        {
            collidable = new Collidable(Textures.Mine, position, Color.Black, 0, new Vector2(25, 25), (float).5);
        }

        protected override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
        {
            throw new Exception("Mines cannot be moved");
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            base.UpdateSubclass(gameTime);
            foreach (CompositePhysicalObject obj in this.GameState.Tree.GetObjectsInCircle(this.Position, this.collidable.BoundingCircle().Radius+Ship.MAX_RADIUS ))
            {
                if (obj is Ship)
                {
                    Ship ship = (Ship)obj;
                    if (ship.CollidesWith(collidable))
                    {
                        ship.DoDamage(40);
                        this.GameState.RemoveGameObject(this);
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            collidable.Draw(graphics);

        }
    }
}
