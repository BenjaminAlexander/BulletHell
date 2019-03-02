using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MyGame.Engine.GameState.ObjectState
{
    class Vector2Field : GenericField<Vector2>
    {
        public Vector2Field(ObjectState obj) : base(obj)
        {
        }

        public override int SerializationSize
        {
            get
            {
                return sizeof(float) * 2;
            }
        }

        public override int Deserialize(byte[] buffer, int bufferOffset)
        {
            float x = BitConverter.ToSingle(buffer, bufferOffset);
            float y = BitConverter.ToSingle(buffer, bufferOffset + sizeof(float));
            this.Value = new Vector2(x, y);
            return bufferOffset + this.SerializationSize;
        }

        public override void Serialize(byte[] buffer, int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(this.Value.X), 0, buffer, bufferOffset, sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(this.Value.Y), 0, buffer, bufferOffset + sizeof(float), sizeof(float));
        }
    }
}