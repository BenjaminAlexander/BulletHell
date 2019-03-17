using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    interface Serializer<Type>
    {
        int SerializationSize(Type obj);
        void Serialize(Type obj, byte[] buffer, int bufferOffset);
    }
}
