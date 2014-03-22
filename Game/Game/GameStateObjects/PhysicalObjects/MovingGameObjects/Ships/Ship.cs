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
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Bullets;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships
{
    abstract public class Ship : MovingGameObject 
    {

        public static float MaxRadius
        {
            get { return 600; }
        }
        private IController controller;

        public new class State : MovingGameObject.State
        {
            private int health = 40;
            private float maxSpeed = 300;
            private float acceleration = 300;
            private float maxAgularSpeed = 0.5f;
            private int shipsKilled = 0;

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
                shipsKilled = message.ReadInt();
            }

            public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
            {
                message = base.ConstructMessage(message);
                message.Append(health);
                message.Append(maxSpeed);
                message.Append(acceleration);
                message.Append(maxAgularSpeed);
                message.Append(targetVelocity);
                message.Append(shipsKilled);
                return message;
            }

            public override void UpdateState(float seconds)
            {
                base.UpdateState(seconds);
                
                if (Game1.IsServer)
                {
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
                    Ship thisShip = (Ship)(this.Object);
                    
                    foreach (GameObject obj in StaticGameObjectCollection.Collection.Tree.GetObjectsInCircle(this.WorldPosition(), Ship.MaxRadius + Bullet.MaxRadius))
                    {
                        if (obj is Bullet)
                        {
                            Bullet bullet = (Bullet)obj;
                            Bullet.State bulletState = bullet.PracticalState<Bullet.State>();
                            if (!bulletState.BelongsTo(thisShip) && thisShip.CollidesWith(bullet))
                            {      
                                //if(thisShip is SmallShip)
                                this.health = this.health - bulletState.Damage;
                                if (this.health <= 0)
                                {
                                    bullet.Hit();
                                }
                                bullet.Destroy();
                            }
                        }
                    }
                    

                }

                this.Velocity = Physics.PhysicsUtils.MoveTowardBounded(this.Velocity, targetVelocity, acceleration * seconds);
                if (this.health <= 0)
                {
                    this.Destroy();
                }
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
                myBlankState.shipsKilled = myS.shipsKilled;
            }

            public void AddKill()
            {
                this.shipsKilled++;
            }

            public int Kills()
            {
                return this.shipsKilled;
            }

            public override void ServerUpdate(float seconds)
            {
                base.ServerUpdate(seconds);
                
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
            Ship.State myState = this.PracticalState<Ship.State>();

            myState.Initialize(health, maxSpeed, acceleration, maxAgularSpeed);
            //40, 300, 300, 0.5f

            this.controller = controller;

            if (this.controller != null)
            {
                this.controller.Focus = this;
            }
        }

        public int Health
        {
            get 
            {
                Ship.State myState = this.PracticalState<Ship.State>();
                return myState.Health;
            }
        }

        public void AddKill()
        {
            Ship.State myState = this.PracticalState<Ship.State>();
            myState.AddKill();
        }

        public int Kills()
        {
            Ship.State myState = this.PracticalState<Ship.State>();
            return myState.Kills();
        }
    }
}
