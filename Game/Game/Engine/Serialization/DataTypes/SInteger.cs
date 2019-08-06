using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization.DataTypes
{
    struct SInteger : Serializable, Deserializable
    {
        int value;

        public SInteger(int value)
        {
            this.value = value;
        }

        public int Value
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


        public static implicit operator int(SInteger s)
        {
            return s.value;
        }

        public static implicit operator SInteger(int value)
        {
            return new SInteger(value);
        }

        public int SerializationSize
        {
            get
            {
                return sizeof(int);
            }
        }

        public void Serialize(byte[] buffer, ref int bufferOffset)
        {
            Utils.Write(this.value, buffer, ref bufferOffset);
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            Utils.Read(out value, buffer, ref bufferOffset);
        }
    }
}