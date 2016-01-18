using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.Networking;
using MyGame.GameServer;
using MyGame.GameClient;

namespace MyGame.GameStateObjects.PhysicalObjects
{
    abstract public class MemberPhysicalObject : PhysicalObject
    {
        private InterpolatedVector2GameObjectMember positionRelativeToParent;
        private InterpolatedAngleGameObjectMember directionRelativeToParent;
        private GameObjectReferenceField<PhysicalObject> parent;

        public static void ServerInitialize(MemberPhysicalObject obj, PhysicalObject parent, Vector2 positionRelativeToParent, float directionRelativeToParent)
        {
            obj.positionRelativeToParent.Value = positionRelativeToParent;
            obj.directionRelativeToParent.Value = directionRelativeToParent;
            obj.parent.Value = new GameObjectReference<PhysicalObject>(parent, obj.Game.GameObjectCollection);
            parent.Add(obj);
        }

        public MemberPhysicalObject(Game1 game)
            : base(game)
        {
            positionRelativeToParent = new InterpolatedVector2GameObjectMember(this, new Vector2(0));
            directionRelativeToParent = new InterpolatedAngleGameObjectMember(this, 0);
            parent = new GameObjectReferenceField<PhysicalObject>(this, this.Game.GameObjectCollection);
        }

        public virtual Vector2 PositionRelativeToParent
        {
            protected set { positionRelativeToParent.Value = value; }
            get { return positionRelativeToParent.Value; }
        }

        public virtual float DirectionRelativeToParent
        {
            protected set { directionRelativeToParent.Value = value; }
            get { return directionRelativeToParent.Value; }
        }

        public PhysicalObject Parent
        {
            get 
            { 
                PhysicalObject p = ((PhysicalObject)parent.Dereference());
                return p;
            }
        }

        public override CompositePhysicalObject Root()
        {
            return this.Parent.Root();
        }

        public override Vector2 WorldPosition()
        {
            if (this.Parent == null)
            {
                return new Vector2(0);
            }
            else
            {
                PhysicalObject parentObj = (PhysicalObject)this.Parent;
                if (parentObj != null)
                {
                    return Vector2Utils.RotateVector2(this.PositionRelativeToParent, parentObj.WorldDirection()) + parentObj.WorldPosition();
                }
                else
                {
                    return new Vector2(float.NaN);
                }
            }
        }


        public override float WorldDirection()
        {
            
            PhysicalObject parentObj = (PhysicalObject)this.Parent;
            if (parentObj != null)
            {
                return this.DirectionRelativeToParent + parentObj.WorldDirection();
            }
            else
            {
                return 0;
            }
        }
    }
}
