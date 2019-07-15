using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.GameState.Instants;
using static MyGame.Engine.GameState.GameObject;

namespace MyGame.Engine.GameState
{
    //TODO: review this file
    public class ReferenceField<SubType> : AbstractField where SubType : GameObject
    {
        private Dictionary<Instant, int> valueDict = new Dictionary<Instant, int>();

        public ReferenceField(CreationToken creationToken) : base(creationToken)
        {
        }

        internal override void SetDefaultValue(Instant instant)
        {
            valueDict[instant] = 0;
        }

        public SubType this[CurrentInstant current]
        {
            get
            {
                if (this.valueDict[current.Instant] == 0)
                {
                    return null;
                }
                else
                {
                    return (SubType)current.GetObject(this.valueDict[current.Instant]);
                }
            }
        }

        public SubType this[NextInstant next]
        {
            set
            {
                if (value == null || value.ID == null)
                {
                    this.valueDict[next.Instant] = 0;
                }
                else
                {
                    this.valueDict[next.Instant] = (int)value.ID;
                }
            }
        }

        internal override void CopyFieldValues(CurrentInstant current, NextInstant next)
        {
            valueDict[next.Instant] = valueDict[current.Instant];
        }

        internal override bool Deserialize(Instant container, byte[] buffer, ref int bufferOffset)
        {
            int newId = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            bool isValueChanged = !valueDict.ContainsKey(container) || newId != valueDict[container];
            this.valueDict[container] = newId;
            return isValueChanged;
        }

        internal override List<Instant> GetInstantSet()
        {
            return new List<Instant>(valueDict.Keys);
        }

        internal override bool IsIdentical(Instant container, AbstractField other, Instant otherContainer)
        {
            if (other is ReferenceField<SubType>)
            {
                ReferenceField<SubType> otherField = (ReferenceField<SubType>)other;
                return this.valueDict[container].Equals(otherField.valueDict[otherContainer]);
            }
            return false;
        }

        internal override int SerializationSize(Instant container)
        {
            return sizeof(int);
        }

        internal override void Serialize(Instant container, byte[] buffer, ref int bufferOffset)
        {
            Serialization.Utils.Write(valueDict[container], buffer, ref bufferOffset);
        }
    }
}
