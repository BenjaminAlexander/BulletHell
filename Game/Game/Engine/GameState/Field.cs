using System.Collections.Generic;
using static MyGame.Engine.GameState.GameObject;
using MyGame.Engine.GameState.FieldValues;
using MyGame.Engine.GameState.Instants;

namespace MyGame.Engine.GameState
{
    //TODO: is the boxing of this struct too inefficient?
    public class Field<FieldValueType> : AbstractField where FieldValueType : struct, FieldValue
    {
        private Dictionary<Instant, FieldValueType> valueDict = new Dictionary<Instant, FieldValueType>();

        public Field(InitialInstant instant) : base(instant)
        {
            this.valueDict[instant.Instant] = default(FieldValueType);
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

        internal override void Deserialize(Instant container, byte[] buffer, ref int bufferOffset)
        {
            FieldValueType fieldValue = default(FieldValueType);
            fieldValue.Deserialize(buffer, ref bufferOffset);
            this.valueDict[container] = fieldValue;
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
