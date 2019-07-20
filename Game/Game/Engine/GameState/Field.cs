using System.Collections.Generic;
using static MyGame.Engine.GameState.GameObject;
using MyGame.Engine.GameState.FieldValues;
using MyGame.Engine.GameState.Instants;
using System;

namespace MyGame.Engine.GameState
{
    public class Field<FieldValueType> : GenericField<FieldValueType> where FieldValueType : struct, FieldValue
    {
        public Field(CreationToken creationToken) : base(creationToken)
        {
        }

        internal override FieldValueType Deserialize(byte[] buffer, ref int bufferOffset)
        {
            FieldValueType value = default(FieldValueType);
            value.Deserialize(buffer, ref bufferOffset);
            return value;
        }

        internal override bool Equals(FieldValueType value1, FieldValueType value2)
        {
            return value1.Equals(value2);
        }

        internal override int SerializationSize(FieldValueType value)
        {
            return value.SerializationSize;
        }

        internal override void Serialize(FieldValueType value, byte[] buffer, ref int bufferOffset)
        {
            value.Serialize(buffer, ref bufferOffset);
        }

        internal override FieldValueType NewDefaultValue()
        {
            return default(FieldValueType);
        }

        public FieldValueType this[CurrentInstant current]
        {
            get
            {
                return GetValue(current);
            }
        }

        public FieldValueType this[NextInstant next]
        {
            set
            {
                SetValue(next, value);
            }
        }
    }
}
