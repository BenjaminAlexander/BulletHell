﻿using System;
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
        private ReferenceListField<MemberPhysicalObject> memberField;

        internal override void DefineFields(InitialInstant instant)
        {
            memberField = new ReferenceListField<MemberPhysicalObject>(instant);
        }

        public virtual void Add(NextInstant next, MemberPhysicalObject obj)
        {
            //GameObjectReferenceListField<MemberPhysicalObject> reflist = memberField[new NextInstant(new Instant(0))];
            //reflist.Add(obj);
            //memberField[new NextInstant(new Instant(0))] = reflist;
            List<MemberPhysicalObject> memberList = memberField.GetList(next);
            memberList.Add(obj);
            memberField.SetList(next, memberList);
        }

        public abstract Vector2 WorldPosition(CurrentInstant current);

        public abstract float WorldDirection(CurrentInstant current);

        public abstract CompositePhysicalObject Root(CurrentInstant current);
    }
}
