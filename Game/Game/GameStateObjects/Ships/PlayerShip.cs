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
        IOEvent forward = new MyGame.IO.Events.KeyDown(Microsoft.Xna.Framework.Input.Keys.Up);
        IOEvent back = new MyGame.IO.Events.KeyDown(Microsoft.Xna.Framework.Input.Keys.Down);
        IOEvent left = new MyGame.IO.Events.KeyDown(Microsoft.Xna.Framework.Input.Keys.Left);
        IOEvent right = new MyGame.IO.Events.KeyDown(Microsoft.Xna.Framework.Input.Keys.Right);

        //private float acceleration = 0;
        private Boolean turnRight = false;
        private Boolean turnLeft = false;
        

        public PlayerShip(GameState gameState, Vector2 position, MyGame.IO.InputManager inputManager)
            : base(gameState, position, new Drawable(Textures.Ship, position, Color.White, 0, new Vector2(100, 50), 1))
        {
            inputManager.Register(forward, this);
            inputManager.Register(back, this);
            inputManager.Register(left, this);
            inputManager.Register(right, this);

            Gun gun = new Gun(this.GameState, this, new Vector2(100, 0), 0, inputManager);
            this.GameState.AddGameObject(gun);
            gun = new Gun(this.GameState, this, new Vector2(-100, 50), (float)(Math.PI/12), inputManager);
            this.GameState.AddGameObject(gun);
            gun = new Gun(this.GameState, this, new Vector2(-100, -50), (float)(-Math.PI / 12), inputManager);
            this.GameState.AddGameObject(gun);
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


        /*public override void Draw(GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            graphics.DrawSolidRectangle(Position, new Vector2(90, 30), new Vector2(45, 15), Direction, Color.Green, 1);
        }*/

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
