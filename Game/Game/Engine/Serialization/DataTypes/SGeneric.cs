using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization.DataTypes
{
    abstract class Generic<T> : Serializable where T : new()
    {
        T value;

        public Generic()
        {
            this.value = new T();
        }

        public Generic(T value)
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

        public static implicit operator T(Generic<T> s)
        {
            return s.value;
        }
    }
}
