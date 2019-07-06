using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.DrawingUtils;
using MyGame.GameServer;
using MyGame.GameClient;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.GameState.FieldValues;
using MyGame.Engine.GameState;

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

        private Field<Vector2Value> position;
        private Field<FloatValue> direction;


        internal override void DefineFields(InitialInstant instant)
        {
            base.DefineFields(instant);
            position = new Field<Vector2Value>(instant);
            direction = new Field<FloatValue>(instant);
        }

        public Field<Vector2Value> Position
        {
            get { return this.position; }
        }

        public Field<FloatValue> Direction
        {
            get { return direction; }
        }

        public override CompositePhysicalObject Root(CurrentInstant current)
        {
            return this;
        }

        public Boolean CollidesWith(CurrentInstant current, CompositePhysicalObject other)
        {
            return this.Collidable.CollidesWith(this.WorldPosition(current), this.WorldDirection(current), other.Collidable, other.WorldPosition(current), other.WorldDirection(current));
        }

        public override Vector2 WorldPosition(CurrentInstant current)
        {
            return this.Position[current];
        }


        public override float WorldDirection(CurrentInstant current)
        {
            return this.Direction[current];
        }

        public override void Draw(CurrentInstant current, DrawingUtils.MyGraphicsClass graphics)
        {
            base.Draw(current, graphics);
            this.Collidable.Draw(graphics, this.Position[current], this.Direction[current]);
        }
    }
}
