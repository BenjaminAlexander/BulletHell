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
        private TwoWayMap<int, SubType> objects = new TwoWayMap<int, SubType>(new IntegerComparer());
        private int instantId;
        private DeserializedObjectTracker<SubType> deserializedTracker = new DeserializedObjectTracker<SubType>();

        public InstantTypeSet(TypeSet<SubType> globalSet, int instantId)
        {
            this.globalSet = globalSet;
            this.instantId = instantId;
        }

        public SubType NewObject(int id)
        {
            SubType obj = globalSet.GetObject(id);
            if (!deserializedTracker.AllDeserialized())
            {
                obj.SetDefaultValue(instantId);
                objects[obj.ID] = obj;
            }
            return obj;
        }

        public GameObject GetObject(int id)
        {
            if(id == 0 || !objects.ContainsKey(id))
            {
                return null;
            }
            else
            {
                return objects[id];
            }
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

        public int TypeID
        {
            get
            {
                return globalSet.TypeID;
            }
        }

        public int ObjectCount
        {
            get
            {
                return objects.Count;
            }
        }

        public void PrepareForUpdate(InstantTypeSet<SubType> next)
        {
            foreach (SubType obj in next.objects.Values)
            {
                if (!obj.RemoveForUpdate(next.instantId))
                {
                    next.objects.RemoveKey(obj.ID);
                }
            }

            if (!next.deserializedTracker.AllDeserialized())
            {
                foreach (SubType obj in this.objects.Values)
                {
                    obj.CopyFields(this.instantId, next.instantId);
                    next.objects[obj.ID] = (SubType)obj;
                }
            }
        }

        public void Update(CurrentInstant current, NextInstant next)
        {
            foreach (SubType obj in this.objects.Values)
            {
                obj.Update(current, next);
            }
        }

        public bool DeserializeRemoveAll()
        {
            bool isChanged = false;
            deserializedTracker.SetCount(0);
            while(objects.Count > 0)
            {
                SubType obj = objects.ElementAt(0).Value;
                obj.DeserializeRemove(instantId);
                objects.RemoveKey(obj.ID);
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
                isChanged = isChanged | obj.Deserialize(instantId, buffer, ref bufferOffset);
                objects[obj.ID] = obj;
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
                        obj.DeserializeRemove(instantId);
                        objects.RemoveKey(obj.ID);
                        isChanged = true;
                    }
                }
            }
            return isChanged;
        }
    }
}
