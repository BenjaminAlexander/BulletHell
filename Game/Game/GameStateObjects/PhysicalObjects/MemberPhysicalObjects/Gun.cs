using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Networking;
using MyGame.PlayerControllers;
using MyGame.GameStateObjects.PhysicalObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Bullets;

namespace MyGame.GameStateObjects.PhysicalObjects.MemberPhysicalObjects
{
    public class Gun : MemberPhysicalObject
    {
        //private Boolean fire = false;
        //private NetworkPlayerController controller;


        public new class State : MemberPhysicalObject.State
        {
            private FloatGameObjectMember cooldownTimer = new FloatGameObjectMember(0);
            private const float COOLDOWN_TIME = .125f;
            private Boolean fire = false;

            public State(GameObject obj) : base(obj) { }

            public void Fire()
            {
                Game1.AsserIsServer();
                fire = true;
            }

            public override void UpdateState(float seconds)
            {
                base.UpdateState(seconds);
                cooldownTimer.Value = cooldownTimer.Value - seconds;
            }

            public override void ServerUpdate(float seconds)
            {
                base.ServerUpdate(seconds);

                if (this.fire && cooldownTimer.Value <= 0)
                {
                    cooldownTimer.Value = COOLDOWN_TIME;
                    //FIRE
                    StaticGameObjectCollection.Collection.Add(new Bullet((Ship)(this.GetObject<PhysicalObject>().Root()), this.WorldPosition(), this.WorldDirection()));
                }
                this.fire = false;
            }
        }

        public Gun(GameObjectUpdate message) : base(message) { }

        public Gun(PhysicalObject parent, Vector2 position, float direction)
            : base(parent, position, direction)
        {
            Gun.State myState = this.PracticalState<Gun.State>();
        }

        public virtual void Fire()
        {
            this.PracticalState<Gun.State>().Fire();
        }

        protected override GameObject.State BlankState(GameObject obj)
        {
            return new Gun.State(obj);
        }
    }
}
