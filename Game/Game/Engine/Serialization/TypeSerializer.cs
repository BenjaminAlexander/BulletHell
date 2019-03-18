using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class TypeSerializer<BaseType> : LinkedSerializer<BaseType>
    {
        private TypeFactory<BaseType> factory;

        public TypeSerializer(TypeFactory<BaseType> factory, Serializer<BaseType> nestedSerializer, Deserializer<BaseType> nestedDeserializer) : base(nestedSerializer, nestedDeserializer)
        {
            this.factory = factory;
        }

        protected override void AdditionalDeserialize(BaseType obj, byte[] buffer, ref int bufferOffset)
        {
            int typeId = Utils.ReadInt(buffer, ref bufferOffset);
            this.CheckType(obj, typeId);
        }

        protected override int AdditionalSerializationSize(BaseType obj)
        {
            return sizeof(int);
        }

        protected override void AdditionalSerialize(BaseType obj, byte[] buffer, ref int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(factory.GetTypeID(obj)), 0, buffer, bufferOffset, sizeof(int));
            bufferOffset = bufferOffset + sizeof(int);
        }

        private void CheckType(BaseType obj, int typeId)
        {
            if (this.factory.GetTypeID(obj) != typeId)
            {
                throw new Exception("Serialized type does not match object type");
            }
        }

        public BaseType Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int tempOffset = bufferOffset;
            int typeId = Utils.ReadInt(buffer, ref tempOffset);
            BaseType obj = this.factory.Construct(typeId);
            this.Deserialize(obj, buffer, ref bufferOffset);
            return obj;
        }
    }
}
