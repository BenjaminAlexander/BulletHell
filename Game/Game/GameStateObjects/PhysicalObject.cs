using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;
using MyGame.Utils;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    public abstract class PhysicalObject : GameObject
    {
        // A Collidable that is the root texture of this physical object.
        private static Collidable collidable;
        
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
                State myS = (State)s;
                State myD = (State)d;
                myD.members = myS.members;
            }

            public void Add(MemberPhysicalObject obj)
            {
                Game1.AssertIsServer();
                members.Add(new GameObjectReference<MemberPhysicalObject>(obj));
            }

            public List<GameObjectReference<MemberPhysicalObject>> GetMembers()
            {
                return members;
            }

            public abstract Vector2 WorldPosition();

            public abstract float WorldDirection();
        }

        // Constructor from an update message.
        public PhysicalObject(GameObjectUpdate message) : base(message) { }

        // Default constructor.
        public PhysicalObject() : base() { }

        // Gets the root node of this physical object (follows parent up until it hits the root).
        public abstract PhysicalObject Root();

        // Adds a member physical object as a member of this physical object.
        public void Add(MemberPhysicalObject obj)
        {
            Game1.AssertIsServer();
            ((State)GetState()).Add(obj);
        }

        // Sets the collidable of the PhysicalObject (useful for if we ever want to change textures on the fly.)
        public void SetCollidable(Collidable c)
        {
            collidable = c;
        }

        public Collidable GetCollidable()
        {
            return collidable;
        }

        //public Boolean CollidesWith(PhysicalObject other)
        //{
        //    //State state = (State)GetState();
        //    //State otherState = (State) other.GetState();
        //    //foreach (GameObjectReference<MemberPhysicalObject> member in state.GetMembers())
        //    //{
        //    //    if (member.CanDereference())
        //    //    {
        //    //        MemberPhysicalObject obj = member.Dereference();
        //    //        obj.
        //    //    }
        //    //    else
        //    //    {
        //    //        throw new Exception("Cannot dereference member.");
        //    //    }
        //    //}
        //    //return collidable.CollidesWith(state.WorldPosition(), state.WorldDirection(), other.GetCollidable(),
        //    //    otherState.WorldPosition(), otherState.WorldDirection());
        //}
    }
}
