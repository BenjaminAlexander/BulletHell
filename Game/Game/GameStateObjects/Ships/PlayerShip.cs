using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.IO;
using Microsoft.Xna.Framework;
using MyGame.Utils;
namespace MyGame.GameStateObjects.Ships
{
    public class PlayerShip : Ship, IOObserver
    {
        // Events to handle the movement of the player ship from keyboard input.
        IOEvent forward = new MyGame.IO.Events.KeyDown(Microsoft.Xna.Framework.Input.Keys.Up);
        IOEvent back = new MyGame.IO.Events.KeyDown(Microsoft.Xna.Framework.Input.Keys.Down);
        IOEvent left = new MyGame.IO.Events.KeyDown(Microsoft.Xna.Framework.Input.Keys.Left);
        IOEvent right = new MyGame.IO.Events.KeyDown(Microsoft.Xna.Framework.Input.Keys.Right);
        IOEvent space = new MyGame.IO.Events.KeyPressEvent(Microsoft.Xna.Framework.Input.Keys.Space);

        //private float acceleration = 0;
        private float maxAcceleration = 100;
        private Boolean turnRight = false;
        private Boolean turnLeft = false;
        private Boolean fire = false;

        public PlayerShip(Vector2 position, MyGame.IO.InputManager inputManager)
            : base(position)
        {
            inputManager.Register(forward, this);
            inputManager.Register(back, this);
            inputManager.Register(left, this);
            inputManager.Register(right, this);
            inputManager.Register(space, this);
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (turnRight)
            {
                TurnRight(gameTime);
            }
            else if (turnLeft)
            {
                TurnLeft(gameTime);
            }

            if (fire)
            {
                this.GameState.AddGameObject(new Bullet(this.Position + Vector2Utils.ConstructVectorFromPolar(200, this.Direction) , this.Direction));
            }
            base.UpdateSubclass(gameTime);
            Acceleration = 0;
            turnRight = false;
            turnLeft = false;
            fire = false;
        }


        /*public override void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            graphics.DrawSolidRectangle(Position, new Vector2(90, 30), new Vector2(45, 15), Direction, Color.Green, 1);
        }*/

        public void UpdateWithIOEvent(IOEvent ioEvent)
        {
            if (ioEvent.Equals(forward))
            {
                Acceleration = maxAcceleration;
            }
            else if (ioEvent.Equals(back))
            {
                Acceleration = -maxAcceleration;
            }
            else if (ioEvent.Equals(left))
            {
                turnRight = false;
                turnLeft = true;
            }
            else if (ioEvent.Equals(right))
            {
                turnRight = true;
                turnLeft = false;
            }
            else if (ioEvent.Equals(space))
            {
                fire = true;
            }
        }

        public override Boolean IsPlayerShip
        {
            get { return true; }
        }
    }
}
