using MyGame.Engine.Serialization;
using MyGame.Engine.Serialization.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState.FieldValues
{
    public struct FloatValue : FieldValue
    {
        private float value;

        public FloatValue(float value)
        {
            this.value = value;
        }

        public float Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public static implicit operator FloatValue(float value)
        {
            return new FloatValue(value);
        }

        public static implicit operator float(FloatValue floatValue)
        {
            return floatValue.value;
        }

        public int SerializationSize
        {
            get
            {
                return sizeof(float);
            }
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            this.Value = Serialization.Utils.ReadFloat(buffer, ref bufferOffset);
        }

        public void Serialize(byte[] buffer, ref int bufferOffset)
        {
            Serialization.Utils.Write(this.Value, buffer, ref bufferOffset);
        }

        public override bool Equals(object obj)
        {
            if (obj is FloatValue)
            {
                return this.value == ((FloatValue)obj).value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }
    }
}
