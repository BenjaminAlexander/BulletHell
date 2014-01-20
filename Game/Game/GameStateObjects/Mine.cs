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
        public const int MAX_RADIUS = 50;
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
        }

        public Collidable Collidable
        {
            get { return collidable; }
        }

        public int Damage
        {
            get { return 40; }
        }

        public override void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            collidable.Draw(graphics);

        }
    }
}
