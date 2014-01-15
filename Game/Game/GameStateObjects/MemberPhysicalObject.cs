using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects
{
    abstract public class MemberPhysicalObject : PhysicalObject
    {
        private Vector2 positionRelativeToParent = new Vector2(0);
        private float directionRelativeToParent = 0;
        private PhysicalObject parent = null;

        public PhysicalObject Parent
        {
            set { parent = value; }
            get { return parent; }
        }

        public Vector2 PositionRelativeToParent
        {
            protected set { positionRelativeToParent = value; }
            get { return positionRelativeToParent; }
        }

        public float DirectionRelativeToParent
        {
            protected set { directionRelativeToParent = value; }
            get { return directionRelativeToParent; }
        }

        public virtual Boolean IsMemberPhysicalObject
        {
            get { return true; }
        }
    }
}
