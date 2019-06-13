using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    partial class GameObject : Serializable
    {
        public static SubType NewObject<SubType>(int instant) where SubType : GameObject, new()
        {
            SubType newObj = new SubType();
            newObj.instant = instant;
            return newObj;
        }

        private List<Field> fields = new List<Field>();
        private int instant;

        public int SerializationSize
        {
            get
            {
                int serializationSize = sizeof(int) + sizeof(bool);
                foreach (Field field in fields)
                {
                    serializationSize = serializationSize + field.SerializationSize;
                }
                return serializationSize;
            }
        }

        public void SetInstant(int instant)
        {
            this.instant = instant;
        }

        private void AddField(Field field)
        {
            this.fields.Add(field);
        }

        public void Serialize(byte[] buffer, ref int bufferOffset)
        {
            if (buffer.Length - bufferOffset < this.SerializationSize)
            {
                throw new Exception("Buffer length does not match expected state length");
            }

            Serialization.Utils.Write(instant, buffer, ref bufferOffset);

            foreach (Field field in fields)
            {
                field.Serialize(buffer, ref bufferOffset);
            }
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int instant = Serialization.Utils.ReadInt(buffer, ref bufferOffset);

            foreach (Field field in fields)
            {
                field.Deserialize(buffer, ref bufferOffset);
            }
        }

        public GameObject NextInstant()
        {
            GameObject next = (GameObject)Activator.CreateInstance(this.GetType());
            next.instant = this.instant + 1;

            for (int i = 0; i < this.fields.Count; i++)
            {
                this.fields[i].SetWriteField(next.fields[i]);
            }
            this.Update();
            return next;
        }

        protected virtual void Update()
        {

        }
    }
}
