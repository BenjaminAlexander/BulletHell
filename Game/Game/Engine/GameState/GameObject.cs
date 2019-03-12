using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;
using static MyGame.Engine.Serialization.Utils;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    partial class GameObject : Serializable
    {
        public static int GetInstant(byte[] buffer, int bufferOffset)
        {
            return BitConverter.ToInt32(buffer, bufferOffset);
        }

        private int currentInstant;
        private int serializationSize = sizeof(int);
        private List<Field> fields = new List<Field>();

        public int CurrentInstant
        {
            get
            {
                return currentInstant;
            }

            set
            {
                currentInstant = value;
            }
        }

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
            serializationSize = serializationSize + field.Size;
            this.fields.Add(field);
            return bufferAddress;
        }

        public void CopyFrom(GameObject other, int instant)
        {
            if (this.GetType() == other.GetType())
            {
                for(int i = 0; i < this.fields.Count; i++)
                {
                    this.fields[i].CopyFrom(other.fields[i], instant);
                }
            }
            else
            {
                throw new Exception("Field type does not match");
            }
        }

        public void Serialize(byte[] buffer, int bufferOffset)
        {
            Serialize(currentInstant, buffer, bufferOffset);
        }

        public void Serialize(int instant, byte[] buffer, int bufferOffset)
        {
            if (buffer.Length - bufferOffset < this.serializationSize)
            {
                throw new Exception("Buffer length does not match expected state length");
            }

            Buffer.BlockCopy(BitConverter.GetBytes(instant), 0, buffer, bufferOffset, sizeof(int));
            bufferOffset = bufferOffset + sizeof(int);

            foreach (Field field in fields)
            {
                field.Serialize(instant, buffer, bufferOffset);
                bufferOffset = bufferOffset + field.Size;
            }
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            if (buffer.Length - bufferOffset < this.serializationSize)
            {
                throw new Exception("Buffer length does not match expected state length");
            }

            int instant = Serialization.Utils.ReadInt(buffer, ref bufferOffset);

            foreach (Field field in fields)
            {
                field.Deserialize(instant, buffer, ref bufferOffset);
            }
        }

        private StateAtInstant DefaultState()
        {
            return new StateAtInstant(fields);
        }
    }
}
