using System.Collections.Generic;
using static MyGame.Engine.GameState.GameObject;
using MyGame.Engine.GameState.FieldValues;
using MyGame.Engine.GameState.Instants;

namespace MyGame.Engine.GameState
{
    class Field<FieldValueType> : AbstractField where FieldValueType : struct, FieldValue
    {
        private Dictionary<Instant, FieldValueType> fieldsDict = new Dictionary<Instant, FieldValueType>();

        public Field(InitialInstant instant) : base(instant)
        {
            this.fieldsDict[instant.Instant] = default(FieldValueType);
        }

        public FieldValueType this[CurrentInstant current]
        {
            get
            {
                return this.fieldsDict[current.Instant];
            }
        }

        public FieldValueType this[NextInstant next]
        {
            get
            {
                return this.fieldsDict[next.Instant];
            }

            set
            {
                this.fieldsDict[next.Instant] = value;
            }
        }

        internal override void CopyFieldValues(CurrentInstant current, NextInstant next)
        {
            this[next] = this[current];
        }

        internal override int SerializationSize(Instant container)
        {
            return this.fieldsDict[container].SerializationSize;
        }

        internal override void Serialize(Instant container, byte[] buffer, ref int bufferOffset)
        {
            this.fieldsDict[container].Serialize(buffer, ref bufferOffset);
        }

        internal override void Deserialize(Instant container, byte[] buffer, ref int bufferOffset)
        {
            FieldValueType fieldValue = default(FieldValueType);
            fieldValue.Deserialize(buffer, ref bufferOffset);
            this.fieldsDict[container] = fieldValue;
        }

        internal override bool IsIdentical(Instant container, AbstractField other, Instant otherContainer)
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
