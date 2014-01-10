using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace MyGame.IO
{
    public class IOState
    {
        public void Update()
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            previousKeyBoardState = currentKeyBoardState;
            currentKeyBoardState = Keyboard.GetState();

            if (leftButtonPressed())
            {
                leftPressedMouseState = currentMouseState;
            }
            if (leftButtonReleased())
            {
                leftReleasedMouseState = currentMouseState;
            }

            if (rightButtonPressed())
            {
                rightPressedMouseState = currentMouseState;
            }
            if (rightButtonReleased())
            {
                rightReleasedMouseState = currentMouseState;
            }
        }

        public bool leftButtonReleased()
        {
            return previousMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released;
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

        private MouseState previousMouseState;
        private MouseState currentMouseState;

        private KeyboardState previousKeyBoardState;
        private KeyboardState currentKeyBoardState;

        private MouseState leftPressedMouseState;
        private MouseState leftReleasedMouseState;

        private MouseState rightPressedMouseState;
        private MouseState rightReleasedMouseState;
    }
}
