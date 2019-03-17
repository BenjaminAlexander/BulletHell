using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class TypeSerializer<BaseType> : Serializer<BaseType>
    {
        private TypeFactory<BaseType> factory;
        private Serializer<BaseType> serializer;

        public TypeSerializer(TypeFactory<BaseType> factory, Serializer<BaseType> serializer)
        {
            this.factory = factory;
            this.serializer = serializer;
        }

        public int SerializationSize(BaseType obj)
        {
            return sizeof(int) + serializer.SerializationSize(obj);
        }

        public void Serialize(BaseType obj, byte[] buffer, int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(factory.GetTypeID(obj)), 0, buffer, bufferOffset, sizeof(int));
            serializer.Serialize(obj, buffer, bufferOffset + sizeof(int));
        }
    }
}
