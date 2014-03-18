using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGame.GameStateObjects.DataStuctures
{
    public class GameObjectListManager
    {
        GameObjectListInterface master = new GameObjectList<GameObject>();
        List<GameObjectListInterface> listOfLists = new List<GameObjectListInterface>();

        public GameObjectListManager()
        {
        }

        public List<T> GetList<T>() where T:GameObject
        {
            if (master.GetListType() == typeof(T))
            {
                return master.GetList().Cast<T>().ToList();
            }

            foreach (GameObjectListInterface list in listOfLists)
            {
                if (list.GetListType() == typeof(T))
                {
                    return list.GetList().Cast<T>().ToList();
                }
            }
            GameObjectListInterface newList = new GameObjectList<T>();
            foreach(GameObject obj in master.GetList())
            {
                newList.Add(obj);
            }
            listOfLists.Add(newList);
            return newList.GetList().Cast<T>().ToList();
        }

        public void Add(GameObject obj)
        {
            master.Add(obj);
            foreach (GameObjectListInterface list in listOfLists)
            {
                list.Add(obj);
            }
        }

        public void Remove(GameObject obj)
        {
            foreach (GameObjectListInterface list in listOfLists)
            {
                list.Remove(obj);
            }
        }

        public List<GameObject> GetMaster()
        {
            return master.GetList();
        }
    }
}
