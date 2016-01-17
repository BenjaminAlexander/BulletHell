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
using MyGame.GameServer;
using MyGame.GameClient;

namespace MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Bullets
{
    public class Bullet : MovingGameObject
    {
        private static Collidable collidable = new Collidable(TextureLoader.GetTexture("Bullet"), Color.White, new Vector2(20, 5), 1);
        public override Collidable Collidable
        {
            get { return collidable; }
        }

        private static float speed = 2000;
        public static float MaxRadius
        {
            get { return 50;}
        }

        private IntegerGameObjectMember damage;
        private Vector2GameObjectMember start;
        private FloatGameObjectMember range;
        private GameObjectReferenceField<Ship> owner;

        public Bullet(Game1 game)
            : base(game)
        {
            damage = this.AddIntegerGameObjectMember(10);
            start = this.AddVector2GameObjectMember(new Vector2(0));
            range = this.AddFloatGameObjectMember(3000);
            owner = this.AddGameObjectReferenceField<Ship>(new GameObjectReference<Ship>(null, this.Game.GameObjectCollection));
        }

        public void BulletInit(Ship owner, Vector2 position, float direction)
        {
            base.MovingGameObjectInit(position, Utils.Vector2Utils.ConstructVectorFromPolar(speed, direction) /*+ owner.Velocity*/, direction, 0, direction);
        
            this.owner.Value = new GameObjectReference<Ship>(owner, this.Game.GameObjectCollection);
            this.start.Value = position;

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
            this.Destroy();
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

        public override void SubclassUpdate(float seconds)
        {
            base.SubclassUpdate(seconds);
        }

        public override void SimulationStateOnlyUpdate(float seconds)
        {
            base.SimulationStateOnlyUpdate(seconds);
            if (Vector2.Distance(this.start.Value, this.Position) > this.range.Value)
            {
                this.Destroy();
            }
        }
    }
}
