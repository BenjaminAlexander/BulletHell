using MyGame.Engine.Serialization;
using MyGame.Engine.Serialization.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    struct IntegerValue : FieldValue
    {
        private int value;

        public IntegerValue(int value)
        {
            this.value = value;
        }

        public int Value
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

        public static implicit operator IntegerValue(int value)
        {
            return new IntegerValue(value);
        }

        public static implicit operator int(IntegerValue integerValue)
        {
            return integerValue.value;
        }

        public int SerializationSize
        {
            get
            {
                return sizeof(int);
            }
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            this.Value = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
        }

        public void Serialize(byte[] buffer, ref int bufferOffset)
        {
            Serialization.Utils.Write(this.Value, buffer, ref bufferOffset);
        }

        public override bool Equals(object obj)
        {
            if (obj is IntegerValue)
            {
                return this.value == ((IntegerValue)obj).value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }
    }
}
