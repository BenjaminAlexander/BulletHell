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

        private Boolean fire = false;
        private const float COOLDOWN_TIME = .125f;

        private FloatGameObjectMember cooldownTimer = new FloatGameObjectMember(0);
        protected override void InitializeFields()
        {
            base.InitializeFields();
            AddField(cooldownTimer);
        }

        public Gun(GameObjectUpdate message) : base(message) { }

        public Gun(PhysicalObject parent, Vector2 position, float direction)
            : base(parent, position, direction)
        {
            Gun.State myState = this.PracticalState<Gun.State>();
        }

        public virtual void Fire()
        {
            Game1.AsserIsServer();
            fire = true;
        }

        protected override GameObject.State BlankState(GameObject obj)
        {
            return new Gun.State(obj);
        }

        public override void UpdateSub(float seconds)
        {
            base.UpdateSub(seconds);
            this.CooldownTimer = this.CooldownTimer - seconds;

            if (Game1.IsServer)
            {
                if (this.fire && this.CooldownTimer <= 0)
                {
                    this.CooldownTimer = COOLDOWN_TIME;
                    //FIRE
                    StaticGameObjectCollection.Collection.Add(new Bullet((Ship)(this.Root()), this.WorldPosition(), this.WorldDirection()));
                }
                this.fire = false;
            }
        }

        public float CooldownTimer
        {
            get { return cooldownTimer.Value; }
            protected set { cooldownTimer.Value = value; }
        }
    }
}
