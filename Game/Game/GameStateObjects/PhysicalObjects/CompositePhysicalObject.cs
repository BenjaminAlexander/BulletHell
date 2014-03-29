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

        private InterpolatedVector2GameObjectMember position = new InterpolatedVector2GameObjectMember(new Vector2(0));
        private InterpolatedAngleGameObjectMember direction = new InterpolatedAngleGameObjectMember(0);
        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.AddField(position);
            this.AddField(direction);
        }

        abstract public new class State : PhysicalObject.State
        {
            

            public State(GameObject obj) : base(obj) { }

            /*
            public float WorldDirection()
            {
                return this.GetObject<CompositePhysicalObject>().Direction;
            }*/

            public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
            {
                this.GetObject<CompositePhysicalObject>().Collidable.Draw(graphics, this.GetObject<CompositePhysicalObject>().Position, Direction);
            }

            public override void CommonUpdate(float seconds)
            {
                base.CommonUpdate(seconds);
                this.GetObject<CompositePhysicalObject>().MoveInTree();
            }

            public abstract void MoveOutsideWorld(Vector2 position, Vector2 movePosition);


            public float Direction
            {
                get { return this.GetObject<CompositePhysicalObject>().Direction; }
                set { this.GetObject<CompositePhysicalObject>().Direction = Utils.Vector2Utils.RestrictAngle(value); }
            }
        }

        public CompositePhysicalObject(GameObjectUpdate message) : base(message) { }
        public CompositePhysicalObject(Vector2 position, float direction) : base() 
        {
            CompositePhysicalObject.State myState = this.PracticalState<CompositePhysicalObject.State>();
            this.position.Value = position;
            this.direction.Value = direction;
        }

        public Vector2 Position
        {
            protected set
            {
                if (!StaticGameObjectCollection.Collection.GetWorldRectangle().Contains(value))
                {
                    this.PracticalState<CompositePhysicalObject.State>().MoveOutsideWorld(this.Position, value);
                }
                else
                {
                    position.Value = value;
                }
            }
            get { return this.position.Value; }
        }

        public float Direction
        {
            get { return direction.Value; }
            set { direction.Value = Utils.Vector2Utils.RestrictAngle(value); }
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
            return this.Collidable.CollidesWith(this.WorldPosition(), this.WorldDirection(), other.Collidable, other.WorldPosition(), other.WorldDirection());
        }

        public override Vector2 WorldPosition()
        {
            return this.Position;
        }


        public override float WorldDirection()
        {
            return this.Direction;
        }
    }
}
