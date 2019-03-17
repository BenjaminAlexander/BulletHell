using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    interface Deserializable
    {
        void Deserialize(byte[] buffer, ref int bufferOffset);
    }
}
