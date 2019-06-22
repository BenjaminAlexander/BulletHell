using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.GameStateObjects.QuadTreeUtils;
using MyGame.DrawingUtils;
using MyGame.GameServer;
using MyGame.GameClient;
using MyGame.Engine.GameState.Instants;

namespace MyGame.GameStateObjects.PhysicalObjects
{

    public abstract class CompositePhysicalObject : PhysicalObject
    {
        public static void ServerInitialize(CompositePhysicalObject obj, Vector2 position, float direction)
        {
            obj.position[new NextInstant(new Instant(0))] = position;
            obj.direction[new NextInstant(new Instant(0))] = direction;
        }

        public abstract Collidable Collidable
        {
            get;
        }

        private InterpolatedVector2GameObjectMember position;
        private InterpolatedAngleGameObjectMember direction;

        public CompositePhysicalObject(Game1 game)
            : base(game)
        {
            position = new InterpolatedVector2GameObjectMember(this, new Vector2(0));
            direction = new InterpolatedAngleGameObjectMember(this, 0);
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
                    position[new NextInstant(new Instant(0))] = value;
                    this.MoveInTree();
                }
            }
            get { return this.position[new NextInstant(new Instant(0))]; }
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
            get { return direction[new NextInstant(new Instant(0))]; }
            set { direction[new NextInstant(new Instant(0))] = Utils.Vector2Utils.RestrictAngle(value); }
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

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, DrawingUtils.MyGraphicsClass graphics)
        {
            base.Draw(gameTime, graphics);
            this.Collidable.Draw(graphics, this.Position, this.Direction);
        }
    }
}
