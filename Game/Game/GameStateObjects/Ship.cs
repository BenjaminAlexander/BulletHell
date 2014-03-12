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

            public void Initialize(int health, float maxSpeed, float acceleration, float maxAgularSpeed)
            {
                this.health = health;
                this.maxSpeed = maxSpeed;
                this.acceleration = acceleration;
                this.maxAgularSpeed = maxAgularSpeed;
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

        public Ship(Vector2 position, Vector2 velocity, int health, float maxSpeed, float acceleration, float maxAgularSpeed,  IController controller)
            : base(position, new Vector2(0), 0, 0, 0)
        {
            Ship.State myState = (Ship.State)this.PracticalState;

            myState.Initialize(health, maxSpeed, acceleration, maxAgularSpeed);
            //40, 300, 300, 0.5f

            this.controller = controller;

            if (this.controller != null)
            {
                this.controller.Focus = this;
            }




        }
    }
}
