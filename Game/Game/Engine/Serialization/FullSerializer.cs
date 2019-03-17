using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    interface FullSerializer<Type> : Serializer<Type>, Deserializer<Type>
    {
    }
}
