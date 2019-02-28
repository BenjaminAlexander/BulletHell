using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    partial class GameObject
    {
        List<Field> fields = new List<Field>();
        int stateSize = 0;
        public int StateSize
        {
            get
            {
                return stateSize;
            }
        }

        private int AddField(Field fields)
        {
            int bufferAddress = stateSize;
            stateSize = stateSize + fields.Size;
            this.fields.Add(fields);
            return bufferAddress;
        }

        public byte[] Serialize(int instant)
        {
            byte[] buffer = new byte[this.stateSize];
            int position = 0;
            foreach (Field field in fields)
            {
                field.Serialize(instant, buffer, position);
                position = position + field.Size;
            }
            return buffer;
        }

        public void Deserialize(int instant, byte[] buffer)
        {
            if (buffer.Length != this.stateSize)
            {
                throw new Exception("Buffer length does not match expected state length");
            }

            int position = 0;
            foreach (Field field in fields)
            {
                field.Deserialize(instant, buffer, position);
                position = position + field.Size;
            }
        }
    }
}
