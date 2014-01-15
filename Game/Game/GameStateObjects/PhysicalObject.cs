using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;

namespace MyGame.GameStateObjects
{
    public abstract class PhysicalObject : GameObject
    {
        PhysicalObject parent = null;
        Vector2 position = new Vector2(0);
        float direction = 0;

        public PhysicalObject(Vector2 position, float direction)
        {
            this.position = position;
            this.direction = direction;
        }

        public override Boolean IsPhysicalObject
        {
            get { return true; }
        }

        public virtual Boolean IsCompositePhysicalObject
        {
            get { return false; }
        }

        public Vector2 Position
        {
            get { return position; }
            protected set { position = value; }
        }

        public float Direction
        {
            get { return direction; }
            protected set { direction = Vector2Utils.RestrictAngle(value); }
        }

        public PhysicalObject Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public virtual Vector2 PositionInWorld()
        {
            if (parent == null)
            {
                return position;
            }
            else
            {
                return position + parent.PositionInWorld();
            }
        }

        public virtual float DirectionInWorld()
        {
            if (parent == null)
            {
                return direction;
            }
            else
            {
                return direction + parent.DirectionInWorld();
            }
        }
    }
}
