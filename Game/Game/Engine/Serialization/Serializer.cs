using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    interface Serializer<T>
    {
        int SerializationSize(T obj);
        void Serialize(T obj, byte[] buffer, ref int bufferOffset);
        void Deserialize(T obj, byte[] buffer, ref int bufferOffset);
    }
}
