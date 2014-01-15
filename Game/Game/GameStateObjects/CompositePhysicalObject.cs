using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects
{

    public abstract class CompositePhysicalObject : PhysicalObject
    {
        private Vector2 position = new Vector2(0);
        private float direction = 0;

        public CompositePhysicalObject(Vector2 position, float direction)
        {
            this.position = position;
            this.direction = direction;
        }

        public override Vector2 WorldPosition()
        {
            return Position;
        }

        public override float WorldDirection()
        {
            return Direction;
        }

        public Vector2 Position
        {
            protected set { position = value; }
            get { return position; }
        }

        public float Direction
        {
            protected set { direction = value; }
            get { return direction; }
        }

        public virtual Boolean IsFlyingGameObject
        {
            get { return false; }
        }

        public override Boolean IsCompositePhysicalObject
        {
            get { return true; }
        }
    }
}

