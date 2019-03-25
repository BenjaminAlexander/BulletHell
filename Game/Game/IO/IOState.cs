using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace MyGame.IO
{
    public class IOState
    {
        private MouseState previousMouseState;
        private MouseState currentMouseState;

        private KeyboardState previousKeyBoardState;
        private KeyboardState currentKeyBoardState;

        private MouseState leftPressedMouseState;
        private MouseState leftReleasedMouseState;
        private MouseState rightPressedMouseState;
        private MouseState rightReleasedMouseState;

        public IOState()
        {
            previousMouseState = Mouse.GetState();
            currentMouseState = Mouse.GetState();

            previousKeyBoardState = Keyboard.GetState();
            currentKeyBoardState = Keyboard.GetState();

            leftPressedMouseState = Mouse.GetState();
            leftReleasedMouseState = Mouse.GetState();
            rightPressedMouseState = Mouse.GetState();
            rightReleasedMouseState = Mouse.GetState();
        }

        public IOState(IOState previousState)
        {
            this.previousMouseState = previousState.currentMouseState;
            this.currentMouseState = Mouse.GetState();

            this.previousKeyBoardState = previousState.currentKeyBoardState;
            this.currentKeyBoardState = Keyboard.GetState();

            this.leftPressedMouseState = previousState.leftPressedMouseState;
            this.leftReleasedMouseState = previousState.leftReleasedMouseState;
            this.rightPressedMouseState = previousState.rightPressedMouseState;
            this.rightReleasedMouseState = previousState.rightReleasedMouseState;

            if (leftButtonPressed())
            {
                this.leftPressedMouseState = this.currentMouseState;
            }
            if (leftButtonReleased())
            {
                this.leftReleasedMouseState = currentMouseState;
            }

            if (rightButtonPressed())
            {
                this.rightPressedMouseState = currentMouseState;
            }
            if (rightButtonReleased())
            {
                this.rightReleasedMouseState = currentMouseState;
            }

            //if (Game1.IsInstanceActive)
            //{
            //mouseDelta = MouseScreenPosition() - new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

            //Mouse.SetPosition(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            //}
        }

        public Vector2 MouseScreenPosition()
        {
            return new Vector2(currentMouseState.X, currentMouseState.Y);
        }

        public bool leftButtonReleased()
        {
            return previousMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released;
        }

        public bool leftButtonDown()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool leftButtonPressed()
        {
            return previousMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Released;
        }

        public bool rightButtonReleased()
        {
            return previousMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Released;
        }

        public bool rightButtonPressed()
        {
            return previousMouseState.RightButton == ButtonState.Released && currentMouseState.RightButton == ButtonState.Released;
        }

        public bool isKeyPressed(Keys key)
        {
            return previousKeyBoardState.IsKeyUp(key) && currentKeyBoardState.IsKeyDown(key);
        }

        public bool isKeyDown(Keys key)
        {
            return currentKeyBoardState.IsKeyDown(key);
        }

        public int MouseWheelDelta
        {
            get { return currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue; }
        }
    }
}
