using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    class SerializableSerializer<T> : Serializer<T> where T : Serializable
    {
        public int SerializationSize(T obj)
        {
            return obj.SerializationSize;
        }

        public void Serialize(T obj, byte[] buffer, ref int bufferOffset)
        {
            obj.Serialize(buffer, ref bufferOffset);
        }

        public void Deserialize(T obj, byte[] buffer, ref int bufferOffset)
        {
            obj.Deserialize(buffer, ref bufferOffset);
        }
    }
}
