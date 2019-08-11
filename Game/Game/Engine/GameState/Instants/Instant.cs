﻿using MyGame.Engine.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using MyGame.Engine.GameState.GameObjectFactory;
using MyGame.Engine.Serialization;
using MyGame.Engine.Serialization.DataTypes;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.GameState.Utils;
using MyGame.Engine.GameState.TypeSets;

namespace MyGame.Engine.GameState.Instants
{
    class Instant
    {
        private TwoWayMap<int, InstantTypeSetInterface> typeSets = new TwoWayMap<int, InstantTypeSetInterface>();
        private TypeManager typeManager;
        private int instantId;
        private DeserializedTracker deserializedTracker = new DeserializedTracker();

        public Instant(TypeManager typeManager, int instantId)
        {
            this.typeManager = typeManager;
            this.instantId = instantId;
        }

        public void Add(TypeSetInterface typeSet)
        {
             typeSets[typeSet.TypeID] = typeSet.NewInstantTypeSetInterface(instantId);
        }

        public int InstantID
        {
            get
            {
                return instantId;
            }
        }

        private bool RemoveAllObjects(int startTypeIdInclusive, int endTypeIdExcluseive)
        {
            bool isChanged = false;
            while (startTypeIdInclusive < endTypeIdExcluseive)
            {
                isChanged = isChanged | typeSets[startTypeIdInclusive].DeserializeRemoveAll();
                startTypeIdInclusive++;
            }
            return isChanged;
        }

        public GameObject GetObject(int typeId, int id)
        {
            lock (deserializedTracker)
            {
                if (id == 0)
                {
                    return null;
                }
                InstantTypeSetInterface instantTypeSet = typeSets[typeId];
                return instantTypeSet.GetObject(id);
            }
        }

        public NextInstant Update(Instant toInstant)
        {
            lock (deserializedTracker)
            {
                ObjectFactory factory = new ObjectFactory(typeManager);

                foreach (KeyValuePair<int, InstantTypeSetInterface> pair in this.typeSets)
                {
                    ObjectTypeFactoryInterface typeFactory = pair.Value.PrepareForUpdate(toInstant.typeSets[pair.Key]);
                    factory.AddTypeFactory(typeFactory);
                }

                CurrentInstant current = new CurrentInstant(this);
                NextInstant next = new NextInstant(toInstant, factory);

                foreach (InstantTypeSetInterface typeSet in typeSets.Values)
                {
                    typeSet.Update(current, next);
                }

                return next;
            }
        }

        public void Draw()
        {
            CurrentInstant current = new CurrentInstant(this);
            foreach (InstantTypeSetInterface typeSet in typeSets.Values)
            {
                typeSet.Draw(current);
            }
        }

        public List<byte[]> Serialize(int maximumBufferSize)
        {
            lock (deserializedTracker)
            {
                Dictionary<int, List<SerializationBuilder>> typeSerizalizations = new Dictionary<int, List<SerializationBuilder>>();
                int nonEmptyTypeCount = 0;
                foreach (KeyValuePair<int, InstantTypeSetInterface> pair in typeSets)
                {
                    List<SerializationBuilder> gameObjects = pair.Value.Serialize();
                    typeSerizalizations[pair.Key] = gameObjects;
                    if (gameObjects.Count > 0)
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

                foreach (KeyValuePair<int, List<SerializationBuilder>> pair in typeSerizalizations)
                {
                    int typeID = pair.Key;
                    List<SerializationBuilder> gameObjects = pair.Value;

                    if (gameObjects.Count <= 0)
                    {
                        continue;
                    }

                    bool typeHeaderAdded = false;
                    SInteger objectsInBufferCount = 0;
                    int objectOffset = 0;

                    //TODO: need to add a type header here for types with zero objects
                    //TODO: or do the same thing for types as objects


                    foreach (SerializationBuilder gameObject in gameObjects)
                    {
                        //SerializableBuffer serializable = obj.Serialize(instantId);
                        int typeHeaderSize = sizeof(int) * 4;
                        int objSize = gameObject.SerializationSize;

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
                            builder.Append(typeID);
                            builder.Append(objectOffset);
                            builder.Append(gameObjects.Count);
                            builder.Append((Serializable)objectsInBufferCount);
                            typeHeaderAdded = true;
                            typesInBufferCount.Value++;
                        }
                        builder.Append(gameObject);
                        objectsInBufferCount.Value++;
                        objectOffset++;
                    }
                    typeOffset++;
                }
                buffers.Add(builder.Serialize());
                return buffers;
            }
        }

        public bool Deserialize(byte[] buffer, ref int bufferOffset)
        {
            lock (deserializedTracker)
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
                    else if (previousTypeId == null)
                    {
                        previousTypeId = deserializedTracker.GetId(typeOffset - 1);
                    }

                    if (previousTypeId != null)
                    {
                        isChanged = isChanged | RemoveAllObjects((int)previousTypeId + 1, typeId);
                    }

                    previousTypeId = typeId;
                    typesInBufferCount--;
                    typeOffset++;
                }

                if (typeOffset == nonEmptyTypeCount)
                {
                    isChanged = isChanged | RemoveAllObjects(typeId + 1, typeSets.GreatestKey + 1);
                }
                else
                {
                    int? nextTypeId = deserializedTracker.GetId(typeOffset + 1);
                    if (nextTypeId != null)
                    {
                        isChanged = isChanged | RemoveAllObjects(typeId + 1, (int)nextTypeId);
                    }
                }

                return isChanged;
            }
        }
    }
}