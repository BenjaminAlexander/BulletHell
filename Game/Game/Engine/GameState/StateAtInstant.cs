using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    class StateAtInstant
    {
        private int instant;
        private int size = 0;
        private List<StateField> fields = new List<StateField>();

        public StateAtInstant(int instant)
        {
             this.instant = instant;
        }

        public int Instant
        {
            get
            {
                return instant;
            }
        }

        public int AddField(StateField field)
        {
            int bufferAddress = size;
            size = size + field.Size;
            fields.Add(field);
            return bufferAddress;
        }

        public byte[] Serialize()
        {
            byte[] buffer = new byte[this.size];
            int position = 0;

            foreach(StateField field in fields)
            {
                field.Serialize(buffer, position);
                position = position + field.Size;
            }

            return buffer;
        }

        public void Deserialize(byte[] buffer)
        {
            if(buffer.Length != this.size)
            {
                throw new Exception("Buffer length does not match expected state length");
            }

            int position = 0;

            foreach (StateField field in fields)
            {
                field.Deserialize(buffer, position);
                position = position + field.Size;
            }
        }
    }
}
