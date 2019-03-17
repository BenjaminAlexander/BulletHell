using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    class InstantSelector : Serializer<GameObject>
    {
        int readInstant;
        int writeInstant;

        private InstantSelector()
        {
            this.readInstant = 0;
            this.writeInstant = 1;
        }

        public int ReadInstant
        {
            get
            {
                return this.readInstant;
            }
        }

        public int WriteInstant
        {
            get
            {
                return this.writeInstant;
            }
        }

        public int SerializeInstant
        {
            get
            {
                return this.writeInstant;
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

        public class InstantController : InstantSelector
        {
            public InstantController() : base()
            {
            }

            public void SetReadInstant(int instant)
            {
                this.readInstant = instant;
            }

            public void SetWriteInstant(int instant)
            {
                this.writeInstant = instant;
            }

            public InstantSelector Selector
            {
                get
                {
                    return this;
                }
            }

            public void SetReadWriteInstant(int readInstant)
            {
                this.SetReadInstant(readInstant);
                this.SetWriteInstant(readInstant + 1);
            }

            public void AdvanceReadWriteInstant()
            {
                this.SetReadWriteInstant(this.ReadInstant + 1);
            }
        }
    }
}
