﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.IO;
using MyGame.IO.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MyGame.GameStateObjects;

namespace MyGame.PlayerControllers
{
    class LocalPlayerController : IOObserver, MyGame.GameStateObjects.IUpdateable
    {
        private InputManager inputManager;

        private Camera camera;
        private IOEvent forward;
        private IOEvent back;
        private IOEvent left;
        private IOEvent right;
        private IOEvent fire;

        private Vector2 move = new Vector2(0);
        private Boolean isFire = false;

        private PlayerControlState currentState = new PlayerControlState(new Vector2(0), new Vector2(0), false);
        public PlayerControlState CurrentState
        {
            get { return currentState; }
        }

        public LocalPlayerController(InputManager inputManager, Camera camera)
        {
            this.inputManager = inputManager;
            this.camera = camera;

            forward = new KeyDown(Keys.W);
            back = new KeyDown(Keys.S);
            left = new KeyDown(Keys.A);
            right = new KeyDown(Keys.D);
            fire = new LeftMouseDown();

            inputManager.Register(forward, this);
            inputManager.Register(back, this);
            inputManager.Register(left, this);
            inputManager.Register(right, this);
            inputManager.Register(fire, this);
        }

        public void Update(float secondsElapsed)
        {
            if(move != new Vector2(0))
            {
                move.Normalize();
            }
            Vector2 aimpoint;
            Ship focus = StaticControllerFocus.GetFocus(Game1.PlayerID);
            if (focus != null)
            {
                aimpoint = Vector2.Transform(IOState.MouseScreenPosition(), camera.GetScreenToWorldTransformation()) - focus.Position;
            }
            else
            {
                aimpoint = new Vector2(0);
            }
            currentState = new PlayerControlState(aimpoint, move, isFire);
            move = new Vector2(0);
            isFire = false;
        }

        public void UpdateWithIOEvent(IOEvent ioEvent)
        {
            if (ioEvent.Equals(forward))
            {
                move = move + new Vector2(0, -1);
            }
            else if (ioEvent.Equals(back))
            {
                move = move + new Vector2(0, 1);
            }
            else if (ioEvent.Equals(left))
            {
                move = move + new Vector2(-1, 0);
            }
            else if (ioEvent.Equals(right))
            {
                move = move + new Vector2(1, 0);
            }
            else if (ioEvent.Equals(fire))
            {
                isFire = true;
            }
        }
    }
}
