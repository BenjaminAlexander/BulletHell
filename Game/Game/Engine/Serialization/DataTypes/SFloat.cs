using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization.DataTypes
{
    class SFloat : Generic<float>
    {
        public static implicit operator SFloat(float value)
        {
            return new SFloat(value);
        }

        public SFloat() : base()
        {
        }

        public SFloat(float value) : base(value)
        {
        }

        public override int SerializationSize
        {
            get
            {
                return sizeof(float);
            }
        }

        public override void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            this.Value = Serialization.Utils.ReadFloat(buffer, ref bufferOffset);
        }

        public override void Serialize(byte[] buffer, ref int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(this.Value), 0, buffer, bufferOffset, this.SerializationSize);
            bufferOffset = bufferOffset + sizeof(float);
        }
    }
}