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

        public PhysicalObject(Game1 game) : base(game)
        {
            memberField = new GameObjectReferenceListField<MemberPhysicalObject>(this); 
        }

        public virtual void Add(MemberPhysicalObject obj)
        {
            memberField.Value.Add(obj);
        }

        public abstract Vector2 WorldPosition();

        public abstract float WorldDirection();

        public override void Destroy()
        {
            base.Destroy();
            foreach (MemberPhysicalObject mem in memberField.Value)
            {
                mem.Destroy();
            }
        }

        public abstract CompositePhysicalObject Root();
    }
}
