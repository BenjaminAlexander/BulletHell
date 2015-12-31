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
        private const float COOLDOWN_TIME = .1f;

        private FloatGameObjectMember cooldownTimer;
        protected override void InitializeFields()
        {
            base.InitializeFields();

            cooldownTimer = new FloatGameObjectMember(this, 0);

            AddField(cooldownTimer);
        }

        public Gun(Game1 game, GameObjectUpdate message) : base(game, message) { }

        public Gun(Game1 game, PhysicalObject parent, Vector2 position, float direction)
            : base(game, parent, position, direction)
        {
        }

        public virtual void Fire()
        {
            Game1.AsserIsServer();
            fire = true;
        }

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);
            this.CooldownTimer = this.CooldownTimer - seconds;

            if (this.Game.IsGameServer)
            {
                if (this.fire && this.CooldownTimer <= 0)
                {
                    this.CooldownTimer = COOLDOWN_TIME;
                    //FIRE
                    StaticGameObjectCollection.Collection.Add(new Bullet(this.Game, (Ship)(this.Root()), this.WorldPosition(), this.WorldDirection()));
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
