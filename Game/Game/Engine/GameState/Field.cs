using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.GameObject;
using MyGame.Engine.Serialization.DataTypes;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    class Field<FieldValueType> : AbstractField where FieldValueType : struct, FieldValue
    {
        private FieldValueType initialValue;

        //private Dictionary<GameObjectContainer, FieldValueType> fieldsDict = new Dictionary<GameObjectContainer, FieldValueType>();
        //private List<FieldValueType> fieldsList = new List<FieldValueType>();

        public Field(GameObject owner) : base(owner)
        {

        }

        //TODO: lock setting intial value outside of the game object constructor
        public FieldValueType InitialValue
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
            return this.InitialValue;
        }

        public FieldValueType this[CurrentContainer container]
        {
            get
            {
                return container.GetFieldValue<FieldValueType>(this);
            }
        }

        public FieldValueType this[NextContainer container]
        {
            get
            {
                return container.GetFieldValue<FieldValueType>(this);
            }

            set
            {
                container.SetFieldValue<FieldValueType>(this, value);
            }
        }

        public FieldValueType this[GameObjectContainer container]
        {
            get
            {
                return container.GetFieldValue<FieldValueType>(this);
            }

            set
            {
                container.SetFieldValue<FieldValueType>(this, value);
            }
        }



        //TODO: Remove these methods
        public FieldValueType GetValue(GameObjectContainer container)
        {
            return container.GetFieldValue<FieldValueType>(this);
        }

        public void SetValue(GameObjectContainer container, FieldValueType value)
        {
            container.SetFieldValue<FieldValueType>(this, value);
        }
    }
}
