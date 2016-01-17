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
using MyGame.GameServer;
using MyGame.GameClient;

namespace MyGame.GameStateObjects.PhysicalObjects.MemberPhysicalObjects
{
    public class Gun : MemberPhysicalObject
    {

        private Boolean fire = false;
        private const float COOLDOWN_TIME = .1f;

        private FloatGameObjectMember cooldownTimer;
        public Gun(Game1 game) : base(game)
        {
            cooldownTimer = this.AddFloatGameObjectMember(0);
        }

        public void GunInit(PhysicalObject parent, Vector2 position, float direction)
        {
            base.MemberPhysicalObjectInit(parent, position, direction);
        }

        public virtual void Fire()
        {
            fire = true;
        }

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);
            this.CooldownTimer = this.CooldownTimer - seconds;
        }

        public override void ServerOnlyUpdate(float seconds)
        {
            base.ServerOnlyUpdate(seconds);
            if (this.fire && this.CooldownTimer <= 0)
            {
                this.CooldownTimer = COOLDOWN_TIME;
                //FIRE
                Bullet b = new Bullet(this.Game);
                b.BulletInit((Ship)(this.Root()), this.WorldPosition(), this.WorldDirection());
                this.Game.GameObjectCollection.Add(b);
            }
            this.fire = false;
        }

        public float CooldownTimer
        {
            get { return cooldownTimer.Value; }
            protected set { cooldownTimer.Value = value; }
        }
    }
}
