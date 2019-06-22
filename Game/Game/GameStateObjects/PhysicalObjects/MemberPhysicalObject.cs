using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.GameServer;
using MyGame.GameClient;
using MyGame.Engine.GameState.Instants;

namespace MyGame.GameStateObjects.PhysicalObjects
{
    abstract public class MemberPhysicalObject : PhysicalObject
    {
        private InterpolatedVector2GameObjectMember positionRelativeToParent;
        private InterpolatedAngleGameObjectMember directionRelativeToParent;
        private GameObjectReferenceField<PhysicalObject> parent;

        public static void ServerInitialize(MemberPhysicalObject obj, PhysicalObject parent, Vector2 positionRelativeToParent, float directionRelativeToParent)
        {
            obj.positionRelativeToParent[new NextInstant(new Instant(0))] = positionRelativeToParent;
            obj.directionRelativeToParent[new NextInstant(new Instant(0))] = directionRelativeToParent;
            obj.parent.Value = parent;
            parent.Add(obj);
        }

        public MemberPhysicalObject(Game1 game)
            : base(game)
        {
            positionRelativeToParent = new InterpolatedVector2GameObjectMember(this, new Vector2(0));
            directionRelativeToParent = new InterpolatedAngleGameObjectMember(this, 0);
            parent = new GameObjectReferenceField<PhysicalObject>(this);
        }

        public virtual Vector2 PositionRelativeToParent
        {
            protected set { positionRelativeToParent[new NextInstant(new Instant(0))] = value; }
            get { return positionRelativeToParent[new NextInstant(new Instant(0))]; }
        }

        public virtual float DirectionRelativeToParent
        {
            protected set { directionRelativeToParent[new NextInstant(new Instant(0))] = value; }
            get { return directionRelativeToParent[new NextInstant(new Instant(0))]; }
        }

        public PhysicalObject Parent
        {
            get 
            {
                PhysicalObject p = parent.Value;
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
