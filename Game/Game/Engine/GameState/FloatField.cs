using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class FloatField : GenericMetaField<float>
    {
        public FloatField(GameObject obj) : base(obj)
        {
        }

        public override int Size
        {
            get
            {
                return sizeof(float);
            }
        }

        public override void Deserialize(int instant, byte[] buffer, int bufferOffset)
        {
            this[instant] = BitConverter.ToSingle(buffer, bufferOffset);
        }

        public override void Serialize(int instant, byte[] buffer, int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(this[instant]), 0, buffer, bufferOffset, this.Size);
        }
    }
}
