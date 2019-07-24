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

            int i = 0;
            foreach (InstantTypeSetInterface set in typeSets.Values)
            {
                while (i < deserializedTracker.Count && deserializedTracker.GetId(i) == null)
                {
                    i++;
                }

                if (i >= deserializedTracker.Count)
                {
                    break;
                }

                if (i == 0 && set.TypeID < deserializedTracker.GetId(i))
                {
                    isChanged = isChanged | set.DeserializeRemoveAll();
                }
                if (i + 1 < deserializedTracker.Count)
                {
                    if (deserializedTracker.GetId(i + 1) != null)
                    {
                        if (deserializedTracker.GetId(i) < set.TypeID)
                        {
                            if(set.TypeID < deserializedTracker.GetId(i + 1))
                            {
                                isChanged = isChanged | set.DeserializeRemoveAll();
                            }
                            else if (set.TypeID == deserializedTracker.GetId(i + 1))
                            {
                                i++;
                            }
                            else
                            {
                                throw new Exception("The instant set must contain type sets for all types");
                            }
                        } 
                    }
                    else
                    {
                        i++;
                    }
                }
                else if(deserializedTracker.GetId(i) < set.TypeID)
                {
                    isChanged = isChanged | set.DeserializeRemoveAll();
                }
            }
            return isChanged;
        }
    }
}
