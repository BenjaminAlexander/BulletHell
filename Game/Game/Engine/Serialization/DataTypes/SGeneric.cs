using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization.DataTypes
{
    abstract class SGeneric<T> : Serializable where T : new()
    {
        T value;

        public SGeneric()
        {
            this.value = new T();
        }

        public SGeneric(T value)
        {
            this.value = value;
        }

        public abstract int SerializationSize { get; }

        public T Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
            }
        }

        public abstract void Deserialize(byte[] buffer, ref int bufferOffset);
        public abstract void Serialize(byte[] buffer, ref int bufferOffset);

        public static implicit operator T(SGeneric<T> s)
        {
            return s.value;
        }
    }
}
