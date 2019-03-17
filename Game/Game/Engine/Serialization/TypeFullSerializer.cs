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
        private DeserializableTypeDeserializer<BaseType> deserializer;
        private SerializableTypeSerializer<BaseType> serializer;

        public FullTypeSerializer(TypeFactory<BaseType> factory)
        {
            deserializer = new DeserializableTypeDeserializer<BaseType>(factory);
            serializer = new SerializableTypeSerializer<BaseType>(factory);
        }

        public void Deserialize(BaseType obj, byte[] buffer, ref int bufferOffset)
        {
            deserializer.Deserialize(obj, buffer, ref bufferOffset);
        }

        public BaseType Deserialize(byte[] buffer, ref int bufferOffset)
        {
            return deserializer.Deserialize(buffer, ref bufferOffset);
        }

        public int SerializationSize(BaseType obj)
        {
            return serializer.SerializationSize(obj);
        }

        public void Serialize(BaseType obj, byte[] buffer, int bufferOffset)
        {
            serializer.Serialize(obj, buffer, bufferOffset);
        }
    }
}
