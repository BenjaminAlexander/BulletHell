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
            InstantSet instantSet = instantSets[instantId];

            int nonEmptyTypeCount = 0;
            foreach (InstantTypeSetInterface typeSet in instantSet)
            {
                if(typeSet.ObjectCount > 0)
                {
                    nonEmptyTypeCount++;
                }
            }

            int messageHeaderSize = sizeof(int) * 2;

            List<byte[]> buffers = new List<byte[]>();
            SerializationBuilder builder = new SerializationBuilder();

            int typeOffset = 0;
            SInteger typesInBufferCount = 0;
            builder.Append(instantId);
            builder.Append(typeOffset);
            builder.Append(nonEmptyTypeCount);
            builder.Append((Serializable)typesInBufferCount);

            foreach (InstantTypeSetInterface typeSet in instantSet)
            {
                if (typeSet.ObjectCount <= 0)
                {
                    continue;
                }

                bool typeHeaderAdded = false;
                SInteger objectsInBufferCount = 0;
                int objectOffset = 0;

                //TODO: need to add a type header here for types with zero objects
                //TODO: or do the same thing for types as objects


                foreach (GameObject obj in typeSet)
                {
                    Serializable serializable = obj.GetSerializable(instantId);
                    int typeHeaderSize = sizeof(int) * 4;
                    int objSize = serializable.SerializationSize + sizeof(int);

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
                        typesInBufferCount = 0;
                        objectsInBufferCount = 0;
                        builder.Append(instantId);
                        builder.Append(typeOffset);
                        builder.Append(nonEmptyTypeCount);
                        builder.Append((Serializable)typesInBufferCount);
                    }

                    if (!typeHeaderAdded)
                    {
                        builder.Append(typeSet.GetMetaData.TypeID);
                        builder.Append(objectOffset);
                        builder.Append(typeSet.ObjectCount);
                        builder.Append((Serializable)objectsInBufferCount);
                        typeHeaderAdded = true;
                        typesInBufferCount.Value++;
                    }
                    builder.Append(obj.ID);
                    builder.Append(serializable);
                    objectsInBufferCount.Value++;
                    objectOffset++;
                }
                typeOffset++;
            }
            buffers.Add(builder.Serialize());
            return buffers;
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int instantId;
            Serialization.Utils.Read(out instantId, buffer, ref bufferOffset);
            InstantSet instantSet = GetInstantSet(instantId);

            //TODO: do something with this return value
            instantSet.Deserialize(buffer, ref bufferOffset);
        }
    }
}
