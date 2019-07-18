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

        public InstantSet(ObjectInstantManager globalSet, int instantId)
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

        public SubType GetObject<SubType>(int id) where SubType : GameObject, new()
        {
            int typeID = typeManager.GetMetaData<SubType>().TypeID;
            InstantTypeSet<SubType> instantTypeSet = (InstantTypeSet<SubType>)typeSets[typeID];
            return instantTypeSet.GetObject(id);
        }

        public void Add(GameObject obj)
        {
            int typeId = obj.TypeID;
            typeSets[typeId].Add(obj);
        }

        public ObjectFactory NewObjectFactory(int nextInstantId)
        {
            return new ObjectFactory(this, nextInstantId);
        }

        public List<byte[]> Serialize(int maximumBufferSize)
        {
            //use a serializable
            List<int> bufferSizes = new List<int>();
            bufferSizes.Add(0);

            foreach (KeyValuePair<int, InstantTypeSetInterface> typeSet in typeSets)
            {
                bufferSizes(bufferSizes);
            }
            List<byte[]> buffers = new List<byte[]>();

            //account for instantId
            int bufferLengthLeft = maximumBufferSize - sizeof(int);
            List<KeyValuePair<int, InstantTypeSetInterface>> queue = new List<KeyValuePair<int, InstantTypeSetInterface>>();
            foreach(KeyValuePair<int, InstantTypeSetInterface> typeSet in typeSets)
            {
                //account for typeId, total count, offset
                bufferLengthLeft = bufferLengthLeft - sizeof(int)
            }
            //for each type
                //

            //TODO: serialize here
            return buffers;
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
