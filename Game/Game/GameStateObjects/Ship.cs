using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.IO;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.Geometry;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.Networking;
using MyGame.PlayerControllers;

namespace MyGame.GameStateObjects
{
    public class Ship : MovingGameObject 
    {

        private static Collidable collidable = new Collidable(Textures.Enemy,  Color.White, new Vector2(25, 25), 1);
        private NetworkPlayerController controller;

        public new class State : MovingGameObject.State
        {
            private int health = 40;
            public int Health
            {
                protected set { health = value; }
                get { return health; }
            }
            public State(GameObject obj) : base(obj) {}

            public override void ApplyMessage(GameObjectUpdate message)
            {
                base.ApplyMessage(message);
                health = message.ReadInt();
            }

            public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
            {
                message = base.ConstructMessage(message);
                message.Append(health);
                return message;
            }

            public override void UpdateState(float seconds)
            {
                base.UpdateState(seconds);
            }

            public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
            {
                collidable.Draw(graphics, this.Position, Direction);
            }

            public override void Interpolate(GameObject.State d, GameObject.State s, float smoothing, GameObject.State blankState)
            {
                base.Interpolate(d, s, smoothing, blankState);
                Ship.State myS = (Ship.State)s;
                Ship.State myD = (Ship.State)d;
                Ship.State myBlankState = (Ship.State)blankState;

                myD.health = myS.health;
            }

            public override void ServerUpdate(float seconds)
            {
                base.ServerUpdate(seconds);
                Ship myself = (Ship)this.Object;
                NetworkPlayerController controller = myself.GetController();

                this.Velocity = this.Velocity + controller.CurrentState.Move * 10;
                if (controller.CurrentState.Aimpoint != this.Position)
                {
                    this.TargetAngle = Utils.Vector2Utils.Vector2Angle(controller.CurrentState.Aimpoint - this.Position);
                    this.AngularSpeed = 10;
                }
            }

            protected override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
            {
                Velocity = new Vector2(0);
            }
        }

        protected override GameObject.State BlankState(GameObject obj)
        {
            return new Ship.State(obj);
        }

        public NetworkPlayerController GetController()
        {
            return controller;
        }

        public Ship(GameObjectUpdate message) : base(message) { }

        public Ship(Vector2 position, Vector2 velocity, InputManager inputManager, NetworkPlayerController controller)
            : base(position, new Vector2(0), 0, 0, 0)
        {
            Ship.State myState = (Ship.State)this.PracticalState;
            this.controller = controller;
            controller.Focus = this;

            Gun gun = new Gun(this, new Vector2(75, 0), 0, controller);
            StaticGameObjectCollection.Collection.Add(gun);
        }
    }
}
