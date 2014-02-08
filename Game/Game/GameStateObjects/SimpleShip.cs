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
using MyGame.PlayerControllers;

namespace MyGame.GameStateObjects
{
    class SimpleShip : GameObject//, IOObserver
    {
        private static Collidable collidable = new Collidable(Textures.Enemy,  Color.White, new Vector2(20, 5), 1);

        public new class State : GameObject.State
        {
            public Vector2 position = new Vector2(0);
            public Vector2 velocity = new Vector2(0);
            public float direction = 0;

            public State(GameObject obj) : base(obj) {}

            public override void ApplyMessage(GameObjectUpdate message)
            {
                base.ApplyMessage(message);
                this.position = message.ReadVector2();
                this.velocity = message.ReadVector2();
                this.direction = message.ReadFloat();
            }

            public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
            {
                message = base.ConstructMessage(message);
                message.Append(this.position);
                message.Append(this.velocity);
                message.Append(direction);
                return message;
            }

            public override void UpdateState(float seconds)
            {
                base.UpdateState(seconds);
                this.position = this.position + (this.velocity * seconds);
            }

            public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
            {
                collidable.Draw(graphics, this.position, direction);
            }

            public override GameObject.State Interpolate(GameObject.State s, float smoothing, GameObject.State blankState)
            {
                blankState = base.Interpolate(s, smoothing, blankState);
                SimpleShip.State myS = (SimpleShip.State)s;
                SimpleShip.State myBlankState = (SimpleShip.State)blankState;

                Vector2 position = Vector2.Lerp(myS.position, this.position, smoothing);
                Vector2 velocity = Vector2.Lerp(myS.velocity, this.velocity, smoothing);
                float direction = Utils.Vector2Utils.Lerp(myS.direction, this.direction, smoothing);


                myBlankState.position = position;
                myBlankState.velocity = velocity;
                myBlankState.direction = direction;
                return blankState;
            }

            public override void ServerUpdate(float seconds)
            {
                base.ServerUpdate(seconds);
                this.velocity = this.velocity + StaticNetworkPlayerManager.GetController(1).CurrentState.Move * 10;
                if (StaticNetworkPlayerManager.GetController(1).CurrentState.Aimpoint != this.position)
                {
                    this.direction = Utils.Vector2Utils.Vector2Angle(StaticNetworkPlayerManager.GetController(1).CurrentState.Aimpoint - this.position);
                }
            }
        }

        protected override GameObject.State BlankState(GameObject obj)
        {
            return new SimpleShip.State(obj);
        }

        public SimpleShip(GameObjectUpdate message) : base(message) { }

        //test controls
       /* private IOEvent forward;
        private IOEvent back;
        private IOEvent left;
        private IOEvent right;
        private IOEvent space;*/

        public SimpleShip(Vector2 position, Vector2 velocity, InputManager inputManager)
            : base()
        {
            SimpleShip.State myState = (SimpleShip.State)this.SimulationState;
            SimpleShip.State myDrawState = (SimpleShip.State)this.DrawState;

            myState.position = position;
            myState.velocity = velocity;

            //test controls
            /*forward = new KeyDown(Keys.Up);
            back = new KeyDown(Keys.Down);
            left = new KeyDown(Keys.Left);
            right = new KeyDown(Keys.Right);
            space = new KeyDown(Keys.Space);
            inputManager.Register(forward, this);
            inputManager.Register(back, this);
            inputManager.Register(left, this);
            inputManager.Register(right, this);
            inputManager.Register(space, this);*/

        }
        /*
        //test controls
        public void UpdateWithIOEvent(IOEvent ioEvent)
        {
            SimpleShip.State myState = (SimpleShip.State)this.SimulationState;
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
        */

    }
}
