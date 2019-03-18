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
    partial class GameObject : Deserializable
    {
        private InstantSelector instantSelector;
        private List<Field> fields = new List<Field>();

        public InstantSelector InstantSelector
        {
            set
            {
                instantSelector = value;
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

        public void Serialize(int instant, byte[] buffer, ref int bufferOffset)
        {
            if (buffer.Length - bufferOffset < this.GetSerializationSize(instant))
            {
                throw new Exception("Buffer length does not match expected state length");
            }

            Buffer.BlockCopy(BitConverter.GetBytes(instant), 0, buffer, bufferOffset, sizeof(int));
            bufferOffset = bufferOffset + sizeof(int);

            foreach (Field field in fields)
            {
                field.Serialize(instant, buffer, ref bufferOffset);
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
            this.InitializeNextInstant(this.instantSelector.ReadInstant);
            this.Update();
        }

        protected virtual void Update()
        {

        }

        public static Type Construct<Type>(InstantSelector selector) where Type : GameObject, new()
        {
            Type newObject = new Type();
            newObject.instantSelector = selector;
            return newObject;
        }
    }
}
