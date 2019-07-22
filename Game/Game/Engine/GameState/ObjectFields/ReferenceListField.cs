using System;
/*using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.GameState.Instants;
using static MyGame.Engine.GameState.GameObject;
using MyGame.Engine.GameState.InstantObjectSet;

namespace MyGame.Engine.GameState.ObjectFields
{
    //TODO: review this file
    //TODO: rename parameters to match abstract field
    public class ReferenceListField<SubType> : AbstractField where SubType : GameObject
    {
        private Dictionary<int, List<int>> valueDict = new Dictionary<int, List<int>>();

        public ReferenceListField(CreationToken creationToken) : base(creationToken)
        {
        }

        internal override void SetDefaultValue(int instant)
        {
            if (!IsInstantDeserialized(instant))
            {
                valueDict[instant] = new List<int>();
            }
        }

        internal override bool HasInstant(int instant)
        {
            return this.valueDict.ContainsKey(instant);
        }

        public List<SubType> GetList(CurrentInstant current)
        {
            return GetList(current);
        }

        public void SetList(NextInstant next, List<SubType> list)
        {
            if (!IsInstantDeserialized(next.Instant.InstantID))
            {
                List<int> idList = new List<int>();
                foreach (SubType obj in list)
                {
                    if (obj == null || obj.ID == null)
                    {
                        idList.Add(0);
                    }
                    else
                    {
                        idList.Add((int)obj.ID);
                    }
                }
                this.valueDict[next.Instant.InstantID] = idList;
            }
        }

        private List<SubType> GetList(InstantSet instant)
        {
            List<SubType> returnList = new List<SubType>();
            foreach(int id in this.valueDict[instant.InstantID])
            {
                if (id == 0)
                {
                    returnList.Add(null);
                }
                else
                {

                    returnList.Add(instant.GetObject<SubType>(id));
                }
            }
            return returnList;
        }

        internal override void CopyFieldValues(int current, int next)
        {
            if (!IsInstantDeserialized(next))
            {
                valueDict[next] = new List<int>(valueDict[current]);
            }
        }

        internal override bool Deserialize(int container, byte[] buffer, ref int bufferOffset)
        {
            int count = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            List<int> newList = new List<int>(count);
            List<int> oldList = null;

            if (valueDict.ContainsKey(container))
            {
                oldList = valueDict[container];
            }

            if (oldList.Count != count)
            {
                oldList = null;
            }

            for (int i = 0; i < count; i++)
            {
                int value = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
                newList.Add(value);

                if (oldList != null && !oldList[i].Equals(value))
                {
                    oldList = null;
                }
            }

            this.valueDict[container] = newList;
            return oldList == null;
        }

        internal override List<int> GetInstantSet()
        {
            return new List<int>(valueDict.Keys);
        }

        internal override bool IsIdentical(int container, AbstractField other, int otherContainer)
        {
            if (other is ReferenceListField<SubType>)
            {
                ReferenceListField<SubType> otherField = (ReferenceListField<SubType>)other;
                List<int> thisList = this.valueDict[container];
                List<int> otherList = otherField.valueDict[container];

                if (thisList.Count == otherList.Count)
                {
                    for (int i = 0; i < thisList.Count; i++)
                    {
                        if (thisList[i] != otherList[i])
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        internal override int SerializationSize(int container)
        {
            int size = sizeof(int) + valueDict[container].Count * sizeof(int);
            return size;
        }

        internal override void Serialize(int container, byte[] buffer, ref int bufferOffset)
        {
            Serialization.Utils.Write(valueDict[container].Count, buffer, ref bufferOffset);
            foreach (int value in valueDict[container])
            {
                Serialization.Utils.Write(value, buffer, ref bufferOffset);
            }
        }
    }
}*/
