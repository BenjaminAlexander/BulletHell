using MyGame.Engine.GameState.Instants;
using System.Collections.Concurrent;
using System;
using System.Collections.Generic;

namespace MyGame.Engine.GameState.GameObjectUtils
{
    public abstract class GenericField<T> : AbstractField
    {
        private ConcurrentDictionary<int, T> valueDict = new ConcurrentDictionary<int, T>();
        private ConcurrentDictionary<int, DeserializedInfo> isDeserialized = new ConcurrentDictionary<int, DeserializedInfo>();

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
            DeserializedInfo info = isDeserialized.GetOrAdd(instantId, new DeserializedInfo(false));
            lock (info)
            {
                valueDict[instantId] = NewDefaultValue();
            }
        }

        internal override void RemoveInstant(int instantId)
        {
            DeserializedInfo info;
            if (isDeserialized.TryGetValue(instantId, out info))
            {
                lock(info)
                {
                    T outValue;
                    valueDict.TryRemove(instantId, out outValue);
                    DeserializedInfo outInfo;
                    isDeserialized.TryRemove(instantId, out outInfo);
                }
            }

        }
    }
}
