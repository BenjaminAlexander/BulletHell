using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.DataStructures;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class SerializableCollection<BaseType> : SerializedCollection<BaseType> where BaseType : Serializable
    {

        public SerializableCollection(TypeFactory<BaseType> factory) : base(factory, new SerializableSerializer<BaseType>())
        {
        }
    }
}
