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
    class InstantSet
    {
        private TwoWayMap<int, InstantTypeSetInterface> typeSets = new TwoWayMap<int, InstantTypeSetInterface>();
        private TypeManager typeManager;
        private int instantId;
        private DeserializedTracker deserializedTracker = new DeserializedTracker();

        public InstantSet(TypeManager typeManager, int instantId)
        {
            this.typeManager = typeManager;
            this.instantId = instantId;
        }

        public void Add(TypeSetInterface typeSet)
        {
             typeSets[typeSet.TypeID] = typeSet.GetInstantTypeSetInterface(instantId);
        }

        public int InstantID
        {
            get
            {
                return instantId;
            }
        }

        public SubType GetObject<SubType>(int id) where SubType : GameObject, new()
        {
            int typeID = typeManager.GetMetaData<SubType>().TypeID;
            InstantTypeSet<SubType> instantTypeSet = (InstantTypeSet<SubType>)typeSets[typeID];
            return instantTypeSet.GetObject(id);
        }

        public InstantTypeSetInterface GetInstantTypeSet(int typeId)
        {
            return typeSets[typeId];
        }

        private bool RemoveAllObjects(int startTypeIdInclusive, int endTypeIdExcluseive)
        {
            bool isChanged = false;
            while(startTypeIdInclusive < endTypeIdExcluseive)
            {
                isChanged = isChanged | typeSets[startTypeIdInclusive].DeserializeRemoveAll();
                startTypeIdInclusive++;
            }
            return isChanged;
        }

        public List<byte[]> Serialize(int maximumBufferSize)
        {
            int nonEmptyTypeCount = 0;
            foreach (InstantTypeSetInterface typeSet in typeSets.Values)
            {
                if (typeSet.ObjectCount > 0)
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

            foreach (InstantTypeSetInterface typeSet in typeSets.Values)
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
                        builder.Append(typeSet.TypeID);
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

        public bool Deserialize(byte[] buffer, ref int bufferOffset)
        {
            bool isChanged = false;

            int typeOffset;
            int nonEmptyTypeCount;
            int typesInBufferCount;

            Serialization.Utils.Read(out typeOffset, buffer, ref bufferOffset);
            Serialization.Utils.Read(out nonEmptyTypeCount, buffer, ref bufferOffset);
            Serialization.Utils.Read(out typesInBufferCount, buffer, ref bufferOffset);

            deserializedTracker.SetCount(nonEmptyTypeCount);

            int? previousTypeId = null;
            int typeId = 0;

            while (typesInBufferCount > 0)
            {
                
                Serialization.Utils.Read(out typeId, buffer, ref bufferOffset);
                deserializedTracker.SetId(typeOffset, typeId);
                InstantTypeSetInterface instantTypeSet = typeSets[typeId];
                isChanged = isChanged | instantTypeSet.Deserialize(buffer, ref bufferOffset);

                if (typeOffset == 0)
                {
                    isChanged = isChanged | RemoveAllObjects(0, typeId);
                }
                else if(previousTypeId == null)
                {
                    previousTypeId = deserializedTracker.GetId(typeOffset - 1);
                }

                if(previousTypeId != null)
                {
                    isChanged = isChanged | RemoveAllObjects((int)previousTypeId + 1, typeId);
                }

                previousTypeId = typeId;
                typesInBufferCount--;
                typeOffset++;
            }

            if(typeOffset == nonEmptyTypeCount)
            {
                isChanged = isChanged | RemoveAllObjects(typeId + 1, typeSets.GreatestKey + 1);
            }
            else
            {
                int? nextTypeId = deserializedTracker.GetId(typeOffset + 1);
                if(nextTypeId != null)
                {
                    isChanged = isChanged | RemoveAllObjects(typeId + 1, (int)nextTypeId);
                }
            }

            return isChanged;
        }
    }
}
