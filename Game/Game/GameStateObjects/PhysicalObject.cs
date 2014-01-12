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
        Vector2 position = new Vector2(0);
        float direction = 0;

        public PhysicalObject(Vector2 position, float direction)
        {
            this.position = position;
            this.direction = direction;
        }

        public virtual Boolean IsFlyingGameObject
        {
            get { return false; }
        }

        public override Boolean IsPhysicalObject
        {
            get { return true; }
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
    }
}
