using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.Networking;
using MyGame.DrawingUtils;
using MyGame.GameServer;
using MyGame.GameClient;

namespace MyGame.GameStateObjects.PhysicalObjects
{

    public abstract class CompositePhysicalObject : PhysicalObject
    {
        public abstract Collidable Collidable
        {
            get;
        }

        private InterpolatedVector2GameObjectMember position;
        private InterpolatedAngleGameObjectMember direction;

        public CompositePhysicalObject(Game1 game)
            : base(game)
        {
            position = this.AddInterpolatedVector2GameObjectMember(new Vector2(0));
            direction = this.AddInterpolatedAngleGameObjectMember(0);
        }

        public void CompositePhysicalObjectInit(Vector2 position, float direction)
        {
            base.PhysicalObjectInit();
            this.position.Value = position;
            this.direction.Value = direction;
        }

        public Vector2 Position
        {
            protected set
            {
                if (!this.Game.GameObjectCollection.GetWorldRectangle().Contains(value))
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

        public Vector2 DrawPosition
        {
            get
            {
                Vector2 val = this.Position;
                return val;
            }
        }

        public float Direction
        {
            get { return direction.Value; }
            set { direction.Value = Utils.Vector2Utils.RestrictAngle(value); }
        }

        public void MoveInTree()
        {
            this.Game.GameObjectCollection.Tree.Move(this);
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

        public override void SimulationStateOnlyUpdate(float seconds)
        {
            base.SimulationStateOnlyUpdate(seconds);
            this.MoveInTree();
        }
    }
}
