using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    class DeserializableDeserializer<Type> : Deserializer<Type> where Type : Deserializable
    {
        public void Deserialize(Type obj, byte[] buffer, ref int bufferOffset)
        {
            obj.Deserialize(buffer, ref bufferOffset);
        }

        public void Deserialize(Type obj, byte[] buffer)
        {
            int offset = 0;
            Deserialize(obj, buffer, ref offset);
        }
    }
}
