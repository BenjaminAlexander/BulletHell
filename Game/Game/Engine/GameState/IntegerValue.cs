using MyGame.Engine.Serialization.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class IntegerValue : GenericFieldValue<int>
    {
        public override int SerializationSize
        {
            get
            {
                return sizeof(int);
            }
        }

        public override void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            this.Value = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
        }

        public override void Serialize(byte[] buffer, ref int bufferOffset)
        {
            Serialization.Utils.Write(this.Value, buffer, ref bufferOffset);
        }
    }
}
