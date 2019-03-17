using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.DataStructures;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class FullSerializableCollection<BaseType> where BaseType : FullSerializable
    {
        int nextID = 0;
        TwoWayMap<int, BaseType> map = new TwoWayMap<int, BaseType>();
        SerializeCollection<BaseType> serializeCollection;

        public FullSerializableCollection(TypeFactory<BaseType> factory)
        {
            serializeCollection = new SerializeCollection<BaseType>(map, factory);
        }

        public int Add(BaseType obj)
        {
            if (!map.ContainsValue(obj))
            {
                int id = nextID;
                nextID++;
                map.Set(id, obj);
                return id;
            }
            else
            {
                return map[obj];
            }
        }

        public BaseType GetObject(int id)
        {
            return map[id];
        }

        public int GetID(BaseType obj)
        {
            return map[obj];
        }

        public int DeserializeObject(byte[] buffer)
        {
            int bufferOffset = 0;
            return this.DeserializeObject(buffer, ref bufferOffset);
        }

        public int DeserializeObject(byte[] buffer, ref int bufferOffset)
        {
            return serializeCollection.DeserializeObject(new DeserializableDeserializer<BaseType>(), buffer, ref bufferOffset);
        }

        public int ObjectSerializationSize(int id)
        {
            return this.ObjectSerializationSize(map[id]);
        }

        public int ObjectSerializationSize(BaseType obj)
        {
            return serializeCollection.ObjectSerializationSize(new SerializableSerializer<BaseType>(), obj);
        }

        public void SerializeObject(int id, byte[] buffer, int bufferOffset)
        {
            SerializeObject(id, map[id], buffer, bufferOffset);
        }

        public void SerializeObject(BaseType obj, byte[] buffer, int bufferOffset)
        {
            SerializeObject(map[obj], obj, buffer, bufferOffset);
        }

        private void SerializeObject(int id, BaseType obj, byte[] buffer, int bufferOffset)
        {
            serializeCollection.SerializeObject(new SerializableSerializer<BaseType>(), obj, buffer, bufferOffset);
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
