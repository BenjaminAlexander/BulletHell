using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Networking;
using MyGame.PlayerControllers;

namespace MyGame.GameStateObjects
{
    public class Gun : MemberPhysicalObject
    {
        //private Boolean fire = false;
        //private NetworkPlayerController controller;


        public new class State : MemberPhysicalObject.State
        {
            private float cooldownTimer = 0;
            private const float COOLDOWN_TIME = .125f;
            private Boolean fire = false;

            public State(GameObject obj) : base(obj) { }

            public override void ApplyMessage(GameObjectUpdate message)
            {
                base.ApplyMessage(message);
                cooldownTimer = message.ReadFloat();
            }

            public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
            {
                message = base.ConstructMessage(message);
                message.Append(cooldownTimer);
                return message;
            }

            public override void Interpolate(GameObject.State d, GameObject.State s, float smoothing, GameObject.State blankState)
            {
                base.Interpolate(d, s, smoothing, blankState);
                Gun.State myS = (Gun.State)s;
                Gun.State myD = (Gun.State)d;
                Gun.State myBlankState = (Gun.State)blankState;

                myD.cooldownTimer = myS.cooldownTimer;
            }

            public void Fire()
            {
                Game1.AsserIsServer();
                fire = true;
            }

            public override void UpdateState(float seconds)
            {
                base.UpdateState(seconds);
                cooldownTimer = cooldownTimer - seconds;
            }

            public override void ServerUpdate(float seconds)
            {
                base.ServerUpdate(seconds);

                if(this.fire &&  cooldownTimer <= 0)
                {
                    cooldownTimer = COOLDOWN_TIME;
                    //FIRE
                    StaticGameObjectCollection.Collection.Add(new Bullet((Ship)(((PhysicalObject)this.Object).Root()), this.WorldPosition(), this.WorldDirection()));
                }
                this.fire = false;
            }
        }

        public Gun(GameObjectUpdate message) : base(message) { }

        public Gun(PhysicalObject parent, Vector2 position, float direction)
            : base(parent, position, direction)
        {
            Gun.State myState = (Gun.State)this.PracticalState;
        }

        public virtual void Fire()
        {
            ((Gun.State)this.PracticalState).Fire();
        }

        protected override GameObject.State BlankState(GameObject obj)
        {
            return new Gun.State(obj);
        }
    }
}
