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

        private Dictionary<GameObjectContainer, FieldValueType> fieldsDict = new Dictionary<GameObjectContainer, FieldValueType>();
        private List<FieldValueType> fieldsList = new List<FieldValueType>();

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

        internal override void SetInitialValue(GameObjectContainer container)
        {
            this[container] = initialValue;
        }

        internal override FieldValue GetValue(GameObjectContainer container)
        {
            return this[container];
        }

        public FieldValueType this[CurrentContainer container]
        {
            get
            {
                return this.fieldsDict[container.Container];
            }
        }

        public FieldValueType this[NextContainer container]
        {
            get
            {
                return this.fieldsDict[container.Container];
            }

            set
            {
                this.fieldsDict[container.Container] = value;
            }
        }

        public FieldValueType this[GameObjectContainer container]
        {
            get
            {
                return this.fieldsDict[container];
            }

            set
            {
                this.fieldsDict[container] = value;
            }
        }

        internal override void CopyFieldValues(GameObjectContainer current, GameObjectContainer next)
        {
            this.fieldsDict[next] = this.fieldsDict[current];
        }

        internal override int SerializationSize(GameObjectContainer container)
        {
            return this.fieldsDict[container].SerializationSize;
        }

        internal override void Serialize(GameObjectContainer container, byte[] buffer, ref int bufferOffset)
        {
            this.fieldsDict[container].Serialize(buffer, ref bufferOffset);
        }

        internal override void Deserialize(GameObjectContainer container, byte[] buffer, ref int bufferOffset)
        {
            //TODO: try to get rid of depending on new() constraint
            this.fieldsDict[container] = new FieldValueType();
            this.fieldsDict[container].Deserialize(buffer, ref bufferOffset);
        }



        //TODO: Remove these methods
        /*public FieldValueType GetValue(GameObjectContainer container)
        {
            return container.GetFieldValue<FieldValueType>(this);
        }

        public void SetValue(GameObjectContainer container, FieldValueType value)
        {
            container.SetFieldValue<FieldValueType>(this, value);
        }*/
    }
}
