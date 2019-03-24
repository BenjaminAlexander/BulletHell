using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class InstantTypeSerializer<BaseType> : TypeDeserializer<BaseType> where BaseType : InstantSerializable
    {
        public InstantTypeSerializer(TypeFactory<BaseType> factory) : base(factory)
        {
        }

        public int SerializationSize(BaseType obj, int instant)
        {
            return sizeof(int) + obj.SerializationSize(instant);
        }

        public void Serialize(BaseType obj, int instant, byte[] buffer, ref int bufferOffset)
        {
            Utils.Write(GetTypeID(obj), buffer, ref bufferOffset);
            obj.Serialize(instant, buffer, ref bufferOffset);
        }

        public byte[] Serialize(BaseType obj, int instant)
        {
            byte[] buffer = new byte[SerializationSize(obj, instant)];
            int offsett = 0;
            Serialize(obj, instant, buffer, ref offsett);
            return buffer;
        }
    }
}
