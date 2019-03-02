using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState.ObjectState
{
    class IntegerField : GenericField<int>
    {
        public IntegerField(ObjectState obj) : base(obj)
        {
        }

        public override int SerializationSize
        {
            get
            {
                return sizeof(int);
            }
        }

        public override int Deserialize(byte[] buffer, int bufferOffset)
        {
            this.Value = BitConverter.ToInt32(buffer, bufferOffset);
            return bufferOffset + this.SerializationSize;
        }

        public override void Serialize(byte[] buffer, int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(this.Value), 0, buffer, bufferOffset, this.SerializationSize);
        }
    }
}
