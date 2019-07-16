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

        public ObjectTypeFactory(TypeSet<SubType> globalSet, int nextId)
        {
            this.globalSet = globalSet;
            this.nextId = nextId;
        }

        public SubType NewObject()
        {
            SubType obj = this.globalSet[nextId];
            nextId++;
            return obj;
        }
    }
}
