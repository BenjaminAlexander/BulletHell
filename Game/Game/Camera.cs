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
using MyGame.PlayerControllers;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;

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
        private float minZoom = (float).3;

        public Camera(Vector2 position, float zoom, float rotation, GraphicsDeviceManager graphics)
        {
            this.position = position;
            this.zoom = zoom;
            this.rotation = rotation;
            this.graphics = graphics;
        }

        public void Update(float seconds)
        {
            Ship focus;
            if (Game1.IsServer)
            {
                focus = StaticControllerFocus.GetFocus(1);
            }
            else
            {
                focus = StaticControllerFocus.GetFocus(Game1.PlayerID);
            }
            if (focus != null)
            {
                int delta = IO.IOState.MouseWheelDelta;
                if (delta != 0)
                {
                    targetZoom = targetZoom + targetZoom * zoomIncrement * IO.IOState.MouseWheelDelta;
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
                Vector2 mousePos = this.ScreenToWorldPosition(IO.IOState.MouseScreenPosition());

                this.position = (mousePos + focus.Position) / 2;
                this.rotation = ((Ship.State)focus.PracticalState).WorldDirection();

                if(Vector2.Distance(this.position, focus.Position) > maxDistanceFromFocus)
                {
                    Vector2 normal = (this.position - focus.Position);
                    normal.Normalize();
                    normal = normal * (maxDistanceFromFocus);
                    this.positionRelativeToFocus = normal;
                    this.position = normal + focus.Position;

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

            float halWidth = graphics.PreferredBackBufferWidth / 2;
            float galfHeight = (graphics.PreferredBackBufferHeight) / 2;

            Matrix stretch = Matrix.CreateScale(new Vector3(zoom, zoom, 1));

            return Matrix.CreateTranslation(-position.X, -position.Y, 0) * stretch * Matrix.CreateRotationZ(-rotation) * Matrix.CreateTranslation(halWidth, galfHeight, 0);
        }

        public Matrix GetScreenToWorldTransformation()
        {
            return Matrix.Invert(GetWorldToScreenTransformation());
        }

        public Vector2 ScreenToWorldPosition(Vector2 vector)
        {
            return Vector2.Transform(vector, GetScreenToWorldTransformation());
        }

        public Vector2 WorldToScreenPosition(Vector2 vector)
        {
            return Vector2.Transform(vector, GetWorldToScreenTransformation());
        }
    }
}
