using Microsoft.Xna.Framework;
using MyGame.Engine.Serialization.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class Vector2Value : GenericFieldValue<Vector2>
    {
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
            Serialization.Utils.Write(this.Value.X, buffer, ref bufferOffset);
            Serialization.Utils.Write(this.Value.Y, buffer, ref bufferOffset);
        }
    }
}
