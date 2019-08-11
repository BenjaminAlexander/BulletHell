using MyGame.Engine.GameState.Instants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState.GameObjectFactory
{
    class ObjectTypeFactory<SubType> : ObjectTypeFactoryInterface where SubType : GameObject, new()
    {
        private int typeId;
        private int nextId;
        private InstantTypeSet<SubType> next;

        public ObjectTypeFactory(int typeId, int nextId, InstantTypeSet<SubType> next)
        {
            this.typeId = typeId;
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
                return typeId;
            }
        }
    }
}
