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
    //TODO: pull ID and instant serialization out of GameObject
    //TODO: give instants a list of game objects that are in that instant
    class GameObjectCollection
    {
        private Logger log = new Logger(typeof(GameObjectCollection));
        private int nextId = 0;
        private TwoWayMap<int, GameObject> objects = new TwoWayMap<int, GameObject>();
        private TwoWayMap<int, Instant> instantMap = new TwoWayMap<int, Instant>();


        //TODO: what is the right type of instant?
        public SubType NewGameObject<SubType>(Instant instant) where SubType : GameObject, new()
        {
            instant = this.GetInstant(instant);
            SubType newObject = GameObject.Construct<SubType>(nextId, instant);
            objects[nextId] = newObject;
            nextId++;
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

        public int SerializationSize(GameObject obj, Instant instant)
        {
            if (objects.ContainsValue(obj))
            {
                return sizeof(int) + obj.SerializationSize(instant) + instant.SerializationSize;
            }
            throw new Exception("Could not find object in this collection.");
        }

        public void Serialize(GameObject obj, Instant instant, byte[] buffer, ref int bufferOffset)
        {
            Serialization.Utils.Write(objects[obj], buffer, ref bufferOffset);
            instant.Serialize(buffer, ref bufferOffset);
            obj.Serialize(instant, buffer, ref bufferOffset);
        }

        public byte[] Serialize(GameObject obj, Instant instant)
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
            int inst = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            Instant instant = this.GetInstant(inst);
            if(objects.ContainsKey(objectId))
            {
                objects[objectId].Deserialize(instant, buffer, ref bufferOffset);
            }
            else
            {
                objects[objectId] = GameObject.Construct(objectId, instant, buffer, ref bufferOffset);
                nextId = objectId + 1;
            }
        }

        public bool CheckObjectIdDictionary()
        {
            foreach(KeyValuePair<int, GameObject> pair in objects)
            {
                if(pair.Value.ID != pair.Key)
                {
                    return false;
                }
            }
            return true;
        }

        public Instant GetInstant(int instant)
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

        public Instant GetInstant(Instant instant)
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


        /*
        public int ReadInstant
        {
            get
            {
                return readInstant;
            }

            set
            {
                readInstant = value;
            }
        }

        public int WriteInstant
        {
            get
            {
                return writeInstant;
            }

            set
            {
                writeInstant = value;
            }
        }

        private GameObjectCollection(NewConstraintTypeFactory<GameObject> factory)
        {
            this.factory = factory;
        }

        public GameObjectCollection() : this(new NewConstraintTypeFactory<GameObject>())
        {

        }

        public void AddType<SubType>() where SubType : GameObject, new()
        {
            factory.AddType<SubType>();
        }

        public void Update(int instant)
        {
            InstantCollection instantCollection = instants[instant];
            if(instantCollection == null)
            {
                throw new Exception("Instant does not exist");
            }

            instants[instant + 1] = instantCollection.NextInstant();
        }*/
    }
}
