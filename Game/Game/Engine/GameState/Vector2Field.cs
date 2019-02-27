using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class Vector2Field : GenericStateField<Vector2>
    {
        public Vector2Field(StateAtInstant state, Vector2 value) : base(state, value)
        {
        }

        public override int Size
        {
            get
            {
                return sizeof(float) * 2;
            }
        }

        public override void Deserialize(byte[] buffer, int bufferOffset)
        {
            float x = BitConverter.ToInt32(buffer, bufferOffset);
            float y = BitConverter.ToInt32(buffer, bufferOffset + sizeof(float));
            this.Value = new Vector2(x, y);
        }

        public override void Serialize(byte[] buffer, int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(this.Value.X), 0, buffer, bufferOffset, sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(this.Value.Y), 0, buffer, bufferOffset + sizeof(float), sizeof(float));
        }
    }
}
