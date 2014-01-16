using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.IO;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
namespace MyGame.GameStateObjects.Ships
{
    public class PlayerShip : Ship, IOObserver
    {
        // Events to handle the movement of the player ship from keyboard input.
        IOEvent forward = new MyGame.IO.Events.KeyDown(Microsoft.Xna.Framework.Input.Keys.W);
        IOEvent back = new MyGame.IO.Events.KeyDown(Microsoft.Xna.Framework.Input.Keys.S);
        IOEvent left = new MyGame.IO.Events.KeyDown(Microsoft.Xna.Framework.Input.Keys.A);
        IOEvent right = new MyGame.IO.Events.KeyDown(Microsoft.Xna.Framework.Input.Keys.D);

        //private float acceleration = 0;
        private Boolean turnRight = false;
        private Boolean turnLeft = false;
        

        public PlayerShip(GameState gameState, Vector2 position, MyGame.IO.InputManager inputManager)
            : base(gameState, position, new Drawable(Textures.Ship, position, Color.White, 0, new Vector2(100, 50), .9f))
        {
            inputManager.Register(forward, this);
            inputManager.Register(back, this);
            inputManager.Register(left, this);
            inputManager.Register(right, this);

            new PlayerGun(this.GameState, this, new Vector2(100, 0), 0, inputManager);
            new PlayerRotatingGun(this.GameState, this, new Vector2(-69, 47), inputManager);
            new PlayerRotatingGun(this.GameState, this, new Vector2(-69, -47), inputManager);
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

            base.UpdateSubclass(gameTime);
            Acceleration = 0;
            turnRight = false;
            turnLeft = false;
        }

        public void UpdateWithIOEvent(IOEvent ioEvent)
        {
            if (ioEvent.Equals(forward))
            {
                Acceleration = MaxAcceleration;
            }
            else if (ioEvent.Equals(back))
            {
                Acceleration = -MaxAcceleration;
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
        }

        public override Boolean IsPlayerShip
        {
            get { return true; }
        }
    }
}
