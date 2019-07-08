/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.DataStructures;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class InstantSerializedCollection<BaseType> : DeserializedCollection<BaseType> where BaseType : InstantSerializable
    {
        InstantTypeSerializer<BaseType> typeSerializer;

        public InstantSerializedCollection(InstantTypeSerializer<BaseType> typeSerializer) : base(typeSerializer)
        {
            this.typeSerializer = typeSerializer;
        }

        public InstantSerializedCollection(TypeFactory<BaseType> factory) : this(new InstantTypeSerializer<BaseType>(factory))
        {
            
        }

        public int SerializationSize(BaseType obj, int instant)
        {
            return sizeof(int) + typeSerializer.SerializationSize(obj, instant);
        }

        public void Serialize(BaseType obj, int instant, byte[] buffer, ref int bufferOffset)
        {
            Utils.Write(GetObjectID(obj), buffer, ref bufferOffset);
            typeSerializer.Serialize(obj, instant, buffer, ref bufferOffset);
        }

        public byte[] Serialize(BaseType obj, int instant)
        {
            byte[] buffer = new byte[SerializationSize(obj, instant)];
            int offset = 0;
            Serialize(obj, instant, buffer, ref offset);
            return buffer;
        }
    }
}
*/