using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
namespace MyGame.IO
{
    public static class IOState
    {
        public static void Initilize()
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

        public static Vector2 MouseScreenPosition()
        {
            return new Vector2(currentMouseState.X, currentMouseState.Y);
        }

        public static void Update()
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

        public static bool leftButtonReleased()
        {
            return previousMouseState.LeftButton == ButtonState.Pressed && currentMouseState.LeftButton == ButtonState.Released;
        }

        public static bool leftButtonDown()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool leftButtonPressed()
        {
            return previousMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Released;
        }

        public static bool rightButtonReleased()
        {
            return previousMouseState.RightButton == ButtonState.Pressed && currentMouseState.RightButton == ButtonState.Released;
        }

        public static bool rightButtonPressed()
        {
            return previousMouseState.RightButton == ButtonState.Released && currentMouseState.RightButton == ButtonState.Released;
        }

        public static bool isKeyPressed(Keys key)
        {
            return previousKeyBoardState.IsKeyUp(key) && currentKeyBoardState.IsKeyDown(key);
        }

        public static bool isKeyDown(Keys key)
        {
            return currentKeyBoardState.IsKeyDown(key);
        }

        private static MouseState previousMouseState;
        private static MouseState currentMouseState;

        private static KeyboardState previousKeyBoardState;
        private static KeyboardState currentKeyBoardState;

        private static MouseState leftPressedMouseState;
        private static MouseState leftReleasedMouseState;

        private static MouseState rightPressedMouseState;
        private static MouseState rightReleasedMouseState;
    }
}
