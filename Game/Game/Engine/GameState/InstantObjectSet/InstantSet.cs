using MyGame.Engine.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using MyGame.Engine.GameState.GameObjectFactory;

namespace MyGame.Engine.GameState.InstantObjectSet
{
    class InstantSet : IEnumerable<InstantTypeSetInterface>
    {
        private TwoWayMap<int, InstantTypeSetInterface> typeSets;
        private TypeManager typeManager;
        private int instantId;

        public InstantSet(GlobalSet globalSet, int instantId)
        {
            this.typeManager = globalSet.TypeManager;
            this.instantId = instantId;

            typeSets = new TwoWayMap<int, InstantTypeSetInterface>();
            foreach (TypeSetInterface globalTypeSet in globalSet)
            {
                typeSets[globalTypeSet.GetMetaData.TypeID] = globalTypeSet.GetInstantTypeSetInterface(instantId);
            }
        }

        public int InstantID
        {
            get
            {
                return instantId;
            }
        }

        public TypeManager TypeManager
        {
            get
            {
                return typeManager;
            }
        }

        public ObjectFactory NewObjectFactory(int nextInstantId)
        {
            return new ObjectFactory(this, nextInstantId);
        }

        public IEnumerator<InstantTypeSetInterface> GetEnumerator()
        {
            return typeSets.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
