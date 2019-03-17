using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    interface Deserializer<Type>
    {
        void Deserialize(Type obj, byte[] buffer, ref int bufferOffset);
    }
}
