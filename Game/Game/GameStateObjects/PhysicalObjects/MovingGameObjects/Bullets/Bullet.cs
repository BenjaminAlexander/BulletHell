using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.Networking;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Ships;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Bullets
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

            private IntegerGameObjectMember damage = new IntegerGameObjectMember(10);
            private Vector2GameObjectMember start = new Vector2GameObjectMember(new Vector2(0));
            private FloatGameObjectMember range = new FloatGameObjectMember(3000);
            private GameObjectReferenceField<Ship> owner = new GameObjectReferenceField<Ship>(new GameObjectReference<Ship>(null));

            protected override void InitializeFields()
            {
 	            base.InitializeFields();
                AddField(damage);
                AddField(start);
                AddField(range);
                AddField(owner);
            }

            public State(GameObject obj) : base(obj) { }

            public void Initialize(Ship owner)
            {
                this.owner.Value = new GameObjectReference<Ship>(owner);
            }

            

            public void Hit()
            {
                if (owner.Value.Dereference() != null)
                {
                    owner.Value.Dereference().AddKill();
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
                get { return damage.Value; }
            }

            public Boolean BelongsTo(GameObject obj)
            {
                if (owner.Value.CanDereference())
                {
                    return owner.Value.Dereference() == obj;
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
            this.PracticalState<Bullet.State>().Initialize(owner);
        }

        public void Hit()
        {
            Bullet.State myState = this.PracticalState<Bullet.State>();
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
