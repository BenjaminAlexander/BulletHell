using MyGame.Engine.DataStructures;
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
using MyGame.Engine.GameState.Instants;

namespace MyGame.Engine.GameState.InstantObjectSet
{
    class InstantTypeSet<SubType> : InstantTypeSetInterface where SubType : GameObject, new()
    {
        private TypeSet<SubType> globalSet;
        private int instantId;
        private TwoWayMap<int, SubType> objects = new TwoWayMap<int, SubType>(new IntegerComparer());
        private DeserializedObjectTracker<SubType> deserializedTracker = new DeserializedObjectTracker<SubType>();

        public InstantTypeSet(TypeSet<SubType> globalSet, int instantId)
        {
            this.globalSet = globalSet;
            this.instantId = instantId;
        }

        public int TypeID
        {
            get
            {
                return globalSet.TypeID;
            }
        }

        public int InstantID
        {
            get
            {
                return instantId;
            }
        }

        public void Add(GameObject obj)
        {
            if (obj.TypeID == globalSet.TypeID)
            {
                objects[obj.ID] = (SubType)obj;
            }
            else
            {
                throw new Exception("Add: Object type does not match typeSet");
            }
        }

        public void Remove(GameObject obj)
        {
            objects.RemoveKey(obj.ID);
        }

        public SubType NewObject(int id)
        {
            SubType obj = globalSet.GetObject(id);
            if (!deserializedTracker.AllDeserialized())
            {
                obj.SetDefaultValue(this);
            }
            return obj;
        }

        public GameObject GetObject(int id)
        {
            if (id == 0)
            {
                return null;
            }

            bool containsKey;
            GameObject obj = objects.GetValue(id, out containsKey);
            if (!containsKey)
            {
                return null;
            }
            else
            {
                return obj;
            }
        }

        //TODO: is prep for update then update thread safe
        public ObjectTypeFactory<SubType> PrepareForUpdate(InstantTypeSet<SubType> next)
        {
            foreach (SubType obj in next.objects.Values)
            {
                obj.RemoveForUpdate(next);
            }

            if (!next.deserializedTracker.AllDeserialized())
            {
                foreach (SubType obj in this.objects.Values)
                {
                    obj.CopyFields(this.instantId, next);
                }
            }

            return new ObjectTypeFactory<SubType>(globalSet.TypeID, objects.GreatestKey + 1, next);
        }

        public void Update(CurrentInstant current, NextInstant next)
        {
            foreach (SubType obj in this.objects.Values)
            {
                obj.Update(current, next);
            }
        }

        public List<SerializationBuilder> Serialize()
        {
            List<SerializationBuilder> builderList = new List<SerializationBuilder>();
            foreach (SubType obj in this.objects.Values)
            {
                SerializationBuilder builder = new SerializationBuilder();
                builder.Append(obj.ID);
                builder.Append(obj.Serialize(instantId));
            }
            return builderList;
        }

        public bool DeserializeRemoveAll()
        {
            bool isChanged = false;
            deserializedTracker.SetCount(0);
            while(objects.Count > 0)
            {
                SubType obj = objects.ElementAt(0).Value;
                obj.DeserializeRemove(this);
                isChanged = true;
            }
            return isChanged;
        }

        public bool Deserialize(byte[] buffer, ref int bufferOffset)
        {
            bool isChanged = false;
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
                SubType obj = globalSet.GetObject(objectId);
                isChanged = isChanged | obj.Deserialize(this, buffer, ref bufferOffset);
                deserializedTracker.Set(objectOffset, obj);
                objectCountInMessage--;
                objectOffset++;
            }

            if(deserializedTracker.AllDeserialized())
            {
                int i = 0;
                SubType expectedObj = deserializedTracker.Get(i);
                while (i < objects.Count)
                {
                    SubType obj = objects.ElementAt(i).Value;
                    if(expectedObj.ID == obj.ID)
                    {
                        i++;
                        expectedObj = deserializedTracker.Get(i);
                    }
                    else
                    {
                        obj.DeserializeRemove(this);
                        isChanged = true;
                    }
                }
            }
            return isChanged;
        }
    }
}
