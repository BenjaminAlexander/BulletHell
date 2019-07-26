using MyGame.Engine.DataStructures;
using MyGame.Engine.GameState.GameObjectFactory;
using MyGame.Engine.GameState.InstantObjectSet;
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

        private TwoWayMap<int, InstantTypeSet<SubType>> instantTypeSets = new TwoWayMap<int, InstantTypeSet<SubType>>(new IntegerComparer());
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
                    newObject.SetUp(id, this);
                    objects[id] = newObject;
                    return newObject;
                }
            }
        }

        public SubType GetObject(int id)
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

        public void PrepareForUpdate(int current, ObjectFactory factory)
        {
            InstantTypeSet<SubType> currentSet = GetInstantTypeSet(current);
            InstantTypeSet<SubType> nextSet = GetInstantTypeSet(current + 1);
            currentSet.PrepareForUpdate(nextSet);
            factory.AddTypeFactory<SubType>(new ObjectTypeFactory<SubType>(this, currentSet, nextSet));
        }

        public void Update(CurrentInstant current, NextInstant next)
        {
            InstantTypeSet<SubType> from = GetInstantTypeSet(current.InstantID);
            from.Update(current, next);
        }

        public IEnumerator<SubType> GetSubTypeEnumerator()
        {
            return objects.Values.GetEnumerator();
        }

        public InstantTypeSetInterface GetInstantTypeSetInterface(int instantId)
        {
            return GetInstantTypeSet(instantId);
        }

        public InstantTypeSet<SubType> GetInstantTypeSet(int instantId)
        {
            if (instantTypeSets.ContainsKey(instantId))
            {
                return instantTypeSets[instantId];
            }
            else
            {
                InstantTypeSet<SubType> instantTypeSet = new InstantTypeSet<SubType>(this, instantId);
                instantTypeSets[instantId] = instantTypeSet;
                return instantTypeSet;
            }
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
