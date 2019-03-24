using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    abstract class LinkedInstantSerializer<T> : InstantSerializer<T>
    {
        InstantSerializer<T> nestedSerializer;

        public LinkedInstantSerializer(InstantSerializer<T> nestedSerializer)
        {
            this.nestedSerializer = nestedSerializer;
        }

        public void Deserialize(T obj, byte[] buffer, ref int bufferOffset)
        {
            this.AdditionalDeserialize(obj, buffer, ref bufferOffset);
            if (nestedSerializer != null)
            {
                nestedSerializer.Deserialize(obj, buffer, ref bufferOffset);
            }
        }

        public int SerializationSize(T obj, int instant)
        {
            int total = this.AdditionalSerializationSize(obj);
            if (nestedSerializer != null)
            {
                total = total + nestedSerializer.SerializationSize(obj, instant);
            }
            return total;
        }

        public void Serialize(T obj, int instant, byte[] buffer, ref int bufferOffset)
        {
            this.AdditionalSerialize(obj, buffer, ref bufferOffset);
            if (nestedSerializer != null)
            {
                nestedSerializer.Serialize(obj, instant, buffer, ref bufferOffset);
            }
        }

        protected abstract int AdditionalSerializationSize(T obj);
        protected abstract void AdditionalDeserialize(T obj, byte[] buffer, ref int bufferOffset);
        protected abstract void AdditionalSerialize(T obj, byte[] buffer, ref int bufferOffset);
    }
}
