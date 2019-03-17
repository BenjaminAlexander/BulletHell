using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class TypeDeserializer<BaseType>
    {
        private TypeFactory<BaseType> factory;

        public TypeDeserializer(TypeFactory<BaseType> factory)
        {
            this.factory = factory;
        }

        public void Deserialize(Deserializer<BaseType> deserializer, BaseType obj, byte[] buffer, ref int bufferOffset)
        {
            int typeId = Utils.ReadInt(buffer, ref bufferOffset);
            this.Deserialize(deserializer, obj, typeId, buffer, ref bufferOffset);
        }

        public BaseType Deserialize(Deserializer<BaseType> deserializer, byte[] buffer, ref int bufferOffset)
        {
            int typeId = Utils.ReadInt(buffer, ref bufferOffset);
            BaseType obj = this.factory.Construct(typeId);
            this.Deserialize(deserializer, obj, typeId, buffer, ref bufferOffset);
            return obj;
        }

        private void Deserialize(Deserializer<BaseType> deserializer, BaseType obj, int typeId, byte[] buffer, ref int bufferOffset)
        {
            if (this.factory.GetTypeID(obj) == typeId)
            {
                deserializer.Deserialize(obj, buffer, ref bufferOffset);
            }
            else
            {
                throw new Exception("Serialized type does not match object type");
            }
        }
    }
}
