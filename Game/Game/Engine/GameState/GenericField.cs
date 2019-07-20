using MyGame.Engine.GameState.Instants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState
{
    public abstract class GenericField<T> : AbstractField
    {
        private Dictionary<int, T> valueDict = new Dictionary<int, T>();

        public GenericField(CreationToken creationToken) : base(creationToken)
        {
        }

        internal abstract T Deserialize(byte[] buffer, ref int bufferOffset);
        internal abstract bool Equals(T value1, T value2);
        internal abstract int SerializationSize(T value);
        internal abstract void Serialize(T value, byte[] buffer, ref int bufferOffset);
        internal abstract T NewDefaultValue();

        internal T GetValue(CurrentInstant current)
        {
            return this.valueDict[current.Instant.InstantID];
        }

        internal void SetValue(NextInstant next, T value)
        {
            if (!IsInstantDeserialized(next.Instant.InstantID))
            {
                this.valueDict[next.Instant.InstantID] = value;
            }
        }

        internal override void CopyFieldValues(int fromInstantId, int toInstantId)
        {
            if (!IsInstantDeserialized(toInstantId))
            {
                valueDict[toInstantId] = valueDict[fromInstantId];
            }
        }

        internal override bool Deserialize(int instantId, byte[] buffer, ref int bufferOffset)
        {
            T value = Deserialize(buffer, ref bufferOffset);
            bool valueIsChanged = !valueDict.ContainsKey(instantId) || !Equals(value, valueDict[instantId]);
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
    }
}
