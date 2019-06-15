using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.Serialization;
using MyGame.Engine.Serialization.DataTypes;

namespace MyGame.Engine.GameState
{
    abstract class GenericFieldValue<DataType> : FieldValue where DataType : struct
    {
        private DataType field = new DataType();

        public DataType Value
        {
            get
            {
                return field;
            }

            set
            {
                field = value;
            }
        }

        protected override void Copy(FieldValue other)
        {
                this.field = ((GenericFieldValue<DataType>)other).field;
        }

        public override bool IsEqual(FieldValue other)
        {
            if(other is GenericFieldValue<DataType>)
            {
                GenericFieldValue<DataType> otherField = (GenericFieldValue<DataType>)other;
                return this.field.Equals(otherField.field);
            }
            return false;
        }
    }
}
