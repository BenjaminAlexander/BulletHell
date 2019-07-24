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
        private DeserializedTracker deserializedTracker = new DeserializedTracker();

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

        public IEnumerator<InstantTypeSetInterface> GetEnumerator()
        {
            return typeSets.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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

            while (typesInBufferCount > 0)
            {
                int typeId;
                Serialization.Utils.Read(out typeId, buffer, ref bufferOffset);
                deserializedTracker.SetId(typeOffset, typeId);
                InstantTypeSetInterface instantTypeSet = typeSets[typeId];
                isChanged = isChanged | instantTypeSet.Deserialize(buffer, ref bufferOffset);
                typesInBufferCount--;
                typeOffset++;
            }

            int trackerIndex = 0;
            int typeSetIndex = 0;
            InstantTypeSetInterface typeSet;
            if (typeSetIndex < typeSets.Count)
            {
                typeSet = typeSets.GetValueByIndex(typeSetIndex);
            }
            else
            {
                return isChanged;
            }

            while(trackerIndex < deserializedTracker.Count)
            {
                while (trackerIndex < deserializedTracker.Count && deserializedTracker.GetId(trackerIndex) == null)
                {
                    trackerIndex++;
                }

                if(trackerIndex >= deserializedTracker.Count)
                {
                    return isChanged;
                }

                if(trackerIndex == 0)
                {
                    //wipe typeset IDs less than deserializedTracker.GetId(trackerIndex)
                    while (typeSet.TypeID < deserializedTracker.GetId(trackerIndex))
                    {
                        isChanged = isChanged | typeSet.DeserializeRemoveAll();
                        typeSetIndex++;
                        if (typeSetIndex < typeSets.Count)
                        {
                            typeSet = typeSets.GetValueByIndex(typeSetIndex);
                        }
                        else
                        {
                            return isChanged;
                        }
                    }
                }
                if (trackerIndex + 1 < deserializedTracker.Count)
                {
                    if (deserializedTracker.GetId(trackerIndex + 1) != null)
                    {
                        //wipe greater than deserializedTracker.GetId(trackerIndex) and less than deserializedTracker.GetId(trackerIndex + 1)
                        while (typeSet.TypeID <= deserializedTracker.GetId(trackerIndex))
                        {
                            typeSetIndex++;
                            if (typeSetIndex < typeSets.Count)
                            {
                                typeSet = typeSets.GetValueByIndex(typeSetIndex);
                            }
                            else
                            {
                                return isChanged;
                            }
                        }

                        while (deserializedTracker.GetId(trackerIndex) < typeSet.TypeID && typeSet.TypeID < deserializedTracker.GetId(trackerIndex + 1))
                        {
                            isChanged = isChanged | typeSet.DeserializeRemoveAll();
                            typeSetIndex++;
                            if (typeSetIndex < typeSets.Count)
                            {
                                typeSet = typeSets.GetValueByIndex(typeSetIndex);
                            }
                            else
                            {
                                return isChanged;
                            }
                        }
                    }
                }
                else
                {
                    //wipe typeset IDs greater than deserializedTracker.GetId(trackerIndex)
                    while (typeSet.TypeID <= deserializedTracker.GetId(trackerIndex))
                    {
                        typeSetIndex++;
                        if (typeSetIndex < typeSets.Count)
                        {
                            typeSet = typeSets.GetValueByIndex(typeSetIndex);
                        }
                        else
                        {
                            return isChanged;
                        }
                    }

                    while (deserializedTracker.GetId(trackerIndex) < typeSet.TypeID)
                    {
                        isChanged = isChanged | typeSet.DeserializeRemoveAll();
                        typeSetIndex++;
                        if (typeSetIndex < typeSets.Count)
                        {
                            typeSet = typeSets.GetValueByIndex(typeSetIndex);
                        }
                        else
                        {
                            return isChanged;
                        }
                    }

                }
                trackerIndex++;
            }

            return isChanged;
        }
    }
}
