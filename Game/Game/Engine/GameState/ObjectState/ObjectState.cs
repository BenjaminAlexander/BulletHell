using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState.ObjectState
{
    partial class ObjectState : Serializable
    {
        private int serializationSize = 0;
        private List<Field> fields = new List<Field>();

        public int SerializationSize
        {
            get
            {
                return serializationSize;
            }
        }

        private int AddField(Field field)
        {
            int bufferAddress = serializationSize;
            serializationSize = serializationSize + field.SerializationSize;
            this.fields.Add(field);
            return bufferAddress;
        }

        public void Serialize(byte[] buffer, int bufferOffset)
        {
            if (buffer.Length - bufferOffset < this.serializationSize)
            {
                throw new Exception("Buffer length does not match expected state length");
            }
            foreach (Field field in fields)
            {
                field.Serialize(buffer, bufferOffset);
                bufferOffset = bufferOffset + field.SerializationSize;
            }
        }

        public int Deserialize(byte[] buffer, int bufferOffset)
        {
            foreach (Field field in fields)
            {
                bufferOffset = field.Deserialize(buffer, bufferOffset);
            }
            return bufferOffset;
        }
    }
}
