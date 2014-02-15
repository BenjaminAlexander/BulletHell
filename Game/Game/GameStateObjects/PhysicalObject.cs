using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public abstract class PhysicalObject : GameObject
    {
        abstract public new class State : GameObject.State
        {
            private List<GameObjectReference<MemberPhysicalObject>> members = new List<GameObjectReference<MemberPhysicalObject>>();
            public State(GameObject obj) : base(obj) { }

            public override void ApplyMessage(GameObjectUpdate message)
            {
                base.ApplyMessage(message);
                this.members = message.ReadGameObjectReferenceList<MemberPhysicalObject>();
            }

            public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
            {
                message = base.ConstructMessage(message);
                message.Append(this.members);
                return message;
            }

            public override void Interpolate(GameObject.State d, GameObject.State s, float smoothing, GameObject.State blankState)
            {
                PhysicalObject.State myS = (PhysicalObject.State)s;
                PhysicalObject.State myD = (PhysicalObject.State)d;
                myD.members = myS.members;
            }

            public void Add(MemberPhysicalObject obj)
            {
                Game1.AsserIsServer();
                members.Add(new GameObjectReference<MemberPhysicalObject>(obj));
            }

            public abstract Vector2 WorldPosition();

            public abstract float WorldDirection();
        }

        public PhysicalObject(GameObjectUpdate message) : base(message) { }
        public PhysicalObject() : base() { }


        public abstract PhysicalObject Root();


        public void Add(MemberPhysicalObject obj)
        {
            Game1.AsserIsServer();
            ((PhysicalObject.State)this.PracticalState).Add(obj);
        }
    }
}
