using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{

    public abstract class CompositePhysicalObject : PhysicalObject
    {

        private Leaf leaf;
        public void SetLeaf(Leaf leaf)
        {
            this.leaf = leaf;
        }

        abstract public new class State : PhysicalObject.State
        {
            //not part of the shared game state

            private Vector2 position = new Vector2(0);
            private float direction = 0;

            public void Initialize(Vector2 position, float direction)
            {
                this.Position = position;
                this.direction = direction;
            }


            public State(GameObject obj) : base(obj) { }

            public override void ApplyMessage(GameObjectUpdate message)
            {
                base.ApplyMessage(message);
                this.position = message.ReadVector2();
                this.direction = message.ReadFloat();
            }

            public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
            {
                message = base.ConstructMessage(message);
                message.Append(this.position);
                message.Append(direction);
                return message;
            }

            public override void Interpolate(GameObject.State d, GameObject.State s, float smoothing, GameObject.State blankState)
            {
                base.Interpolate(d, s, smoothing, blankState);
                CompositePhysicalObject.State myS = (CompositePhysicalObject.State)s;
                CompositePhysicalObject.State myD = (CompositePhysicalObject.State)d;
                CompositePhysicalObject.State myBlankState = (CompositePhysicalObject.State)blankState;

                Vector2 position = Vector2.Lerp(myS.position, myD.position, smoothing);
                float direction = Utils.Vector2Utils.Lerp(myS.direction, myD.direction, smoothing);


                myBlankState.position = position;
                myBlankState.direction = direction;
            }

            public override Vector2 WorldPosition()
            {
                return position;
            }

            public override float WorldDirection()
            {
                return direction;
            }

            public override void CommonUpdate(float seconds)
            {
                base.CommonUpdate(seconds);
                ((CompositePhysicalObject)this.Object).MoveInTree();
            }

            protected abstract void MoveOutsideWorld(Vector2 position, Vector2 movePosition);

            public virtual Vector2 Position
            {
                protected set
                {
                    if (!StaticGameObjectCollection.Collection.GetWorldRectangle().Contains(value))
                    {
                        MoveOutsideWorld(this.Position, value);
                    }
                    else
                    {
                        position = value;
                    }
                }
                get { return position; }
            }

            public float Direction
            {
                get { return direction; }
                set { direction = Utils.Vector2Utils.RestrictAngle(value); }
            }
        }

        public CompositePhysicalObject(GameObjectUpdate message) : base(message) { }
        public CompositePhysicalObject(Vector2 position, float direction) : base() 
        {
            CompositePhysicalObject.State myState = (CompositePhysicalObject.State)this.PracticalState;
            myState.Initialize(position, direction);
        }

        /*
        public void SetLeaf(Leaf leaf)
        {
            ((CompositePhysicalObject.State)this.PracticalState).SetLeaf(leaf);
        }*/

        public Vector2 Position
        {
            get { return ((CompositePhysicalObject.State)this.PracticalState).Position; }
        }

        public void MoveInTree()
        {
            leaf.Move(this);
        }

        public override PhysicalObject Root()
        {
            return this;
        }       
    }
}
