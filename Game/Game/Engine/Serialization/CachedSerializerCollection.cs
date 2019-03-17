using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.DataStructures;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class CachedSerializerCollection<BaseType> : SerializedCollection<BaseType>
    {
        Serializer<BaseType> serializer = null;
        Deserializer<BaseType> deserializer = null;

        public CachedSerializerCollection(TwoWayMap<int, BaseType> map, TypeFactory<BaseType> factory, Serializer<BaseType> serializer, Deserializer<BaseType> deserializer) : base(map, factory)
        {
            this.serializer = serializer;
            this.deserializer = deserializer;
        }

        public CachedSerializerCollection(TypeFactory<BaseType> factory) : this(new TwoWayMap<int, BaseType>(), factory, null, null) { }
        public CachedSerializerCollection(TypeFactory<BaseType> factory, Deserializer<BaseType> deserializer) : this(new TwoWayMap<int, BaseType>(), factory, null, deserializer) { }
        public CachedSerializerCollection(TypeFactory<BaseType> factory, Serializer<BaseType> serializer) : this(new TwoWayMap<int, BaseType>(), factory, serializer, null) { }
        public CachedSerializerCollection(TypeFactory<BaseType> factory, Serializer<BaseType> serializer, Deserializer<BaseType> deserializer) : this(new TwoWayMap<int, BaseType>(), factory, serializer, deserializer) { }

        public int ObjectSerializationSize(int id)
        {
            return ObjectSerializationSize(serializer, id);
        }

        public int ObjectSerializationSize(BaseType obj)
        {
            return ObjectSerializationSize(serializer, obj);
        }

        public void SerializeObject(int id, byte[] buffer, ref int bufferOffset)
        {
            SerializeObject(serializer, id, buffer, ref bufferOffset);
        }

        public void SerializeObject(BaseType obj, byte[] buffer, ref int bufferOffset)
        {
            SerializeObject(serializer, obj, buffer, ref bufferOffset);
        }

        public byte[] SerializeObject(int id)
        {
            return SerializeObject(serializer, id);
        }

        public byte[] SerializeObject(BaseType obj)
        {
            return SerializeObject(serializer, obj);
        }

        public int DeserializeObject(byte[] buffer)
        {
            return DeserializeObject(deserializer, buffer);
        }

        public int DeserializeObject(byte[] buffer, ref int bufferOffset)
        {
            return DeserializeObject(deserializer, buffer, ref bufferOffset);
        }
    }
}
