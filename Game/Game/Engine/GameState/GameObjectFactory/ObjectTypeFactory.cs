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

        public ObjectTypeFactory(TypeSet<SubType> globalSet, int nextId, InstantTypeSet<SubType> next)
        {
            this.globalSet = globalSet;
            this.nextId = nextId;
            this.next = next;
        }

        public SubType NewObject()
        {
            SubType obj = next.NewObject(nextId);
            nextId++;
            return obj;
        }

        public int TypeID
        {
            get
            {
                return globalSet.TypeID;
            }
        }
    }
}
