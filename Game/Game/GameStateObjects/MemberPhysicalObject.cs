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
            private Boolean init = false;
            private Vector2 positionRelativeToParent = new Vector2(0);
            private float directionRelativeToParent = 0;
            private GameObjectReference<PhysicalObject> parent;// = new GameObjectReference<PhysicalObject>(null);

            public State(GameObject obj) : base(obj) { }

            public override void ApplyMessage(GameObjectUpdate message)
            {
                base.ApplyMessage(message);
                positionRelativeToParent = message.ReadVector2();
                directionRelativeToParent = message.ReadFloat();
                parent = message.ReadGameObjectReference<PhysicalObject>();
                init = true;
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
                Game1.AsserIsServer();
                this.parent = new GameObjectReference<PhysicalObject>(parent);
                this.positionRelativeToParent = positionRelativeToParent;
                this.directionRelativeToParent = directionRelativeToParent;
            }

            public override void Interpolate(GameObject.State d, GameObject.State s, float smoothing, GameObject.State blankState)
            {
                base.Interpolate(d, s, smoothing, blankState);
                MemberPhysicalObject.State myS = (MemberPhysicalObject.State)s;
                MemberPhysicalObject.State myD = (MemberPhysicalObject.State)d;
                MemberPhysicalObject.State myBlankState = (MemberPhysicalObject.State)blankState;

                Vector2 position = Vector2.Lerp(myS.positionRelativeToParent, myD.positionRelativeToParent, smoothing);
                float direction = Utils.Vector2Utils.Lerp(myS.directionRelativeToParent, myD.directionRelativeToParent, smoothing);

                myBlankState.positionRelativeToParent = position;
                myBlankState.directionRelativeToParent = direction;
                myBlankState.parent = myS.parent;
            }

            public PhysicalObject Parent
            {
                get
                {
                    PhysicalObject p = ((PhysicalObject)parent.Dereference());
                    return p; }
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
                else
                {
                    PhysicalObject parentObj = (PhysicalObject)parent.Dereference();
                    if (parentObj != null)
                    {
                        PhysicalObject.State parentState = (PhysicalObject.State)parentObj.PracticalState;
                        return Vector2Utils.RotateVector2(positionRelativeToParent, parentState.WorldDirection()) + parentState.WorldPosition();
                    }
                    else
                    {
                        return new Vector2(float.NaN);
                    }
                }
            }

            public override float WorldDirection()
            {
                PhysicalObject parentObj = (PhysicalObject)parent.Dereference();
                if (parentObj != null)
                {
                    PhysicalObject.State parentState = (PhysicalObject.State)parentObj.PracticalState;
                    return directionRelativeToParent + parentState.WorldDirection();
                }
                else
                {
                    return 0;
                }
            }
        }

        public MemberPhysicalObject(GameObjectUpdate message) : base(message) { }
        public MemberPhysicalObject(PhysicalObject parent, Vector2 positionRelativeToParent, float directionRelativeToParent)
            : base() 
        {
            MemberPhysicalObject.State myState = (MemberPhysicalObject.State)this.PracticalState;
            myState.Initialize(parent, positionRelativeToParent, directionRelativeToParent);
            parent.Add(this);
        }

        public PhysicalObject Parent
        {
            get { return ((MemberPhysicalObject.State)this.PracticalState).Parent; }
        }

        public override PhysicalObject Root()
        {
            return ((MemberPhysicalObject.State)this.PracticalState).Parent.Root();
        }
    }
}
