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

        struct State
        {
            public Vector2 position;
            public Vector2 velocity;
            public State(Vector2 position, Vector2 velocity)
            {
                this.velocity = velocity;
                this.position = position;
            }
        }
        State state = new State(new Vector2(0), new Vector2(0));

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
            state = new State(position, velocity);

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

        private State UpdateState(State s, float seconds)
        {
            return new State(s.position + (s.velocity * seconds), s.velocity);
        }
        
        protected override void UpdateSubclass(GameTime gameTime)
        {
            float secondsElapsed = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            state = UpdateState(state, secondsElapsed);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {

            collidable.Draw(graphics, state.position, 0);
        }

        public override void UpdateMemberFields(GameObjectUpdate message)
        {
            base.UpdateMemberFields(message);
            state = new State(message.ReadVector2(), message.ReadVector2());
        }

        public override GameObjectUpdate MemberFieldUpdateMessage(GameObjectUpdate message)
        {
            message = base.MemberFieldUpdateMessage(message);
            message.Append(state.position);
            message.Append(state.velocity);
            return message;
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
