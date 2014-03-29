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

        private IntegerGameObjectMember health = new IntegerGameObjectMember(40);

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.AddField(health);
        }

        public new class State : MovingGameObject.State
        {

            private FloatGameObjectMember maxSpeed = new FloatGameObjectMember(300);
            private FloatGameObjectMember acceleration = new FloatGameObjectMember(300);
            private FloatGameObjectMember maxAgularSpeed = new FloatGameObjectMember(0.5f);
            private IntegerGameObjectMember shipsKilled = new IntegerGameObjectMember(0);
            private Vector2GameObjectMember targetVelocity = new Vector2GameObjectMember(new Vector2(0));

            protected override void InitializeFields()
            {
                base.InitializeFields();
                this.AddField(shipsKilled);
                this.AddField(maxSpeed);
                this.AddField(acceleration);
                this.AddField(maxAgularSpeed);
                this.AddField(targetVelocity);
            }

            public int Health
            {
                protected set { this.GetObject<Ship>().Health = value; }
                get { return this.GetObject<Ship>().Health; }
            }

            public void Initialize(int health, float maxSpeed, float acceleration, float maxAgularSpeed)
            {

                this.maxSpeed.Value = maxSpeed;
                this.acceleration.Value = acceleration;
                this.maxAgularSpeed.Value = maxAgularSpeed;
            }

            public State(GameObject obj) : base(obj) {}

            public virtual void UpdateState(float seconds)
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
                        this.GetObject<Ship>().TargetAngle = controller.CurrentState.TargetAngle;
                        this.GetObject<Ship>().AngularSpeed = maxAgularSpeed.Value * controller.CurrentState.AngleControl;
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
                                this.GetObject<Ship>().Health = this.GetObject<Ship>().Health - bulletState.Damage;
                                if (this.GetObject<Ship>().Health <= 0)
                                {
                                    bullet.Hit();
                                }
                                bullet.Destroy();
                            }
                        }
                    }
                    

                }

                this.GetObject<Ship>().Velocity = Physics.PhysicsUtils.MoveTowardBounded(this.GetObject<Ship>().Velocity, targetVelocity.Value, acceleration.Value * seconds);
                if (this.GetObject<Ship>().Health <= 0)
                {
                    this.GetObject<GameObject>().Destroy();
                }
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
                this.GetObject<Ship>().Velocity = new Vector2(0);
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
            this.health.Value = health;
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
            protected set { health.Value = value; }
            get { return health.Value; }
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

        public override void UpdateSub(float seconds)
        {
            base.UpdateSub(seconds);
            this.PracticalState<Ship.State>().UpdateState(seconds);
        }
    }
}
