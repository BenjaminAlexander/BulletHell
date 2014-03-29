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
            private IntegerGameObjectMember health = new IntegerGameObjectMember(40);
            private FloatGameObjectMember maxSpeed = new FloatGameObjectMember(300);
            private FloatGameObjectMember acceleration = new FloatGameObjectMember(300);
            private FloatGameObjectMember maxAgularSpeed = new FloatGameObjectMember(0.5f);
            private IntegerGameObjectMember shipsKilled = new IntegerGameObjectMember(0);
            private Vector2GameObjectMember targetVelocity = new Vector2GameObjectMember(new Vector2(0));

            protected override void InitializeFields()
            {
                base.InitializeFields();
                this.AddField(health);
                this.AddField(shipsKilled);
                this.AddField(maxSpeed);
                this.AddField(acceleration);
                this.AddField(maxAgularSpeed);
                this.AddField(targetVelocity);
            }

            public int Health
            {
                protected set { health.Value = value; }
                get { return health.Value; }
            }

            public void Initialize(int health, float maxSpeed, float acceleration, float maxAgularSpeed)
            {
                this.health.Value = health;
                this.maxSpeed.Value = maxSpeed;
                this.acceleration.Value = acceleration;
                this.maxAgularSpeed.Value = maxAgularSpeed;
            }

            public State(GameObject obj) : base(obj) {}

            public override void UpdateState(float seconds)
            {
                

                if (Game1.IsServer)
                {
                    Ship thisShip = this.GetObject<Ship>();
                    IController controller = thisShip.GetController();
                    controller.Update(seconds);

                    //this.Velocity = this.Velocity + controller.CurrentState.Move * 10;
                    if (controller != null)
                    {
                        this.targetVelocity.Value = Utils.Vector2Utils.ConstructVectorFromPolar(this.maxSpeed.Value * controller.CurrentState.MovementControl, this.GetObject<Ship>().WorldDirection());
                        this.TargetAngle = controller.CurrentState.TargetAngle;
                        this.AngularSpeed = maxAgularSpeed.Value * controller.CurrentState.AngleControl;
                    }

                    foreach (GameObject obj in StaticGameObjectCollection.Collection.Tree.GetObjectsInCircle(this.GetObject<Ship>().WorldPosition(), Ship.MaxRadius + Bullet.MaxRadius))
                    {
                        if (obj is Bullet)
                        {
                            Bullet bullet = (Bullet)obj;
                            Bullet.State bulletState = bullet.PracticalState<Bullet.State>();
                            if (!bulletState.BelongsTo(thisShip) && thisShip.CollidesWith(bullet))
                            {      
                                //if(thisShip is SmallShip)
                                this.health.Value = this.health.Value - bulletState.Damage;
                                if (this.health.Value <= 0)
                                {
                                    bullet.Hit();
                                }
                                bullet.Destroy();
                            }
                        }
                    }
                    

                }

                this.Velocity = Physics.PhysicsUtils.MoveTowardBounded(this.Velocity, targetVelocity.Value, acceleration.Value * seconds);
                if (this.health.Value <= 0)
                {
                    this.GetObject<GameObject>().Destroy();
                }
                base.UpdateState(seconds);
            }

            public void AddKill()
            {
                this.shipsKilled.Value++;
            }

            public int Kills()
            {
                return this.shipsKilled.Value;
            }

            public void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
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

        public override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
        {
            this.PracticalState<Ship.State>().MoveOutsideWorld(position, movePosition);
        }
    }
}
