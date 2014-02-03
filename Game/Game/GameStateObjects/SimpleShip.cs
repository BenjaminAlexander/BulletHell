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
using System.Reflection;

namespace MyGame.GameStateObjects
{
    class SimpleShip : GameObject, IOObserver
    {
        private static Collidable collidable = new Collidable(Textures.Enemy,  Color.White, new Vector2(20, 5), 1);

        new class State : GameObject.State
        {
            public Vector2 position = new Vector2(0);
            public Vector2 velocity = new Vector2(0);

            public override void ApplyMessage(GameObjectUpdate message)
            {
                base.ApplyMessage(message);
                this.position = message.ReadVector2();
                this.velocity = message.ReadVector2();
            }

            public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
            {
                message = base.ConstructMessage(message);
                message.Append(this.position);
                message.Append(this.velocity);
                return message;
            }
        }

        public SimpleShip(int id) : base(id)
        {
            this.state = new SimpleShip.State();
            this.drawState = new SimpleShip.State();
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
            this.state = new SimpleShip.State();
            this.drawState = new SimpleShip.State();

            SimpleShip.State myState = (SimpleShip.State)this.state;
            SimpleShip.State myDrawState = (SimpleShip.State)this.drawState;

            myState.position = position;
            myState.velocity = velocity;

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

        protected override void UpdateState(GameObject.State s, float seconds)
        {
            base.UpdateState(s, seconds);
            SimpleShip.State myS = (SimpleShip.State)s;
            myS.position = myS.position + (myS.velocity * seconds);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, DrawingUtils.MyGraphicsClass graphics, GameObject.State s)
        {
            SimpleShip.State myS = (SimpleShip.State)s;
            collidable.Draw(graphics, myS.position, 0);
        }

        protected override void Interpolate(GameObject.State d, GameObject.State s, float smoothing)
        {
            base.Interpolate(d, s, smoothing);
            SimpleShip.State myS = (SimpleShip.State)s;
            SimpleShip.State myDraw = (SimpleShip.State)d;
            Vector2 position = Vector2.Lerp(myS.position, myDraw.position, smoothing);

            myDraw.position = position;
            myDraw.velocity = myS.velocity;
        }

        //test code
        public void UpdateWithIOEvent(IOEvent ioEvent)
        {
            SimpleShip.State myState = (SimpleShip.State)this.state;
            if (ioEvent.Equals(forward))
            {
                myState.velocity = myState.velocity + new Vector2(0, -10);
            }
            else if (ioEvent.Equals(back))
            {
                myState.velocity = myState.velocity + new Vector2(0, 10);
            }
            else if (ioEvent.Equals(left))
            {
                myState.velocity = myState.velocity + new Vector2(-10, 0);
            }
            else if (ioEvent.Equals(right))
            {
                myState.velocity = myState.velocity + new Vector2(10, 0);
            }
            else if (ioEvent.Equals(space))
            {
                myState.velocity = new Vector2(0, 0);
            }
        }


    }
}
