using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization.DataTypes
{
    class SInteger : Generic<int>
    {
        public static implicit operator SInteger(int value)
        {
            return new SInteger(value);
        }

        public SInteger() : base()
        {
        }

        public SInteger(int value) : base(value)
        {
        }

        public override int SerializationSize
        {
            get
            {
                return sizeof(int);
            }
        }

        public override void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            this.Value = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
        }

        public override void Serialize(byte[] buffer, ref int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(this.Value), 0, buffer, bufferOffset, this.SerializationSize);
            bufferOffset = bufferOffset + sizeof(int);
        }
    }
}
