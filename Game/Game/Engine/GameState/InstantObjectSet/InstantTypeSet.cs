﻿using MyGame.Engine.DataStructures;
using MyGame.Engine.GameState.GameObjectFactory;
using MyGame.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.TypeManager;
using System.Collections;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState.InstantObjectSet
{
    class InstantTypeSet<SubType> : InstantTypeSetInterface where SubType : GameObject, new()
    {
        private TypeSet<SubType> globalSet;
        private TwoWayMap<int, SubType> objects = new TwoWayMap<int, SubType>(new IntegerComparer());
        private int instantId;
        //TODO: make this apply to new objects
        private DeserializedTracker deserializedTracker = new DeserializedTracker();

        public InstantTypeSet(TypeSet<SubType> globalSet, int instantId)
        {
            this.globalSet = globalSet;
            this.instantId = instantId;
        }

        public ObjectTypeFactoryInterface NewObjectTypeFactory(InstantTypeSetInterface nextInstantTypeSet)
        {
            InstantTypeSet<SubType> next = (InstantTypeSet<SubType>)nextInstantTypeSet;
            return new ObjectTypeFactory<SubType>(globalSet, this, next);
        }

        //TODO: check if object is serialized and check against total object counts
        public void Add(GameObject obj)
        {
            objects[obj.ID] = (SubType)obj;
        }

        public SubType NewObject(int id)
        {
            SubType obj = globalSet[id];
            //TODO: need to call/not call set default based on seraluzatiion complsmvmsvmslv
            //TODO: same for add
            obj.SetDefaultValue(instantId);
            this.Add(obj);
            return obj;
        }

        public SubType GetObject(int id)
        {
            if (objects.ContainsKey(id))
            {
                return objects[id];
            }
            return null;
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            return objects.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int GreatestID
        {
            get
            {
                return objects.GreatestKey;
            }
        }

        public TypeMetadataInterface GetMetaData
        {
            get
            {
                return globalSet.GetMetaData;
            }
        }

        public int InstantID
        {
            get
            {
                return instantId;
            }
        }

        public int TypeID
        {
            get
            {
                return globalSet.GetMetaData.TypeID;
            }
        }

        public int ObjectCount
        {
            get
            {
                return objects.Count;
            }
        }

        public bool DeserializeRemoveAll()
        {
            bool isChanged = false;
            deserializedTracker.SetCount(0);
            while(objects.Count > 0)
            {
                SubType obj = objects.GetValueByIndex(0);
                obj.RemoveInstant(instantId);
                objects.RemoveKey(obj.ID);
                isChanged = true;
            }
            return isChanged;
        }

        public bool Deserialize(byte[] buffer, ref int bufferOffset)
        {
            bool isChanged = false;
            //TODO: do something with this
            int objectOffset;
            int totalObjectCountOfType;
            int objectCountInMessage;
            Serialization.Utils.Read(out objectOffset, buffer, ref bufferOffset);
            Serialization.Utils.Read(out totalObjectCountOfType, buffer, ref bufferOffset);
            Serialization.Utils.Read(out objectCountInMessage, buffer, ref bufferOffset);

            deserializedTracker.SetCount(totalObjectCountOfType);

            while (objectCountInMessage > 0)
            {
                int objectId;
                Serialization.Utils.Read(out objectId, buffer, ref bufferOffset);
                GameObject obj = globalSet.GetObject(objectId);
                isChanged = isChanged | obj.Deserialize(instantId, buffer, ref bufferOffset);
                this.Add(obj);
                deserializedTracker.SetId(objectOffset, objectId);
                objectCountInMessage--;
                objectOffset++;
            }

            if(deserializedTracker.AllDeserialized())
            {
                int i = 0;
                int expectedId = (int)deserializedTracker.GetId(i);
                while (i < objects.Count)
                {
                    SubType obj = objects.GetValueByIndex(i);
                    if(expectedId == obj.ID)
                    {
                        i++;
                        expectedId = (int)deserializedTracker.GetId(i);
                    }
                    else
                    {
                        obj.RemoveInstant(instantId);
                        objects.RemoveKey(obj.ID);
                        isChanged = true;
                    }
                }
            }
            return isChanged;
        }
    }
}
