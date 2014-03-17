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

        //private float cameraSpeed = (float)12;
        //private float cameraMoveZone = 50;

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



                
                if(Vector2.Distance(this.position, focus.Position) > maxDistanceFromFocus)
                {
                    Vector2 normal = (this.position - focus.Position);
                    normal.Normalize();
                    normal = normal * (maxDistanceFromFocus);
                    this.positionRelativeToFocus = normal;
                    this.position = normal + focus.Position;

                }

                    
                /*
                float minScreenSide = Math.Min(graphics.PreferredBackBufferHeight, graphics.PreferredBackBufferWidth);

                if(Vector2.Distance(this.position, this.ScreenToWorldPosition(IO.IOState.MouseScreenPosition())) > 0)
                {
                this.zoom = Math.Min(1, (minScreenSide - 400) / Vector2.Distance(this.position, this.ScreenToWorldPosition(IO.IOState.MouseScreenPosition())));
                }
                else
                {
                    this.zoom = 1;
                }*/
            }
        }

        /*public void Update()
        {
            Vector2 oldViewSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight - Constants.minimapSize.Y) / zoom;
            Vector2 oldCornerPosition = position - oldViewSize / 2;

            float oldZoom = zoom;
            Vector2 preWorldMousePosition = Vector2.Transform(UserInput.MouseScreenPosition(), this.GetScreenToWorldTransformation());

            float minZoom = Math.Max(Math.Max((float)graphics.PreferredBackBufferWidth / state.MapSize.X, (float)graphics.PreferredBackBufferHeight / state.MapSize.Y), absMinZoom);
            if (minZoom > maxZoom)
            {
                minZoom = 1;
            }
            if (this.ScreenView().Contains(UserInput.MouseScreenPosition()))
            {
                zoom = zoom + zoom * zoomIncrement * UserInput.MouseWheelDelta();
            }
            
            if (zoom < minZoom)
            {
                zoom = minZoom;
            }
            if (zoom > maxZoom)
            {
                zoom = maxZoom;
            }

            Vector2 centerScreen = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight - Constants.minimapSize.Y) / 2;

            Position = preWorldMousePosition - ((UserInput.MouseScreenPosition() - centerScreen) / zoom) + (this.cameraMoveNormal() * (cameraSpeed / zoom));


        }*/

        public Vector2 Position
        {
            get { return position; }

            set 
            {
                Vector2 viewSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight) / zoom;
                Vector2 cornerPosition = value - (viewSize / 2);

                /*
                RectangleF mapRectangle = new RectangleF(new Vector2(0), {worldSize} - viewSize);
                cornerPosition = mapRectangle.ClosestPoint(cornerPosition);
                */

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

        /*public RectangleF ScreenView()
        {
            return new RectangleF(new Vector2(0), new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight - Constants.minimapSize.Y));
        }*/

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

        /*private Vector2 cameraMoveNormal()
        {
            Vector2 currentMouse = UserInput.MouseScreenPosition();
            Vector2 move = new Vector2(0);
            if (currentMouse.Y >= 0 &&
                currentMouse.Y <= (graphics.PreferredBackBufferHeight) &&
                currentMouse.X >= 0 &&
                currentMouse.X <= graphics.PreferredBackBufferWidth)
            {
                if (currentMouse.Y > (graphics.PreferredBackBufferHeight) - cameraMoveZone)
                {
                    move = move + new Vector2(0, (currentMouse.Y - ((float)graphics.PreferredBackBufferHeight) + cameraMoveZone) / cameraMoveZone);
                }
                if (currentMouse.X > graphics.PreferredBackBufferWidth - cameraMoveZone)
                {
                    move = move + new Vector2((currentMouse.X - (float)graphics.PreferredBackBufferWidth + cameraMoveZone) / cameraMoveZone, 0);
                }
                if (currentMouse.X < cameraMoveZone)
                {
                    move = move + new Vector2((currentMouse.X - cameraMoveZone) / cameraMoveZone, 0);
                }
                if (currentMouse.Y < cameraMoveZone)
                {
                    move = move + new Vector2(0, (currentMouse.Y - cameraMoveZone) / cameraMoveZone);
                }
                /*if (move.X != 0 && move.Y != 0)
                {
                    move.Normalize();
                }*
            }
            return move;

        }
         * */

        /*
        public RectangleF GetWorldView()
        {
            Matrix transform = this.GetScreenToWorldTransformation();
            Vector2 corner1Position = Vector2.Transform(new Vector2(0), transform);
            Vector2 corner2Position = Vector2.Transform(new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight - Constants.minimapSize.Y), transform);

            return new RectangleF(corner1Position, corner2Position - corner1Position);
        }
         */

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
