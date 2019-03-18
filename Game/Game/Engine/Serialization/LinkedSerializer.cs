using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    abstract class LinkedSerializer<Type> : Serializer<Type>, Deserializer<Type>
    {
        Serializer<Type> nestedSerializer;
        Deserializer<Type> nestedDeserializer;

        public LinkedSerializer(Serializer<Type> nestedSerializer, Deserializer<Type> nestedDeserializer)
        {
            this.nestedSerializer = nestedSerializer;
            this.nestedDeserializer = nestedDeserializer;
        }

        public void Deserialize(Type obj, byte[] buffer, ref int bufferOffset)
        {
            this.AdditionalDeserialize(obj, buffer, ref bufferOffset);
            nestedDeserializer.Deserialize(obj, buffer, ref bufferOffset);
        }

        public int SerializationSize(Type obj)
        {
            return nestedSerializer.SerializationSize(obj) + this.AdditionalSerializationSize(obj);
        }

        public void Serialize(Type obj, byte[] buffer, ref int bufferOffset)
        {
            this.AdditionalSerialize(obj, buffer, ref bufferOffset);
            nestedSerializer.Serialize(obj, buffer, ref bufferOffset);
        }

        protected abstract int AdditionalSerializationSize(Type obj);
        protected abstract void AdditionalDeserialize(Type obj, byte[] buffer, ref int bufferOffset);
        protected abstract void AdditionalSerialize(Type obj, byte[] buffer, ref int bufferOffset);
    }
}
