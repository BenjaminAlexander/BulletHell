using MyGame.Engine.Serialization.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class FloatValue : GenericFieldValue<float>
    {
        public override int SerializationSize
        {
            get
            {
                return sizeof(float);
            }
        }

        public override void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            this.Value = Serialization.Utils.ReadFloat(buffer, ref bufferOffset);
        }

        public override void Serialize(byte[] buffer, ref int bufferOffset)
        {
            Serialization.Utils.Write(this.Value, buffer, ref bufferOffset);
        }
    }
}
