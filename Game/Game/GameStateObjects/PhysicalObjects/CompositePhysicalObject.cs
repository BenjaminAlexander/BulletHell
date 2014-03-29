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
                    this.MoveOutsideWorld(this.Position, value);
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

        public abstract void MoveOutsideWorld(Vector2 position, Vector2 movePosition);

        public override void DrawSub(Microsoft.Xna.Framework.GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            base.DrawSub(gameTime, graphics);
            this.Collidable.Draw(graphics, this.Position, this.Direction);
        }

        public override void CommonUpdateSub(float seconds)
        {
            base.CommonUpdateSub(seconds);
            this.MoveInTree();
        }
    }
}
