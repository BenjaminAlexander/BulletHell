using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Reflection;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    partial class GameObject : InstantSerializable
    {
        public static SubType NewObject<SubType>(InstantSelector instantSelector) where SubType : GameObject, new()
        {
            SubType newObj = new SubType();
            newObj.instantSelector = instantSelector;
            return newObj;
        }

        private InstantSelector instantSelector;
        private List<Field> fields = new List<Field>();

        internal InstantSelector InstantSelector
        {
            get
            {
                return instantSelector;
            }
        }

        public void SetDependencies(InstantSelector instantSelector)
        {
            this.instantSelector = instantSelector;
        }

        public SubType NewObject<SubType>() where SubType : GameObject, new()
        {
            SubType newObj = new SubType();
            newObj.instantSelector = this.instantSelector;
            return newObj;
        }

        public int SerializationSize(int instant)
        {
            int serializationSize = sizeof(int) + sizeof(bool);
            if (StateAtInstantExists(instant))
            {
                foreach (Field field in fields)
                {
                    serializationSize = serializationSize + field.SerializationSize(instant);
                }
            }
            return serializationSize;
        }

        private void AddField(Field field)
        {
            this.fields.Add(field);
        }

        internal bool StateAtInstantExists(int instant)
        {
            foreach(Field field in fields)
            {
                if(!field.FieldAtInstantExists(instant))
                {
                    return false;
                }
            }
            return true;
        }

        public void Serialize(int instant, byte[] buffer, ref int bufferOffset)
        {
            if (buffer.Length - bufferOffset < this.SerializationSize(instant))
            {
                throw new Exception("Buffer length does not match expected state length");
            }

            bool stateExists = StateAtInstantExists(instant);
            Serialization.Utils.Write(instant, buffer, ref bufferOffset);
            Serialization.Utils.Write(stateExists, buffer, ref bufferOffset);

            if (stateExists)
            {
                foreach (Field field in fields)
                {
                    field.Serialize(instant, buffer, ref bufferOffset);
                }
            }
        }

        public void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            int instant = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            bool stateExists = Serialization.Utils.ReadBool(buffer, ref bufferOffset);

            if (stateExists)
            {
                foreach (Field field in fields)
                {
                    field.Deserialize(instant, buffer, ref bufferOffset);
                }
            }
            else
            {
                foreach (Field field in fields)
                {
                    field.Remove(instant);
                }
            }
        }

        internal void CopyInstant(int from, int to)
        {
            foreach(Field field in fields)
            {
                field.CopyInstant(from, to);
            }
        }

        internal void Update(int read, int write)
        {
            this.CopyInstant(read, write);
            this.Update();
        }

        protected virtual void Update()
        {

        }
    }
}
