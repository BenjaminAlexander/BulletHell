using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MyGame.Engine.Serialization.DataTypes
{
    class SVector2 : Generic<Vector2>
    {
        public static implicit operator SVector2(Vector2 value)
        {
            return new SVector2(value);
        }

        public SVector2() : base()
        {
        }

        public SVector2(SVector2 value) : base(value)
        {
        }

        public override int SerializationSize
        {
            get
            {
                return sizeof(float) * 2;
            }
        }

        public override void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            float x = Serialization.Utils.ReadFloat(buffer, ref bufferOffset);
            float y = Serialization.Utils.ReadFloat(buffer, ref bufferOffset);
            this.Value = new Vector2(x, y);
        }

        public override void Serialize(byte[] buffer, ref int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(this.Value.X), 0, buffer, bufferOffset, sizeof(float));
            bufferOffset = bufferOffset + sizeof(float);
            Buffer.BlockCopy(BitConverter.GetBytes(this.Value.Y), 0, buffer, bufferOffset, sizeof(float));
            bufferOffset = bufferOffset + sizeof(float);
        }
    }
}
