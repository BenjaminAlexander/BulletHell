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

        public TypeSerializer(TypeFactory<BaseType> factory) : base(null)
        {
            this.factory = factory;
        }

        public TypeSerializer(TypeFactory<BaseType> factory, Serializer<BaseType> nestedSerializer) : base(nestedSerializer)
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
            Serialization.Utils.Write(factory.GetTypeID(obj), buffer, ref bufferOffset);
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
            BaseType obj = Construct(buffer, bufferOffset);
            this.Deserialize(obj, buffer, ref bufferOffset);
            return obj;
        }

        public BaseType Construct(byte[] buffer, int bufferOffset)
        {
            int typeId = Utils.ReadInt(buffer, ref bufferOffset);
            return this.factory.Construct(typeId);
        }
    }
}
