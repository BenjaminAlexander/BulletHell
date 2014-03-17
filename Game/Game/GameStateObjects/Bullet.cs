using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public class Bullet : MovingGameObject
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Bullet"), Color.White, new Vector2(20, 5), 1);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        private static float speed = 1500;
        public static float MaxRadius
        {
            get { return 50;}
        }

        public new class State : MovingGameObject.State
        {

            private int damage = 10;
            private Vector2 start;
            private float range = 3000;
            private GameObjectReference<Ship> owner;

            public State(GameObject obj) : base(obj) { }

            public void Initialize(Ship owner)
            {
                this.owner = new GameObjectReference<Ship>(owner);
            }

            public override void ApplyMessage(GameObjectUpdate message)
            {
                base.ApplyMessage(message);
                this.damage = message.ReadInt();
                this.start = message.ReadVector2();
                this.range = message.ReadFloat();
                this.owner = message.ReadGameObjectReference<Ship>();
            }

            public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
            {
                message = base.ConstructMessage(message);
                message.Append(this.damage);
                message.Append(this.start);
                message.Append(this.range);
                message.Append(this.owner);
                return message;
            }

            public override void Interpolate(GameObject.State d, GameObject.State s, float smoothing, GameObject.State blankState)
            {
                base.Interpolate(d, s, smoothing, blankState);
                Bullet.State myS = (Bullet.State)s;
                Bullet.State myBlankState = (Bullet.State)blankState;

                myBlankState.damage = myS.damage;
                myBlankState.start = myS.start;
                myBlankState.range = myS.range;
            }

            public void Hit()
            {
                if (owner.Dereference() != null)
                {
                    owner.Dereference().AddKill();
                }
            }

            protected override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
            {
                if (Game1.IsServer)
                {
                    this.Destroy();
                }
            }

            public int Damage
            {
                get { return damage; }
            }

            public Boolean BelongsTo(GameObject obj)
            {
                if (owner.CanDereference())
                {
                    return owner.Dereference() == obj;
                }
                else
                {
                    return false;
                }
            }
        }


        protected override GameObject.State BlankState(GameObject obj)
        {
            return new Bullet.State(obj);
        }

        public Bullet(GameObjectUpdate message) : base(message) {
        }

        public Bullet(Ship owner, Vector2 position, float direction)
            : base(position, Utils.Vector2Utils.ConstructVectorFromPolar(speed, direction) /*+ ((Ship.State)(owner.PracticalState)).Velocity*/, direction, 0, direction)
        {
            ((Bullet.State)(this.PracticalState)).Initialize(owner);
        }

        public void Hit()
        {
            Bullet.State myState = (Bullet.State)this.PracticalState;
            myState.Hit();
        }

        protected override float SecondsBetweenUpdateMessage
        {
            get
            {
                return 4;
            }
        }
    }
}
