using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;
using MyGame.IO;
using MyGame.IO.Events;
using MyGame.GameStateObjects;
using Microsoft.Xna.Framework.Input;

namespace MyGame.PlayerControllers
{
    class GunnerController : IDrawableUpdatable, IOObserver
    {
        //private static Vector2 screenSize;
        private static MyGame.Utils.RectangleF screen;
        private static InputManager inputManager;
        private static Camera camera;

        private Vector2 aimPoint = new Vector2(0);
        private Vector2 Aimpoint
        {
            get { return aimPoint; }
            set
            {
                if (!screen.Contains(value))
                {
                    aimPoint = screen.ClosestPoint(value);
                }
                else
                {
                    aimPoint = value;
                }
            }
        }

        private Vector2 aimPointMove = new Vector2(0);

        private Vector2 aimPointMaxSpeed = new Vector2(700,700);
        private Vector2 aimPointSpeed = new Vector2(0);
        private float aimPointAcceleration = 5000;

        private List<GunnerObserver> observerList = new List<GunnerObserver>();


        private IOEvent forward;
        private IOEvent back;
        private IOEvent left;
        private IOEvent right;
        private IOEvent fire;
        private Color color;

        public static void Initialize(MyGraphicsClass graphics, InputManager inputManager, Camera camera)
        {
            Vector2 screenSize = new Vector2(graphics.getGraphics().PreferredBackBufferWidth, graphics.getGraphics().PreferredBackBufferHeight);
            GunnerController.inputManager = inputManager;
            GunnerController.camera = camera;
            screen = new Utils.RectangleF(new Vector2(0), screenSize);
        }

        public GunnerController(Color color, Keys upKey, Keys downKey, Keys leftKey, Keys rightKey, Keys fireKey)
        {
            this.color = color;
            forward = new KeyDown(upKey);
            back = new KeyDown(downKey);
            left = new KeyDown(leftKey);
            right = new KeyDown(rightKey);
            fire = new KeyDown(fireKey);

            inputManager.Register(forward, this);
            inputManager.Register(back, this);
            inputManager.Register(left, this);
            inputManager.Register(right, this);
            inputManager.Register(fire, this);
        }

        public static GunnerController CreateGunner(int id)
        {
            if (id == 0)
            {
                return new GunnerController(Color.Green, Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.RightControl);
            }
            if (id == 1)
            {
                return new GunnerController(Color.Blue, Keys.I, Keys.K, Keys.J, Keys.L, Keys.Space);
            }
            if (id == 2)
            {
                return new GunnerController(Color.Purple, Keys.NumPad8, Keys.NumPad5, Keys.NumPad4, Keys.NumPad6, Keys.NumPad0);
            }
            throw new Exception("Bad gunner id");
        }

        public void Register(GunnerObserver observer)
        {
            observerList.Add(observer);
        }

        public void UnRegister(GunnerObserver observer)
        {
            observerList.Remove(observer);
        }

        public void UpdateWithIOEvent(IOEvent ioEvent)
        {
            if (ioEvent.Equals(forward))
            {
                aimPointMove = aimPointMove + new Vector2(0, -1);
            }
            else if (ioEvent.Equals(back))
            {
                aimPointMove = aimPointMove + new Vector2(0, 1);
            }
            else if (ioEvent.Equals(left))
            {
                aimPointMove = aimPointMove + new Vector2(-1, 0);
            }
            else if (ioEvent.Equals(right))
            {
                aimPointMove = aimPointMove + new Vector2(1, 0);
            }
            else if (ioEvent.Equals(fire))
            {
                NotifyObservers(new GunnerFire());
            }
        }

        private void NotifyObservers(GunnerEvent e)
        {
            foreach (GunnerObserver ob in observerList)
            {
                ob.UpdateWithEvent(e);
            }
        }

        public void Update(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            /*
            if (Math.Abs(aimPointMove.X) > 0)
            {
                aimPointSpeed.X = aimPointSpeed.X + aimPointMove.X * secondsElapsed * aimPointAcceleration;
                if (Math.Abs(aimPointSpeed.X) > aimPointMaxSpeed.X)
                {
                    aimPointSpeed.X = Math.Sign(aimPointSpeed.X) * aimPointMaxSpeed.X;
                }
            }
            else
            {
                float newSpeed = aimPointSpeed.X - Math.Sign(aimPointSpeed.X) * secondsElapsed * aimPointAcceleration;
                if(Math.Sign(newSpeed) != Math.Sign(aimPointSpeed.X))
                {
                    aimPointSpeed.X = 0;
                }
                else
                {
                    aimPointSpeed.X = newSpeed;
                }             
            }

            if (Math.Abs(aimPointMove.Y) > 0)
            {
                aimPointSpeed.Y = aimPointSpeed.Y + aimPointMove.Y * secondsElapsed * aimPointAcceleration;
                if (Math.Abs(aimPointSpeed.Y) > aimPointMaxSpeed.Y)
                {
                    aimPointSpeed.Y = Math.Sign(aimPointSpeed.Y) * aimPointMaxSpeed.Y;
                }
            }
            else
            {
                float newSpeed = aimPointSpeed.Y - Math.Sign(aimPointSpeed.Y) * secondsElapsed * aimPointAcceleration;
                if (Math.Sign(newSpeed) != Math.Sign(aimPointSpeed.Y))
                {
                    aimPointSpeed.Y = 0;
                }
                else
                {
                    aimPointSpeed.Y = newSpeed;
                }
            }
            */
            /*
             * Code for polar control of aimpoint
            Vector2 screenCenter = screen.Size / 2;
            Vector2 aimPolar = Aimpoint - screenCenter;
            float newLength = aimPolar.Length() - aimPointSpeed.Y * secondsElapsed;
            float arcDistance = aimPointSpeed.X * secondsElapsed;

            Vector2 newPolar = Utils.Vector2Utils.ConstructVectorFromPolar(Math.Max(newLength, 0.0001f), Utils.Vector2Utils.Vector2Angle(aimPolar) + arcDistance/newLength);
            Aimpoint = newPolar + screenCenter;
             */
            aimPointMove.Normalize();
            if (aimPointMove.Length() > 0)
            {
                aimPointSpeed = Physics.PhysicsUtils.MoveTowardBounded(aimPointSpeed, aimPointMove * aimPointMaxSpeed, secondsElapsed * aimPointAcceleration);
            }
            else
            {
                aimPointSpeed = Physics.PhysicsUtils.MoveTowardBounded(aimPointSpeed, new Vector2(0), secondsElapsed * aimPointAcceleration);
            }

            Aimpoint = Aimpoint + aimPointSpeed * secondsElapsed;
            aimPointMove = new Vector2(0);
            //Aimpoint = IO.IOState.MouseScreenPosition();
        }

        public void Draw(GameTime gameTime, MyGraphicsClass graphics)
        {
            //throw new NotImplementedException();
            graphics.getSpriteBatch().Draw(Textures.AimPoint, aimPoint, null, color, 0, new Vector2((float)(Textures.AimPoint.Width / 2.0)), 1, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1);
        }

        public Vector2 AimPointInWorld
        {
            get { return Vector2.Transform(aimPoint, camera.GetScreenToWorldTransformation()); }
        }

    }
}
