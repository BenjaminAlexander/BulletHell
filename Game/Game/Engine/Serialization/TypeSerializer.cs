using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class TypeSerializer<BaseType>
    {
        private TypeFactory<BaseType> factory;

        public TypeSerializer(TypeFactory<BaseType> factory)
        {
            this.factory = factory;
        }

        public int SerializationSize(Serializer<BaseType> serializer, BaseType obj)
        {
            return sizeof(int) + serializer.SerializationSize(obj);
        }

        public void Serialize(Serializer<BaseType> serializer, BaseType obj, byte[] buffer, int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(factory.GetTypeID(obj)), 0, buffer, bufferOffset, sizeof(int));
            serializer.Serialize(obj, buffer, bufferOffset + sizeof(int));
        }
    }
}
