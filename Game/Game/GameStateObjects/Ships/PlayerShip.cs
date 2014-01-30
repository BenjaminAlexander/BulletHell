using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.IO;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.PlayerControllers;
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
        

        public PlayerShip(int id)
            : base(id)
        {
            
        }

        public void Initialize(Vector2 position, MyGame.IO.InputManager inputManager)
        {
            
            inputManager.Register(forward, this);
            inputManager.Register(back, this);
            inputManager.Register(left, this);
            inputManager.Register(right, this);

            GunnerController gunner0 = GunnerController.CreateGunner(0);
            GunnerController gunner1 = GunnerController.CreateGunner(1);

            PlayerGun pGun = new PlayerGun(GameObject.NextID);

            pGun.Initialize(this, new Vector2(100, 0), 0, inputManager);

            PlayerRotatingGun gun1 = new PlayerRotatingGun(GameObject.NextID);
            gun1.Initialize(this, new Vector2(0, 25), (float)(Math.PI / 2), gunner0);


            PlayerRotatingGun gun2 = new PlayerRotatingGun(GameObject.NextID);
            gun2.Initialize(this, new Vector2(0, -25), (float)(-Math.PI / 2), gunner1);

            this.GameState.AddOutOfWorldGameObject(gunner0);
            this.GameState.AddOutOfWorldGameObject(gunner1);
            base.Initialize(position, new Collidable(Textures.Ship, position, Color.White, 0, new Vector2(100, 50), .9f), 500);
        }

        protected override void UpdateSubclass(GameTime gameTime)
        {
            //this.Health = 100;
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
            turnRight = false;
            turnLeft = false;
        }

        public void UpdateWithIOEvent(IOEvent ioEvent)
        {
            if (ioEvent.Equals(forward))
            {
                this.SpeedUp = true;
            }
            else if (ioEvent.Equals(back))
            {

                this.SlowDown = true;
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
    }
}
