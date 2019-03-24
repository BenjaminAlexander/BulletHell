using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class TypeDeserializer<BaseType> where BaseType : Deserializable
    {
        private TypeFactory<BaseType> factory;

        public TypeDeserializer(TypeFactory<BaseType> factory)
        {
            this.factory = factory;
        }

        protected int GetTypeID(BaseType obj)
        {
            return factory.GetTypeID(obj);
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

        public BaseType Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int typeId = Utils.ReadInt(buffer, ref bufferOffset);
            BaseType obj = this.factory.Construct(typeId);
            obj.Deserialize(buffer, ref bufferOffset);
            return obj;
        }

        public void Deserialize(BaseType obj, byte[] buffer)
        {
            int offset = 0;
            int typeId = Utils.ReadInt(buffer, ref offset);
            obj.Deserialize(buffer, ref offset);
        }
    }
}
