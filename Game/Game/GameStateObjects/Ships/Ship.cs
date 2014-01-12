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
    public abstract class Ship : FlyingGameObject  //simple player ship
    {
        // All ships have a position and a direction (speed).
        public Ship(Vector2 position)
            : base(new Drawable(Textures.Ship, position, Color.White, 0, new Vector2(100, 50), 1), position, 0, 00.0f, 200.0f, 0, 100, 1.0f)
        {
        }

        /*public override void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            graphics.DrawSolidRectangle(Position, new Vector2(90, 30), new Vector2(45, 15), Direction, Color.Red, 1);
        }*/

        protected override void UpdateSubclass(GameTime gameTime)
        {
            if (!GameState.GetWorldRectangle().Contains(this.Position))
            {
                throw new ShipOutOfBoundsException();
            }
            Vector2 preUpdatePosition = this.Position;
            base.UpdateSubclass(gameTime);
            if (!GameState.GetWorldRectangle().Contains(this.Position))
            {
                this.Position = preUpdatePosition;
                this.Speed = 0;
                this.Acceleration = 0;
            }
        }

        public virtual Boolean IsPlayerShip
        {
            get{ return false; }
        }

        public virtual Boolean IsNPCShip
        {
            get{ return false; }
        }

        public override Boolean IsShip
        {
            get { return true; }
        }

        public class ShipOutOfBoundsException : Exception
        {
        }
    }
}
