using MyGame.Engine.DataStructures;
using MyGame.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.TypeManager;
using System.Collections;
using MyGame.Engine.GameState.InstantObjectSet;
using MyGame.Engine.Serialization.DataTypes;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    //TODO: Serialize/deserialize
    //TODO: update
    //TODO: drop
    class ObjectInstantManager : IEnumerable<GameObject>, IEnumerable<TypeSetInterface>, IEnumerable<InstantSet>
    {
        private static Logger log = new Logger(typeof(ObjectInstantManager));

        private TypeManager typeManager;
        private TwoWayMap<int, TypeSetInterface> typeSets = new TwoWayMap<int, TypeSetInterface>(new IntegerComparer());
        //TODO: to deserialize go instant -> type -> id
        //TODO: instant, type, total count of type, offset of type, id, ....data...
        private TwoWayMap<int, InstantSet> instantSets = new TwoWayMap<int, InstantSet>(new IntegerComparer());

        public ObjectInstantManager(TypeManager typeManager)
        {
            this.typeManager = typeManager;
            foreach (TypeMetadataInterface metaData in typeManager)
            {
                typeSets.Set(metaData.TypeID, metaData.NewTypeSet());
            }
        }

        public TypeManager TypeManager
        {
            get
            {
                return typeManager;
            }
        }

        public InstantSet GetInstantSet(int instantId)
        {
            if (instantSets.ContainsKey(instantId))
            {
                return instantSets[instantId];
            }
            else
            {
                InstantSet instantSet = new InstantSet(this, instantId);
                instantSets[instantId] = instantSet;
                return instantSet;
            }
        }

        IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
        {
            IEnumerable<GameObject> enumerable = null;
            foreach(KeyValuePair<int, TypeSetInterface> pair in typeSets)
            {
                if(enumerable == null)
                {
                    enumerable = pair.Value;
                }
                else
                {
                    enumerable = Enumerable.Concat<GameObject>(enumerable, pair.Value);
                }
            }
            return enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TypeSetInterface>)this).GetEnumerator();
        }

        IEnumerator<TypeSetInterface> IEnumerable<TypeSetInterface>.GetEnumerator()
        {
            return typeSets.Values.GetEnumerator();
        }

        IEnumerator<InstantSet> IEnumerable<InstantSet>.GetEnumerator()
        {
            return instantSets.Values.GetEnumerator();
        }

        public List<byte[]> Serialize(int instantId, int maximumBufferSize)
        {
            int messageHeaderSize = sizeof(int) * 2;

            List<byte[]> buffers = new List<byte[]>();

            SInteger typeCount = 0;
            SerializationBuilder builder = new SerializationBuilder();
            builder.Append(instantId);
            builder.Append((Serializable)typeCount);

            foreach (InstantTypeSetInterface typeSet in instantSets[instantId])
            {
                bool typeHeaderAdded = false;
                SInteger objectCount = 0;

                foreach (GameObject obj in typeSet)
                {
                    int typeHeaderSize = sizeof(int) * 3;
                    int objSize = obj.SerializationSize(instantId) + sizeof(int);

                    if (messageHeaderSize + typeHeaderSize + objSize > maximumBufferSize)
                    {
                        throw new Exception("An object was too big to fit into the maximum buffer size");
                    }

                    //do we need to start a new builder
                    if ((typeHeaderAdded && objSize + builder.SerializationSize > maximumBufferSize) ||
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

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int instantId;
            int typeCount;
            Serialization.Utils.Read(out instantId, buffer, ref bufferOffset);
            Serialization.Utils.Read(out typeCount, buffer, ref bufferOffset);

            InstantSet instantSet = GetInstantSet(instantId);

            while (typeCount > 0)
            {
                //TODO: pass total type counts to instant
                int typeId;
                int totalObjectCountOfType;
                int objectCountInMessage;
                Serialization.Utils.Read(out typeId, buffer, ref bufferOffset);
                Serialization.Utils.Read(out totalObjectCountOfType, buffer, ref bufferOffset);
                Serialization.Utils.Read(out objectCountInMessage, buffer, ref bufferOffset);

                TypeSetInterface typeSet = typeSets[typeId];
                InstantTypeSetInterface instantTypeSet = instantSet.GetInstantTypeSet(typeId);
                instantTypeSet.SetDeserializedObjectCount(totalObjectCountOfType);
                while (objectCountInMessage > 0)
                {
                    int objectId;
                    Serialization.Utils.Read(out objectId, buffer, ref bufferOffset);
                    GameObject obj = typeSet.GetObject(objectId);
                    //TODO: do something with the return of this method
                    obj.Deserialize(instantTypeSet, buffer, ref bufferOffset);
                    objectCountInMessage--;
                }
                typeCount--;
            }
        }
    }
}
