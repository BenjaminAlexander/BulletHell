using MyGame.Engine.GameState.Instants;
using System.Collections.Generic;
using System;

namespace MyGame.Engine.GameState.ObjectFields
{
    public abstract class GenericField<T> : AbstractField
    {
        private Dictionary<int, T> valueDict = new Dictionary<int, T>();

        public GenericField(CreationToken creationToken) : base(creationToken)
        {
        }

        internal abstract T Deserialize(byte[] buffer, ref int bufferOffset);
        internal abstract int SerializationSize(T value);
        internal abstract void Serialize(T value, byte[] buffer, ref int bufferOffset);
        internal abstract T NewDefaultValue();
        internal abstract T Copy(T value);

        internal T GetValue(CurrentInstant current)
        {
            return this.valueDict[current.InstantID];
        }

        internal void SetValue(NextInstant next, T value)
        {
            if (this.valueDict.ContainsKey(next.InstantID) && !IsInstantDeserialized(next.InstantID))
            {
                this.valueDict[next.InstantID] = value;
            }
        }

        internal override void CopyFieldValues(int fromInstantId, int toInstantId)
        {
            T originialValue = valueDict[fromInstantId];
            T newValue = Copy(originialValue);
            if(typeof(T).IsClass && (object)originialValue == (object)newValue)
            {
                throw new Exception("Copy cannot return the same instance as the original");
            }

            valueDict[toInstantId] = newValue;
        }

        internal override bool Deserialize(int instantId, byte[] buffer, ref int bufferOffset)
        {
            T value = Deserialize(buffer, ref bufferOffset);
            bool valueIsChanged;
            if(valueDict.ContainsKey(instantId))
            {
                T oldValue = valueDict[instantId];
                if(oldValue == null)
                {
                    valueIsChanged = value != null;
                }
                else
                {
                    valueIsChanged = !oldValue.Equals(value);
                }
            }
            else
            {
                valueIsChanged = true;
            }

            valueDict[instantId] = value;
            return valueIsChanged;
        }

        internal override List<int> GetInstantSet()
        {
            return new List<int>(valueDict.Keys);
        }

        internal override bool HasInstant(int instantId)
        {
            return valueDict.ContainsKey(instantId);
        }

        internal override bool IsIdentical(int instantId, AbstractField other, int otherInstantId)
        {
            if (other is GenericField<T>)
            {
                GenericField<T> otherField = (GenericField<T>)other;
                return valueDict[instantId].Equals(otherField.valueDict[otherInstantId]);
            }
            return false;
        }

        internal override int SerializationSize(int instantId)
        {
            return SerializationSize(valueDict[instantId]);
        }

        internal override void Serialize(int instantId, byte[] buffer, ref int bufferOffset)
        {
            Serialize(valueDict[instantId], buffer, ref bufferOffset);
        }

        internal override void SetDefaultValue(int instantId)
        {
            valueDict[instantId] = NewDefaultValue();
        }

        internal override void RemoveInstant(int instantId)
        {
            valueDict.Remove(instantId);
        }
    }
}
