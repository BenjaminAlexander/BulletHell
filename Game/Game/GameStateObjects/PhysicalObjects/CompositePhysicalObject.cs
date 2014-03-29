using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.Networking;
using MyGame.DrawingUtils;

namespace MyGame.GameStateObjects.PhysicalObjects
{

    public abstract class CompositePhysicalObject : PhysicalObject
    {
        public abstract Collidable Collidable
        {
            get;
        }

        abstract public new class State : PhysicalObject.State
        {
            private InterpolatedVector2GameObjectMember position = new InterpolatedVector2GameObjectMember(new Vector2(0));
            private InterpolatedAngleGameObjectMember direction = new InterpolatedAngleGameObjectMember(0);

            protected override void InitializeFields()
            {
                base.InitializeFields();
                this.AddField(position);
                this.AddField(direction);
            }

            public void Initialize(Vector2 position, float direction)
            {
                this.Position = position;
                this.direction.Value = direction;
            }

            public State(GameObject obj) : base(obj) { }

            public Vector2 WorldPosition()
            {
                return position.Value;
            }

            public float WorldDirection()
            {
                return direction.Value;
            }

            public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
            {
                this.GetObject<CompositePhysicalObject>().Collidable.Draw(graphics, this.Position, Direction);
            }

            public override void CommonUpdate(float seconds)
            {
                base.CommonUpdate(seconds);
                this.GetObject<CompositePhysicalObject>().MoveInTree();
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
                        position.Value = value;
                    }
                }
                get { return position.Value; }
            }

            public float Direction
            {
                get { return direction.Value; }
                set { direction.Value = Utils.Vector2Utils.RestrictAngle(value); }
            }
        }

        public CompositePhysicalObject(GameObjectUpdate message) : base(message) { }
        public CompositePhysicalObject(Vector2 position, float direction) : base() 
        {
            CompositePhysicalObject.State myState = this.PracticalState<CompositePhysicalObject.State>();
            myState.Initialize(position, direction);
        }

        public Vector2 Position
        {
            get { return this.PracticalState<CompositePhysicalObject.State>().Position; }
        }

        public float Direction
        {
            get { return this.PracticalState<CompositePhysicalObject.State>().Direction; }
        }

        public void MoveInTree()
        {
            StaticGameObjectCollection.Collection.Move(this);
        }

        public override CompositePhysicalObject Root()
        {
            return this;
        }

        public Boolean CollidesWith(CompositePhysicalObject other)
        {
            CompositePhysicalObject.State thisState = this.PracticalState<CompositePhysicalObject.State>();
            CompositePhysicalObject.State otherState = other.PracticalState<CompositePhysicalObject.State>();
            return this.Collidable.CollidesWith(thisState.WorldPosition(), thisState.WorldDirection(), other.Collidable, otherState.WorldPosition(), otherState.WorldDirection());
        }

        public override Vector2 WorldPosition()
        {
            return this.PracticalState<CompositePhysicalObject.State>().WorldPosition();
        }


        public override float WorldDirection()
        {
            return this.PracticalState<CompositePhysicalObject.State>().WorldDirection();
        }
    }
}
