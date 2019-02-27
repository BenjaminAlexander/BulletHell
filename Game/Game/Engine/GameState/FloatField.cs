using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class FloatField : GenericStateField<float>
    {
        public FloatField(StateAtInstant state, float value) : base(state, value)
        {
        }

        public override int Size
        {
            get
            {
                return sizeof(float);
            }
        }

        public override void Deserialize(byte[] buffer, int bufferOffset)
        {
            this.Value = BitConverter.ToSingle(buffer, bufferOffset);
        }

        public override void Serialize(byte[] buffer, int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(this.Value), 0, buffer, bufferOffset, this.Size);
        }
    }
}
