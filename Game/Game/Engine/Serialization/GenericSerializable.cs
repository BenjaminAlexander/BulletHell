using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    abstract class GenericSerializable<T> : Serializable
    {
        public static implicit operator T(GenericSerializable<T> obj)
        {
            return obj.value;
        }
        //  User-defined conversion from double to Digit
        public static implicit operator GenericSerializable<T>(T value)
        {
            return new GenericSerializable<T>(value);
        }

        private T value;
        public GenericSerializable(T value)
        {
            this.value = value;
        }
    }
}
