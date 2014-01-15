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
        private int health = 40;

        // All ships have a position and a direction (speed).
        public Ship(GameState gameState, Vector2 position, Drawable drawable)
            : base(gameState, drawable, position, 0, 00.0f, 400.0f, 0, 1200, 2.0f)
        {
        }

        /*public override void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            graphics.DrawSolidRectangle(Position, new Vector2(90, 30), new Vector2(45, 15), Direction, Color.Red, 1);
        }*/

        public void DoDamage(int damage)
        {
            health = health - damage;
        }

        public Boolean IsAlive
        {
            get { return health > 0; }
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            if (!GameState.GetWorldRectangle().Contains(this.Position))
            {
                throw new ShipOutOfBoundsException();
            }
            if (IsAlive)
            {

                Vector2 preUpdatePosition = this.Position;
                base.UpdateSubclass(gameTime);
                if (!GameState.GetWorldRectangle().Contains(this.Position))
                {
                    this.Position = preUpdatePosition;
                    this.Speed = 0;
                    this.Acceleration = 0;
                }
            }
            else if(GameState != null)
            {
                GameState.RemoveShip(this);
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
