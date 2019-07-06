using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MyGame.Utils;
using MyGame.GameServer;
using MyGame.GameClient;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.GameState.FieldValues;
using MyGame.Engine.GameState;

namespace MyGame.GameStateObjects.PhysicalObjects
{
    abstract public class MemberPhysicalObject : PhysicalObject
    {
        private Field<Vector2Value> positionRelativeToParent;
        private Field<FloatValue> directionRelativeToParent;
        private Field<GameObjectReference<PhysicalObject>> parent;

        public MemberPhysicalObject()
        {
        }

        public static void ServerInitialize(MemberPhysicalObject obj, PhysicalObject parent, Vector2 positionRelativeToParent, float directionRelativeToParent)
        {
            obj.positionRelativeToParent[new NextInstant(new Instant(0))] = positionRelativeToParent;
            obj.directionRelativeToParent[new NextInstant(new Instant(0))] = directionRelativeToParent;
            obj.parent[new NextInstant(new Instant(0))] = parent;
            parent.Add(obj);
        }

        public MemberPhysicalObject(Game1 game)
            : base(game)
        {
        }

        internal override void DefineFields(InitialInstant instant)
        {
            base.DefineFields(instant);
            positionRelativeToParent = new Field<Vector2Value>(instant);
            directionRelativeToParent = new Field<FloatValue>(instant);
            parent = new Field<GameObjectReference<PhysicalObject>>(instant);
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
                PhysicalObject p = parent[new NextInstant(new Instant(0))];
                return p;
            }
        }

        public override CompositePhysicalObject Root()
        {
            if(this.Parent == null)
            {
                return null;
            }
            return this.Parent.Root();
        }

        public override Vector2 WorldPosition(CurrentInstant current)
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
                    return Vector2Utils.RotateVector2(this.PositionRelativeToParent, parentObj.WorldDirection(current)) + parentObj.WorldPosition(current);
                }
                else
                {
                    return new Vector2(float.NaN);
                }
            }
        }


        public override float WorldDirection(CurrentInstant current)
        {
            
            PhysicalObject parentObj = (PhysicalObject)this.Parent;
            if (parentObj != null)
            {
                return this.DirectionRelativeToParent + parentObj.WorldDirection(current);
            }
            else
            {
                return 0;
            }
        }
    }
}
