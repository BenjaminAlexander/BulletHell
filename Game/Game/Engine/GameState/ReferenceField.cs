/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.GameState.Instants;
using static MyGame.Engine.GameState.GameObject;

namespace MyGame.Engine.GameState
{
    //TODO: review this file
    //TODO: rename parameters to match abstract field
    public class ReferenceField<SubType> : AbstractField where SubType : GameObject
    {
        private Dictionary<int, int> valueDict = new Dictionary<int, int>();

        public ReferenceField(CreationToken creationToken) : base(creationToken)
        {
        }

        internal override void SetDefaultValue(int instant)
        {
            if (!IsInstantDeserialized(instant))
            {
                valueDict[instant] = 0;
            }
        }

        public SubType this[CurrentInstant current]
        {
            get
            {
                if (this.valueDict[current.Instant.InstantID] == 0)
                {
                    return null;
                }
                else
                {
                    return (SubType)current.GetObject(this.valueDict[current.Instant.InstantID]);
                }
            }
        }

        public SubType this[NextInstant next]
        {
            set
            {
                if (!IsInstantDeserialized(next.Instant.ID))
                {
                    if (value == null || value.ID == null)
                    {
                        this.valueDict[next.Instant.ID] = 0;
                    }
                    else
                    {
                        this.valueDict[next.Instant.ID] = (int)value.ID;
                    }
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
                valueDict[next] = valueDict[current];
            }
        }

        internal override bool Deserialize(int container, byte[] buffer, ref int bufferOffset)
        {
            int newId = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            bool isValueChanged = !valueDict.ContainsKey(container) || newId != valueDict[container];
            this.valueDict[container] = newId;
            return isValueChanged;
        }

        internal override List<int> GetInstantSet()
        {
            return new List<int>(valueDict.Keys);
        }

        internal override bool IsIdentical(int container, AbstractField other, int otherContainer)
        {
            if (other is ReferenceField<SubType>)
            {
                ReferenceField<SubType> otherField = (ReferenceField<SubType>)other;
                return this.valueDict[container].Equals(otherField.valueDict[otherContainer]);
            }
            return false;
        }

        internal override int SerializationSize(int container)
        {
            return sizeof(int);
        }

        internal override void Serialize(int container, byte[] buffer, ref int bufferOffset)
        {
            Serialization.Utils.Write(valueDict[container], buffer, ref bufferOffset);
        }
    }
}*/
