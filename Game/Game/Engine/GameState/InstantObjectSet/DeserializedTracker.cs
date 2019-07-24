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

        public int? Count
        {
            get
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

        public void SetCount(int count)
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

        public int? GetId(int index)
        {
            return deserializedIds[index];
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
