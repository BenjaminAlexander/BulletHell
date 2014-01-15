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
        private List<MemberPhysicalObject> members = new List<MemberPhysicalObject>();

        public abstract Vector2 WorldPosition();

        public abstract float WorldDirection();

        public void Add(MemberPhysicalObject obj)
        {
            members.Add(obj);
            obj.Parent = this;
        }

        public void Remove(MemberPhysicalObject obj)
        {
            if (members.Contains(obj))
            {
                obj.Parent = null;
                members.Remove(obj);
            }
        }

        public override Boolean IsPhysicalObject
        {
            get { return true; }
        }

        public virtual Boolean IsCompositePhysicalObject
        {
            get { return false; }
        }

        public virtual Boolean IsMemberPhysicalObject
        {
            get { return false; }
        }

        /*
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
        }*/
    }
}
