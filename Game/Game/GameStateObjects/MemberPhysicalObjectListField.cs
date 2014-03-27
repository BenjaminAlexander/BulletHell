using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGame.GameStateObjects.PhysicalObjects;
using Microsoft.Xna.Framework;
using MyGame.Networking;

namespace MyGame.GameStateObjects
{
    class MemberPhysicalObjectListField : AbstractGameObjectMember<List<GameObjectReference<MemberPhysicalObject>>>
    {
        public MemberPhysicalObjectListField(List<GameObjectReference<MemberPhysicalObject>> v)
        {
            this.Value = v;
        }

        public override void ApplyMessage(GameObjectUpdate message)
        {
            message.Append(this.Value);
        }

        public override GameObjectUpdate ConstructMessage(GameObjectUpdate message)
        {
            this.Value = message.ReadGameObjectReferenceList<MemberPhysicalObject>();
            return message;
        }

        public override void Interpolate(IGameObjectMember d, IGameObjectMember s, float smoothing)
        {
            MemberPhysicalObjectListField myS = (MemberPhysicalObjectListField)s;
            MemberPhysicalObjectListField myD = (MemberPhysicalObjectListField)d;

            this.Value = myS.Value;
        }
    }
}
