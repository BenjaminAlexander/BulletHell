using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    //TODO: probably delete this class
    class SerializerClosure<Type> : Serializable
    {
        Serializer<Type> serializer;
        Type obj;

        public SerializerClosure(Serializer<Type> serializer, Type obj)
        {
            this.serializer = serializer;
            this.obj = obj;
        }

        public int SerializationSize
        {
            get
            {
                return serializer.SerializationSize(obj);
            }
        }

        public void Serialize(byte[] buffer, int bufferOffset)
        {
            serializer.Serialize(obj, buffer, bufferOffset);
        }
    }
}
