using MyGame.Engine.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using MyGame.Engine.GameState.GameObjectFactory;
using MyGame.Engine.Serialization;
using MyGame.Engine.Serialization.DataTypes;

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
            foreach (TypeSetInterface globalTypeSet in (IEnumerable<TypeSetInterface>)globalSet)
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

        public ObjectFactory NewObjectFactory(InstantSet next)
        {
            return new ObjectFactory(this, next);
        }

        public InstantTypeSetInterface GetInstantTypeSet(int typeId)
        {
            return typeSets[typeId];
        }

        //TODO: unit Test
        //Instants that are bigger that max size
        //Types split over max size
        public List<byte[]> Serialize(int maximumBufferSize)
        {
            int messageHeaderSize = sizeof(int) * 2;

            List<byte[]> buffers = new List<byte[]>();

            SInteger typeCount = 0;
            SerializationBuilder builder = new SerializationBuilder();
            builder.Append(instantId);
            builder.Append((Serializable)typeCount);

            foreach (InstantTypeSetInterface typeSet in typeSets.Values)
            {
                bool typeHeaderAdded = false;
                SInteger objectCount = 0;

                foreach(GameObject obj in typeSet)
                {
                    int typeHeaderSize = sizeof(int) * 3;
                    int objSize = obj.SerializationSize(instantId) + sizeof(int);

                    if(messageHeaderSize + typeHeaderSize + objSize > maximumBufferSize)
                    {
                        throw new Exception("An object was too big to fit into the maximum buffer size");
                    }

                    //do we need to start a new builder
                    if((typeHeaderAdded && objSize + builder.SerializationSize > maximumBufferSize) ||
                        (!typeHeaderAdded && typeHeaderSize + objSize + builder.SerializationSize > maximumBufferSize))
                    {
                        buffers.Add(builder.Serialize());

                        builder = new SerializationBuilder();
                        typeHeaderAdded = false;
                        typeCount = 0;
                        objectCount = 0;
                        builder.Append(instantId);
                        builder.Append((Serializable)typeCount);
                    }

                    if (!typeHeaderAdded)
                    {
                        builder.Append(typeSet.GetMetaData.TypeID);
                        builder.Append(typeSet.ObjectCount);
                        builder.Append((Serializable)objectCount);
                        typeHeaderAdded = true;
                        typeCount.Value++;
                    }
                    builder.Append(obj.ID);
                    builder.Append(obj.GetSerializable(instantId));
                    objectCount.Value++;
                }
            }
            buffers.Add(builder.Serialize());
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
