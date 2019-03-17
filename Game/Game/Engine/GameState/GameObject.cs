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
    partial class GameObject : FullSerializable
    {
        public static int GetInstant(byte[] buffer, int bufferOffset)
        {
            return BitConverter.ToInt32(buffer, bufferOffset);
        }

        private InstantSelector instantSelector = new InstantSelector.InstantController();
        private List<Field> fields = new List<Field>();

        public InstantSelector InstantSelector
        {
            get
            {
                return instantSelector;
            }

            set
            {
                //TODO: revisit this design
                instantSelector = value;
                foreach(Field field in fields)
                {
                    field.InstantSelector = value;
                }
            }
        }

        public int SerializationSize
        {
            get
            {
                return this.GetSerializationSize(instantSelector.WriteInstant); ;
            }
        }

        public int GetSerializationSize(int instant)
        {
            int serializationSize = sizeof(int);
            foreach (Field field in fields)
            {
                serializationSize = serializationSize + field.SerializationSize(instant);
            }
            return serializationSize;
        }

        private void AddField(Field field)
        {
            this.fields.Add(field);
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
            Serialize(instantSelector.WriteInstant, buffer, bufferOffset);
        }

        public void Serialize(int instant, byte[] buffer, int bufferOffset)
        {
            if (buffer.Length - bufferOffset < this.GetSerializationSize(instant))
            {
                throw new Exception("Buffer length does not match expected state length");
            }

            Buffer.BlockCopy(BitConverter.GetBytes(instant), 0, buffer, bufferOffset, sizeof(int));
            bufferOffset = bufferOffset + sizeof(int);

            foreach (Field field in fields)
            {
                field.Serialize(instant, buffer, bufferOffset);
                bufferOffset = bufferOffset + field.SerializationSize(instant);
            }
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int instant = Serialization.Utils.ReadInt(buffer, ref bufferOffset);

            foreach (Field field in fields)
            {
                field.Deserialize(instant, buffer, ref bufferOffset);
            }
        }

        private void InitializeNextInstant(int instant)
        {
            foreach(Field field in fields)
            {
                field.InitializeNextInstant(instant);
            }
        }

        public void UpdateNextInstant()
        {
            this.InitializeNextInstant(this.instantSelector.WriteInstant);
            this.Update();
        }

        public virtual void Update()
        {

        }
    }
}
