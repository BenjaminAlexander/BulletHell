using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.Reflection;
using MyGame.Engine.DataStructures;
using MyGame.Engine.GameState.Instants;
using MyGame.Engine.Utils;

namespace MyGame.Engine.GameState
{
    //TODO: Add code to update from one instant to the next
    public class GameObjectCollection
    {
        private Logger log = new Logger(typeof(GameObjectCollection));
        private int nextId = 1;  //0 is null
        private TwoWayMap<int, GameObject> objects = new TwoWayMap<int, GameObject>();
        private TwoWayMap<int, Instant> instantMap = new TwoWayMap<int, Instant>();



        //TODO: what is the right type of instant?
        internal SubType NewGameObject<SubType>(NextInstant next) where SubType : GameObject, new()
        {
            Instant instant = this.GetInstant(next.Instant);
            SubType newObject = GameObject.Construct<SubType>(nextId, instant);
            objects[nextId] = newObject;
            nextId++;
            return newObject;
        }

        //TODO: remove this method
        internal GameObject NewGameObject(int id, Instant instant, int typeID)
        {
            instant = this.GetInstant(instant);
            GameObject newObject = GameObject.Construct(id, instant, typeID);
            objects[id] = newObject;
            return newObject;
        }

        public GameObject GetGameObject(int id)
        {
            return objects[id];
        }

        public int GetGameObjectID(GameObject obj)
        {
            return objects[obj];
        }

        internal int SerializationSize(GameObject obj, Instant instant)
        {
            if (objects.ContainsValue(obj))
            {
                return sizeof(int) * 2 + obj.SerializationSize(instant);
            }
            throw new Exception("Could not find object in this collection.");
        }

        internal void Serialize(GameObject obj, Instant instant, byte[] buffer, ref int bufferOffset)
        {
            Serialization.Utils.Write(objects[obj], buffer, ref bufferOffset);
            Serialization.Utils.Write(instant.ID, buffer, ref bufferOffset);
            obj.Serialize(instant, buffer, ref bufferOffset);
        }

        internal byte[] Serialize(GameObject obj, Instant instant)
        {
            byte[] buffer = new byte[this.SerializationSize(obj, instant)];
            int bufferOffset = 0;
            this.Serialize(obj, instant, buffer, ref bufferOffset);
            return buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            int bufferOffset = 0;
            this.Deserialize(buffer, ref bufferOffset);
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int objectId = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            int instantId = Serialization.Utils.ReadInt(buffer, ref bufferOffset);

            Instant instant;
            if(instantMap.ContainsKey(instantId))
            {
                instant = instantMap[instantId];
            }
            else
            {
                instant = new Instant(instantId);
                instantMap[instantId] = instant;
            }

            GameObject obj;
            if (objects.ContainsKey(objectId))
            {
                obj = objects[objectId];
            }
            else
            {
                obj = GameObject.Construct(objectId, instant, buffer, bufferOffset);
                nextId = objectId + 1;
                objects[objectId] = obj;
            }
            obj.Deserialize(instant, buffer, ref bufferOffset);
        }

        public bool CheckCollectionIntegrety()
        {
            foreach(KeyValuePair<int, GameObject> pair in objects)
            {
                if(pair.Value.ID != pair.Key)
                {
                    log.Error("A GameObject with an id of " + pair.Value.ID + " was in the dictionary under the key of " + pair.Key);
                    return false;
                }

                if(!pair.Value.CheckThatInstantKeysContainThis())
                {
                    log.Error("A GameObject failed its Instant <-> GameObject containment check");
                    return false;
                }
            }

            foreach (KeyValuePair<int, Instant> pair in instantMap)
            {
                if(!pair.Value.CheckIntegrety(objects))
                {
                    log.Error("An instant failed its integerty check");
                    return false;
                }
            }
            return true;
        }

        internal Instant GetInstant(int instant)
        {
            if(instantMap.ContainsKey(instant))
            {
                return instantMap[instant];
            }
            else
            {
                Instant newInstant = new Instant(instant);
                instantMap[instant] = newInstant;
                return newInstant;
            }
        }

        internal Instant GetInstant(Instant instant)
        {
            int hash = instant.GetHashCode();
            if (instantMap.ContainsKey(hash))
            {
                return instantMap[hash];
            }
            else
            {
                Instant newInstant = new Instant(hash);
                instantMap[hash] = newInstant;
                return newInstant;
            }
        }

        public void Update(int current)
        {
            Instant currentInstant = GetInstant(current);
            Instant nextInstant = GetInstant(current + 1);
            Instant.Update(currentInstant, nextInstant);
        }

        public void Update(int current, int next)
        {
            Instant currentInstant = GetInstant(current);
            Instant nextInstant = GetInstant(next);
            Instant.Update(currentInstant, nextInstant);
        }
    }
}
