using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.Networking;

namespace MyGame.GameStateObjects.PhysicalObjects
{
    abstract public class MemberPhysicalObject : PhysicalObject
    {
        private InterpolatedVector2GameObjectMember positionRelativeToParent = new InterpolatedVector2GameObjectMember(new Vector2(0));
        private InterpolatedAngleGameObjectMember directionRelativeToParent = new InterpolatedAngleGameObjectMember(0);
        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.AddField(positionRelativeToParent);
            this.AddField(directionRelativeToParent);
        }

        public virtual Vector2 PositionRelativeToParent
        {
            protected set { positionRelativeToParent.Value = value; }
            get { return positionRelativeToParent.Value; }
        }

        public virtual float DirectionRelativeToParent
        {
            protected set { directionRelativeToParent.Value = value; }
            get { return directionRelativeToParent.Value; }
        }

        public new class State : PhysicalObject.State
        {
            
            
            private GameObjectReferenceField<PhysicalObject> parent = new GameObjectReferenceField<PhysicalObject>(new GameObjectReference<PhysicalObject>(null));

            public State(GameObject obj) : base(obj) { }

            protected override void InitializeFields()
            {
                base.InitializeFields();
                this.AddField(parent);
                
            }

            public void Initialize(PhysicalObject parent, Vector2 positionRelativeToParent, float directionRelativeToParent)
            {
                Game1.AsserIsServer();
                this.parent.Value = new GameObjectReference<PhysicalObject>(parent);

            }

            public PhysicalObject Parent
            {
                get
                {
                    PhysicalObject p = ((PhysicalObject)parent.Value.Dereference());
                    return p; }
            }

            

            

            public virtual Vector2 WorldPosition()
            {
                if (parent == null)
                {
                    return new Vector2(0);
                }
                else
                {
                    PhysicalObject parentObj = (PhysicalObject)parent.Value.Dereference();
                    if (parentObj != null)
                    {
                        PhysicalObject.State parentState = parentObj.PracticalState<PhysicalObject.State>();
                        return Vector2Utils.RotateVector2(this.GetObject<MemberPhysicalObject>().positionRelativeToParent.Value, parentObj.WorldDirection()) + parentObj.WorldPosition();
                    }
                    else
                    {
                        return new Vector2(float.NaN);
                    }
                }
            }

            public virtual float WorldDirection()
            {
                PhysicalObject parentObj = (PhysicalObject)parent.Value.Dereference();
                if (parentObj != null)
                {
                    PhysicalObject.State parentState = parentObj.PracticalState<PhysicalObject.State>();
                    return this.GetObject<MemberPhysicalObject>().DirectionRelativeToParent + parentObj.WorldDirection();

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
            MemberPhysicalObject.State myState = this.PracticalState<MemberPhysicalObject.State>();
            this.positionRelativeToParent.Value = positionRelativeToParent;
            this.directionRelativeToParent.Value = directionRelativeToParent;
            myState.Initialize(parent, positionRelativeToParent, directionRelativeToParent);
            parent.Add(this);
        }

        public PhysicalObject Parent
        {
            get { return this.PracticalState<MemberPhysicalObject.State>().Parent; }
        }

        public override CompositePhysicalObject Root()
        {
            return this.PracticalState<MemberPhysicalObject.State>().Parent.Root();
        }

        public override Vector2 WorldPosition()
        {
            return this.PracticalState<MemberPhysicalObject.State>().WorldPosition();
        }


        public override float WorldDirection()
        {
            return this.PracticalState<MemberPhysicalObject.State>().WorldDirection();
        }
    }
}
