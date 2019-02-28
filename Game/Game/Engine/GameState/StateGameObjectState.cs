using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    class StateGameObjectState : Serializable
    {
        abstract class StateField : Serializable
        {
            public StateField(StateGameObjectState obj)
            {
                obj.AddField(this);
            }

            public abstract int SerializationSize { get; }
            public abstract void Deserialize(byte[] buffer, int bufferOffset);
            public abstract void Serialize(byte[] buffer, int bufferOffset);
        }

        List<StateField> stateFields = new List<StateField>();
        int stateSize = 0;

        private int AddField(StateField field)
        {
            int bufferAddress = stateSize;
            stateSize = stateSize + field.SerializationSize;
            this.stateFields.Add(field);
            return bufferAddress;
        }

        public int SerializationSize
        {
            get
            {
                return stateSize;
            }
        }

        public void Deserialize(byte[] buffer, int bufferOffset)
        {
            if (buffer.Length - bufferOffset < this.SerializationSize)
            {
                throw new Exception("Buffer length does not match expected state length");
            }

            int position = bufferOffset;
            foreach (StateField field in stateFields)
            {
                field.Deserialize(buffer, position);
                position = position + field.SerializationSize;
            }
        }

        public void Serialize(byte[] buffer, int bufferOffset)
        {
            int position = bufferOffset;
            foreach (StateField field in stateFields)
            {
                field.Serialize(buffer, position);
                position = position + field.SerializationSize;
            }
        }
    }
}
