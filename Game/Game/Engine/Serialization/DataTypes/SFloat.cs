using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization.DataTypes
{
    struct SFloat : Serializable, Deserializable
    {
        float value;

        public SFloat(float value)
        {
            this.value = value;
        }

        public float Value
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


        public static implicit operator float(SFloat s)
        {
            return s.value;
        }

        public static implicit operator SFloat(float value)
        {
            return new SFloat(value);
        }

        public int SerializationSize
        {
            get
            {
                return sizeof(float);
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