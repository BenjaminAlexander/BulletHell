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
    class SimpleShip : MovingGameObject//, IOObserver
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Enemy"), Color.White, new Vector2(25, 25), 1);
        private NetworkPlayerController controller;

        public new class State : MovingGameObject.State
        {
            public State(GameObject obj) : base(obj) {}

            public override void ApplyMessage(GameObjectUpdate message)
            {
                base.ApplyMessage(message);
            }

            public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
            {
                message = base.ConstructMessage(message);
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
                SimpleShip.State myS = (SimpleShip.State)s;
                SimpleShip.State myD = (SimpleShip.State)d;
                SimpleShip.State myBlankState = (SimpleShip.State)blankState;
            }

            public override void ServerUpdate(float seconds)
            {
                base.ServerUpdate(seconds);
                SimpleShip myself = (SimpleShip)this.Object;
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
            return new SimpleShip.State(obj);
        }

        public NetworkPlayerController GetController()
        {
            return controller;
        }

        public SimpleShip(GameObjectUpdate message) : base(message) { }

        public SimpleShip(Vector2 position, Vector2 velocity, InputManager inputManager, NetworkPlayerController controller)
            : base(position, new Vector2(0), 0, 0, 0)
        {
            SimpleShip.State myState = (SimpleShip.State)this.PracticalState;
            this.controller = controller;
        }
    }
}
