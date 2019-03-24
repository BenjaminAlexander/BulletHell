using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    interface Deserializer<T>
    {
        void Deserialize(T obj, byte[] buffer, ref int bufferOffset);
    }
}
