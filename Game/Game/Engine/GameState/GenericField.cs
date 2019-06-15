using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.GameObject;
using MyGame.Engine.Serialization.DataTypes;

namespace MyGame.Engine.GameState
{
    class GenericField<DataType, FieldValueType> : Field where DataType : struct where FieldValueType : GenericFieldValue<DataType>, new()
    {
        private DataType initialValue = new DataType();

        public GenericField(GameObject owner) : base(owner)
        {

        }

        public DataType InitialValue
        {
            get
            {
                return initialValue;
            }

            set
            {
                initialValue = value;
            }
        }

        internal override FieldValue GetInitialField()
        {
            FieldValueType value = new FieldValueType();
            value.Value = this.InitialValue;
            return value;
        }

        public DataType this[CurrentContainer container]
        {
            get
            {
                return ((FieldValueType)(container[this])).Value;
            }
        }

        public DataType this[NextContainer container]
        {
            get
            {
                return ((FieldValueType)(container[this])).Value;
            }

            set
            {
                ((FieldValueType)(container[this])).Value = value;
            }
        }

        public DataType GetValue(GameObjectContainer container)
        {
            GenericFieldValue<DataType> field = (GenericFieldValue<DataType>)(container[this]);
            return field.Value;
        }

        public void SetValue(GameObjectContainer container, DataType value)
        {
            GenericFieldValue<DataType> field = (GenericFieldValue<DataType>)(container[this]);
            field.Value = value;
        }
    }
}
