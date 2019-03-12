using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    class SerializableInteger : GenericSerializable<int>
    {
        public static implicit operator SerializableInteger(int value)
        {
            return new SerializableInteger(value);
        }

        public SerializableInteger() : base()
        {
        }

        public SerializableInteger(int value) : base(value)
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

        public override void Serialize(byte[] buffer, int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(this.Value), 0, buffer, bufferOffset, this.SerializationSize);
        }
    }
}
