using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.DrawingUtils;
using MyGame.GameStateObjects.QuadTreeUtils;
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
            damage = new IntegerGameObjectMember(this, 10);
            start = new Vector2GameObjectMember(this, new Vector2(0));
            range = new FloatGameObjectMember(this, 3000);
            owner = new GameObjectReferenceField<Ship>(this);
        }

        public static void ServerInitialize(Bullet obj, Ship owner, Vector2 position, float direction)
        {
            MovingGameObject.ServerInitialize(obj, position, Utils.Vector2Utils.ConstructVectorFromPolar(speed, direction) /*+ owner.Velocity*/, direction, 0, direction);

            obj.owner.Value = owner;
            obj.start.Value = position;

        }

        public void Hit()
        {
            if (owner.Value != null)
            {
                owner.Value.AddKill();
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
            if (obj == null)
            {
                return false;
            }
            return owner.Value == obj;
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
