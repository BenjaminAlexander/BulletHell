using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.GameServer;
using MyGame.GameClient;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.GameState;

namespace MyGame.GameStateObjects.PhysicalObjects
{
    public abstract class PhysicalObject : GameObject
    {
        private Field<GameObjectReferenceListField<MemberPhysicalObject>> memberField;

        public PhysicalObject()
        {
        }

        public PhysicalObject(Game1 game) : base(game)
        {
        }

        internal override void DefineFields(InitialInstant instant)
        {
            base.DefineFields(instant);
            memberField = new Field<GameObjectReferenceListField<MemberPhysicalObject>>(instant);
        }

        public virtual void Add(MemberPhysicalObject obj)
        {
            GameObjectReferenceListField<MemberPhysicalObject> reflist = memberField[new NextInstant(new Instant(0))];
            reflist.Add(obj);
            memberField[new NextInstant(new Instant(0))] = reflist;
        }

        public abstract Vector2 WorldPosition();

        public abstract float WorldDirection();

        public abstract CompositePhysicalObject Root();
    }
}
