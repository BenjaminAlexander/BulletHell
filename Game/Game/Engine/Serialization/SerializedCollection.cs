using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.DataStructures;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class SerializedCollection<BaseType>
    {
        int nextID = 0;
        TwoWayMap<int, BaseType> map;
        TypeSerializer<BaseType> typeSerializer;

        public SerializedCollection(TwoWayMap<int, BaseType> map, TypeFactory<BaseType> factory)
        {
            this.map = map;
            this.typeSerializer = new TypeSerializer<BaseType>(factory);
        }

        public SerializedCollection(TypeFactory<BaseType> factory) : this(new TwoWayMap<int, BaseType>(), factory)
        {

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

        public int ObjectSerializationSize(Serializer<BaseType> serializer, int id)
        {
            return this.ObjectSerializationSize(serializer, map[id]);
        }

        public int ObjectSerializationSize(Serializer<BaseType> serializer, BaseType obj)
        {
            return typeSerializer.SerializationSize(serializer, obj) + sizeof(int);
        }

        public void SerializeObject(Serializer<BaseType> serializer, int id, byte[] buffer, int bufferOffset)
        {
            SerializeObject(serializer, id, map[id], buffer, bufferOffset);
        }

        public void SerializeObject(Serializer<BaseType> serializer, BaseType obj, byte[] buffer, int bufferOffset)
        {
            SerializeObject(serializer, map[obj], obj, buffer, bufferOffset);
        }

        private void SerializeObject(Serializer<BaseType> serializer, int id, BaseType obj, byte[] buffer, int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(id), 0, buffer, bufferOffset, sizeof(int));
            typeSerializer.Serialize(serializer, obj, buffer, bufferOffset + sizeof(int));
        }

        public byte[] SerializeObject(Serializer<BaseType> serializer, int id)
        {
            byte[] serialization = new byte[ObjectSerializationSize(serializer, id)];
            this.SerializeObject(serializer, id, serialization, 0);
            return serialization;
        }

        public byte[] SerializeObject(Serializer<BaseType> serializer, BaseType obj)
        {
            byte[] serialization = new byte[ObjectSerializationSize(serializer, obj)];
            this.SerializeObject(serializer, obj, serialization, 0);
            return serialization;
        }

        public int DeserializeObject(Deserializer<BaseType> deserializer, byte[] buffer)
        {
            int bufferOffset = 0;
            return this.DeserializeObject(deserializer, buffer, ref bufferOffset);
        }

        public int DeserializeObject(Deserializer<BaseType> deserializer, byte[] buffer, ref int bufferOffset)
        {
            int objectId = Utils.ReadInt(buffer, ref bufferOffset);
            if (map.ContainsKey(objectId))
            {
                typeSerializer.Deserialize(deserializer, map[objectId], buffer, ref bufferOffset);
            }
            else
            {
                BaseType newObject = typeSerializer.Deserialize(deserializer, buffer, ref bufferOffset);
                map.Set(objectId, newObject);
            }
            return objectId;
        }
    }
}
