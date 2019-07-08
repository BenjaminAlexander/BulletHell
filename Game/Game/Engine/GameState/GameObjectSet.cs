using MyGame.Engine.DataStructures;
using MyGame.Engine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using MyGame.Engine.GameState.Instants;

namespace MyGame.Engine.GameState
{
    //TODO: Add a test to check that object IDs match objects, including during contains
    class GameObjectSet : ICollection<GameObject>
    {
        private static Logger log = new Logger(typeof(GameObjectSet));

        public static GameObjectSet Union(GameObjectSet setA, GameObjectSet setB)
        {
            GameObjectSet newSet = new GameObjectSet();
            newSet.Add(setA);
            newSet.Add(setB);
            return newSet;
        }

        private TwoWayMap<int, GameObject> objects = new TwoWayMap<int, GameObject>(new IntegerComparer());
        private bool isReadOnly;

        public GameObjectSet()
        {
            isReadOnly = false;
        }

        public GameObjectSet(bool isReadOnly)
        {
            this.isReadOnly = isReadOnly;
        }

        public GameObject this[int id]
        {
            get
            {
                return objects[id];
            }
        }

        public int Count
        {
            get
            {
                return objects.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return isReadOnly;
            }
        }

        public void Add(GameObject item)
        {
            if (item.ID == null)
            {
                throw new Exception("The GameObject does not have its ID set");
            }
            objects.Set((int)item.ID, item);
        }

        public void Add(GameObjectSet set)
        {
            foreach (GameObject obj in set)
            {
                this.Add(obj);
            }
        }

        public void Clear()
        {
            objects = new TwoWayMap<int, GameObject>(new IntegerComparer());
        }

        public bool Contains(GameObject item)
        {
            return objects.ContainsValue(item);
        }

        public bool Contains(int id)
        {
            return objects.ContainsKey(id);
        }

        public void CopyTo(GameObject[] array, int arrayIndex)
        {
            foreach (KeyValuePair<int, GameObject> pair in objects)
            {
                array[arrayIndex] = pair.Value;
                arrayIndex++;
            }
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            return objects.Values.GetEnumerator();
        }

        public bool Remove(GameObject item)
        {
            return objects.RemoveValue(item);
        }

        public bool Remove(int id)
        {
            return objects.RemoveKey(id);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool CheckIntegrety()
        {
            foreach (KeyValuePair<int, GameObject> pair in objects)
            {
                if (pair.Key != pair.Value.ID)
                {
                    log.Error("Object ID does not match object key in map");
                    return false;
                }
            }
            return true;
        }

        public bool CheckIntegrety(GameObjectSet globalSet)
        {
            foreach (KeyValuePair<int, GameObject> pair in objects)
            {
                if (pair.Key != pair.Value.ID)
                {
                    log.Error("Object ID does not match object key in map");
                    return false;
                }

                GameObject globalReference = globalSet.objects[pair.Key];
                if (globalReference.ID != pair.Value.ID)
                {
                    log.Error("Object ID does not match object ID in global map");
                    return false;
                }

                if (globalReference != pair.Value)
                {
                    log.Error("Object reference does not match object reference in global map");
                    return false;
                }
            }
            return true;
        }

        public int GreatestID
        {
            get
            {
                return objects.GreatestKey;
            }
        }
        
        public SubType NewGameObject<SubType>(Instant instant) where SubType : GameObject, new()
        {
            SubType newObject = GameObject.NewGameObject<SubType>(GreatestID + 1, instant);
            this.Add(newObject);
            return newObject;
        }

        public GameObject GetGameObject(int id, Instant instant, byte[] buffer, int bufferOffset)
        {
            if(Contains(id))
            {
                return this[id];
            }

            GameObject newObject = GameObject.NewGameObject(id, instant, buffer, bufferOffset);
            this.Add(newObject);
            return newObject;
        }
    }
}
