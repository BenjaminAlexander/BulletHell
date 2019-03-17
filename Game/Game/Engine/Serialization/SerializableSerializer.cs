using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    class SerializableSerializer<Type> : Serializer<Type> where Type : Serializable
    {
        public int SerializationSize(Type obj)
        {
            return obj.SerializationSize;
        }

        public void Serialize(Type obj, byte[] buffer, int bufferOffset)
        {
            obj.Serialize(buffer, bufferOffset);
        }
    }
}
