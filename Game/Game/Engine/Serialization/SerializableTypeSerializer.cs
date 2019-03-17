using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class SerializableTypeSerializer<BaseType> : TypeSerializer<BaseType> where BaseType : Serializable
    {
        public SerializableTypeSerializer(TypeFactory<BaseType> factory) : base(factory, new SerializableSerializer<BaseType>())
        {
        }
    }
}
