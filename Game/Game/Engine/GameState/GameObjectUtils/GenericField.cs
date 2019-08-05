using MyGame.Engine.GameState.Instants;
using System.Collections.Concurrent;
using System;
using System.Collections.Generic;

namespace MyGame.Engine.GameState.GameObjectUtils
{
    public abstract class GenericField<T> : AbstractField
    {
        private ConcurrentDictionary<int, T> valueDict = new ConcurrentDictionary<int, T>();

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
            this.SetValue<T>(next.InstantID, this, value);
        }

        internal void ForceSet(int instantId, T value)
        {
            this.valueDict[instantId] = value;
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
            T outValue;
            valueDict.TryRemove(instantId, out outValue);
        }
    }
}
