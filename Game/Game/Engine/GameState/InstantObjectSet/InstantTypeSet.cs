using MyGame.Engine.DataStructures;
using MyGame.Engine.GameState.GameObjectFactory;
using MyGame.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.TypeManager;
using System.Collections;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState.InstantObjectSet
{
    class InstantTypeSet<SubType> : InstantTypeSetInterface where SubType : GameObject, new()
    {
        private TypeSet<SubType> globalSet;
        private TwoWayMap<int, SubType> objects = new TwoWayMap<int, SubType>(new IntegerComparer());
        private int instantId;
        private Nullable<int> deserializedObjectCount = null;

        public InstantTypeSet(TypeSet<SubType> globalSet, int instantId)
        {
            this.globalSet = globalSet;
            this.instantId = instantId;
        }

        public ObjectTypeFactoryInterface NewObjectTypeFactory(InstantTypeSetInterface nextInstantTypeSet)
        {
            InstantTypeSet<SubType> next = (InstantTypeSet<SubType>)nextInstantTypeSet;
            return new ObjectTypeFactory<SubType>(globalSet, this, next);
        }

        //TODO: check if object is serialized and check against total object counts
        public void Add(GameObject obj)
        {
            objects[obj.ID] = (SubType)obj;
        }

        public bool SetDeserializedObjectCount(int count)
        {
            deserializedObjectCount = count;
            //TODO: do something with this info
            return false;
        }

        public SubType NewObject(int id)
        {
            SubType obj = globalSet[id];
            //TODO: need to call/not call set default based on seraluzatiion complsmvmsvmslv
            //TODO: same for add
            obj.SetDefaultValue(instantId);
            this.Add(obj);
            return obj;
        }

        public SubType GetObject(int id)
        {
            if (objects.ContainsKey(id))
            {
                return objects[id];
            }
            return null;
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            return objects.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int GreatestID
        {
            get
            {
                return objects.GreatestKey;
            }
        }

        public TypeMetadataInterface GetMetaData
        {
            get
            {
                return globalSet.GetMetaData;
            }
        }

        public int InstantID
        {
            get
            {
                return instantId;
            }
        }

        public int ObjectCount
        {
            get
            {
                return objects.Count;
            }
        }
    }
}
