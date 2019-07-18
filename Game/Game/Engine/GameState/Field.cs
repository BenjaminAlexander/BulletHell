using System.Collections.Generic;
using static MyGame.Engine.GameState.GameObject;
using MyGame.Engine.GameState.FieldValues;
using MyGame.Engine.GameState.Instants;
using System;

namespace MyGame.Engine.GameState
{
    //TODO: we need a pattern for initialization
    //TODO: is the boxing of this struct too inefficient?
    //TODO: rename parameters to match abstract field
    public class Field<FieldValueType> : AbstractField where FieldValueType : struct, FieldValue
    {
        private Dictionary<int, FieldValueType> valueDict = new Dictionary<int, FieldValueType>();

        public Field(CreationToken creationToken) : base(creationToken)
        {
        }

        internal override void SetDefaultValue(int instant)
        {
            if (!IsInstantDeserialized(instant))
            {
                this.valueDict[instant] = default(FieldValueType);
            }
        }

        public FieldValueType this[CurrentInstant current]
        {
            get
            {
                return this.valueDict[current.Instant.InstantID];
            }
        }

        public FieldValueType this[NextInstant next]
        {
            set
            {
                if (!IsInstantDeserialized(next.Instant.InstantID))
                {
                    this.valueDict[next.Instant.InstantID] = value;
                }
            }
        }

        internal override bool HasInstant(int instant)
        {
            return this.valueDict.ContainsKey(instant);
        }

        internal override void CopyFieldValues(int current, int next)
        {
            if (!IsInstantDeserialized(next))
            {
                this.valueDict[next] = this.valueDict[current];
            }
        }

        internal override int SerializationSize(int container)
        {
            return this.valueDict[container].SerializationSize;
        }

        internal override void Serialize(int container, byte[] buffer, ref int bufferOffset)
        {
            this.valueDict[container].Serialize(buffer, ref bufferOffset);
        }

        internal override bool Deserialize(int container, byte[] buffer, ref int bufferOffset)
        {
            FieldValueType fieldValue = default(FieldValueType);
            fieldValue.Deserialize(buffer, ref bufferOffset);

            bool valueIsChanged = !valueDict.ContainsKey(container) || !fieldValue.Equals(valueDict[container]);
            this.valueDict[container] = fieldValue;
            return valueIsChanged;
        }

        internal override bool IsIdentical(int container, AbstractField other, int otherContainer)
        {
            if (other is Field<FieldValueType>)
            {
                Field<FieldValueType> otherField = (Field<FieldValueType>)other;
                return this.valueDict[container].Equals(otherField.valueDict[otherContainer]);
            }
            return false;
        }

        internal override List<int> GetInstantSet()
        {
            return new List<int>(valueDict.Keys);
        }
    }
}
