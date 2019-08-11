using System.Collections.Generic;

namespace MyGame.Engine.GameState.Utils
{
    class DeserializedTracker
    {
        private object lockObj = new object();
        private List<int?> deserializedIds = null;
        private volatile int deserializedCount = 0;

        public int? Count
        {
            get
            {
                lock (lockObj)
                {
                    if (deserializedIds != null)
                    {
                        return deserializedIds.Count;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public void SetCount(int count)
        {
            lock (lockObj)
            {
                deserializedCount = 0;
                if (deserializedIds == null)
                {
                    deserializedIds = new List<int?>(count);
                    for (int i = 0; i < count; i++)
                    {
                        deserializedIds[i] = null;
                    }
                }
                else if (deserializedIds.Count != count)
                {
                    List<int?> newList = new List<int?>(count);
                    for (int i = 0; i < count; i++)
                    {
                        if (i < deserializedIds.Count)
                        {
                            newList[i] = deserializedIds[i];
                            if (newList[i] != null)
                            {
                                deserializedCount++;
                            }
                        }
                        else
                        {
                            newList[i] = null;
                        }
                    }
                    deserializedIds = newList;
                }
            }
        }

        public int? GetId(int index)
        {
            lock (lockObj)
            {
                return deserializedIds[index];
            }
        }

        public void SetId(int index, int id)
        {
            lock (lockObj)
            {
                if (deserializedIds[index] == null)
                {
                    deserializedCount++;
                }
                deserializedIds[index] = id;
            }
        }

        public bool AllDeserialized()
        {
            lock (lockObj)
            {
                return deserializedIds != null && deserializedIds.Count == deserializedCount;
            }
        }
    }
}
