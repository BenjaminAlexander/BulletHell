using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class TypeSerializer<BaseType> : TypeDeserializer<BaseType> where BaseType : Serializable, Deserializable
    {
        public TypeSerializer(TypeFactory<BaseType> factory) : base(factory)
        {
        }

        public int SerializationSize(BaseType obj)
        {
            return sizeof(int) + obj.SerializationSize;
        }

        public void Serialize(BaseType obj, byte[] buffer, ref int bufferOffset)
        {
            Utils.Write(GetTypeID(obj), buffer, ref bufferOffset);
            obj.Serialize(buffer, ref bufferOffset);
        }

        public byte[] Serialize(BaseType obj)
        {
            byte[] buffer = new byte[SerializationSize(obj)];
            int offsett = 0;
            Serialize(obj, buffer, ref offsett);
            return buffer;
        }
    }
}
