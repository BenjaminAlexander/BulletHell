using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.DrawingUtils;
using MyGame.Networking;
using Microsoft.Xna.Framework;
using MyGame.IO;
using MyGame.IO.Events;
using Microsoft.Xna.Framework.Input;

namespace MyGame.GameStateObjects
{
    class SimpleShip : GameObject, IOObserver
    {
        private static Collidable collidable = new Collidable(Textures.Enemy,  Color.White, new Vector2(20, 5), 1);

        class StateClass
        {
            public Vector2 position;
            public Vector2 velocity;
            public StateClass(Vector2 position, Vector2 velocity)
            {
                this.velocity = velocity;
                this.position = position;
            }
            public StateClass(StateClass other)
            {
                this.velocity = other.velocity;
                this.position = other.position;
            }
        }
        StateClass state = new StateClass(new Vector2(0), new Vector2(0));
        StateClass drawState = new StateClass(new Vector2(0), new Vector2(0));
        StateClass previousState = new StateClass(new Vector2(0), new Vector2(0));

        public SimpleShip(int id) : base(id)
        {

        }

        //test controls
        private IOEvent forward;
        private IOEvent back;
        private IOEvent left;
        private IOEvent right;
        private IOEvent space;

        public SimpleShip(Vector2 position, Vector2 velocity, InputManager inputManager)
            : base()
        {
            state = new StateClass(position, velocity);

            forward = new KeyDown(Keys.Up);
            back = new KeyDown(Keys.Down);
            left = new KeyDown(Keys.Left);
            right = new KeyDown(Keys.Right);
            space = new KeyDown(Keys.Space);
            inputManager.Register(forward, this);
            inputManager.Register(back, this);
            inputManager.Register(left, this);
            inputManager.Register(right, this);
            inputManager.Register(space, this);

        }

        private StateClass UpdateState(StateClass s, float seconds)
        {
            return new StateClass(s.position + (s.velocity * seconds), s.velocity);
        }
        
        protected override void UpdateSubclass(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            state = UpdateState(state, secondsElapsed);
            previousState = new StateClass(drawState);
            previousState = UpdateState(previousState, secondsElapsed);
            if (Game1.IsServer)
            {
                drawState = state;
            }
            else
            {
                drawState = Interpolate(previousState, state, this.CurrentSmoothing);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            collidable.Draw(graphics, drawState.position, 0);
        }

        public override void UpdateMemberFields(GameObjectUpdate message)
        {
            base.UpdateMemberFields(message);
            state = new StateClass(message.ReadVector2(), message.ReadVector2());
        }

        public override GameObjectUpdate MemberFieldUpdateMessage(GameObjectUpdate message)
        {
            message = base.MemberFieldUpdateMessage(message);
            message.Append(state.position);
            message.Append(state.velocity);
            return message;
        }

        private static StateClass Interpolate(StateClass d, StateClass s, float smoothing)
        {
            if (smoothing == 0 || smoothing == 1)
            {
                int i;
            }
            Vector2 position = Vector2.Lerp(s.position, d.position, smoothing);

            return new StateClass(position, s.velocity);
        }

        //test code
        public void UpdateWithIOEvent(IOEvent ioEvent)
        {
            if (ioEvent.Equals(forward))
            {
                state.velocity = state.velocity + new Vector2(0, -10);
            }
            else if (ioEvent.Equals(back))
            {
                state.velocity = state.velocity + new Vector2(0, 10);
            }
            else if (ioEvent.Equals(left))
            {
                state.velocity = state.velocity + new Vector2(-10, 0);
            }
            else if (ioEvent.Equals(right))
            {
                state.velocity = state.velocity + new Vector2(10, 0);
            }
            else if (ioEvent.Equals(space))
            {
                state.velocity = new Vector2(0, 0);
            }
        }


    }
}
