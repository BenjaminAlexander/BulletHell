using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class WIPTypeSerializer<BaseType> : Serializer<BaseType> where BaseType : Serializable
    {
        private TypeFactory<BaseType> factory;

        public WIPTypeSerializer(TypeFactory<BaseType> factory)
        {
            this.factory = factory;
        }

        public int SerializationSize(BaseType obj)
        {
            return sizeof(int) + obj.SerializationSize;
        }

        public void Serialize(BaseType obj, byte[] buffer, int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(factory.GetTypeID(obj)), 0, buffer, bufferOffset, sizeof(int));
            obj.Serialize(buffer, bufferOffset + sizeof(int));
        }
    }
}
