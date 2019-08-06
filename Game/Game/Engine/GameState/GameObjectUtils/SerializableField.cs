using MyGame.Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState.GameObjectUtils
{
    abstract class SerializableField<T> : GenericField<T> where T : Serializable, Deserializable
    {
        public SerializableField(CreationToken creationToken) : base(creationToken)
        {
        }

        internal override T Deserialize(byte[] buffer, ref int bufferOffset)
        {
            T value = NewDefaultValue();
            value.Deserialize(buffer, ref bufferOffset);
            return value;
        }

        internal override int SerializationSize(T value)
        {
            return value.SerializationSize;
        }

        internal override void Serialize(T value, byte[] buffer, ref int bufferOffset)
        {
            value.Serialize(buffer, ref bufferOffset);
        }
    }
}
