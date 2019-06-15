using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.GameObject;
using MyGame.Engine.Serialization.DataTypes;

namespace MyGame.Engine.GameState
{
    abstract class GenericField<DataType> : Field where DataType : struct
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

        protected abstract GenericFieldValue<DataType> GetInitialGenericField();

        public override FieldValue GetInitialField()
        {
            GenericFieldValue<DataType> field = this.GetInitialGenericField();
            field.Value = initialValue;
            return field;
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
