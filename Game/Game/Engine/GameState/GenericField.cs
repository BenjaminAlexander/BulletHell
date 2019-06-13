using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.Serialization.DataTypes;

namespace MyGame.Engine.GameState
{
    class GenericField<DataType, SerializableType> : GameObject.Field where DataType : new() where SerializableType : SGeneric<DataType>, new()
    {
        private SerializableType field = new SerializableType();
        private GenericField<DataType, SerializableType> writeField = null;

        public GenericField(GameObject obj) : base(obj)
        {
            //TODO: this feels like a hack to allow initialization
            writeField = this;
        }

        private DataType ThisValue
        {
            get
            {
                return field;
            }

            set
            {
                field.Value = value;
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
                return this.ThisValue;
            }
        }

        public DataType Write
        {
            get
            {
                return this.writeField.ThisValue;
            }

            set
            {
                this.writeField.ThisValue = value;
            }
        }

        public override int SerializationSize
        {
            get
            {
                return field.SerializationSize;
            }
        }

        protected override void Copy(GameObject.Field other)
        {
            field.Value = ((GenericField<DataType, SerializableType>)other).field.Value;
        }

        public override void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            field.Deserialize(buffer, ref bufferOffset);
        }

        public override void Serialize(byte[] buffer, ref int bufferOffset)
        {
            field.Serialize(buffer, ref bufferOffset);
        }

        public override void SetWriteField(GameObject.Field writeField)
        {
            this.writeField = (GenericField<DataType, SerializableType>)writeField;
        }
    }
}
