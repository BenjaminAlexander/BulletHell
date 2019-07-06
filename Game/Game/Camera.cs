using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MyGame.GameStateObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.Engine.GameState.Instants;

namespace MyGame
{
    public class Camera
    {
        private GraphicsDeviceManager graphics;
        private Vector2 position = new Vector2(0);
        private Vector2 positionRelativeToFocus = new Vector2(0);
        private float zoom = 1;
        private float targetZoom = 1;
        private float zoomInterpolationTime = .25f;
        private float currentZoomInterpolationTime = 0;
        private float rotation = 0;

        private float zoomIncrement = (float).001;
        private float maxZoom = 3;
        private float minZoom = (float).1;
        private IO.InputManager inputManager;

        public Camera(Vector2 position, float zoom, float rotation, GraphicsDeviceManager graphics, IO.InputManager inputManager)
        {
            this.position = position;
            this.zoom = zoom;
            this.rotation = rotation;
            this.graphics = graphics;
            this.inputManager = inputManager;
        }

        public void Update(CurrentInstant current, Ship focus, float seconds)
        {
            if (focus != null)
            {
                int delta = inputManager.IOState.MouseWheelDelta;
                if (delta != 0)
                {
                    targetZoom = targetZoom + targetZoom * zoomIncrement * inputManager.IOState.MouseWheelDelta;
                    currentZoomInterpolationTime = 0;

                    if (targetZoom < minZoom)
                    {
                        targetZoom = minZoom;
                    }
                    if (targetZoom > maxZoom)
                    {
                        targetZoom = maxZoom;
                    }

                }
                currentZoomInterpolationTime = currentZoomInterpolationTime + seconds;

                zoom = MathHelper.Lerp(zoom, targetZoom, currentZoomInterpolationTime / zoomInterpolationTime);

                float minScreenSide = Math.Min(graphics.PreferredBackBufferHeight, graphics.PreferredBackBufferWidth);
                float maxDistanceFromFocus = (minScreenSide/2 - 100)/zoom;
                //Vector2 mousePos = this.ScreenToWorldPosition(IO.IOState.MouseScreenPosition());

                this.position = focus.Position[current];// (mousePos + focus.Position) / 2;

                if (Vector2.Distance(this.position, focus.Position[current]) > maxDistanceFromFocus)
                {
                    Vector2 normal = (this.position - focus.Position[current]);
                    normal.Normalize();
                    normal = normal * (maxDistanceFromFocus);
                    this.positionRelativeToFocus = normal;
                    this.position = normal + focus.Position[current];

                }
            }
        }

        public Vector2 Position
        {
            get { return position; }

            set 
            {
                Vector2 viewSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight) / zoom;
                Vector2 cornerPosition = value - (viewSize / 2);

                position = cornerPosition + (viewSize / 2); 
            }
        }

        public float X
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public float Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public float Zoom
        {
            get { return zoom; }
            set { zoom = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Matrix GetWorldToScreenTransformation()
        {
            float halfWidth = graphics.PreferredBackBufferWidth / 2;
            float halfHeight = (graphics.PreferredBackBufferHeight) / 2;

            Matrix stretch = Matrix.CreateScale(new Vector3(zoom, zoom, 1));

            return Matrix.CreateTranslation(-position.X, -position.Y, 0) * stretch * Matrix.CreateRotationZ(-rotation) * Matrix.CreateTranslation(halfWidth, halfHeight, 0);
        }

        public Matrix GetScreenToWorldTransformation()
        {
            return Matrix.Invert(GetWorldToScreenTransformation());
        }

        public Vector2 ScreenToWorldPosition(Vector2 vector)
        {
            //we need to account for a difference of size between the viewport and the actual window
            //TODO: I don't know if this is the most general way to do this
            Viewport vp = this.graphics.GraphicsDevice.Viewport;
            DisplayMode dm = this.graphics.GraphicsDevice.DisplayMode;
            float yScale = ((float)vp.Height) / dm.Height;
            float xScale = ((float)vp.Width) / dm.Width;
            vector = new Vector2((float)(vector.X * xScale), (float)(vector.Y * yScale));
            return Vector2.Transform(vector, GetScreenToWorldTransformation());
        }

        public Vector2 WorldToScreenPosition(Vector2 vector)
        {
            return Vector2.Transform(vector, GetWorldToScreenTransformation());
        }
    }
}
