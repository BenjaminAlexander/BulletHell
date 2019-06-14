using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.GameObject;
using MyGame.Engine.Serialization.DataTypes;

namespace MyGame.Engine.GameState
{
    class GenericField<DataType, SerializableType> : Field where DataType : new() where SerializableType : SGeneric<DataType>, new()
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

        public override FieldValue GetInitialField()
        {
            GenericFieldValue<DataType, SerializableType> field = new GenericFieldValue<DataType, SerializableType>();
            field.Value = initialValue;
            return field;
        }

        public DataType GetValue(GameObjectContainer container)
        {
            GenericFieldValue<DataType, SerializableType> field = (GenericFieldValue<DataType, SerializableType>)(this.GetField(container));
            return field.Value;
        }

        public void SetValue(GameObjectContainer container, DataType value)
        {
            GenericFieldValue<DataType, SerializableType> field = (GenericFieldValue<DataType, SerializableType>)(this.GetField(container));
            field.Value = value;
        }
    }
}
