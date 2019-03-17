using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    class DeserializableDeserializer : Deserializer<Deserializable>
    {
        public void Deserialize(Deserializable obj, byte[] buffer, ref int bufferOffset)
        {
            obj.Deserialize(buffer, ref bufferOffset);
        }
    }
}
