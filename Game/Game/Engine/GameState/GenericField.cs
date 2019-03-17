using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    class GenericField<DataType, SerializableType> : GameObject.Field where DataType : new() where SerializableType : GenericSerializable<DataType>, new()
    {
        private Dictionary<int, SerializableType> fieldAtInstant = new Dictionary<int, SerializableType>();

        public GenericField(GameObject obj) : base(obj)
        {
        }

        private DataType this[int instant]
        {
            get
            {
                return fieldAtInstant[instant];
            }

            set
            {
                if (!fieldAtInstant.ContainsKey(instant))
                {
                    fieldAtInstant[instant] = new SerializableType();
                }
                fieldAtInstant[instant].Value = value;
            }
        }

        public DataType Value
        {
            get
            {
                return Read;
            }

            set
            {
                Write = value;
            }
        }

        public DataType Read
        {
            get
            {
                return this[this.InstantSelector.ReadInstant];
            }
        }

        public DataType Write
        {
            get
            {
                return this[this.InstantSelector.WriteInstant];
            }

            set
            {
                this[this.InstantSelector.WriteInstant] = value;
            }
        }

        protected override void Copy(GameObject.Field other, int instant)
        {
            this[instant] = ((GenericField<DataType, SerializableType>)other)[instant];
        }

        public override int SerializationSize(int instant)
        {
            return fieldAtInstant[instant].SerializationSize;
        }

        public override void Deserialize(int instant, byte[] buffer, ref int bufferOffset)
        {
            if(!fieldAtInstant.ContainsKey(instant))
            {
                this[instant] = new SerializableType();
            }
            fieldAtInstant[instant].Deserialize(buffer, ref bufferOffset);
        }

        public override void Serialize(int instant, byte[] buffer, ref int bufferOffset)
        {
            fieldAtInstant[instant].Serialize(buffer, ref bufferOffset);
        }

        public override sealed void InitializeNextInstant(int currentInstant)
        {
            this[currentInstant + 1] = this[currentInstant];
        }

        public override void InitializeInstant(int instant)
        {
            this[instant] = new SerializableType();
        }
    }
}
