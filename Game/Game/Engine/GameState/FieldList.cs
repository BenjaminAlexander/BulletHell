using MyGame.Engine.GameState.FieldValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyGame.Engine.GameState.GameObject;
using MyGame.Engine.GameState.Instants;

namespace MyGame.Engine.GameState
{
    //TODO: unit test this
    class FieldList<FieldValueType> : AbstractField where FieldValueType : struct, FieldValue
    {
        private Dictionary<Instant, List<FieldValueType>> valueDict = new Dictionary<Instant, List<FieldValueType>>();

        public FieldList(InitialInstant instant) : base(instant)
        {
            //TODO: this will fail
        }

        //TODO: is this the best way to prevent editing the current state
        public List<FieldValueType> this[CurrentInstant current]
        {
            get
            {
                return new List<FieldValueType>(this.valueDict[current.Instant]);
            }
        }

        public List<FieldValueType> this[NextInstant next]
        {
            get
            {
                return this.valueDict[next.Instant];
            }
        }

        internal override void CopyFieldValues(CurrentInstant current, NextInstant next)
        {
            valueDict[next.Instant] = new List<FieldValueType>(valueDict[current.Instant]);
        }

        internal override bool Deserialize(Instant container, byte[] buffer, ref int bufferOffset)
        {
            int count = Serialization.Utils.ReadInt(buffer, ref bufferOffset);
            List<FieldValueType> newList = new List<FieldValueType>(count);
            List<FieldValueType> oldList = null;

            if(valueDict.ContainsKey(container))
            {
                oldList = valueDict[container];
            }

            if(oldList.Count != count)
            {
                oldList = null;
            }

            for (int i = 0; i < count; i ++)
            {
                FieldValueType value = default(FieldValueType);
                value.Deserialize(buffer, ref bufferOffset);
                newList.Add(value);

                if(oldList != null && !oldList[i].Equals(value))
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
            if (other is FieldList<FieldValueType>)
            {
                FieldList<FieldValueType> otherField = (FieldList<FieldValueType>)other;
                List<FieldValueType> thisList = this.valueDict[container];
                List<FieldValueType> otherList = otherField.valueDict[container];

                if (thisList.Count == otherList.Count)
                {
                    for(int i = 0; i < thisList.Count; i++)
                    {
                        if(!thisList[i].Equals(otherList[i]))
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
            int size = sizeof(int);
            foreach(FieldValueType value in valueDict[container])
            {
                size = size + value.SerializationSize;
            }
            return size;
        }

        internal override void Serialize(Instant container, byte[] buffer, ref int bufferOffset)
        {
            Engine.Serialization.Utils.Write(valueDict[container].Count, buffer, ref bufferOffset);
            foreach (FieldValueType value in valueDict[container])
            {
                value.Serialize(buffer, ref bufferOffset);
            }
        }
    }
}
