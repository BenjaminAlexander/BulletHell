using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.Networking;
using MyGame.GameStateObjects.PhysicalObjects.MovingGameObjects.Bullets;
using MyGame.GameServer;
using MyGame.GameClient;

namespace MyGame.GameStateObjects.PhysicalObjects
{
    public abstract class PhysicalObject : GameObject
    {
        private GameObjectReferenceListField<MemberPhysicalObject> memberField;

        protected override void InitializeFields()
        {
            base.InitializeFields();

            memberField = this.AddGameObjectReferenceListField<MemberPhysicalObject>();
        }

        public virtual void Add(MemberPhysicalObject obj)
        {
            memberField.Value.Add(new GameObjectReference<MemberPhysicalObject>(obj, this.Game.GameObjectCollection));
        }

        public abstract Vector2 WorldPosition();

        public abstract float WorldDirection();

        public override void Destroy()
        {
            base.Destroy();
            foreach (GameObjectReference<MemberPhysicalObject> mem in memberField.Value)
            {
                if (mem.CanDereference())
                {
                    mem.Dereference().Destroy();
                }
            }
        }

        public PhysicalObject(ClientGame game, GameObjectUpdate message) : base(game, message) 
        {
            memberField = new GameObjectReferenceListField<MemberPhysicalObject>(this, new List<GameObjectReference<MemberPhysicalObject>>(), game.GameObjectCollection);
        }
        public PhysicalObject(ServerGame game) : base(game)
        {
            memberField = new GameObjectReferenceListField<MemberPhysicalObject>(this, new List<GameObjectReference<MemberPhysicalObject>>(), game.GameObjectCollection);
        }


        public abstract CompositePhysicalObject Root();

    }
}
