using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.GameState.InstantObjectSet
{
    class DeserializedTracker
    {
        private List<int?> deserializedIds = null;
        private int deserializedCount = 0;

        public void SetObjectCount(int objectCount)
        {
            deserializedCount = 0;
            if (deserializedIds == null)
            {
                deserializedIds = new List<int?>(objectCount);
                for (int i = 0; i < objectCount; i++)
                {
                    deserializedIds[i] = null;
                }
            }
            else if (deserializedIds.Count != objectCount)
            {
                List<int?> newList = new List<int?>(objectCount);
                for (int i = 0; i < objectCount; i++)
                {
                    if (i < deserializedIds.Count)
                    {
                        newList[i] = deserializedIds[i];
                        if(newList[i] != null)
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

        public int GetId(int index)
        {
            return (int)deserializedIds[index];
        }

        public void SetId(int index, int id)
        {
            if(deserializedIds[index] == null)
            {
                deserializedCount++;
            }
            deserializedIds[index] = id;
        }

        public bool AllDeserialized()
        {
            return deserializedIds.Count == deserializedCount;
        }
    }
}
