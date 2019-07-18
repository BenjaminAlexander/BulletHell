using MyGame.Engine.GameState.InstantObjectSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState.GameObjectFactory
{
    class ObjectTypeFactory<SubType> : ObjectTypeFactoryInterface where SubType : GameObject, new()
    {
        private TypeSet<SubType> globalSet;
        private int nextId;
        private InstantTypeSet<SubType> next;

        public ObjectTypeFactory(TypeSet<SubType> globalSet, InstantTypeSet<SubType> current, int nextInstantId)
        {
            this.globalSet = globalSet;
            this.nextId = current.GreatestID + 1;
            this.next = globalSet.GetInstantTypeSet(nextInstantId);
        }

        public SubType NewObject()
        {
            SubType obj = globalSet[nextId];
            obj.SetDefaultValue(next);
            nextId++;
            return obj;
        }
    }
}
