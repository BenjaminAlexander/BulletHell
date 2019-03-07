using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    /*
    class Instant : Serializable
    {
        private int instantId;

        public Instant()
        {

        }

        public Instant(int instantId)
        {
            this.instantId = instantId;
        }

        public int SerializationSize
        {
            get
            {
                return sizeof(int);
            }
        }

        public int Deserialize(byte[] buffer, int bufferOffset)
        {
            this.instantId = BitConverter.ToInt32(buffer, bufferOffset);
            return bufferOffset + SerializationSize;
        }

        public override bool Equals(object obj)
        {
            return obj is Instant && ((Instant)obj).instantId == this.instantId;
        }

        public override int GetHashCode()
        {
            return instantId;
        }

        public void Serialize(byte[] buffer, int bufferOffset)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(instantId), 0, buffer, bufferOffset, SerializationSize);
        }
    }*/
}
