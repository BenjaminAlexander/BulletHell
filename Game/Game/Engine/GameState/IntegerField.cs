using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class IntegerField : GenericMetaField<int>
    {
        public IntegerField(GameObject obj) : base(obj)
        {
        }

        public override int Size
        {
            get
            {
                return sizeof(int);
            }
        }

        public override void Deserialize(int instant, byte[] buffer, int bufferOffset)
        {
            this[instant] = BitConverter.ToInt32(buffer, bufferOffset);
        }

        public override void Serialize(int instant, byte[] buffer, int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(this[instant]), 0, buffer, bufferOffset, this.Size);
        }
    }
}
