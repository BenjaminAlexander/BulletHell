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
        private ReferenceField<PhysicalObject> parent;

        public static void ServerInitialize(NextInstant next, MemberPhysicalObject obj, PhysicalObject parent, Vector2 positionRelativeToParent, float directionRelativeToParent)
        {
            obj.positionRelativeToParent[next] = positionRelativeToParent;
            obj.directionRelativeToParent[next] = directionRelativeToParent;
            obj.parent[next] = parent;
            parent.Add(next, obj);
        }

        internal override void DefineFields(CreationToken creationToken)
        {
            base.DefineFields(creationToken);
            positionRelativeToParent = new Field<Vector2Value>(creationToken);
            directionRelativeToParent = new Field<FloatValue>(creationToken);
            parent = new ReferenceField<PhysicalObject>(creationToken);
        }

        public Field<Vector2Value> PositionRelativeToParent
        {
            get { return positionRelativeToParent; }
        }

        public virtual Field<FloatValue> DirectionRelativeToParent
        {
            get { return directionRelativeToParent; }
        }

        public ReferenceField<PhysicalObject> Parent
        {
            get 
            {
                return parent;
            }
        }

        public override CompositePhysicalObject Root(CurrentInstant current)
        {
            if(this.Parent[current] == null)
            {
                return null;
            }
            return this.Parent[current].Root(current);
        }

        public override Vector2 WorldPosition(CurrentInstant current)
        {
            if (this.Parent == null)
            {
                return new Vector2(0);
            }
            else
            {
                PhysicalObject parentObj = (PhysicalObject)this.Parent[current];
                if (parentObj != null)
                {
                    return Vector2Utils.RotateVector2(this.PositionRelativeToParent[current], parentObj.WorldDirection(current)) + parentObj.WorldPosition(current);
                }
                else
                {
                    return new Vector2(float.NaN);
                }
            }
        }


        public override float WorldDirection(CurrentInstant current)
        {
            
            PhysicalObject parentObj = (PhysicalObject)this.Parent[current];
            if (parentObj != null)
            {
                return this.DirectionRelativeToParent[current] + parentObj.WorldDirection(current);
            }
            else
            {
                return 0;
            }
        }
    }
}
