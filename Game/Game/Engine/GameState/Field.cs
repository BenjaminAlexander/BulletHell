using System.Collections.Generic;
using static MyGame.Engine.GameState.GameObject;
using MyGame.Engine.GameState.FieldValues;
using MyGame.Engine.GameState.Instants;
using System;

namespace MyGame.Engine.GameState
{
    //TODO: we need a pattern for initialization
    //TODO: is the boxing of this struct too inefficient?
    public class Field<FieldValueType> : AbstractField where FieldValueType : struct, FieldValue
    {
        private Dictionary<Instant, FieldValueType> valueDict = new Dictionary<Instant, FieldValueType>();

        public Field(CreationToken creationToken) : base(creationToken)
        {
        }

        internal override void SetDefaultValue(Instant instant)
        {
            this.valueDict[instant] = default(FieldValueType);
        }

        public FieldValueType this[CurrentInstant current]
        {
            get
            {
                return this.valueDict[current.Instant];
            }
        }

        public FieldValueType this[NextInstant next]
        {
            get
            {
                return this.valueDict[next.Instant];
            }

            set
            {
                this.valueDict[next.Instant] = value;
            }
        }

        internal override void CopyFieldValues(CurrentInstant current, NextInstant next)
        {
            this[next] = this[current];
        }

        internal override int SerializationSize(Instant container)
        {
            return this.valueDict[container].SerializationSize;
        }

        internal override void Serialize(Instant container, byte[] buffer, ref int bufferOffset)
        {
            this.valueDict[container].Serialize(buffer, ref bufferOffset);
        }

        internal override bool Deserialize(Instant container, byte[] buffer, ref int bufferOffset)
        {
            FieldValueType fieldValue = default(FieldValueType);
            fieldValue.Deserialize(buffer, ref bufferOffset);

            bool valueIsChanged = !valueDict.ContainsKey(container) || !fieldValue.Equals(valueDict[container]);
            this.valueDict[container] = fieldValue;
            return valueIsChanged;
        }

        internal override bool IsIdentical(Instant container, AbstractField other, Instant otherContainer)
        {
            if (other is Field<FieldValueType>)
            {
                Field<FieldValueType> otherField = (Field<FieldValueType>)other;
                return this.valueDict[container].Equals(otherField.valueDict[otherContainer]);
            }
            return false;
        }

        internal override List<Instant> GetInstantSet()
        {
            return new List<Instant>(valueDict.Keys);
        }
    }
}
