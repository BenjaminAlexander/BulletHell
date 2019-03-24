using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class TypeSerializer<BaseType> : InstantSerializer<BaseType>, Deserializer<BaseType> where BaseType : InstantSerializable
    {
        private TypeFactory<BaseType> factory;

        public TypeSerializer(TypeFactory<BaseType> factory)
        {
            this.factory = factory;
        }

        public void Deserialize(BaseType obj, byte[] buffer, ref int bufferOffset)
        {
            int typeId = Utils.ReadInt(buffer, ref bufferOffset);
            if (this.factory.GetTypeID(obj) != typeId)
            {
                throw new Exception("Serialized type does not match object type");
            }
            obj.Deserialize(buffer, ref bufferOffset);
        }

        public int SerializationSize(BaseType obj, int instant)
        {
            return sizeof(int) + obj.SerializationSize(instant);
        }

        public void Serialize(BaseType obj, int instant, byte[] buffer, ref int bufferOffset)
        {
            Utils.Write(factory.GetTypeID(obj), buffer, ref bufferOffset);
            obj.Serialize(instant, buffer, ref bufferOffset);
        }

        public BaseType Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int typeId = Utils.ReadInt(buffer, ref bufferOffset);
            BaseType obj = this.factory.Construct(typeId);
            obj.Deserialize(buffer, ref bufferOffset);
            return obj;
        }
    }
}
