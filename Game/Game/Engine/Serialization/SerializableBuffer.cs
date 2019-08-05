using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.Serialization
{
    class SerializableBuffer : Serializable
    {
        private byte[] buffer;

        public SerializableBuffer(byte[] buffer)
        {
            this.buffer = buffer;
        }

        public int SerializationSize
        {
            get
            {
                return buffer.Length;
            }
        }

        public void Serialize(byte[] buffer, ref int bufferOffset)
        {
            Utils.Write(this.buffer, buffer, ref bufferOffset);
        }

        public static implicit operator SerializableBuffer(byte[] buffer)
        {
            return new SerializableBuffer(buffer);
        }
    }
}
