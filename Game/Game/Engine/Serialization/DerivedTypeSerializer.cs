using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class DerivedTypeSerializer<BaseType> where BaseType : Serializable
    {
        private DerivedTypeFactory<BaseType> factory;

        public DerivedTypeSerializer(DerivedTypeFactory<BaseType> factory)
        {
            this.factory = factory;
        }

        public BaseType Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int typeId = BitConverter.ToInt32(buffer, bufferOffset);
            bufferOffset = bufferOffset + sizeof(int);

            BaseType obj = this.factory.Construct(typeId);
            bufferOffset = obj.Deserialize(buffer, bufferOffset);
            return obj;
        }

        public int SerializationSize(BaseType obj)
        {
            return sizeof(int) + obj.SerializationSize;
        }

        public void Serialize(BaseType obj, byte[] buffer, int bufferOffset)
        {
            obj.Serialize(buffer, bufferOffset);
        }
    }
}
