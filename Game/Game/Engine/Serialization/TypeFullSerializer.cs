using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class FullTypeSerializer<BaseType> : FullSerializer<BaseType> where BaseType : FullSerializable
    {
        private TypeDeserializer<BaseType> deserializer;
        private TypeSerializer<BaseType> serializer;

        public FullTypeSerializer(TypeFactory<BaseType> factory)
        {
            deserializer = new TypeDeserializer<BaseType>(factory);
            serializer = new TypeSerializer<BaseType>(factory);
        }

        public void Deserialize(BaseType obj, byte[] buffer, ref int bufferOffset)
        {
            deserializer.Deserialize(new DeserializableDeserializer<BaseType>(), obj, buffer, ref bufferOffset);
        }

        public BaseType Deserialize(byte[] buffer, ref int bufferOffset)
        {
            return deserializer.Deserialize(new DeserializableDeserializer<BaseType>(), buffer, ref bufferOffset);
        }

        public int SerializationSize(BaseType obj)
        {
            return serializer.SerializationSize(new SerializableSerializer<BaseType>(), obj);
        }

        public void Serialize(BaseType obj, byte[] buffer, int bufferOffset)
        {
            serializer.Serialize(new SerializableSerializer<BaseType>(), obj, buffer, bufferOffset);
        }
    }
}
