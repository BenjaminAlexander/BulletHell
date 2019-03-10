using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class Vector2Field : GenericMetaField<Vector2>
    {
        public Vector2Field(GameObject obj) : base(obj)
        {
        }

        public override int Size
        {
            get
            {
                return sizeof(float) * 2;
            }
        }

        public override void Deserialize(int instant, byte[] buffer, ref int bufferOffset)
        {
            float x = Serialization.Utils.ReadFloat(buffer, ref bufferOffset);
            float y = Serialization.Utils.ReadFloat(buffer, ref bufferOffset);
            this[instant] = new Vector2(x, y);
        }

        public override void Serialize(int instant, byte[] buffer, int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(this[instant].X), 0, buffer, bufferOffset, sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(this[instant].Y), 0, buffer, bufferOffset + sizeof(float), sizeof(float));
        }
    }
}
