using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    class SimpleInstantSelector : InstantSelector
    {
        int readInstant;
        int writeInstant;

        public SimpleInstantSelector()
        {
            readInstant = 0;
            writeInstant = 1;
        }

        public int ReadInstant
        {
            get
            {
                return readInstant;
            }

            set
            {
                readInstant = value;
            }
        }

        public int WriteInstant
        {
            get
            {
                return writeInstant;
            }

            set
            {
                writeInstant = value;
            }
        }

        public int SerializeInstant
        {
            get
            {
                return readInstant;
            }
        }

        public int SerializationSize(GameObject obj)
        {
            return obj.GetSerializationSize(SerializeInstant);
        }

        public void Serialize(GameObject obj, byte[] buffer, ref int bufferOffset)
        {
            obj.Serialize(SerializeInstant, buffer, ref bufferOffset);
        }

        public void Deserialize(GameObject obj, byte[] buffer, ref int bufferOffset)
        {
            obj.Deserialize(buffer, ref bufferOffset);
            obj.InstantSelector = this;
        }

        public void SetReadWriteInstant(int readInstant)
        {
            this.ReadInstant = readInstant;
            this.WriteInstant = readInstant + 1;
        }

        public void AdvanceReadWriteInstant()
        {
            this.SetReadWriteInstant(this.ReadInstant + 1);
        }
    }
}
