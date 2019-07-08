using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyGame.Engine.GameState.Instants;
using static MyGame.Engine.GameState.GameObject;

namespace MyGame.Engine.GameState
{
    public class ReferenceListField<SubType> : AbstractField where SubType : GameObject
    {
        private Dictionary<Instant, List<int>> valueDict = new Dictionary<Instant, List<int>>();

        public ReferenceListField(InitialInstant instant) : base(instant)
        {
            //TODO: this will fail
        }

        public List<SubType> GetList(CurrentInstant current)
        {
            return GetList(current.Instant);
        }

        public List<SubType> GetList(NextInstant  next)
        {
            return GetList(next.Instant);
        }

        public void SetList(NextInstant next, List<SubType> list)
        {
            List<int> idList = new List<int>();
            foreach(SubType obj in list)
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
            this.valueDict[next.Instant] = idList;
        }

        private List<SubType> GetList(Instant instant)
        {
            List<SubType> returnList = new List<SubType>();
            foreach(int id in this.valueDict[instant])
            {
                if (id == 0)
                {
                    returnList.Add(null);
                }
                else
                {

                    returnList.Add((SubType)instant.GetObject(id));
                }
            }
            return returnList;
        }

        internal override void CopyFieldValues(CurrentInstant current, NextInstant next)
        {
            valueDict[next.Instant] = new List<int>(valueDict[current.Instant]);
        }

        internal override bool Deserialize(Instant container, byte[] buffer, ref int bufferOffset)
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

        internal override List<Instant> GetInstantSet()
        {
            return new List<Instant>(valueDict.Keys);
        }

        internal override bool IsIdentical(Instant container, AbstractField other, Instant otherContainer)
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

        internal override int SerializationSize(Instant container)
        {
            int size = sizeof(int) + valueDict[container].Count * sizeof(int);
            return size;
        }

        internal override void Serialize(Instant container, byte[] buffer, ref int bufferOffset)
        {
            Serialization.Utils.Write(valueDict[container].Count, buffer, ref bufferOffset);
            foreach (int value in valueDict[container])
            {
                Serialization.Utils.Write(value, buffer, ref bufferOffset);
            }
        }
    }
}
