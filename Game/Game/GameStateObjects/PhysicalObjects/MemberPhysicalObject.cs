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

        protected override void InitializeFields()
        {
            base.InitializeFields();

            positionRelativeToParent = this.AddInterpolatedVector2GameObjectMember(new Vector2(0));
            directionRelativeToParent = this.AddInterpolatedAngleGameObjectMember(0);
            parent = this.AddGameObjectReferenceField<PhysicalObject>(new GameObjectReference<PhysicalObject>(null, this.Game.GameObjectCollection));
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

        public void MemberPhysicalObjectInit(ServerGame game, PhysicalObject parent, Vector2 positionRelativeToParent, float directionRelativeToParent)
        {
            base.PhysicalObjectInit(game);
            this.positionRelativeToParent.Value = positionRelativeToParent;
            this.directionRelativeToParent.Value = directionRelativeToParent;
            this.parent.Value = new GameObjectReference<PhysicalObject>(parent, this.Game.GameObjectCollection);
            parent.Add(this);
        }

        public PhysicalObject Parent
        {
            get { 
                PhysicalObject p = ((PhysicalObject)parent.Value.Dereference());
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
