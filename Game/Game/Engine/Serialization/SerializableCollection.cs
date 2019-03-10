using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.Serialization
{
    class SerializableCollection<BaseType> where BaseType : Serializable
    {
        static int nextID = 0;
        Dictionary<int, BaseType> idToObject = new Dictionary<int, BaseType>();
        Dictionary<BaseType, int> objectToId = new Dictionary<BaseType, int>();

        TypeSerializer<BaseType> serializer;

        public SerializableCollection(TypeSerializer<BaseType> serializer)
        {
            this.serializer = serializer;
        }

        public int Add(BaseType obj)
        {
            if (!objectToId.ContainsKey(obj))
            {
                int id = nextID;
                nextID++;
                this.Add(id, obj);
                return id;
            }
            else
            {
                return objectToId[obj];
            }
        }

        private void Add(int id, BaseType obj)
        {
            idToObject[id] = obj;
            objectToId[obj] = id;
        }

        public BaseType GetObject(int id)
        {
            return idToObject[id];
        }

        public int GetID(BaseType obj)
        {
            return objectToId[obj];
        }

        public int DeserializeObject(byte[] buffer)
        {
            int bufferOffset = 0;
            return this.DeserializeObject(buffer, ref bufferOffset);
        }

        public int DeserializeObject(byte[] buffer, ref int bufferOffset)
        {
            int objectId = Utils.ReadInt(buffer, ref bufferOffset);
            if (idToObject.ContainsKey(objectId))
            {
                serializer.Deserialize(idToObject[objectId], buffer, ref bufferOffset);
            }
            else
            {
                BaseType newObject = serializer.Deserialize(buffer, ref bufferOffset);
                this.Add(objectId, newObject);
            }
            return objectId;
        }

        public int ObjectSerializationSize(int id)
        {
            return this.ObjectSerializationSize(idToObject[id]);
        }

        public int ObjectSerializationSize(BaseType obj)
        {
            return serializer.SerializationSize(obj) + sizeof(int);
        }

        public void SerializeObject(int id, byte[] buffer, int bufferOffset)
        {
            SerializeObject(id, idToObject[id], buffer, bufferOffset);
        }

        public void SerializeObject(BaseType obj, byte[] buffer, int bufferOffset)
        {
            SerializeObject(objectToId[obj], obj, buffer, bufferOffset);
        }

        private void SerializeObject(int id, BaseType obj, byte[] buffer, int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(id), 0, buffer, bufferOffset, sizeof(int));
            serializer.Serialize(obj, buffer, bufferOffset + sizeof(int));
        }

        public byte[] SerializeObject(int id)
        {
            byte[] serialization = new byte[ObjectSerializationSize(id)];
            this.SerializeObject(id, serialization, 0);
            return serialization;
        }

        public byte[] SerializeObject(BaseType obj)
        {
            byte[] serialization = new byte[ObjectSerializationSize(obj)];
            this.SerializeObject(obj, serialization, 0);
            return serialization;
        }
    }
}
