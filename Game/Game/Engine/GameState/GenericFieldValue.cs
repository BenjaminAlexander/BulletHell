using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.Serialization.DataTypes;

namespace MyGame.Engine.GameState
{
    class GenericFieldValue<DataType, SerializableType> : GameObject.FieldValue where DataType : new() where SerializableType : SGeneric<DataType>, new()
    {
        private SerializableType field = new SerializableType();

        public DataType Value
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

        public override int SerializationSize
        {
            get
            {
                return field.SerializationSize;
            }
        }

        protected override void Copy(GameObject.FieldValue other)
        {
            if (this.GetType() == other.GetType())
            {
                this.field.Value = ((GenericFieldValue<DataType, SerializableType>)other).field.Value;
            }
            else
            {
                throw new Exception("Field type does not match");
            }
            
        }

        public override void Deserialize(byte[] buffer, ref int bufferOffset)
        {
            field.Deserialize(buffer, ref bufferOffset);
        }

        public override void Serialize(byte[] buffer, ref int bufferOffset)
        {
            field.Serialize(buffer, ref bufferOffset);
        }

        public override bool IsEqual(GameObject.FieldValue other)
        {
            if(other is GenericFieldValue<DataType, SerializableType>)
            {
                GenericFieldValue<DataType, SerializableType> otherField = (GenericFieldValue<DataType, SerializableType>)other;
                return this.field.Value.Equals(otherField.field.Value);
            }
            return false;
        }
    }
}
