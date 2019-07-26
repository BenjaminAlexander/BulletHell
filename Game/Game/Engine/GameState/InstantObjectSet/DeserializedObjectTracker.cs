using MyGame.Engine.DataStructures;
using MyGame.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace MyGame.Engine.GameState.InstantObjectSet
{
    class DeserializedObjectTracker<T> : IEnumerable<T> where T : class
    {
        private TwoWayMap<int, T> objects = new TwoWayMap<int, T>(new IntegerComparer());

        //private List<int?> deserializedIds = null;
        private int deserializedCount = 0;
        private int? expectedCount = null;

        public void SetCount(int count)
        {
            expectedCount = count;
            deserializedCount = 0;

            foreach(KeyValuePair<int, T> pair in objects)
            {
                if(pair.Key < expectedCount)
                {
                    deserializedCount++;
                }
                else
                {
                    objects.RemoveKey(pair.Key);
                }
            }
        }

        public T Get(int index)
        {
            if(objects.ContainsKey(index))
            {
                return objects[index];
            }
            return null;
        }

        public void Set(int index, T obj)
        {
            if (expectedCount != null && index < (int)expectedCount)
            {
                deserializedCount++;
            }

            if (expectedCount == null || index < (int)expectedCount)
            {
                objects[index] = obj;
            }
        }

        public bool AllDeserialized()
        {
            return expectedCount != null && expectedCount == deserializedCount;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return objects.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

