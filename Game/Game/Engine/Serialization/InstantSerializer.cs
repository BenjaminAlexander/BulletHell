using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    interface InstantSerializer<T>
    {
        int SerializationSize(T obj, int instant);
        void Serialize(T obj, int instant, byte[] buffer, ref int bufferOffset);
        void Deserialize(T obj, byte[] buffer, ref int bufferOffset);
    }
}
