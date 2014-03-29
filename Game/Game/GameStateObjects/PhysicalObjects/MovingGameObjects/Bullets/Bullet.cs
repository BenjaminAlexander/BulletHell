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

        private IntegerGameObjectMember damage;
        private Vector2GameObjectMember start;
        private FloatGameObjectMember range;
        private GameObjectReferenceField<Ship> owner;

        protected override void InitializeFields()
        {
            base.InitializeFields();
            damage = new IntegerGameObjectMember(this, 10);
            start = new Vector2GameObjectMember(this, new Vector2(0));
            range = new FloatGameObjectMember(this, 3000);
            owner = new GameObjectReferenceField<Ship>(this, new GameObjectReference<Ship>(null));

            AddField(damage);
            AddField(start);
            AddField(range);
            AddField(owner);
        }

        public Bullet(GameObjectUpdate message) : base(message) {
        }

        public Bullet(Ship owner, Vector2 position, float direction)
            : base(position, Utils.Vector2Utils.ConstructVectorFromPolar(speed, direction) /*+ ((Ship.State)(owner.PracticalState)).Velocity*/, direction, 0, direction)
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

        protected override float SecondsBetweenUpdateMessage
        {
            get
            {
                return 4;
            }
        }

        public override void MoveOutsideWorld(Vector2 position, Vector2 movePosition)
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
}
