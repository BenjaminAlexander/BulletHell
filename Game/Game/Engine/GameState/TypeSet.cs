using MyGame.Engine.DataStructures;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.Reflection;
using MyGame.Engine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.TypeManager;

namespace MyGame.Engine.GameState
{
    class TypeSet<SubType> : TypeSetInterface where SubType : GameObject, new()
    {
        private static Logger log = new Logger(typeof(TypeSet<SubType>));

        private TwoWayMap<int, SubType> objects = new TwoWayMap<int, SubType>(new IntegerComparer());
        private TypeMetadata<SubType> metaData;

        public TypeSet(TypeMetadata<SubType> metaData)
        {
            this.metaData = metaData;
        }

        public SubType this[int id]
        {
            get
            {
                if (objects.ContainsKey(id))
                {
                    return objects[id];
                }
                else
                {
                    SubType newObject = new SubType();
                    newObject.SetUp(id);
                    objects[id] = newObject;
                    return newObject;
                }
            }
        }

        public GameObject GetObject(int id)
        {
            return this[id];
        }

        public TypeMetadataInterface GetMetaData
        {
            get
            {
                return metaData;
            }
        }

        public int Count
        {
            get
            {
                return objects.Count;
            }
        }

        public bool Contains(SubType item)
        {
            return objects.ContainsValue(item);
        }

        public bool Contains(int id)
        {
            return objects.ContainsKey(id);
        }

        public IEnumerator<SubType> GetSubTypeEnumerator()
        {
            return objects.Values.GetEnumerator();
        }

        public bool CheckIntegrety()
        {
            foreach (KeyValuePair<int, SubType> pair in objects)
            {
                if (pair.Key != pair.Value.ID)
                {
                    log.Error("Object ID does not match object key in map");
                    return false;
                }
            }
            return true;
        }

        public InstantTypeSetInterface NewInstantTypeSet()
        {
            return new InstantTypeSet<SubType>(this);
        }

        IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
        {
            return objects.Values.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
