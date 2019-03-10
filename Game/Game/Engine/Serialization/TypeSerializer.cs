using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class TypeSerializer<BaseType> where BaseType : Serializable
    {
        private TypeFactory<BaseType> factory;

        public TypeSerializer(TypeFactory<BaseType> factory)
        {
            this.factory = factory;
        }

        public void Deserialize(BaseType obj, byte[] buffer, ref int bufferOffset)
        {
            int typeId = Utils.ReadInt(buffer, ref bufferOffset);
            this.Deserialize(obj, typeId, buffer, ref bufferOffset);
        }

        public BaseType Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int typeId = Utils.ReadInt(buffer, ref bufferOffset);
            BaseType obj = this.factory.Construct(typeId);
            this.Deserialize(obj, typeId, buffer, ref bufferOffset);
            return obj;
        }

        private void Deserialize(BaseType obj, int typeId, byte[] buffer, ref int bufferOffset)
        {
            if (this.factory.GetTypeID(obj) == typeId)
            {
                obj.Deserialize(buffer, ref bufferOffset);
            }
            else
            {
                throw new Exception("Serialized type does not match object type");
            }
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
