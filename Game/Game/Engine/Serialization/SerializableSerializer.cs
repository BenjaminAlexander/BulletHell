using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    class SerializableSerializer : Serializer<Serializable>
    {
        public int SerializationSize(Serializable obj)
        {
            return obj.SerializationSize;
        }

        public void Serialize(Serializable obj, byte[] buffer, int bufferOffset)
        {
            obj.Serialize(buffer, bufferOffset);
        }
    }
}
