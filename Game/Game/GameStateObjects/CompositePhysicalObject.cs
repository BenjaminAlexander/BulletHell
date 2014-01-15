using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MyGame.GameStateObjects
{

    public abstract class CompositePhysicalObject : PhysicalObject
    {
        private List<PhysicalObject> members = new List<PhysicalObject>();

        public CompositePhysicalObject(Vector2 position, float direction)
            : base(position, direction)
        {
        }

        public void Add(PhysicalObject obj)
        {
            members.Add(obj);
            obj.Parent = this;
        }

        public void Remove(PhysicalObject obj)
        {
            if (members.Contains(obj))
            {
                obj.Parent = null;
                members.Remove(obj);
            }
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

