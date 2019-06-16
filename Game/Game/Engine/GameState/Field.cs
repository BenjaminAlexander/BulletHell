using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.GameObject;
using MyGame.Engine.GameState.FieldValues;
using MyGame.Engine.Serialization.DataTypes;
using MyGame.Engine.Serialization;

namespace MyGame.Engine.GameState
{
    class Field<FieldValueType> : AbstractField where FieldValueType : struct, FieldValue
    {
        private Dictionary<GameObjectContainer, FieldValueType> fieldsDict = new Dictionary<GameObjectContainer, FieldValueType>();

        public Field(GameObject owner, NextContainer container) : base(owner)
        {
            this[container] = default(FieldValueType);
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
            FieldValueType fieldValue = default(FieldValueType);
            fieldValue.Deserialize(buffer, ref bufferOffset);
            this.fieldsDict[container] = fieldValue;
        }

        internal override bool IsIdentical(GameObjectContainer container, AbstractField other, GameObjectContainer otherContainer)
        {
            if (other is Field<FieldValueType>)
            {
                Field<FieldValueType> otherField = (Field<FieldValueType>)other;
                return this.fieldsDict[container].Equals(otherField.fieldsDict[otherContainer]);
            }
            return false;
        }
    }
}
