/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;

namespace MyGame.Engine.Serialization
{
    class SerializedCollection<BaseType> : DeserializedCollection<BaseType> where BaseType : Serializable
    {
        TypeSerializer<BaseType> typeSerializer;

        public SerializedCollection(TypeSerializer<BaseType> typeSerializer) : base(typeSerializer)
        {
            this.typeSerializer = typeSerializer;
        }

        public SerializedCollection(TypeFactory<BaseType> factory) : this(new TypeSerializer<BaseType>(factory))
        {

        }

        public int SerializationSize(BaseType obj)
        {
            return sizeof(int) + typeSerializer.SerializationSize(obj);
        }

        public void Serialize(BaseType obj, byte[] buffer, ref int bufferOffset)
        {
            Utils.Write(GetObjectID(obj), buffer, ref bufferOffset);
            typeSerializer.Serialize(obj, buffer, ref bufferOffset);
        }

        public byte[] Serialize(BaseType obj)
        {
            byte[] buffer = new byte[SerializationSize(obj)];
            int offset = 0;
            Serialize(obj, buffer, ref offset);
            return buffer;
        }
    }
}
*/