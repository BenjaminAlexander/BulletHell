using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization.DataTypes
{
    class SInteger : SGeneric<int>
    {
        public static implicit operator SInteger(int value)
        {
            return new SInteger(value);
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
            int value;
            Utils.Read(out value, buffer, ref bufferOffset);
            this.Value = value;
        }

        public override void Serialize(byte[] buffer, ref int bufferOffset)
        {
            Utils.Write(this.Value, buffer, ref bufferOffset);
        }
    }
}
