using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    abstract public class MemberPhysicalObject : PhysicalObject
    {
        public new class State : PhysicalObject.State
        {
            private Vector2 positionRelativeToParent = new Vector2(0);
            private float directionRelativeToParent = 0;
            private GameObjectReference<PhysicalObject> parent = new GameObjectReference<PhysicalObject>(null);

            public State(GameObject obj) : base(obj) { }

            public override void ApplyMessage(GameObjectUpdate message)
            {
                base.ApplyMessage(message);
                positionRelativeToParent = message.ReadVector2();
                directionRelativeToParent = message.ReadFloat();
                parent = message.ReadGameObjectReference<PhysicalObject>();
            }

            public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
            {
                message = base.ConstructMessage(message);
                message.Append(positionRelativeToParent);
                message.Append(directionRelativeToParent);
                message.Append(parent);
                return message;
            }

            public void Initialize(PhysicalObject parent, Vector2 positionRelativeToParent, float directionRelativeToParent)
            {
                Game1.AssertIsServer();
                this.parent = new GameObjectReference<PhysicalObject>(parent);
                this.positionRelativeToParent = positionRelativeToParent;
                this.directionRelativeToParent = directionRelativeToParent;
            }

            public PhysicalObject Parent
            {
                get { return ((PhysicalObject)parent.Dereference()); }
            }

            public Vector2 PositionRelativeToParent
            {
                protected set { positionRelativeToParent = value; }
                get { return positionRelativeToParent; }
            }

            public virtual float DirectionRelativeToParent
            {
                protected set { directionRelativeToParent = value; }
                get { return directionRelativeToParent; }
            }

            public override Vector2 WorldPosition()
            {
                if (parent == null)
                {
                    return new Vector2(0);
                }
                
                PhysicalObject parentObj = (PhysicalObject)parent.Dereference();
                if (parentObj != null)
                {
                    PhysicalObject.State parentState = (PhysicalObject.State)parentObj.GetState();
                    return Vector2Utils.RotateVector2(positionRelativeToParent, parentState.WorldDirection()) + parentState.WorldPosition();
                }
                
                return new Vector2(float.NaN);
            }

            public override float WorldDirection()
            {
                PhysicalObject parentObj = (PhysicalObject)parent.Dereference();
                if (parentObj != null)
                {
                    PhysicalObject.State parentState = (PhysicalObject.State)parentObj.GetState();
                    return directionRelativeToParent + parentState.WorldDirection();
                }
                return 0;
            }
        }

        public MemberPhysicalObject(GameObjectUpdate message) : base(message) { }
        public MemberPhysicalObject(PhysicalObject parent, Vector2 positionRelativeToParent, float directionRelativeToParent)
            : base() 
        {
            MemberPhysicalObject.State myState = (MemberPhysicalObject.State)this.GetState();
            myState.Initialize(parent, positionRelativeToParent, directionRelativeToParent);
            parent.Add(this);
        }

        public PhysicalObject Parent
        {
            get { return ((State)GetState()).Parent; }
        }

        public override PhysicalObject Root()
        {
            return ((State)GetState()).Parent.Root();
        }
    }
}
