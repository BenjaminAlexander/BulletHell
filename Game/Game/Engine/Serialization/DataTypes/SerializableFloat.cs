using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization.DataTypes
{
    class SerializableFloat : GenericSerializable<float>
    {
        public static implicit operator SerializableFloat(float value)
        {
            return new SerializableFloat(value);
        }

        public SerializableFloat() : base()
        {
        }

        public SerializableFloat(float value) : base(value)
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