using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    interface InstantSerializable : Deserializable
    {
        int SerializationSize(int instant);
        void Serialize(int instant, byte[] buffer, ref int bufferOffset);
    }
}
