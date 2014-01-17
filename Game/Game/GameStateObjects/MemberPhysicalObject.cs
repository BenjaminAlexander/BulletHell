using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;

namespace MyGame.GameStateObjects
{
    abstract public class MemberPhysicalObject : PhysicalObject
    {
        private Vector2 positionRelativeToParent = new Vector2(0);
        private float directionRelativeToParent = 0;
        private PhysicalObject parent = null;

        public MemberPhysicalObject(GameState gameState, PhysicalObject parent, Vector2 positionRelativeToParent, float directionRelativeToParent) : base(gameState)
        {
            this.parent = parent;
            this.positionRelativeToParent = positionRelativeToParent;
            this.directionRelativeToParent = directionRelativeToParent;
            this.parent.Add(this);
        } 

        public PhysicalObject Parent
        {
            //set { parent = value; }
            get { return parent; }
        }

        public override PhysicalObject Root()
        {
            return parent.Root();
        }

        public Vector2 PositionRelativeToParent
        {
            protected set { positionRelativeToParent = value; }
            get { return positionRelativeToParent; }
        }

        public virtual float DirectionRelativeToParent
        {
            protected set { directionRelativeToParent = value; }
            get { return directionRelativeToParent; }
        }

        public virtual Boolean IsMemberPhysicalObject
        {
            get { return true; }
        }

        public override Vector2 WorldPosition()
        {
            if (parent == null)
            {
                return new Vector2(0);
            }
            else
            {
                return Vector2Utils.RotateVector2(positionRelativeToParent, parent.WorldDirection()) + parent.WorldPosition();
            }
        }

        public override float WorldDirection()
        {
            if (parent == null)
            {
                return 0;
            }
            else
            {
                return directionRelativeToParent + parent.WorldDirection();
            }
        }
    }
}
