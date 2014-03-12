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

        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Ship"), Color.White, TextureLoader.GetTexture("Ship").CenterOfMass, .9f);
        private IController controller;

        public new class State : MovingGameObject.State
        {
            private int health = 40;
            private float maxSpeed = 300;
            private float acceleration = 300;
            private float maxAgularSpeed = 0.5f;

            private Vector2 targetVelocity = new Vector2(0);

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
                maxSpeed = message.ReadFloat();
                acceleration = message.ReadFloat();
                maxAgularSpeed = message.ReadFloat();
                targetVelocity = message.ReadVector2();
            }

            public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
            {
                message = base.ConstructMessage(message);
                message.Append(health);
                message.Append(maxSpeed);
                message.Append(acceleration);
                message.Append(maxAgularSpeed);
                message.Append(targetVelocity);
                return message;
            }

            public override void UpdateState(float seconds)
            {
                base.UpdateState(seconds);
                this.Velocity = Physics.PhysicsUtils.MoveTowardBounded(this.Velocity, targetVelocity, acceleration * seconds);
            }

            public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
            {
                collidable.Draw(graphics, this.WorldPosition(), this.WorldDirection());

                
            }

            public override void Interpolate(GameObject.State d, GameObject.State s, float smoothing, GameObject.State blankState)
            {
                base.Interpolate(d, s, smoothing, blankState);
                Ship.State myS = (Ship.State)s;
                Ship.State myD = (Ship.State)d;
                Ship.State myBlankState = (Ship.State)blankState;

                myBlankState.health = myS.health;
                myBlankState.maxSpeed = myS.maxSpeed;
                myBlankState.acceleration = myS.acceleration;
                myBlankState.maxAgularSpeed = myS.maxAgularSpeed;
                myBlankState.targetVelocity = myS.targetVelocity;
            }

            public override void ServerUpdate(float seconds)
            {
                base.ServerUpdate(seconds);
                Ship myself = (Ship)this.Object;
                IController controller = myself.GetController();
                controller.Update(seconds);

                //this.Velocity = this.Velocity + controller.CurrentState.Move * 10;
                if (controller != null)
                {
                    this.targetVelocity = Utils.Vector2Utils.ConstructVectorFromPolar(this.maxSpeed * controller.CurrentState.MovementControl, this.WorldDirection());
                    this.TargetAngle = controller.CurrentState.TargetAngle;
                    this.AngularSpeed = maxAgularSpeed * controller.CurrentState.AngleControl;
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

        public IController GetController()
        {
            return controller;
        }

        public Ship(GameObjectUpdate message) : base(message) { }

        public Ship(Vector2 position, Vector2 velocity, InputManager inputManager, IController controller1, IController controller2, IController controller3, IController controller4)
            : base(position, new Vector2(0), 0, 0, 0)
        {
            Ship.State myState = (Ship.State)this.PracticalState;
            this.controller = controller1;

            if (controller != null)
            {
                controller.Focus = this;
            }

            if (controller4 != null)
            {
                controller4.Focus = this;
            }
            if (controller2 != null)
            {
                controller2.Focus = this;
            }
            if (controller3 != null)
            {
                controller3.Focus = this;
            }


            Turret t = new Turret(this, new Vector2(119, 95) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(Math.PI / 2), (float)(Math.PI / 3), controller2);
            Turret t2 = new Turret(this, new Vector2(119, 5) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(-Math.PI / 2), (float)(Math.PI / 3), controller3);
            Turret t3 = new Turret(this, new Vector2(145, 50) - TextureLoader.GetTexture("Ship").CenterOfMass, (float)(0), (float)(Math.PI / 4), controller4);
            StaticGameObjectCollection.Collection.Add(t);
            StaticGameObjectCollection.Collection.Add(t2);
            StaticGameObjectCollection.Collection.Add(t3);


        }
    }
}
