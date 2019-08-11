using MyGame.Engine.GameState.Instants;
using MyGame.Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState.Fields
{
    class StructField<T> : SerializableField<T> where T : struct, Serializable, Deserializable
    {
        public StructField(CreationToken creationToken) : base(creationToken)
        {
        }

        internal override T Copy(T value)
        {
            return value;
        }

        internal override T NewDefaultValue()
        {
            return default(T);
        }

        public T this[CurrentInstant current]
        {
            get
            {
                return GetValue(current);
            }
        }

        public T this[NextInstant next]
        {
            set
            {
                SetValue(next, value);
            }
        }
    }
}
