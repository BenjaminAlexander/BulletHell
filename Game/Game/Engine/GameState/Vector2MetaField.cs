using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class Vector2MetaField : GenericMetaField<Vector2>
    {
        public Vector2MetaField(GameObject obj) : base(obj)
        {
        }

        public override int Size
        {
            get
            {
                return sizeof(float) * 2;
            }
        }

        public override void Deserialize(int instant, byte[] buffer, int bufferOffset)
        {
            float x = BitConverter.ToSingle(buffer, bufferOffset);
            float y = BitConverter.ToSingle(buffer, bufferOffset + sizeof(float));
            this[instant] = new Vector2(x, y);
        }

        public override void Serialize(int instant, byte[] buffer, int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(this[instant].X), 0, buffer, bufferOffset, sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(this[instant].Y), 0, buffer, bufferOffset + sizeof(float), sizeof(float));
        }
    }
}
