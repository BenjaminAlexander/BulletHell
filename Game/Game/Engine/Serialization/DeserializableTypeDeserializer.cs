using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class DeserializableTypeDeserializer<BaseType> : TypeDeserializer<BaseType> where BaseType : Deserializable
    {
        public DeserializableTypeDeserializer(TypeFactory<BaseType> factory) : base(factory, new DeserializableDeserializer<BaseType>())
        {
        }
    }
}
