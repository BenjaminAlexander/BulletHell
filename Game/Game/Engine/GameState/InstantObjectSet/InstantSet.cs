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

        public IEnumerator<InstantTypeSetInterface> GetEnumerator()
        {
            return typeSets.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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
